using System.ComponentModel.DataAnnotations.Schema;

namespace Rockaway.WebApp.Data.Entities;

public class Show {

	public Guid Id { get; set; }

	/// <summary>Which venue is hosting this show?</summary>
	public Venue Venue { get; set; } = default!;

	/// <summary>What date is the show on? This is local to the venue's time zone.</summary>
	public LocalDate Date { get; set; }

	/// <summary>What time do the doors open?</summary>
	public LocalTime DoorsOpen { get; set; }

	public Artist HeadlineArtist { get; set; } = default!;

	public List<SupportSlot> SupportSlots { get; set; } = [];

	public List<TicketType> TicketTypes { get; set; } = [];

	public List<TicketOrder> TicketOrders { get; set; } = [];

	public int NextSupportSlotNumber
		=> (this.SupportSlots.Count > 0 ? this.SupportSlots.Max(s => s.SlotNumber) : 0) + 1;

	public Dictionary<string, string> RouteData => new() {
		{ "venue", this.Venue.Slug },
		{ "date", this.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) }
	};

	[NotMapped]
	public IEnumerable<Artist> SupportArtists
		=> this.SupportSlots.OrderBy(s => s.SlotNumber).Select(s => s.Artist);

	public TicketOrder CreateOrder(Dictionary<Guid, int> contents, Instant now) {
		var order = new TicketOrder {
			Show = this,
			CreatedAt = now
		};
		foreach (var (id, quantity) in contents) {
			var ticketType = this.TicketTypes.FirstOrDefault(tt => tt.Id == id);
			if (ticketType == default) continue;
			order.AddTickets(ticketType, quantity);
		}
		this.TicketOrders.Add(order);
		return order;
	}

	public bool StartsAfter(Instant now) {
		var showStart = this.Date.AtStartOfDayInZone(this.Venue.DateTimeZone).PlusHours(24).ToInstant();
		return showStart > now;
	}

	public SupportSlot AddSupportAct(Artist artist) {
		var slot = new SupportSlot() {
			Show = this,
			Artist = artist,
			SlotNumber = this.NextSupportSlotNumber
		};
		artist.SupportSlots.Add(slot);
		this.SupportSlots.Add(slot);
		return slot;
	}
}
