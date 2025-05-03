using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Rockaway.WebApp.Services.Mail;

public class SmtpMailSender(
	SmtpSettings smtpSettings,
	ILogger<SmtpMailSender> logger) : IMailSender, IDisposable {

	public async Task<string> SendAsync(MimeMessage message) {
		using var smtp = new SmtpClient();
		logger.LogTrace("connecting");
		await smtp.ConnectAsync(smtpSettings.Host, smtpSettings.Port,
			SecureSocketOptions.StartTlsWhenAvailable);
		logger.LogTrace("authenticating");
		if (smtpSettings.Authenticate)
			await smtp.AuthenticateAsync(smtpSettings.Username, smtpSettings.Password);
		logger.LogTrace("sending");
		var result = await smtp.SendAsync(message);
		logger.LogTrace("disconnecting");
		await smtp.DisconnectAsync(true);
		logger.LogTrace("done");
		return result;
	}

	public async Task<string> SendAsync(MimeMessage message, CancellationToken token) {
		await semaphore.WaitAsync(token);
		var result = "ERROR";
		try {
			await ConnectAsync(token);
			result = await smtpClient.SendAsync(message, token);
		}
		catch (Exception ex) {
			logger.LogError(ex, "SendAsync");
			throw;
		}
		finally {
			semaphore.Release();
		}
		return result;
	}

	private readonly SmtpClient smtpClient = new();

	private static readonly TimeSpan smtpClientTimeout = TimeSpan.FromSeconds(10);
	private DateTimeOffset disconnectAfter = DateTimeOffset.MinValue;
	private readonly SemaphoreSlim semaphore = new(1, 1);

	private void ScheduleDisconnect(CancellationToken token) {
		disconnectAfter = DateTimeOffset.UtcNow.Add(smtpClientTimeout);
		Task.Run(async () => {
			await Task.Delay(smtpClientTimeout.Add(TimeSpan.FromSeconds(1)), token);
			if (DateTimeOffset.UtcNow > disconnectAfter) await DisconnectAsync(token);
		}, token);
	}

	private async Task ConnectAsync(CancellationToken token) {
		if (smtpClient.IsConnected) {
			await smtpClient.NoOpAsync(token);
			logger.LogTrace("Reusing SMTP connection for {host}", smtpSettings.Host);
		} else {
			logger.LogTrace("Connecting SMTP client to {host}", smtpSettings.Host);
			await smtpClient.ConnectAsync(smtpSettings.Host, smtpSettings.Port,
				SecureSocketOptions.StartTlsWhenAvailable, token);
			if (smtpSettings.Authenticate)
				await smtpClient.AuthenticateAsync(smtpSettings.Username, smtpSettings.Password, token);
		}
		ScheduleDisconnect(token);
	}

	private async Task DisconnectAsync(CancellationToken token) {
		await semaphore.WaitAsync(token);
		try {
			if (!smtpClient.IsConnected) return;
			logger.LogTrace("Disconnecting SMTP client");
			await smtpClient.DisconnectAsync(true, token);
			disconnectAfter = DateTimeOffset.MinValue;
		}
		catch (Exception ex) {
			logger.LogError(ex, "DisconnectAsync");
			throw;
		}
		finally {
			semaphore.Release();
		}
	}

	public void Dispose() {
		logger.LogTrace("Disconnecting SMTP client via Dispose()");
		smtpClient.DisconnectAsync(true);
		smtpClient.Dispose();
		semaphore.Dispose();
		GC.SuppressFinalize(this);
	}
}