using Rockaway.WebApp.Data.Entities;
using Rockaway.WebApp.Data.Sample;
using Rockaway.WebApp.Models;
using Rockaway.WebApp.Services;
using Rockaway.WebApp.Services.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rockaway.WebApp.Tests.Mail {
	public sealed class TextMailRendererTests
	{

		private static TicketOrder CreateSampleOrder() => SampleData.Shows.TestShow.CreateTestOrder(Guid.Empty,
			"Test Customer",
			"test.customer@example.com");
		[Fact]
		public void TextMailRendererRendersTextMail() {
			var order = CreateSampleOrder();
			var mailData = new TicketOrderMailData(order, new Uri("https://rockaway.dev"));
			var renderedText = new StringBuilderTextMailRenderer().RenderTextEmail(mailData);
			var expectedText = EmbeddedResource.ReadAllText("order-emai.txt", Assembly.GetExecutingAssembly());
			renderedText.ShouldBe(expectedText, StringCompareShould.IgnoreLineEndings);
		}
	}
}
