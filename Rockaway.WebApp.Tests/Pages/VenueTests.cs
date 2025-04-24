using System.Net;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NodaTime.Testing;
using NodaTime;
using Rockaway.WebApp.Data;
using Rockaway.WebApp.Data.Sample;

namespace Rockaway.WebApp.Tests.Pages;

public class VenueTests {
	[Fact]
	public async Task Venues_Page_Contains_All_Venues() {
		await using var factory = new WebApplicationFactory<Program>().WithTestDatabase();
		var client = factory.CreateClient();
		var html = await client.GetStringAsync("/venues");
		var decodedHtml = WebUtility.HtmlDecode(html);
		using var scope = factory.Services.CreateScope();
		var db = scope.ServiceProvider.GetService<RockawayDbContext>()!;
		var expected = db.Venues.ToList();
		foreach (var venue in expected) decodedHtml.ShouldContain(venue.Name);
	}

	[Fact]
	public async Task Venue_Page_Contains_Upcoming_Shows() {
		var browsingContext = BrowsingContext.New(Configuration.Default);
		var fakeClock = new FakeClock(SampleData.TODAY.AtStartOfDayInZone(DateTimeZone.Utc).ToInstant());
		await using var factory = new WebApplicationFactory<Program>()
			.WithTestDatabase()
			.WithWebHostBuilder(builder => builder.ConfigureServices(services => {
				services.RemoveAll<IClock>();
				services.AddSingleton<IClock>(fakeClock);
			}));
		var client = factory.CreateClient();
		var venue = SampleData.Venues.Columbia;
		var html = await client.GetStringAsync($"/venue/{venue.Slug}");
		var dom = await browsingContext.OpenAsync(req => req.Content(html));
		var ticketLinks = dom.QuerySelectorAll("a.buy-tickets");
		ticketLinks.Length.ShouldBe(venue.Shows.Count);
		foreach (var show in venue.Shows) {
			ticketLinks.ShouldContain(element => element.GetAttribute("href")!.Equals(show.MakeHref()));
		}
	}
}
