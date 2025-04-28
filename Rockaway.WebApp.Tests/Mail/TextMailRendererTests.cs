using Rockaway.WebApp.Models;
using Rockaway.WebApp.Services;
using Rockaway.WebApp.Services.Mail;
using System.Reflection;

namespace Rockaway.WebApp.Tests.Mail {
	public sealed class TextMailRendererTests : MailRendererTestBase {
		[Fact]
		public void TextMailRendererRendersTextMail() {
			var order = CreateSampleOrder();
			var mailData = new TicketOrderMailData(order, new Uri("https://rockaway.dev"));
			var renderedText = new StringBuilderTextMailRenderer().RenderTextEmail(mailData);
			var expectedText = EmbeddedResource.ReadAllText("order-email.txt", Assembly.GetExecutingAssembly());
			renderedText.ShouldBe(expectedText, StringCompareShould.IgnoreLineEndings);
		}
	}
}	