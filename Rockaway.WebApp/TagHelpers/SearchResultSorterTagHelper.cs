using Microsoft.AspNetCore.Razor.TagHelpers;
using Rockaway.WebApp.Areas.Admin.Controllers;

namespace Rockaway.WebApp.TagHelpers;

public class SearchResultSorterTagHelper : TagHelper {
	public required ICanSortSearchResults Results { get; set; }
	public string OrderBy { get; set; } = default!;
	public override void Process(TagHelperContext context, TagHelperOutput output) {
		output.TagName = "a";
		output.TagMode = TagMode.StartTagAndEndTag;
		var fasClass = "fa-solid fa-sort";
		if (OrderBy == Results.OrderBy) fasClass = Results.Desc ? "fa-solid fa-sort-down" : "fa-solid fa-sort-up";
		output.Attributes.SetAttribute("class", fasClass);
		output.Attributes.SetAttribute("href", Results.Href + $"&orderby={OrderBy}&desc={!Results.Desc}");
	}
}
