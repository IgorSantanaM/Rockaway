using Rockaway.WebApp.Data;
using Rockaway.WebApp.Data.Entities;
using Rockaway.WebApp.Models;

namespace Rockaway.WebApp.Pages {
	public class VenueModel(RockawayDbContext db, IClock clock) : PageModel {

		internal IEnumerable<ShowViewData> Shows => Venue.Shows
			.Where(s => s.StartsAfter(clock.GetCurrentInstant()))
			.Select(s => new ShowViewData(s))
			.OrderBy(s => s.ShowDate);

		internal Venue Venue { get; set; } = new();

		public async Task<IActionResult> OnGet(string slug) {
			var venue = await db.Venues
				.Include(v => v.Shows)
				.ThenInclude(show => show.HeadlineArtist)
				.Include(v => v.Shows)
				.ThenInclude(show => show.SupportSlots)
				.ThenInclude(slot => slot.Artist)
				.Include(v => v.Shows)
				.ThenInclude(show => show.TicketTypes)
				.FirstOrDefaultAsync(v => v.Slug == slug);
			if (venue == default) return NotFound();
			Venue = venue;
			return Page();
		}
	}
}
