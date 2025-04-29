using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Rockaway.WebApp.Services.Mail {
	public class SmtpMailSender(SmtpSettings settings) : IMailSender {
		public async Task<string> SendAsync(MimeMessage message) {
			using var smtp = new SmtpClient();
			await smtp.ConnectAsync(settings.Host, settings.Port, SecureSocketOptions.StartTls);
			if(settings.Authenticate) await smtp.AuthenticateAsync(settings.Username, settings.Password);
			var result = await smtp.SendAsync(message);
			await smtp.DisconnectAsync(true);
			return result;
		}
	}
}