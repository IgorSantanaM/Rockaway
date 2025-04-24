namespace Rockaway.WebApp.Data.Entities;

public class Venue {

	public Venue() { }

	public Venue(Guid id, string name, string slug, string address, string city, string cultureName, string? postalCode, string? telephone,
		string? websiteUrl, string dateTimeZoneId) {
		Id = id;
		Name = name;
		Slug = slug;
		Address = address;
		City = city;
		CultureName = cultureName;
		PostalCode = postalCode;
		Telephone = telephone;
		WebsiteUrl = websiteUrl;
		DateTimeZoneId = dateTimeZoneId;
	}

	public Guid Id { get; set; }

	[MaxLength(100)]
	public string Name { get; set; } = String.Empty;

	[MaxLength(100)]
	[Unicode(false)]
	[RegularExpression("^[a-z0-9-]{2,100}$",
		ErrorMessage = "Slug must be 2-100 characters and can only contain a-z, 0-9 and the hyphen - character")]
	public string Slug { get; set; } = String.Empty;

	[MaxLength(500)]
	public string Address { get; set; } = String.Empty;

	public string City { get; set; } = String.Empty;

	[Unicode(false)]
	[MaxLength(16)]
	public string CultureName { get; set; } = String.Empty;

	[Unicode(false)]
	[MaxLength(64)]
	public string DateTimeZoneId { get; set; } = String.Empty;

	public string CountryCode => CultureName.Split("-").Last();

	[MaxLength(64)]
	public string? PostalCode { get; set; }

	[Phone]
	[MaxLength(64)]
	public string? Telephone { get; set; }

	[Url]
	[MaxLength(128)]
	public string? WebsiteUrl { get; set; }

	public List<Show> Shows { get; set; } = [];

	public Show BookShow(Artist artist, LocalDate date, LocalTime doorsOpen) {
		var show = new Show {
			Venue = this,
			HeadlineArtist = artist,
			Date = date,
			DoorsOpen = doorsOpen
		};
		artist.HeadlineShows.Add(show);
		Shows.Add(show);
		return show;
	}

	public CultureInfo Culture
		=> CultureInfo.GetCultures(CultureTypes.SpecificCultures)
			   .FirstOrDefault(ci => ci.Name == CultureName)
		   ??
		   CultureInfo.InvariantCulture;

	public string FormatPrice(decimal price) => price.ToString("C", Culture);

	private IEnumerable<string?> AddressTokens => [Address, City, PostalCode];
	private IEnumerable<string?> SummaryTokens => [Name, Address, City, PostalCode, Country.GetName(CountryCode)];

	public string FullAddress => String.Join(", ", AddressTokens.Where(s => !String.IsNullOrWhiteSpace(s)));
	public string Summary => String.Join(", ", SummaryTokens.Where(s => !String.IsNullOrWhiteSpace(s)));

	public DateTimeZone DateTimeZone => DateTimeZoneProviders.Tzdb[DateTimeZoneId];

	public LocalDate FindFreeDateAfter(LocalDate showDate) {
		while (this.Shows.Any(s => s.Date == showDate)) showDate = showDate.PlusDays(1 + this.Name.Length / 20);
		return showDate;
	}
}