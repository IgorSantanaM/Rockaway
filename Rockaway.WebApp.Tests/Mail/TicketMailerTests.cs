using MimeKit;
using Rockaway.WebApp.Data.Entities;
using Rockaway.WebApp.Data.Sample;
using Rockaway.WebApp.Models;
using Rockaway.WebApp.Services.Mail;

namespace Rockaway.WebApp.Tests.Mail;

public class TicketMailerTests {

	private readonly FakeMailSender sender = new();
	private readonly TicketMailer mailer;

	public TicketMailerTests()
		=> this.mailer = sender.CreateTikcetMailer();

	private static readonly Guid testOrderId
		= Guid.Parse("ACDC1234-0000-0000-0000-000000000000");

	private async Task<(MimeMessage, TicketOrder)> SendOrderConfirmationAsync(
		string name = "Test Customer",
		string email = "test.customer@example.com"
	) {
		sender.Messages.ShouldBeEmpty();
		var order = SampleData.Shows.TestShow.CreateTestOrder(testOrderId, name, email);
		var data = new TicketOrderMailData(order, new("https://rockaway.dev"));
		await mailer.SendOrderConfirmationAsync(data);
		return (sender.Messages.Single(), order);
	}

	[Fact]
	public async Task TicketMailer_Sends_Ticket_Email_With_Correct_Subject() {
		var (mail, order) = await SendOrderConfirmationAsync();
		var expectedSubject
			= $"Your tickets to {order.Show.HeadlineArtist.Name} at {order.Show.Venue.Name}";
		mail.Subject.ShouldBe(expectedSubject);
	}

	[Fact]
	public async Task TicketMailer_Sends_Ticket_Email_From_Correct_Mailbox() {
		var (mail, _) = await SendOrderConfirmationAsync();
		var from = mail.From.Single().ShouldBeOfType<MailboxAddress>();
		from.Name.ShouldBe("Rockaway");
		from.Address.ShouldBe("tickets@rockaway.dev");
	}

	[Fact]
	public async Task TicketMailer_Sends_Ticket_Email_To_Correct_Mailbox() {
		var name = Guid.NewGuid().ToString();
		var email = $"{name}@example.com";
		var (mail, _) = await SendOrderConfirmationAsync(name, email);
		var to = mail.To.Single().ShouldBeOfType<MailboxAddress>();
		to.Name.ShouldBe(name);
		to.Address.ShouldBe(email);
	}

	[Fact]
	public async Task TicketMailer_Sends_Ticket_Email_With_Html_Body() {
		var (mail, order) = await SendOrderConfirmationAsync();
		mail.HtmlBody.ShouldContain(order.Show.HeadlineArtist.Name);
	}

	[Fact]
	public async Task TicketMailer_Sends_Ticket_Email_With_Text_Body() {
		var (mail, order) = await SendOrderConfirmationAsync();
		mail.TextBody.ShouldContain(order.Show.HeadlineArtist.Name);
	}

	[Fact]
	public async Task TicketMailer_Sends_Ticket_Email_With_Pdf_Attachment() {
		var (mail, order) = await SendOrderConfirmationAsync();
		var expectedFilename = $"rockaway-tickets-{order.Reference}.pdf";
		mail.Attachments.Count().ShouldBe(1);
		var attachment = mail.Attachments.Single().ShouldBeOfType<MimePart>();
		attachment.ContentType.MimeType.ShouldBe("application/pdf");
		attachment.FileName.ShouldBe(expectedFilename);
	}
}
