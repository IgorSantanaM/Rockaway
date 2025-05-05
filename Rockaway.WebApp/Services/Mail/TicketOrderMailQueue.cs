using Rockaway.WebApp.Models;
using System.Threading.Channels;

namespace Rockaway.WebApp.Services.Mail {
	/// <summary>
	/// Producer
	/// </summary>
	public class TicketOrderMailQueue : IMailQueue {
		private readonly Channel<TicketOrderMailItem> channel;

		public TicketOrderMailQueue(int capacity = 100)
		{
			var options =  new BoundedChannelOptions(capacity) {
				FullMode = BoundedChannelFullMode.Wait,
			};
			channel = Channel.CreateBounded<TicketOrderMailItem>(options);

		}
		public async Task AddMailToQueueAsync(TicketOrderMailItem data) =>
			await channel.Writer.WriteAsync(data);

		public async Task<TicketOrderMailItem> FetchMailFromQueueAsync(CancellationToken token)
			=> await channel.Reader.ReadAsync(token);
	}

}
