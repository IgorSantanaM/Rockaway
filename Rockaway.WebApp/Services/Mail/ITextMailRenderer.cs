using Rockaway.WebApp.Models;

namespace Rockaway.WebApp.Services.Mail {
	public interface ITextMailRenderer {
		string RenderTextEmail(TicketOrderViewData data);
	}
}
