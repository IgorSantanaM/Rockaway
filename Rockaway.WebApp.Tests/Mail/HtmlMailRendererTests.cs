using MailKit.Security;
using Mjml.Net;
using RazorEngine.Templating;
using Rockaway.WebApp.Data.Entities;
using Rockaway.WebApp.Data.Sample;
using Rockaway.WebApp.Models;
using Rockaway.WebApp.Services.Mail;

namespace Rockaway.WebApp.Tests.Mail {
	public sealed class HtmlMailRendererTests : MailRendererTestBase {
		
		[Fact]
		public async Task HtmlMailRendererRenderHtmlMail() {
			var order = CreateSampleOrder();
			var mailData = new TicketOrderMailData(order, new Uri("https://rockaway.dev"));

			var renderer = new RazorEngineMjmlMailRenderer(
					new EmbeddedResourceMailTemplateProvider(),
					new MjmlRenderer(),
					RazorEngineService.Create());

			var renderedHtml = renderer.RenderHtmlEmail(mailData);
			var browsingContext = BrowsingContext.New(Configuration.Default);
			// Use AngleSharp to get value of the orderRefence and the artist name using the IDs
			var dom = await browsingContext.OpenAsync(req => req.Content(renderedHtml));
			var orderReference = dom.QuerySelector("#rockaway-order-reference");
			orderReference.ShouldNotBeNull();
			orderReference.InnerHtml.ShouldBe(order.Reference);

			var artistName = dom.QuerySelector("#rockaway-artist-name");
			artistName.ShouldNotBeNull();
			artistName.InnerHtml.ShouldBe(order.Show.HeadlineArtist.Name);
		}
	}
}
