using Microsoft.Extensions.Logging.Abstractions;
using MimeKit;
using Mjml.Net;
using RazorEngine.Templating;
using Rockaway.WebApp.Services.Mail;

namespace Rockaway.WebApp.Tests.Mail {
	public class FakeMailSender : IMailSender {
		public IList<MimeMessage> Messages { get; } = [];
		public Task<string> SendAsync(MimeMessage message) {
			Messages.Add(message);
			return Task.FromResult("Ok");
		}
		public TicketMailer CreateTikcetMailer() {
			var htmlMailRenderer = new RazorEngineMjmlMailRenderer(new EmbeddedResourceMailTemplateProvider(), new MjmlRenderer(), RazorEngineService.Create());
			var textMailRenderer = new StringBuilderTextMailRenderer();
			var pdfMaker = new PdfMaker(new());
			return new(htmlMailRenderer,
				textMailRenderer,
				this,
				pdfMaker,
				new NullLogger<TicketMailer>());

		}
	}
}
