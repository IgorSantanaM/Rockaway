using MimeKit;

namespace Rockaway.WebApp.Services.Mail {
	public interface IMailSender {
		Task<string> SendAsync(MimeMessage message);
	}

	public class SmtpSettings {
		public string Host { get; set; } = "localhost";
		public int Port { get; set; } = 1025;
		public string? Username { get; set; }
		public string? Password { get; set; }
		public bool Authenticate => Username != null || Password != null;
	}
}