using Rockaway.WebApp.Data.Entities;

namespace Rockaway.WebApp.Models;

public class ShowViewData(Show show) {

	public bool IsSupport { get; set; }

	public LocalDate ShowDate { get; } = show.Date;

	public string VenueName { get; } = show.Venue.Name;

	public string DoorsOpen { get; } = show.DoorsOpen.ToString("HH:mm", CultureInfo.InvariantCulture);

	public string CultureName { get; } = show.Venue.CultureName;

	public string VenueAddress { get; } = show.Venue.FullAddress;

	public ArtistViewData HeadlineArtist { get; } = new(show.HeadlineArtist);

	public string CountryCode { get; } = show.Venue.CountryCode;

	public List<ArtistViewData> SupportActs { get; }
		= show.SupportArtists.Select(a => new ArtistViewData(a)).ToList();

	public List<string> SupportActNames { get; } = show.SupportSlots
			.OrderBy(s => s.SlotNumber)
			.Select(s => s.Artist.Name).ToList();

	public List<TicketTypeViewData> TicketTypes { get; }
		= show.TicketTypes.Select(tt => new TicketTypeViewData(tt)).ToList();

	public Dictionary<string, string> RouteData { get; } = show.RouteData;

	public bool StartsAfter(Instant now) => show.StartsAfter(now);

	public List<TicketOrderViewData> TicketOrders { get; set; } = [];

}