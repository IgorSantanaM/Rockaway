namespace Rockaway.WebApp.Services.Mail {
	public class TicketOrderMailItem(Guid orderId, Uri websiteBaseUri) {
		public Guid OrderId { get; } = orderId;
		public Uri WebSiteBaseUri { get; } = websiteBaseUri;
	}
}
