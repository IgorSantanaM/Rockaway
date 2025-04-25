using QRCoder;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Rockaway.WebApp.Models;

namespace Rockaway.WebApp.Services.Mail;

public class PdfMaker(QRCodeGenerator qrCodeGenerator) : IPdfMaker {

	private static readonly SvgImage rockawayLogotypeSvg
		= SvgImage.FromText(EmbeddedResource.ReadAllText("rockaway-logotype.svg"));

	static PdfMaker() {
		QuestPDF.Settings.License = LicenseType.Community;
		using var fontStream = EmbeddedResource.OpenStream("PTSansNarrow-Regular.ttf");
		FontManager.RegisterFontWithCustomName("PT Sans", fontStream);
	}

	private void DrawShowDetails(RowDescriptor row, TicketOrderMailData order) {
		row.ConstantItem(12, Unit.Centimetre).Column(column => {
			column.Spacing(5);
			column.Item().Width(140).Svg(rockawayLogotypeSvg).FitWidth();
			column.Item().Text("gig tickets for good people").FontSize(16);
			column.Item().BorderTop(1).Height(4, Unit.Centimetre).Column(c => {
				c.Item().PaddingTop(10).Text(order.Headliner).FontSize(20).Bold();
				if (order.HasSupport) {
					c.Item().Text(text => {
						text.Span("Plus special guests: ");
						text.Span(order.SupportArtistsText).Bold();
					});
				}
			});
			column.Item().Text(text => {
				text.DefaultTextStyle(style => style.Bold());
				text.Span(order.ShowDate.ToString("dddd dd MMMM yyyy", CultureInfo.InvariantCulture));
				text.Span(" Doors ");
				text.Span(order.DoorsOpen.ToString("HH:mm", CultureInfo.InvariantCulture));
			});
		});
	}

	private byte[] CreateQrCode(Guid id) {
		using var qrCodeData = qrCodeGenerator.CreateQrCode(id.ToString(), QRCodeGenerator.ECCLevel.Q);
		using var qrCode = new PngByteQRCode(qrCodeData);
		return qrCode.GetGraphic(20, false);
	}

	private void DrawTicketQrCode(RowDescriptor row, TicketViewData ticket) {
		var qrCodeBytes = CreateQrCode(ticket.TicketId);
		row.RelativeItem().Padding(10).Column(column => {
			column.Spacing(5);
			column.Item().Image(qrCodeBytes).FitWidth();
			column.Item().AlignCenter()
				.Text(ticket.TicketId.ToString())
				.FontSize(7).FontFamily("Consolas");
		});
	}

	private void DrawShowAndQrCode(ColumnDescriptor column, TicketOrderMailData order, TicketViewData ticket)
		=> column.Item().Height(8, Unit.Centimetre).Row(row => {
			DrawShowDetails(row, order);
			DrawTicketQrCode(row, ticket);
		});

	private void DrawTicket(ColumnDescriptor column, TicketOrderMailData order, TicketViewData ticket) {
		column.Spacing(30);
		column
			.Item().Height(13, Unit.Centimetre).Border(1).Padding(10)
			.Column(sections => {
				DrawShowAndQrCode(sections, order, ticket);
				DrawVenueDetails(sections, order);
				DrawTicketDetails(sections, ticket);
			});
	}

	private void DrawVenueDetails(ColumnDescriptor column, TicketOrderMailData order) {
		column.Item().Text(text => {
			text.Span(order.VenueName).FontSize(16).Bold();
			text.Span(" ");
			text.Span(order.VenueFullAddress);
		});
		column.Item().Text(text => {
			text.Span(order.VenueTelephone);
			text.Span(" ");
			text.Span(order.VenueWebsiteUrl);
		});
	}

	private void DrawTicketDetails(ColumnDescriptor column, TicketViewData ticket) {
		column.Item().PaddingTop(20).Text(text => {
			text.Span(ticket.Description).Bold().FontSize(20);
			text.Span(" ");
			text.Span($"({ticket.Price})");
		});
	}

	public byte[] CreatePdfTickets(TicketOrderMailData data) {
		var pdf = Document.Create(container => {
			container.Page(page => {
				page.Size(PageSizes.A4);
				page.DefaultTextStyle(text => text.FontFamily("PT Sans").FontSize(14));
				page.Margin(1, Unit.Centimetre);
				page.Content().Column(column => {
					foreach (var ticket in data.Tickets) {
						DrawTicket(column, data, ticket);
					}
				});
			});
		});
		return pdf.GeneratePdf();
	}
}
