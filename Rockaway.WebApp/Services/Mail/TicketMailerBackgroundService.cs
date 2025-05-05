using Rockaway.WebApp.Data;
using Rockaway.WebApp.Models;
using System.Data;
using System.Runtime.InteropServices;

namespace Rockaway.WebApp.Services.Mail {
	public class TicketMailerBackgroundService(
		IMailQueue queue,
		IServiceProvider services,
		ILogger<TicketMailerBackgroundService> logger,
		ITicketMailer mailer,
		IMailDeliveryReporter reporter) : BackgroundService {

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
			=> await ProcessMailQueue(stoppingToken);

		private async Task ProcessMailQueue(CancellationToken token) {
			TicketOrderMailItem mailItem;
			while (!token.IsCancellationRequested) {
				try {
					mailItem = await queue.FetchMailFromQueueAsync(token);
				}
				catch (OperationCanceledException){
					logger.LogDebug("Exiting {ProcessMailQueue} due to OperationCanceledException (this is fine!)", nameof(ProcessMailQueue));
					break;
				}
				using var scope = services.CreateScope();
				var sender = scope.ServiceProvider.GetRequiredService<ITicketMailer>();
				var clock = scope.ServiceProvider.GetRequiredService<IClock>();
				var db = scope.ServiceProvider.GetRequiredService<RockawayDbContext>();
				try {
					var order = await db.TicketOrders
						.Include(o => o.Show).ThenInclude(s => s.HeadlineArtist)
						.Include(o => o.Show).ThenInclude(s => s.Venue)
						.Include(o => o.Tickets).ThenInclude(i => i.TicketType)
						.Include(o => o.Show).ThenInclude(s => s.SupportSlots).ThenInclude(slot => slot.Artist)
						.AsNoTracking()
						.FirstOrDefaultAsync(o => o.Id == mailItem.OrderId, token);

					if (order is null) continue;
					try {
						var mailData = new TicketOrderMailData(order, mailItem.WebSiteBaseUri);
						await mailer.SendOrderConfirmationAsync(mailData, token);
						order.MailError = null;
						order.MailSentAt = clock.GetCurrentInstant();
						await reporter.ReporteSuccessAsync(mailItem.OrderId);
					}
					catch (Exception ex) {
						order.MailError = ex.Message;
						order.MailSentAt = null;
						throw;
					}
					finally {
						await db.TicketOrders.Where(t => t.Id == mailItem.OrderId)
							.ExecuteUpdateAsync(t => t.SetProperty(o => o.MailError, order.MailError)
								.SetProperty(o => o.MailSentAt, order.MailSentAt), CancellationToken.None);
					}
				}
				catch (Exception ex) {
					logger.LogError(ex, "Error sending tickets  for order {OrderId}.", mailItem.OrderId);
					await reporter.ReporteFailureAsync(mailItem.OrderId, ex.Message);
				}

			}
		}
	}
}
