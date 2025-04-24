using Rockaway.WebApp.Areas.Admin.Models;
using Rockaway.WebApp.Data;
using Rockaway.WebApp.Models;

namespace Rockaway.WebApp.Areas.Admin.Controllers;

[Area("admin")]
public class ShowsController(RockawayDbContext context, IClock clock) : Controller {
	public async Task<IActionResult> Index() {
		var shows = await context.Shows
			.Include(show => show.HeadlineArtist)
			.Include(show => show.Venue)
			.Include(show => show.SupportSlots).ThenInclude(slot => slot.Artist)
			.Include(show => show.TicketOrders).ThenInclude(order => order.Tickets).ThenInclude(item => item.TicketType)
			.OrderBy(show => show.Date).ThenBy(show => show.Venue.Name)
			.ToListAsync();
		var dates = shows.Select(s => new ShowViewData(s)).GroupBy(show => show.ShowDate).ToList();
		var model = new ShowListViewData(dates, clock.GetCurrentInstant());
		return View(model);
	}

	public async Task<IActionResult> View(string venue, LocalDate date) {
		var show = await context.Shows
			.Include(s => s.HeadlineArtist)
			.Include(s => s.Venue)
			.Include(s => s.SupportSlots).ThenInclude(ss => ss.Artist)
			.Include(s => s.TicketTypes)
			.Include(s => s.TicketOrders).ThenInclude(to => to.Tickets).ThenInclude(toi => toi.TicketType)
			.FirstOrDefaultAsync(s => s.Venue.Slug == venue && s.Date == date);
		if (show == null) return NotFound();
		var model = new ShowViewData(show) {
			TicketOrders = show.TicketOrders.Select(to => new TicketOrderViewData(to)).ToList()
		};
		return View(model);
	}
}