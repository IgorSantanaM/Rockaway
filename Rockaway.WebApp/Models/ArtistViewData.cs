using Rockaway.WebApp.Data.Entities;

namespace Rockaway.WebApp.Models;

public class ArtistViewData(Artist artist) {

	public string Name { get; } = artist.Name;

	public string Slug { get; } = artist.Slug;

	public string Description { get; } = artist.Description;

	public string GetImageUrl(int width, int height)
		=> $"/images/artists/{Slug}.jpg?width={width}&height={height}";

	public string CssClass => Name.Length > 20 ? "long-name" : "";

	public IEnumerable<ShowViewData> HeadlineShows => artist.HeadlineShows
		.Select(show => new ShowViewData(show));

	public IEnumerable<ShowViewData> SupportShows => artist.SupportSlots
		.Select(s => s.Show).Select(show => new ShowViewData(show) { IsSupport = true });

	public IEnumerable<ShowViewData> AllShows => HeadlineShows.Concat(SupportShows);

	public Guid Id { get; set; } = artist.Id;
}