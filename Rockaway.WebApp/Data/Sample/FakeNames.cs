using Rockaway.WebApp.Services;

namespace Rockaway.WebApp.Data.Sample;

public static class FakeNames {

	public static T AtModulo<T>(this T[] array, int index) => array[index % array.Length];

	private static readonly string[] firstNames = EmbeddedResource.ReadAllLines("firstnames.txt");
	private static readonly string[] surnames = EmbeddedResource.ReadAllLines("surnames.txt");

	private static string FirstName(Guid guid)
		=> firstNames.AtModulo(Int32.Parse(guid.ToString("N")[8..12], NumberStyles.HexNumber));

	private static string MiddleName(Guid guid)
		=> Int32.Parse(guid.ToString("N")[13..14], NumberStyles.HexNumber) % 4 == 0
			? " " + firstNames.AtModulo(Int32.Parse(guid.ToString("N")[15..18], NumberStyles.HexNumber))
			: "";

	private static string LastName(Guid guid)
		=> surnames.AtModulo(Int32.Parse(guid.ToString("N")[19..25], NumberStyles.HexNumber));

	public static string FullName(Guid guid)
		=> $"{FirstName(guid)}{MiddleName(guid)} {LastName(guid)}";

	public static string Email(Guid guid)
		=> FullName(guid).ToLower().Replace(" ", ".") + "@example.com";
}
