using Rockaway.WebApp.Data;
using System.Data;
using System.Runtime.InteropServices;

namespace Rockaway.WebApp.Services.Mail {
	public class TicketMailerBackgroundService(
		IMailQueue queue,
		IServiceProvider services,
		ILogger<TicketMailerBackgroundService> logger) : BackgroundService {

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
			=> await ProcessMailQueue(stoppingToken);

		private async Task ProcessMailQueue(CancellationToken token) {
			while (!token.IsCancellationRequested) {
				var mailData = await queue.FetchMailFromQueueAsync(token);
				using var scope = services.CreateScope();
				var sender = scope.ServiceProvider.GetRequiredService<ITicketMailer>();
				var clock = scope.ServiceProvider.GetRequiredService<IClock>();
				var db = scope.ServiceProvider.GetRequiredService<RockawayDbContext>();
				try {

					var order = await db.TicketOrders.FindAsync(mailData.OrderId, CancellationToken.None);
					if (order is null) continue;
					try {
						await sender.SendOrderConfirmationAsync(mailData);
						order.MailError = null;
						order.MailSentAt = clock.GetCurrentInstant();
					}
					catch (Exception ex) {
						order.MailError = ex.Message;
						order.MailSentAt = null;
						throw;
					}
					finally {
						await db.SaveChangesAsync(CancellationToken.None);
					}


				}
				catch (Exception ex) {
					logger.LogError(ex, "Error sending tickets  for order {Order} to {Email}", mailData.OrderReference, mailData.CustomerEmail);
				}

			}
		}
	}
}
