using Rockaway.WebApp.Areas.Admin.Models;
using Rockaway.WebApp.Data;
using Rockaway.WebApp.Data.Entities;
using Rockaway.WebApp.Models;
using Rockaway.WebApp.Services.Mail;
using Rockaway.WebApp.Services;

namespace Rockaway.WebApp.Areas.Admin.Controllers;

[Area("admin")]
public class TicketOrdersController(
	RockawayDbContext db,
	ITicketMailer ticketMailer,
	IClock clock) : Controller {

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
	public async Task<IActionResult> ResendFailed() {
		var orders = await db.TicketOrders.Where(o => o.MailError != null)
			.Take(10)
			.ToListAsync();
		return View(orders);
	}

	public async Task<IActionResult> Resend(Guid orderId) {
		var order = await db.TicketOrders
			.Include(o => o.Show).ThenInclude(s => s.HeadlineArtist)
			.Include(o => o.Show).ThenInclude(s => s.Venue)
			.Include(o => o.Tickets).ThenInclude(i => i.TicketType)
			.Include(o => o.Show).ThenInclude(s => s.SupportSlots).ThenInclude(slot => slot.Artist)
			.FirstOrDefaultAsync(o => o.Id == orderId);
		if (order == null) return NotFound();

		var data = new TicketOrderMailData(order, Request.GetWebsiteBaseUri());

		try {
			await ticketMailer.SendOrderConfirmationAsync(data);
			order.MailError = null;
			order.MailSentAt = clock.GetCurrentInstant();
		}
		catch (Exception ex) {
			order.MailSentAt = null;
			order.MailError = ex.Message;
		}
		finally {
			await db.SaveChangesAsync();
		}

		return RedirectToAction(nameof(Details), new { id = orderId });
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

