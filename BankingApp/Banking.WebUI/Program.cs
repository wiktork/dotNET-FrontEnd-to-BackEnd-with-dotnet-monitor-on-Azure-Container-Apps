using Banking.WebUI;
using Banking.WebUI.Services;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllersWithViews();

Uri uri = new Uri(builder.Configuration.GetValue<string>("AccountApi"));
builder.Services.AddHttpClient("Accounts", (httpClient) => httpClient.BaseAddress = uri);
builder.Services.AddScoped<IAccountBackendClient, AccountBackendClient>();

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

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
