using Rockaway.WebApp.Models;
using System.Threading.Channels;

namespace Rockaway.WebApp.Services.Mail {
	/// <summary>
	/// Producer
	/// </summary>
	public class TicketOrderMailQueue : IMailQueue {
		private readonly Channel<TicketOrderMailData> channel;
		public TicketOrderMailQueue(int capacity = 100)
		{
			var options =  new BoundedChannelOptions(capacity) {
				FullMode = BoundedChannelFullMode.Wait,
			};
			channel = Channel.CreateBounded<TicketOrderMailData>(options);

		}

		public async Task AddMailToQueueAsync(TicketOrderMailData data) =>
			await channel.Writer.WriteAsync(data);

		public async Task<TicketOrderMailData> FetchMailFromQueueAsync(CancellationToken token)
			=> await channel.Reader.ReadAsync(token);
	}

}
