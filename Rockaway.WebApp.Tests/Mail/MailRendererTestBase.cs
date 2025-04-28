using Rockaway.WebApp.Data.Entities;
using Rockaway.WebApp.Data.Sample;

namespace Rockaway.WebApp.Tests.Mail {
	public abstract class MailRendererTestBase {
		public static TicketOrder CreateSampleOrder() => SampleData.Shows.TestShow.CreateTestOrder(Guid.Parse("ACDC1234-0000-0000-0000-000000000000"),
			"Test Customer",
			"test.customer@example.com");
	}
}
