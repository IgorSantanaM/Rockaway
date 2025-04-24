using System.Globalization;
using Rockaway.WebApp.Data.Entities;

namespace Rockaway.WebApp.Tests.Pages;

public static class EntityExtensions {
	public static string MakeHref(this Show show)
		=> $"/show/{show.Venue.Slug}/{show.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";
}
