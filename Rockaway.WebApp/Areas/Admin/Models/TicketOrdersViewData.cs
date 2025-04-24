using System.Web;
using Rockaway.WebApp.Areas.Admin.Controllers;
using Rockaway.WebApp.Models;

namespace Rockaway.WebApp.Areas.Admin.Models;

public class TicketOrdersViewData : ICanSortSearchResults {
	public List<TicketOrderViewData> Orders { get; set; } = [];
	public int Total { get; set; }
	public int Count { get; set; }
	public int Index { get; set; }
	public string Search { get; set; } = String.Empty;
	public string OrderBy { get; set; } = nameof(TicketOrderViewData.OrderCreatedAt);
	public bool Desc { get; set; }
	public string Href => "?" + (String.IsNullOrEmpty(Search) ? "" : $"search={HttpUtility.UrlEncode(Search)}");
}