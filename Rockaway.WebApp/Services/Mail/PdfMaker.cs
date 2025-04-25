using QuestPDF.Drawing;
using QuestPDF.Infrastructure;
using Rockaway.WebApp.Models;

namespace Rockaway.WebApp.Services.Mail {
	public class PdfMaker : IPdfMaker {
		private static readonly SvgImage rockawayLogotypeSvg
			= SvgImage.FromText(EmbeddedResource.ReadAllText("rockaway-logotype.svg"));

		static PdfMaker() {
			using var fontStream = EmbeddedResource.OpenStream("PTSansNarrow-Regular.ttf");
			FontManager.RegisterFontWithCustomName("PT Sans", fontStream);
		}
		public byte[] CreatePdfTitle(TicketOrderMailData data)
		{

		}
	}
}

