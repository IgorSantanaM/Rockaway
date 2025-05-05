using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Rockaway.WebApp.Services;
using Rockaway.WebApp.Services.Mail;

namespace Rockaway.WebApp.Areas.Admin.Hubs {
	[Authorize]
	public class EmailHub(IMailQueue mailQueue) : Hub
	{
		public async Task<string> QueueEmail(string user, string orderId) {
			var ctx = this.Context.GetHttpContext();
			var websiteBaseUri = ctx?.Request.GetWebsiteBaseUri() ?? throw new Exception("The request must come from an HTTP request");
			var id = Guid.Parse(orderId);
			var item = new TicketOrderMailItem(id, websiteBaseUri);
			await mailQueue.AddMailToQueueAsync(item);
			return "queued";
		}
	}
}
