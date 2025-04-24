using Rockaway.WebApp.Services;
using Rockaway.WebApp.Services.Mail;

public class EmbeddedResourceMailTemplateProvider : IMailTemplateProvider {
	public string OrderConfirmationMjml => EmbeddedResource.ReadAllText("OrderConfirmation.csmjml");
}

