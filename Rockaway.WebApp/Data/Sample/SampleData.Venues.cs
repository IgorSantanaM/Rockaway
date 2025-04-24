using Rockaway.WebApp.Data.Entities;

namespace Rockaway.WebApp.Data.Sample;

public static partial class SampleData {

	public static class Venues {
		public const string ELECTRIC = "electric-brixton";
		public const string BATACLAN = "bataclan-paris";
		public const string COLUMBIA = "columbia-berlin";
		public const string GAGARIN = "gagarin-athens";
		public const string JOHN_DEE = "john-dee-oslo";
		public const string STENGADE = "stengade-cph";
		public const string BARRACUDA = "barracuda-porto";
		public const string PUB_ANCHOR = "pub-anchor-stockholm";
		public const string NEW_CROSS_INN = "new-cross-inn-london";

		public static IEnumerable<(string name, decimal price, int? limit)> CreateSampleTicketTypes(Venue venue) {
			switch (venue.Slug) {
				case ELECTRIC:
					yield return ("Upstairs Seated", 22.50m, 60);
					yield return ("Standing", 27.50m, 100);
					break;
				case NEW_CROSS_INN:
					yield return ("Standing", 27.50m, 100);
					break;
				case BATACLAN:
				case COLUMBIA:
				case GAGARIN:
				case BARRACUDA:
					yield return ("General Admission", 34.50m, null);
					yield return ("VIP Meet & Greet", 74.50m, null);
					break;
				case JOHN_DEE:
				case PUB_ANCHOR:
					yield return ("General Admission", 280m, null);
					yield return ("VIP Meet & Greet", 550m, null);
					break;
			}
		}

		public static Venue Electric = new(NextId, "Electric Brixton", ELECTRIC, "Town Hall Parade", "London", "en-GB", "SW2 1RJ",
			"020 7274 2290", "https://www.electricbrixton.uk.com/", "Europe/London");

		public static Venue Bataclan = new(NextId, "Bataclan", BATACLAN, "50 Boulevard Voltaire", "Paris", "fr-FR", "75011",
			"+33 1 43 14 00 30", "https://www.bataclan.fr/", "Europe/Paris");

		public static Venue Columbia = new(NextId, "Columbia Theatre", COLUMBIA, "Columbiadamm 9 - 11", "Berlin", "de-DE", "10965",
			"+49 30 69817584", "https://columbia-theater.de/", "Europe/Berlin");

		public static Venue Gagarin = new(NextId, "Gagarin 205", GAGARIN, "Liosion 205", "Athens", "el-GR", "104 45",
			"+45 35 35 50 69", "", "Europe/Athens");

		public static Venue JohnDee = new(NextId, "John Dee", JOHN_DEE, "Torggata 16", "Oslo", "nn-NO", "0181",
			"+47 22 20 32 32", "https://www.rockefeller.no/", "Europe/Oslo");

		public static Venue Stengade = new(NextId, "Stengade", STENGADE, "Stengade 18", "Copenhagen", "da-DK", "2200",
			"+45 35355069", "https://www.stengade.dk", "Europe/Copenhagen");

		public static Venue Barracuda = new(NextId, "Barracuda", BARRACUDA, "R da Madeira 186", "Porto", "pt-PT", "4000-433", null,
			null, "Europe/Lisbon");

		public static Venue PubAnchor = new(NextId, "Pub Anchor", PUB_ANCHOR, "Sveavägen 90", "Stockholm", "sv-SE", "113 59",
			"+46 8 15 20 00", "https://www.instagram.com/pubanchor/?hl=en", "Europe/Stockholm");

		public static Venue NewCrossInn = new(NextId, "New Cross Inn", NEW_CROSS_INN, "323 New Cross Road", "London", "en-GB", "SE14 6AS",
			"+44 20 8469 4382", "https://www.newcrossinn.com/", "Europe/London");

		public static Venue[] AllVenues => [
			Electric, Bataclan, Columbia, Gagarin, JohnDee, Stengade, Barracuda, PubAnchor, NewCrossInn
		];
	}
}