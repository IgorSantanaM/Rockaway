using Microsoft.AspNetCore.SignalR;
using Rockaway.WebApp.Areas.Admin.Hubs;

namespace Rockaway.WebApp.Services.Mail {
	public class SignalRMailDeliveryReporter(IHubContext<EmailHub> hub) : IMailDeliveryReporter {
		public async Task ReporteSuccessAsync(Guid orderId) =>
			await hub.Clients.All.SendAsync("success", orderId);
		
		public async Task ReporteFailureAsync(Guid orderId, string error) =>
			await hub.Clients.All.SendAsync("failure", orderId, error);
	}
}
