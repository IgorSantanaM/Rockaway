using Rockaway.WebApp.Models;

namespace Rockaway.WebApp.Services.Mail {
	public interface ITicketMailer {
		Task SendOrderConfirmationAsync(TicketOrderMailData order);
		Task SendOrderConfirmationAsync(TicketOrderMailData order, CancellationToken token);
	}
}