using Rockaway.WebApp.Data;
using Rockaway.WebApp.Data.Entities;
using Rockaway.WebApp.Models;
using Rockaway.WebApp.Services;
using Rockaway.WebApp.Services.Mail;

namespace Rockaway.WebApp.Areas.Admin.Controllers {
	[Area("admin")]
	public class EmailsController(
		RockawayDbContext db,
		ITextMailRenderer textMailRenderer,
		IHtmlMailRenderer htmlMailRenderer) : Controller {

		public async Task<IActionResult> Text(Guid id) {
			var order = await FindTicketOrder(id);
			if (order == null) return NotFound();
			var data = new TicketOrderViewData(order);
			var text = textMailRenderer.RenderTextEmail(data);
			return Content(text, "text/plain", Encoding.UTF8);
		}

		public async Task<IActionResult> Html(Guid id) {
			var order = await FindTicketOrder(id);
			if (order == null) return NotFound();
			var data = new TicketOrderMailData(order, Request.GetWebsiteBaseUri());
			var html = htmlMailRenderer.RenderHtmlEmail(data);
			return Content(html, "text/html", Encoding.UTF8);
		}

		private async Task<TicketOrder?> FindTicketOrder(Guid id) {
			var order = await db.TicketOrders
				.Include(o => o.Show).ThenInclude(s => s.HeadlineArtist)
				.Include(o => o.Show).ThenInclude(s => s.Venue)
				.Include(o => o.Show).ThenInclude(s => s.SupportSlots).ThenInclude(slot => slot.Artist)
				.Include(o => o.Tickets).ThenInclude(t => t.TicketType)
				.FirstOrDefaultAsync(o => o.Id == id);
			return order;
		}
	}
}
