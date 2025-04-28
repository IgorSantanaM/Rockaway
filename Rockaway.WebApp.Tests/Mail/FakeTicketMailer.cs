using Rockaway.WebApp.Models;
using Rockaway.WebApp.Services.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockaway.WebApp.Tests.Mail {
	public class FakeTicketMailer : ITicketMailer {
		public Task SendOrderConfirmationAsync(TicketOrderMailData order) {
			return Task.CompletedTask;
		}
	}
}
