using Microsoft.AspNetCore.Mvc;

namespace Rockaway.WebApp.Tests.Controllers;

public static class ControllerExtensions {
	public static T WithRequestUrl<T>(this T controller, string uriString) where T : ControllerBase
		=> controller.WithRequestUrl(new Uri(uriString));

	public static T WithRequestUrl<T>(this T controller, Uri uri) where T : ControllerBase {
		var httpContext = new DefaultHttpContext() {
			Request = { Scheme = uri.Scheme, Host = new(uri.Host) }
		};
		controller.ControllerContext.HttpContext = httpContext;
		return controller;
	}
}
