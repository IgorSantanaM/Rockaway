using Rockaway.WebApp.Areas.Admin.Models;
using Rockaway.WebApp.Data;
using Rockaway.WebApp.Data.Entities;
using Rockaway.WebApp.Models;

namespace Rockaway.WebApp.Areas.Admin.Controllers;

[Area("admin")]
public class TicketOrdersController(RockawayDbContext db) : Controller {

	public async Task<IActionResult> Index(
		int index = 0, int count = 25,
		string orderBy = "",
		bool desc = false, string search = "") {
		var query = db.TicketOrders
			.Where(t => t.CompletedAt != null)
			.Include(order => order.Show).ThenInclude(show => show.HeadlineArtist)
			.Include(order => order.Show).ThenInclude(show => show.Venue)
			.Include(order => order.Tickets).ThenInclude(item => item.TicketType)
			.Matching(search);

		query = orderBy switch {
			nameof(TicketOrder.CompletedAt) => query.OrderBy(o => o.CompletedAt),
			nameof(TicketOrder.CustomerName) => query.OrderBy(o => o.CustomerName),
			nameof(TicketOrder.Show) => query.OrderBy(o => o.Show.HeadlineArtist.Name),
			nameof(TicketOrder.Id) => query.OrderBy(o => o.Id),
			_ => query.OrderBy(o => o.CompletedAt)
		};

		if (desc) query = query.Reverse();

		var orders = await query.Skip(index).Take(count).Select(o => new TicketOrderViewData(o)).ToListAsync();
		var total = await query.CountAsync();

		var model = new TicketOrdersViewData {
			Orders = orders,
			Total = total,
			Index = index,
			Count = Math.Min(count, orders.Count),
			Search = search,
			OrderBy = orderBy,
			Desc = desc
		};
		return View(model);
	}

	public async Task<IActionResult> Details(Guid id) {
		var order = await db.TicketOrders
			.Include(o => o.Show).ThenInclude(s => s.HeadlineArtist)
			.Include(o => o.Show).ThenInclude(s => s.Venue)
			.Include(o => o.Tickets).ThenInclude(i => i.TicketType)
			.Include(o => o.Show).ThenInclude(s => s.SupportSlots).ThenInclude(slot => slot.Artist)
			.FirstOrDefaultAsync(o => o.Id == id);
		if (order == null) return NotFound();
		return View(new TicketOrderViewData(order));
	}
}

