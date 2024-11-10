using System.Text.Encodings.Web;
using System.Text.Unicode;
using TCViettelFC_Client.ApiServices;
using TCViettelFC_Client.Services;
using TCViettetlFC_Client.Services;
using TCViettetlFC_Client.VNPayHelper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient("ApiClient")
    .ConfigureHttpClient(options =>
    {
        options.BaseAddress = new Uri(builder.Configuration["ApiConfig:BaseAddress"]);
        options.DefaultRequestHeaders.Add("Accept", "application/json");

    });
// Register UserService with the configured HttpClient
builder.Services.AddHttpClient<UserService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiConfig:BaseAddress"]);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<GoShipService>();
builder.Services.AddSingleton<HtmlEncoder>(
    HtmlEncoder.Create(allowedRanges: new[] {
        UnicodeRanges.All
    }));

// Register UserService with the configured HttpClient
builder.Services.AddHttpClient<FeedbackService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiConfig:BaseAddress"]);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<CheckOutService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiConfig:BaseAddress"]);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});


builder.Services.AddHttpClient<OrderService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiConfig:BaseAddress"]);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IApiHelper, ApiHelper>();
builder.Services.AddSingleton<IVnPayService, VnPayService>();
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
