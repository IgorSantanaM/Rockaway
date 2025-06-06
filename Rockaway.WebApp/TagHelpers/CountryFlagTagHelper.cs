using Microsoft.AspNetCore.Razor.TagHelpers;
using Rockaway.WebApp.Data;

namespace Rockaway.WebApp.TagHelpers;

public class CountryFlagTagHelper : TagHelper {

	public string IsoCode { get; set; } = default!;
	public string Format { get; set; } = "png";

	private class FlagInfo {
		private readonly string src;
		private readonly string name;

		public FlagInfo(string code) {
			var country = Country.FromCode(code);
			src = (country?.Code ?? "unknown").ToLowerInvariant();
			name = country?.Name ?? $"unknown country {code}";
		}

		public string PngSrc => $"/img/flags/1x/{src}.png";
		public string SvgSrc => $"/img/flags/svg/{src}.svg";
		public string SrcSet => $"/img/flags/2x/{src}.png 2x,/img/flags/3x/{src}.png 3x";
		public string Alt => $"Flag of {name}";
		public string Title => name;
	}

	public override void Process(TagHelperContext context, TagHelperOutput output) {

		output.TagName = "img";
		output.TagMode = TagMode.SelfClosing;
		output.Attributes.Add("class", "country-flag");
		var info = new FlagInfo(IsoCode);
		output.Attributes.Add("title", info.Title);
		output.Attributes.Add("alt", info.Alt);
		if (Format == "svg") {
			output.Attributes.Add("src", info.SvgSrc);
			output.Attributes.Add("type", "image/svg+xml");
		} else {
			output.Attributes.Add("src", info.PngSrc);
			output.Attributes.Add("srcset", info.SrcSet);
		}
	}
}