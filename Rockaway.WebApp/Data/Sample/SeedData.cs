using Rockaway.WebApp.Data.Entities;

namespace Rockaway.WebApp.Data.Sample;

public static class SeedData {
	public static IEnumerable<object> For(IEnumerable<Artist> artists)
		=> artists.Select(ToSeedData);

	public static IEnumerable<object> For(IEnumerable<Venue> venues)
		=> venues.Select(ToSeedData);

	public static IEnumerable<object> For(IEnumerable<Show> shows)
		=> shows.Select(ToSeedData);

	public static IEnumerable<object> For(IEnumerable<SupportSlot> supportSlots)
		=> supportSlots.Select(ToSeedData);

	static object ToSeedData(Artist artist) => new {
		artist.Id,
		artist.Name,
		artist.Description,
		artist.Slug
	};

	static object ToSeedData(Venue venue) => new {
		venue.Id,
		venue.Name,
		venue.Slug,
		venue.Address,
		venue.City,
		venue.PostalCode,
		venue.CultureName,
		venue.Telephone,
		venue.WebsiteUrl,
		venue.DateTimeZoneId
	};

	static object ToSeedData(Show show) => new {
		show.Id,
		VenueId = show.Venue.Id,
		show.Date,
		HeadlineArtistId = show.HeadlineArtist.Id,
		show.DoorsOpen
	};

	static object ToSeedData(SupportSlot slot) => new {
		ShowId = slot.Show.Id,
		slot.SlotNumber,
		ArtistId = slot.Artist.Id
	};

	public static IEnumerable<object> For(IEnumerable<TicketType> ticketTypes)
		=> ticketTypes.Select(ToSeedData);

	static object ToSeedData(TicketType tt) => new {
		tt.Id,
		ShowId = tt.Show.Id,
		tt.Price,
		tt.Name
	};

	public static IEnumerable<object> For(IEnumerable<TicketOrder> ticketOrders)
		=> ticketOrders.Select(o => new {
			o.Id,
			o.CustomerName,
			o.CustomerEmail,
			o.CreatedAt,
			o.CompletedAt,
			ShowId = o.Show.Id
		});

	public static IEnumerable<object> For(IEnumerable<Ticket> tickets)
		=> tickets.Select(item => new {
			item.Id,
			TicketOrderId = item.TicketOrder.Id,
			TicketTypeId = item.TicketType.Id,
		});
}