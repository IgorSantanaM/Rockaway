using Rockaway.WebApp.Data;
using Rockaway.WebApp.Models;

namespace Rockaway.WebApp.Pages;

public class ShowsModel(RockawayDbContext db, IClock clock) : PageModel {
	public IEnumerable<ShowViewData> Shows = default!;
	public int Count { get; set; } = 24;
	public int Index { get; set; } = 0;

	public LocalDate Today = LocalDate.FromDateTime(clock.GetCurrentInstant().ToDateTimeUtc());

	public void OnGet() => Shows = db.Shows
		.Include(s => s.Venue)
		.Include(show => show.HeadlineArtist)
		.Include(show => show.SupportSlots).ThenInclude(slot => slot.Artist)
		.Include(show => show.TicketTypes)
		.Where(s => s.Date >= Today)
		.OrderBy(s => s.Date)
		.Skip(Index)
		.Take(Count)
		.Select(s => new ShowViewData(s))
		.ToList();
}