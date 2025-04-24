using Rockaway.WebApp.Data.Entities;

namespace Rockaway.WebApp.Models;

public class TicketOrderItemViewData(TicketType ticketType, int quantity) {
	public int Quantity { get; } = quantity;
	public string Description { get; } = ticketType.Name;
	public string QuantityAndType { get; } = $"{quantity} x {ticketType.Name}";
	public string UnitPrice { get; } = ticketType.FormattedPrice;
	public string TotalPrice { get; } = $"{ticketType.FormatPrice(quantity)}";
}

public class TicketViewData(Ticket ticket) {
	public Guid TicketId { get; } = ticket.Id;
	public string Description { get; } = ticket.TicketType.Name;
	public string Price { get; } = ticket.TicketType.FormattedPrice;
}
