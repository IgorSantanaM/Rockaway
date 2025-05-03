using Rockaway.WebApp.Models;

namespace Rockaway.WebApp.Services.Mail {
	public interface IMailQueue {
		public Task AddMailToQueueAsync(TicketOrderMailData data);
		public Task<TicketOrderMailData> FetchMailFromQueueAsync(CancellationToken token);
	}
}
