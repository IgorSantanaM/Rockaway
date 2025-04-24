using System.Net;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NodaTime;
using NodaTime.Testing;
using Rockaway.WebApp.Data;
using Rockaway.WebApp.Data.Sample;

namespace Rockaway.WebApp.Tests.Pages;

public class ArtistTests {
	[Fact]
	public async Task Artists_Page_Contains_All_Artists() {
		await using var factory = new WebApplicationFactory<Program>().WithTestDatabase(); ;
		var client = factory.CreateClient();
		var html = await client.GetStringAsync("/artists");
		var decodedHtml = WebUtility.HtmlDecode(html);
		using var scope = factory.Services.CreateScope();
		var db = scope.ServiceProvider.GetService<RockawayDbContext>()!;
		var expected = db.Artists.ToList();
		foreach (var artist in expected) decodedHtml.ShouldContain(artist.Name);
	}

	[Fact]
	public async Task Artist_Page_Contains_Upcoming_Shows() {
		var browsingContext = BrowsingContext.New(Configuration.Default);
		var fakeClock = new FakeClock(SampleData.TODAY.AtStartOfDayInZone(DateTimeZone.Utc).ToInstant());
		await using var factory = new WebApplicationFactory<Program>()
			.WithTestDatabase()
			.WithWebHostBuilder(builder => builder.ConfigureServices(services => {
				services.RemoveAll<IClock>();
				services.AddSingleton<IClock>(fakeClock);
			}));
		var client = factory.CreateClient();
		var artist = SampleData.Artists.DevLeppard;
		var html = await client.GetStringAsync($"/artist/{artist.Slug}");
		var dom = await browsingContext.OpenAsync(req => req.Content(html));
		var ticketLinks = dom.QuerySelectorAll("a.buy-tickets");
		ticketLinks.Length.ShouldBe(artist.AllShows.Count);
		foreach (var show in artist.AllShows) {
			ticketLinks.ShouldContain(element => element.GetAttribute("href")!.Equals(show.MakeHref()));
		}
	}
}