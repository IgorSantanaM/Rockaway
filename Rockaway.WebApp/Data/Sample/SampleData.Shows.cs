using Rockaway.WebApp.Data.Entities;
// ReSharper disable InconsistentNaming

namespace Rockaway.WebApp.Data.Sample;

public static partial class SampleData {

	// Set this to today's date to generate realistic sample data for past and future shows.
	public static readonly LocalDate TODAY = new(2025, 2, 14);
	public static readonly Instant NOW = TODAY.At(new(19, 00)).InUtc().ToInstant();

	public static Show WithTicketTypes(this Show show, IEnumerable<(string name, decimal price, int? limit)> ticketTypeInfo) {
		foreach (var (name, price, limit) in ticketTypeInfo) {
			var adjustedPrice = show.HeadlineArtist.AdjustPrice(price);
			show.TicketTypes.Add(new(NextId, show, name, adjustedPrice, limit));
		}
		return show;
	}

	public static Show WithTicketType(this Show show, Guid id, string name, decimal price, int? limit = null) {
		show.TicketTypes.Add(new(id, show, name, price, limit));
		return show;
	}

	public static Show WithSupportActs(this Show show, params Artist[] artists) {
		foreach(var artist in artists) show.AddSupportAct(artist);
		return show;
	}

	public static class Shows {

		private static readonly List<Show> shows = [];

		public static Show TestShow => shows.First();

		static Shows() {
			shows.AddRange(Artists.Coda.CreateSampleTour(TODAY.PlusDays(3), [Artists.BodyBag, Artists.AlterColumn],
				Venues.NewCrossInn, Venues.Bataclan, Venues.Columbia, Venues.Gagarin, Venues.Barracuda, Venues.JohnDee, Venues.PubAnchor));

			shows.AddRange(Artists.DevLeppard.CreateSampleTour(TODAY.PlusDays(2), [Artists.Elektronika, Artists.HaskellsAngels],
				Venues.Gagarin, Venues.Barracuda, Venues.JohnDee, Venues.PubAnchor, Venues.NewCrossInn, Venues.Bataclan, Venues.Columbia));

			shows.AddRange(Artists.ForEarTransform.CreateSampleTour(TODAY.PlusDays(1), [Artists.GarbageCollectors],
				Venues.JohnDee, Venues.PubAnchor, Venues.NewCrossInn, Venues.Gagarin, Venues.Barracuda, Venues.Bataclan, Venues.Columbia));

			shows.AddRange(Artists.JavasCrypt.CreateSampleTour(TODAY.PlusDays(14), [Artists.IronMedian, Artists.Xslte],
				Venues.JohnDee, Venues.PubAnchor, Venues.NewCrossInn, Venues.Bataclan, Venues.Columbia));

			shows.AddRange(Artists.KillerBite.CreateSampleTour(TODAY.PlusDays(22), [],
				Venues.JohnDee, Venues.PubAnchor, Venues.NewCrossInn, Venues.Gagarin, Venues.Barracuda, Venues.Bataclan, Venues.Columbia));

			shows.AddRange(Artists.LambdaOfGod.CreateSampleTour(TODAY.PlusDays(25), [Artists.MottTheTuple, Artists.Overflow, Artists.Yamb],
				Venues.Bataclan, Venues.Columbia, Venues.JohnDee, Venues.PubAnchor, Venues.NewCrossInn, Venues.Electric, Venues.Gagarin, Venues.Barracuda));

			shows.AddRange(Artists.NullTerminatedStringBand.CreateSampleTour(TODAY.PlusDays(19), [Artists.ZeroBasedIndex],
				Venues.PubAnchor, Venues.JohnDee, Venues.NewCrossInn, Venues.Electric, Venues.Gagarin, Venues.Barracuda, Venues.Bataclan, Venues.Columbia));

			shows.AddRange(Artists.PascalsWager.CreateSampleTour(TODAY.PlusDays(33), [Artists.QuantumGate, Artists.RunCmd, Artists.ScriptKiddies],
				Venues.Gagarin, Venues.Barracuda, Venues.Bataclan, Venues.Columbia, Venues.PubAnchor, Venues.JohnDee, Venues.NewCrossInn, Venues.Electric));

			shows.AddRange(Artists.Terrorform.CreateSampleTour(TODAY.PlusDays(47), [Artists.Unicoder],
				Venues.Columbia, Venues.PubAnchor, Venues.JohnDee, Venues.Gagarin, Venues.Barracuda, Venues.Bataclan, Venues.NewCrossInn, Venues.Electric));

			shows.AddRange(Artists.Ærbårn.CreateSampleTour(TODAY.PlusDays(49), [Artists.WebmasterOfPuppets],
				Venues.Columbia, Venues.Gagarin, Venues.PubAnchor, Venues.JohnDee, Venues.NewCrossInn, Venues.Electric, Venues.Barracuda, Venues.Bataclan));

			shows.AddRange(Artists.SilverMountainStringBand.CreateSampleTour(TODAY.PlusDays(60),
				[Artists.ZeroBasedIndex, Artists.VirtualMachine],
				Venues.Gagarin, Venues.Barracuda, Venues.Bataclan, Venues.Columbia, Venues.NewCrossInn, Venues.Electric, Venues.PubAnchor, Venues.JohnDee));

		}

		public static IEnumerable<Show> AllShows => shows;

		public static IEnumerable<TicketType> AllTicketTypes
			=> shows.SelectMany(show => show.TicketTypes);

		public static IEnumerable<SupportSlot> AllSupportSlots
			=> shows.SelectMany(show => show.SupportSlots);
	}
}

