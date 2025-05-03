using MimeKit;

namespace Rockaway.WebApp.Services.Mail {
	public interface IMailSender {
		Task<string> SendAsync(MimeMessage message);
		Task<string> SendAsync(MimeMessage message, CancellationToken token);
	}
}