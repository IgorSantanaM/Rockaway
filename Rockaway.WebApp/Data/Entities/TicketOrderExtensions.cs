namespace Rockaway.WebApp.Data.Entities;

public static class TicketOrderExtensions {
	public static IQueryable<TicketOrder> Matching(this IQueryable<TicketOrder> orders, string searchText)
		=> String.IsNullOrEmpty(searchText) ? orders :
			orders.Where(o => o.CustomerName.Contains(searchText)
			                  ||
			                  o.CustomerEmail.Contains(searchText)
			                  ||
			                  o.Id.ToString().Contains(searchText));
}