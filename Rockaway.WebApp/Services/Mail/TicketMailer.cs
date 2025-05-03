using MimeKit;
using Rockaway.WebApp.Models;

namespace Rockaway.WebApp.Services.Mail {
	public class TicketMailer(IHtmlMailRenderer htmlRenderer,
		ITextMailRenderer textMailRenderer,
		IMailSender sender,
		IPdfMaker pdfMaker,
		ILogger<TicketMailer> logger) : ITicketMailer {
		public async Task SendOrderConfirmationAsync(TicketOrderMailData order) {
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress("Rockaway", "tickets@rockaway.dev"));
			message.To.Add(new MailboxAddress(order.CustomerName, order.CustomerEmail));
			message.Subject = $"Your tickets to {order.Artist.Name} at {order.VenueName}";
			var bb = new BodyBuilder {
				HtmlBody = htmlRenderer.RenderHtmlEmail(order),
				TextBody = textMailRenderer.RenderTextEmail(order)
			};

			var pdfBytes = pdfMaker.CreatePdfTickets(order);
			var fileName = $"rockaway-tickets-{order.OrderReference}.pdf";
			bb.Attachments.Add(fileName, pdfBytes, ContentType.Parse("application/pdf"));
			message.Body = bb.ToMessageBody();

			try {
				await sender.SendAsync(message);
			}
			catch (Exception ex) {
				logger.LogError(ex, "Error sending order confirmation email");
				throw;
			}
		}
	}
}