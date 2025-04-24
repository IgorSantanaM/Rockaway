using Rockaway.WebApp.Models;

namespace Rockaway.WebApp.Services.Mail {
	public interface IHtmlMailRenderer {
		string RenderHtmlEmail(TicketOrderMailData data);
	}
}
