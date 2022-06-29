using Fanex.Data;
using Fanex.Data.SqlServer;
using Fanex.NexHR.WebSite2;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connections = builder.Configuration.GetSection("ConnectionStrings")
                                    .GetChildren()
                                    .AsEnumerable()
                                    .Where(x => !string.IsNullOrWhiteSpace(x.GetValue<string>("Name")))
                                    .ToDictionary(x => x.GetValue<string>("Name"),
                                    y => new ConnectionConfiguration(y.GetValue<string>("Name"), y.GetValue<string>("ConnectionString"), y.GetValue<string>("ProviderName")));

DbSettingProviderManager
   .StartNewSession()
   .Use(connections)
   .WithSqlServer()
   .UseDefaultDbSettingProvider(resourcePath: $"~\\App_Data\\SpNames.xml")
   .Run();

DIExtensions.RegisterServices(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
