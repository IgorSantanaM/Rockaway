namespace Rockaway.WebApp.Hosting;

public static class HostEnvironmentExtensions {

	static HostEnvironmentExtensions() {
		XUnitAssemblyLoaded = AppDomain.CurrentDomain.GetAssemblies()
			.Any(a => a.FullName?.Contains("xUnit", StringComparison.OrdinalIgnoreCase) ?? false);
	}

	private static bool XUnitAssemblyLoaded { get; }

	private const string UNIT_TEST_ENVIRONMENT = "UnitTest";

	public static bool IsUnitTest(this IHostEnvironment env)
		=> XUnitAssemblyLoaded || env.EnvironmentName == UNIT_TEST_ENVIRONMENT;
}