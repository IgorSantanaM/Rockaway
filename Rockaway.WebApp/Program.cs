using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Rockaway.WebApp.Data;
using Rockaway.WebApp.Hosting;
using Rockaway.WebApp.Components;
using SixLabors.ImageSharp.Web.DependencyInjection;
using Rockaway.WebApp.Services.Mail;
using RazorEngine.Templating;
using Mjml.Net;
using QRCoder;

var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<Program>();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options => options.Conventions.AuthorizeAreaFolder("admin", "/"));

builder.Services.AddControllersWithViews(options => {
	options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});

builder.Services.AddSingleton<IClock>(SystemClock.Instance);

#if DEBUG
if (!builder.Environment.IsUnitTest()) builder.Services.AddSassCompiler();
#endif


logger.LogInformation("Rockaway running in {environment} environment", builder.Environment.EnvironmentName);

var useInMemoryDatabase = builder.Environment.IsUnitTest();
SqliteConnection sqliteConnection;
if (useInMemoryDatabase) {
	logger.LogInformation("Using in-memory database");
	sqliteConnection = new($"Data Source=:memory:");
	sqliteConnection.Open();
} else {
	logger.LogInformation("Using Sqlite database");
	sqliteConnection = new($"Data Source=rockaway.db");
}

builder.Services.AddDbContext<RockawayDbContext>(options
	=> options.UseSqlite(sqliteConnection, sqliteOptions
			=> sqliteOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

builder.Services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<RockawayDbContext>();

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddImageSharp();

builder.Services.AddSingleton<ITextMailRenderer>(new StringBuilderTextMailRenderer());

builder.Services.AddSingleton(_ => RazorEngineService.Create());
builder.Services.AddSingleton<IMailTemplateProvider>(new EmbeddedResourceMailTemplateProvider());
builder.Services.AddSingleton<IMjmlRenderer>(_ => new MjmlRenderer());
builder.Services.AddSingleton<IHtmlMailRenderer, RazorEngineMjmlMailRenderer>();
builder.Services.AddSingleton<QRCodeGenerator>();
builder.Services.AddSingleton<IPdfMaker, PdfMaker>();

var smtpSettings = builder.Configuration.GetSection("Smtp").Get<SmtpSettings>() ?? new SmtpSettings();

builder.Services.AddSingleton(smtpSettings);
builder.Services.AddTransient<IMailSender, SmtpMailSender>();
builder.Services.AddTransient<ITicketMailer, TicketMailer>();

var app = builder.Build();

if (app.Environment.IsProduction()) {
	app.UseHttpsRedirection();
	app.UseExceptionHandler("/Error");
	app.UseHsts();
} else {
	app.UseDeveloperExceptionPage();
}

using (var scope = app.Services.CreateScope()) {
	using var db = scope.ServiceProvider.GetService<RockawayDbContext>()!;
	db.Database.EnsureCreated();
}

app.UseImageSharp();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.MapRazorComponents<TicketPicker>().AddInteractiveServerRenderMode();
app.MapAreaControllerRoute(
	name: "admin",
	areaName: "Admin",
	pattern: "Admin/{controller=Home}/{action=Index}/{id?}"
).RequireAuthorization();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();
app.Run();
