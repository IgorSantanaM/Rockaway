﻿namespace Rockaway.WebApp.Services.Mail {
	public class SmtpSettings {
		public string Host { get; set; } = "localhost";
		public int Port { get; set; } = 1025;
		public string? Username { get; set; }
		public string? Password { get; set; }
		public bool Authenticate => Username != null || Password != null;
	}
}