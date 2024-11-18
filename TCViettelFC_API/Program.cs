using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using System.Text;
using TCViettelFC_API.Installers;
using TCViettelFC_API.Mapper;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Implementations;
using TCViettelFC_API.Repositories.Interfaces;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddControllers().AddOData(option => option.Select().Filter().Count().OrderBy().Expand().SetMaxTop(100)
            .AddRouteComponents("odata", GetEdmModel()));


static IEdmModel GetEdmModel()
{
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<News>("NewsOdata");
    return builder.GetEdmModel();
}
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Your API",
        Version = "v1"
    });

    // Cấu hình cho JWT Bearer
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Nhập JWT Bearer token vào ô dưới đây (ví dụ: Bearer {token})"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


builder.Services.AddDbContext<Sep490G53Context>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("value")));
builder.Services.AddScoped<ISupplementaryItemRepository, SupplementaryItemRepository>();
builder.Services.AddScoped<IHelloWorldRepository, HelloWorldRepository>();
builder.Services.AddScoped<INewRepository, NewRepository>();
builder.Services.AddScoped<ITicketOrderRepository, TicketOrderRepository>();
builder.Services.AddScoped<ICategoryNewRepository, CategoryNewRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICloudinarySetting, CloudinarySettings>();
builder.Services.AddScoped<ISeasonRepository, SeasonRepository>();
builder.Services.AddScoped<IDiscountRepository, DiscoutRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICheckoutRepository, CheckoutRepository>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddAuthorization();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddAuthentication().AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
        ValidAudience = builder.Configuration["JwtConfig:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true


    };
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMvcClient", builder =>
    {
        builder.WithOrigins("https://localhost:7004")  // MVC app origin
               .AllowCredentials()                   // Allow cookies and credentials
               .AllowAnyHeader()                     // Allow any headers
               .AllowAnyMethod();                    // Allow any HTTP methods (GET, POST, etc.)
    });
});
builder.Services.AddAuthorization(options =>
{
    // Policy cho admin
    options.AddPolicy("admin", policy =>
        policy.RequireClaim("RoleId", "2"));

    // Policy cho staff
    options.AddPolicy("staff", policy =>
        policy.RequireClaim("RoleId", "1"));

    options.AddPolicy("entry", policy =>
      policy.RequireClaim("RoleId", "3"));
});
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.InstallerServicesInAssembly(builder.Configuration);

var app = builder.Build();
app.UseCors("AllowMvcClient");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });

}


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseODataBatching();
app.MapControllers();

app.Run();
