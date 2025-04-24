using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using NodaTime.Testing;
using Rockaway.WebApp.Controllers;
using Rockaway.WebApp.Models;

namespace Rockaway.WebApp.Tests.Controllers;

public class TicketsControllerTests {
	[Fact]
	public async Task GET_Show_Returns_View() {
		var db = TestDatabase.Create();
		var clock = new FakeClock(SystemClock.Instance.GetCurrentInstant());
		var controller = new TicketsController(db, clock);
		var show = await db
			.Shows
			.Include(show => show.Venue)
			.Include(show => show.HeadlineArtist)
			.FirstAsync();
		var result = await controller.Show(show.Venue.Slug, show.Date) as ViewResult;
		result.ShouldNotBeNull();
		var model = result.Model as ShowViewData;
		model.ShouldNotBeNull();
		model.VenueName.ShouldBe(show.Venue.Name);
		model.HeadlineArtist.Name.ShouldBe(show.HeadlineArtist.Name);
	}

	[Fact]
	public async Task POST_Show_Creates_Order() {
		var db = TestDatabase.Create();
		var clock = new FakeClock(SystemClock.Instance.GetCurrentInstant());
		var controller = new TicketsController(db, clock);
		var show = await db
			.Shows
			.Include(show => show.Venue)
			.Include(show => show.HeadlineArtist)
			.Include(show => show.TicketTypes)
			.FirstAsync();
		var tickets
			= show.TicketTypes.ToDictionary(tt => tt.Id, tt => 1);
		var result = (await controller.Show(show.Venue.Slug, show.Date, tickets)).ShouldBeOfType<RedirectToActionResult>();
		result.RouteValues.ShouldNotBeNull();

		var orderId = result.RouteValues["id"].ShouldBeAssignableTo<Guid>();
		var order = await db.TicketOrders.Include(tt => tt.Tickets).FirstOrDefaultAsync(o => o.Id == orderId);
		order.ShouldNotBeNull();
		order.Show.ShouldBe(show);
		order.Tickets.Count.ShouldBe(show.TicketTypes.Count);
	}
}