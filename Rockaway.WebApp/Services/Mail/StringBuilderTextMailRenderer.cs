using Rockaway.WebApp.Models;

namespace Rockaway.WebApp.Services.Mail;

public class StringBuilderTextMailRenderer : ITextMailRenderer {
	public string RenderTextEmail(TicketOrderViewData data) {
		var sb = new StringBuilder();
		sb.AppendLine($"Hey {data.CustomerName},");
		sb.AppendLine();
		sb.AppendLine($"Great news - you're going to see {data.Headliner}!");
		sb.AppendLine();
		sb.AppendLine($"Order Reference: {data.Reference}");
		sb.AppendLine();
		sb.AppendLine("Your order details are below.");
		sb.AppendLine();
		sb.AppendLine($"{data.Headliner}");
		if (data.HasSupport) {
			sb.AppendLine("+ special guests: ").Append(data.SupportArtistsText).AppendLine();
		}
		sb.AppendLine($"Venue: {data.VenueName}, {data.VenueSummary}");
		sb.AppendLine($"Date: {data.ShowDate:dddd, dd MMMM yyyy}, doors {data.DoorsOpen:HH:mm}");
		sb.AppendLine();
		sb.AppendLine("Your tickets are attached to this email:");
		sb.AppendLine();
		foreach (var item in data.Contents) {
			sb.AppendPadRight(item.Quantity.ToString(), 4)
				.Append("x ").AppendPadRight(item.Description, 50)
				.AppendPadLeft(item.UnitPrice, 16)
				.AppendLine();
		}
		sb.AppendLine();
		sb.AppendPadRight("Total:", 56).AppendPadLeft(data.FormattedTotalPrice, 16).AppendLine();
		sb.AppendLine();
		sb.AppendLine("Any questions or problems with your order, just reply to this email.");
		sb.AppendLine();
		sb.AppendLine("Enjoy the show!");
		sb.AppendLine();
		sb.AppendLine("rockaway: gig tickets for good people.");
		return sb.ToString();
	}
}
