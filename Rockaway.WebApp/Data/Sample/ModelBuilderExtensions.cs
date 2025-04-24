using Microsoft.AspNetCore.Identity;
using Rockaway.WebApp.Data.Entities;

namespace Rockaway.WebApp.Data.Sample;

public static class ModelBuilderExtensions {
	public static void HasRockawaySampleData(this ModelBuilder modelBuilder, bool isUnitTest) {
		var howManyTicketOrders = (isUnitTest ? 10 : 5000);
		var shows = SampleData.Shows.AllShows.ToList();
		var orders = SampleData.TicketOrders.CreateSampleTicketOrders(shows, howManyTicketOrders).ToArray();
		var tickets = SampleData.TicketOrders.ExtractTickets(orders);

		modelBuilder.Entity<Artist>().HasData(SeedData.For(SampleData.Artists.AllArtists));
		modelBuilder.Entity<Venue>().HasData(SeedData.For(SampleData.Venues.AllVenues));
		modelBuilder.Entity<Show>().HasData(SeedData.For(shows));
		modelBuilder.Entity<TicketType>().HasData(SeedData.For(SampleData.Shows.AllTicketTypes));
		modelBuilder.Entity<SupportSlot>().HasData(SeedData.For(SampleData.Shows.AllSupportSlots));
		modelBuilder.Entity<TicketOrder>().HasData(SeedData.For(orders));
		modelBuilder.Entity<Ticket>().HasData(SeedData.For(tickets));
		modelBuilder.Entity<IdentityUser>().HasData(SampleData.Users.Admin);
	}
}