using Rockaway.WebApp.Models;

namespace Rockaway.WebApp.Areas.Admin.Models;

public class ShowListViewData(IList<IGrouping<LocalDate, ShowViewData>> dates, Instant now) {

	public IList<IGrouping<LocalDate, ShowViewData>> Dates { get; } = dates;

	public IList<LocalDate> Calendar { get; } = BuildCalendar(dates, now).ToList();

	public List<ShowViewData> GetShowsFor(LocalDate date) => Dates.FirstOrDefault(d => d.Key == date)?.ToList() ?? [];

	public static IEnumerable<LocalDate> BuildCalendar(IList<IGrouping<LocalDate, ShowViewData>> dates, Instant now) {
		var today = now.InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault()).Date;
		var first = dates.Min(d => d.Key);
		var last = dates.Max(d => d.Key);
		first = LocalDate.Min(first, today);
		while (first.DayOfWeek != IsoDayOfWeek.Monday) first = first.PlusDays(-1);
		while (last.DayOfWeek != IsoDayOfWeek.Sunday) last = last.PlusDays(1);
		while (first <= last) {
			yield return first;
			first = first.PlusDays(1);
		}
	}
}
