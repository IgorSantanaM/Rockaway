using Rockaway.WebApp.Data;
using Rockaway.WebApp.Data.Entities;
using Rockaway.WebApp.Models;
using Rockaway.WebApp.Services;
using Rockaway.WebApp.Services.Mail;

namespace Rockaway.WebApp.Controllers;

public class CheckoutController(
	RockawayDbContext db,
	IClock clock,
	ITicketMailer mailer) : Controller {

	[HttpPost]
	public async Task<IActionResult> Confirm(Guid id, OrderConfirmationPostData post) {
		if (id != post.TicketOrderId) return BadRequest();
		var ticketOrder = await FindTicketOrderAsync(post.TicketOrderId);
		if (ticketOrder == null) return NotFound();
		post.TicketOrder = new(ticketOrder);
		if (!ModelState.IsValid) return View(post);
		ticketOrder.CustomerEmail = post.CustomerEmail;
		ticketOrder.CustomerName = post.CustomerName;
		ticketOrder.CompletedAt = clock.GetCurrentInstant();
		await db.SaveChangesAsync();

		var mailData = new TicketOrderMailData(ticketOrder, Request.GetWebsiteBaseUri());
		await mailer.SendOrderConfirmationAsync(mailData);
		try {
			await mailer.SendOrderConfirmationAsync(mailData);
			ticketOrder.MailSentAt = clock.GetCurrentInstant();
		}
		catch(Exception ex) {
			ticketOrder.MailError = ex.Message; 
		}
		finally {
			await db.SaveChangesAsync();
		}
		return RedirectToAction(nameof(Completed), new { id = ticketOrder.Id });
	}

	[HttpGet]
	public async Task<IActionResult> Completed(Guid id) {
		var ticketOrder = await FindTicketOrderAsync(id);
		if (ticketOrder == null) return NotFound();
		var data = new TicketOrderViewData(ticketOrder);
		return View(data);
	}

	[HttpGet]
	public async Task<IActionResult> Confirm(Guid id) {
		var ticketOrder = await FindTicketOrderAsync(id);
		if (ticketOrder == default) return NotFound();
		var model = new OrderConfirmationPostData() {
			TicketOrderId = id,
			TicketOrder = new(ticketOrder)
		};
		return View(model);
	}

	private async Task<TicketOrder?> FindTicketOrderAsync(Guid id) =>
		await db.TicketOrders
			.Include(tt => tt.Tickets)
				.ThenInclude(ticket => ticket.TicketType)
			.Include(order => order.Show)
				.ThenInclude(show => show.HeadlineArtist)
			.Include(order => order.Show)
				.ThenInclude(show => show.SupportSlots)
					.ThenInclude(slot => slot.Artist)
			.Include(order => order.Show)
				.ThenInclude(show => show.Venue)
			.FirstOrDefaultAsync(order => order.Id == id);
}