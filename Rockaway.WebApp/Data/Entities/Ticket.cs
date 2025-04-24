namespace Rockaway.WebApp.Data.Entities;

public class Ticket {

	public Guid Id { get; set; }

	public TicketOrder TicketOrder { get; set; } = default!;

	public TicketType TicketType { get; set; } = default!;

}
