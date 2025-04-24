using System.ComponentModel;

namespace Rockaway.WebApp.Data.Entities;

public class TicketOrder {
	public Guid Id { get; set; }
	public Show Show { get; set; } = default!;
	public List<Ticket> Tickets { get; set; } = [];

	[DisplayName("Name")]
	public string CustomerName { get; set; } = String.Empty;

	[DisplayName("Email")]
	public string CustomerEmail { get; set; } = String.Empty;
	public Instant CreatedAt { get; set; }
	public Instant? CompletedAt { get; set; }

	public string FormattedTotalPrice
		=> Show.Venue.FormatPrice(Tickets.Sum(item => item.TicketType.Price));

	public string Reference => Id.ToString("D")[..8].ToUpperInvariant();

	public IList<Ticket> AddTickets(TicketType ticketType, int quantity) {
		for (var i = 0; i < quantity; i++) {
			this.Tickets.Add(new Ticket { TicketOrder = this, TicketType = ticketType });
		}
		return this.Tickets;
	}
}