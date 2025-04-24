using Rockaway.WebApp.Data.Entities;
using Rockaway.WebApp.Services;

namespace Rockaway.WebApp.Data.Sample;

public static partial class SampleData {

	public static IEnumerable<T> PickRandom<T>(this T[] items, int filter)
		=> items.Where((_, index) => (filter & (1 << index)) != 0);

	private static readonly Guid[] sampleGuids
		= EmbeddedResource.ReadAllLines("guids.txt").Select(Guid.Parse).ToArray();

	private static int guidIndex = 0;

	public static Guid NextId => (guidIndex < sampleGuids.Length ? sampleGuids[guidIndex++] : Guid.NewGuid());

	public static decimal AdjustPrice(this Artist artist, decimal price) {
		var adjustment = Math.Floor(price * (artist.Name.Length / 20m)) / 2m;
		return price + adjustment;
	}

	public static IEnumerable<Show> CreateSampleTour(this Artist artist, LocalDate startingDate, Artist[] supportActs,
		params Venue[] venues) {
		var showDate = startingDate;
		foreach (var venue in venues) {
			showDate = showDate.PlusDays(1 + venue.Name.Length / 16);
			showDate = venue.FindFreeDateAfter(showDate);
			var support = supportActs.PickRandom((int) showDate.DayOfWeek).ToArray();
			var doors = new LocalTime(19,0).PlusMinutes(15 * artist.Name.Length % 5);
			var show = venue.BookShow(artist, showDate, doors)
				.WithSupportActs(support)
				.WithTicketTypes(Venues.CreateSampleTicketTypes(venue));
			show.Id = NextId;
			yield return show;
		}
	}

	public static Duration ToDuration(this Guid guid, Duration range) {
		var factor = (uint) guid.GetHashCode() / (double) UInt32.MaxValue;
		var adjustedRange = range.TotalMilliseconds * factor;
		return Duration.FromMilliseconds(adjustedRange);
	}
}