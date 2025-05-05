using Rockaway.WebApp.Models;

namespace Rockaway.WebApp.Services.Mail {
	public interface IMailQueue {
		public Task AddMailToQueueAsync(TicketOrderMailItem data);
		public Task<TicketOrderMailItem> FetchMailFromQueueAsync(CancellationToken token);
	}
}
