using Rockaway.WebApp.Models;

namespace Rockaway.WebApp.Services.Mail {
	public interface IPdfMaker {
		byte[] CreatePdfTitle(TicketOrderMailData data);
	}
}

