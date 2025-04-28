using NodaTime;
using QRCoder;
using Rockaway.WebApp.Data.Entities;
using Rockaway.WebApp.Models;
using Rockaway.WebApp.Services;
using Rockaway.WebApp.Services.Mail;
using System.Reflection;
using System.Text;
using UglyToad.PdfPig;

namespace Rockaway.WebApp.Tests.Mail {
	public class PdfkMakerTests : MailRendererTestBase {
		private (TicketOrder, byte[]) CreateTicketOrderPdf() {
			var order = CreateSampleOrder();
			order.CreatedAt = Instant.FromUtc(2025, 6, 7, 8, 9, 10);
			order.CompletedAt = Instant.FromUtc(2025, 6, 7, 8, 9, 11);
			foreach (var ticket in order.Tickets) ticket.Id = Guid.Empty;
			var mailData = new TicketOrderMailData(order, new("https://rockaway.dev"));
			var qrCode = new QRCodeGenerator();
			var bytes = new PdfMaker(qrCode).CreatePdfTickets(mailData);
			return (order, bytes);
		}

		private static readonly byte[] validPdfHeaderBytes = "%PDF-1.7"u8.ToArray();

		[Fact]
		public void CreatesPdf_ValidHeader_ReturnsTrue() {
			var (_, pdfBytes) = CreateTicketOrderPdf();
			pdfBytes[..8].ShouldBe(validPdfHeaderBytes);
		}

		[Fact]
		public void CreatesPdf_MatchesEntireFile_ReturnsTrue() {
			var (_, pdfBytes) = CreateTicketOrderPdf();

			var expectedPdfBytes = EmbeddedResource.ReadBytes("tickets.pdf", Assembly.GetExecutingAssembly());

			Encoding.UTF8.GetString(pdfBytes).ShouldBe(Encoding.UTF8.GetString(expectedPdfBytes));
		}

		[Fact]
		public void CreatesPdf_ContainingCorrectText_ReturnsTrue() {
			var (order, pdfBytes)  = CreateTicketOrderPdf();
			var document = PdfDocument.Open(pdfBytes);
			var pdfText = document.GetPages()
				.SelectMany(page => page.GetWords())
				.Aggregate(string.Empty, (text, word) => text + " " + word);

			pdfText.ShouldContain(order.Show.HeadlineArtist.Name);
			pdfText.ShouldContain(order.Show.Venue.Name);
		}
	}
}
