using Rockaway.WebApp.Data.Entities;

namespace Rockaway.WebApp.Models;

public class TicketOrderViewData(TicketOrder ticketOrder) {
	public Guid Id { get; set; } = ticketOrder.Id;
	public string Headliner { get; } = ticketOrder.Show.HeadlineArtist.Name;
	public string VenueSummary { get; } = ticketOrder.Show.Venue.Summary;
	public string? VenueTelephone { get; } = ticketOrder.Show.Venue.Telephone;
	public string? VenueWebsiteUrl { get; } = ticketOrder.Show.Venue.WebsiteUrl;
	public string? VenueFullAddress { get; } = ticketOrder.Show.Venue.FullAddress;

	public string VenueName { get; } = ticketOrder.Show.Venue.Name;
	public LocalDate ShowDate { get; } = ticketOrder.Show.Date;
	public bool HasSupport { get; } = ticketOrder.Show.SupportSlots.Count > 0;
	public string CustomerName { get; } = ticketOrder.CustomerName;
	public string CustomerEmail { get; } = ticketOrder.CustomerEmail;

	public ZonedDateTime? OrderCompletedAt { get; } = ticketOrder.CompletedAt?.InZone(DateTimeZone.Utc);

	public string FormattedOrderCompletedAt
		=> OrderCompletedAt?.ToString("ddd dd MMM yyyy HH:mm", CultureInfo.InvariantCulture) ?? "(not yet)";

	public Instant OrderCreatedAt { get; } = ticketOrder.CreatedAt;
	public ArtistViewData Artist { get; } = new(ticketOrder.Show.HeadlineArtist);
	public string Reference { get; } = ticketOrder.Reference;

	public string SupportArtistsText { get; }
		= ticketOrder.Show.SupportArtists.Any()
			? String.Join(" + ", ticketOrder.Show.SupportArtists.Select(a => a.Name))
			: "(no support)";

	public IEnumerable<TicketOrderItemViewData> Contents { get; }
		= ticketOrder.Tickets
			.GroupBy(ticket => ticket.TicketType)
			.Select(group => new TicketOrderItemViewData(group.Key, group.Count()));

	public IEnumerable<TicketViewData> Tickets { get; }
		= ticketOrder.Tickets.Select(item => new TicketViewData(item));

	public string FormattedTotalPrice { get; } = ticketOrder.FormattedTotalPrice;
	public string OrderReference { get; set; } = ticketOrder.Reference;
	public Guid OrderId { get; set; } = ticketOrder.Id;

	public LocalTime DoorsOpen => ticketOrder.Show.DoorsOpen;

	public override string ToString()
	=> $"{CustomerName} ({CustomerEmail}) ({Headliner}, {VenueName}, {ShowDate}, {FormattedTotalPrice})";
}