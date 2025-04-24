using Rockaway.WebApp.Data.Entities;

namespace Rockaway.WebApp.Data.Sample;

public static partial class SampleData {
	public static class TicketOrders {
		public static IEnumerable<TicketOrder> CreateSampleTicketOrders(IList<Show> shows, int count) {
			for (var i = 0; i < count; i++) {
				var show = shows[i % shows.Count];
				var id = NextId;
				yield return show.CreateTestOrder(id, FakeNames.FullName(id), FakeNames.Email(id));
			}
		}

		public static IEnumerable<Ticket> ExtractTickets(IEnumerable<TicketOrder> orders)
			=> orders.SelectMany(order => order.Tickets);
	}

	public static TicketOrder CreateTestOrder(this Show show, Guid id, string name, string email) {
		var quantities = show.TicketTypes.ToDictionary(tt => tt.Id, tt => 1 + (name.Length + tt.Name.Length) % 5);
		var howLongAgoWereTicketsPurchased = id.ToDuration(Duration.FromDays(90));
		var timeTakenToPurchaseTickets = id.ToDuration(Duration.FromMinutes(30)).Plus(Duration.FromMinutes(1));
		var completedAt = TODAY.AtMidnight().InUtc().Minus(howLongAgoWereTicketsPurchased).ToInstant();
		var createdAt = completedAt.Minus(timeTakenToPurchaseTickets);
		var o = show.CreateOrder(quantities, createdAt);
		o.CustomerEmail = email;
		o.CustomerName = name;
		o.CompletedAt = completedAt;
		o.Id = id;
		// Assign GUIDs so they stay the same when we rebuild the database
		foreach (var ticket in o.Tickets) ticket.Id = NextId;
		return o;
	}
}