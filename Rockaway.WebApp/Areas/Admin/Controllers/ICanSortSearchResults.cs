namespace Rockaway.WebApp.Areas.Admin.Controllers;

public interface ICanSortSearchResults {
	public string OrderBy { get; set; }
	public bool Desc { get; set; }
	public string Href { get; }
}
