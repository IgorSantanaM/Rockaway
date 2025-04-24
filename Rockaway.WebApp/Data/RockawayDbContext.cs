using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Rockaway.WebApp.Data.Entities;
using Rockaway.WebApp.Data.Sample;

namespace Rockaway.WebApp.Data;

public class RockawayDbContext(DbContextOptions<RockawayDbContext> options)
	: IdentityDbContext<IdentityUser>(options) {

	private readonly bool unitTestMode = false;

	public RockawayDbContext(DbContextOptions<RockawayDbContext> options, bool unitTestMode) : this(options) {
		this.unitTestMode = unitTestMode;
	}

	public DbSet<Artist> Artists { get; set; } = default!;
	public DbSet<Venue> Venues { get; set; } = default!;
	public DbSet<Show> Shows { get; set; } = default!;
	public DbSet<TicketOrder> TicketOrders { get; set; } = default!;

	protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) {
		base.ConfigureConventions(configurationBuilder);
		if (Database.ProviderName!.Contains("Sqlite")) {
			configurationBuilder.AddSqliteConvertors();
		} else {
			configurationBuilder.AddNodaTimeConverters();
		}
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		base.OnModelCreating(modelBuilder);
		// Override EF Core's default table naming (which pluralizes entity names)
		// and use the same names as the C# classes instead
		var rockawayEntityNamespace = typeof(Artist).Namespace;
		var rockawayEntities = modelBuilder.Model.GetEntityTypes().Where(e => e.ClrType.Namespace == rockawayEntityNamespace);
		foreach (var entity in rockawayEntities) entity.SetTableName(entity.DisplayName());

		modelBuilder.Entity<Artist>(entity => {
			entity.HasIndex(artist => artist.Slug).IsUnique();
			entity.HasMany(a => a.HeadlineShows).WithOne(s => s.HeadlineArtist).OnDelete(DeleteBehavior.Restrict);
			entity.HasMany(a => a.SupportSlots).WithOne(ss => ss.Artist).OnDelete(DeleteBehavior.Restrict);
		});

		modelBuilder.Entity<Venue>(entity => {
			entity.HasIndex(venue => venue.Slug).IsUnique();
			entity.HasMany(v => v.Shows)
				.WithOne(s => s.Venue)
				.OnDelete(DeleteBehavior.Restrict);
		});

		modelBuilder.Entity<Show>(entity => {
			entity.HasAlternateKey(show => show.Venue.Id, show => show.Date);
			entity.HasMany(show => show.SupportSlots).WithOne(ss => ss.Show).OnDelete(DeleteBehavior.Cascade);
			entity.HasMany(show => show.TicketTypes).WithOne(tt => tt.Show).OnDelete(DeleteBehavior.Cascade);
			entity.HasMany(show => show.TicketOrders).WithOne(to => to.Show).OnDelete(DeleteBehavior.Restrict);
		});

		modelBuilder.Entity<SupportSlot>(entity => {
			entity.HasKey(slot => slot.Show.Id, slot => slot.SlotNumber);
		});

		modelBuilder.Entity<TicketOrder>(entity => {
			entity.Property(o => o.CustomerName).HasMaxLength(100);
			entity.Property(o => o.CustomerEmail).HasMaxLength(250);
		});

		modelBuilder.Entity<TicketType>(entity => {
			entity.Property(tt => tt.Price).HasColumnType("money");
		});

		modelBuilder.HasRockawaySampleData(unitTestMode);
	}

	public async Task<TicketOrder?> FindOrderAsync(Guid ticketOrderId)
		=> await TicketOrders
			.Include(order => order.Tickets).ThenInclude(item => item.TicketType)
			.Include(order => order.Show).ThenInclude(s => s.Venue)
			.FirstOrDefaultAsync(to => to.Id == ticketOrderId);
}