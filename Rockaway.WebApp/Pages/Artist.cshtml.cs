using Rockaway.WebApp.Data;
using Rockaway.WebApp.Models;

namespace Rockaway.WebApp.Pages;

public class ArtistModel(RockawayDbContext db, IClock clock) : PageModel {
	public ArtistViewData Artist = default!;

	public IEnumerable<ShowViewData> UpcomingShows {
		get {
			var now = clock.GetCurrentInstant();
			return Artist.AllShows
				.Where(show => show.StartsAfter(now))
				.OrderBy(show => show.ShowDate);
		}
	}

	public IActionResult OnGet(string slug) {
		var artist = db.Artists
			.Include(a => a.HeadlineShows)
				.ThenInclude(show => show.Venue)
			.Include(a => a.HeadlineShows)
				.ThenInclude(show => show.SupportSlots)
				.ThenInclude(slot => slot.Artist)
			.Include(a => a.HeadlineShows)
				.ThenInclude(show => show.TicketTypes)
			.Include(a => a.SupportSlots)
				.ThenInclude(slot => slot.Show)
				.ThenInclude(show => show.Venue)
			.Include(a => a.SupportSlots)
				.ThenInclude(slot => slot.Show)
				.ThenInclude(show => show.HeadlineArtist)
			.Include(a => a.SupportSlots)
				.ThenInclude(slot => slot.Show)
				.ThenInclude(show => show.TicketTypes)
			.FirstOrDefault(a => a.Slug == slug);
		if (artist == default) return NotFound();
		Artist = new(artist);
		return Page();
	}
}