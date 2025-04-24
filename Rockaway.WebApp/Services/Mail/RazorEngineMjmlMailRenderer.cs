using Mjml.Net;
using RazorEngine.Templating;
using Rockaway.WebApp.Models;

namespace Rockaway.WebApp.Services.Mail;

public class RazorEngineMjmlMailRenderer : IHtmlMailRenderer {
	const string TEMPLATE_KEY = "OrderConfirmationMjml";
	private readonly IRazorEngineService razor;
	private readonly IMjmlRenderer mjml;
	private readonly IMailTemplateProvider templates;
	private readonly MjmlOptions options = new();

	public RazorEngineMjmlMailRenderer(IMailTemplateProvider templates,
		IMjmlRenderer mjml,
		IRazorEngineService razor) {
		this.templates = templates;
		this.mjml = mjml;
		this.razor = razor;
	}

	private readonly string[] cssAtRules = [
		"bottom-center", "bottom-left", "bottom-left-corner", "bottom-right", "bottom-right-corner", "charset", "counter-style",
		"document", "font-face", "font-feature-values", "import", "left-bottom", "left-middle", "left-top", "keyframes", "media",
		"namespace", "page", "right-bottom", "right-middle", "right-top", "supports", "top-center", "top-left", "top-left-corner",
		"top-right", "top-right-corner"
	];

	private string EscapeCssRulesInRazorTemplate(string mjmlOutput) =>
		cssAtRules.Aggregate(mjmlOutput,
			(current, rule) => current.Replace($"@{rule}", $"@@{rule}"));

	private string EscapeCssFontWeightsInRazorTemplate(string mjmlOutput) =>
		mjmlOutput.Replace(":wght@", ":wght@@");

	public string RenderHtmlEmail(TicketOrderMailData data) {
		if (!razor.IsTemplateCached(TEMPLATE_KEY, typeof(TicketOrderMailData))) CacheTemplate();
		return razor.Run(TEMPLATE_KEY, typeof(TicketOrderMailData), data);
	}

	private void CacheTemplate() {
		var razorSource = CompileMjml();
		razor.AddTemplate(TEMPLATE_KEY, razorSource);
		razor.Compile(TEMPLATE_KEY, typeof(TicketOrderMailData));
	}

	private string CompileMjml() {
		var mjmlSource = templates.OrderConfirmationMjml;
		var (mjmlOutput, errors) = mjml.Render(mjmlSource, options);
		if (errors.Any()) throw new(errors.First().Error);
		mjmlOutput = EscapeCssRulesInRazorTemplate(mjmlOutput);
		mjmlOutput = EscapeCssFontWeightsInRazorTemplate(mjmlOutput);
		return mjmlOutput;
	}
}