using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using System.Text;
using TCViettelFC_API.Installers;
using TCViettelFC_API.Mapper;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Implementations;
using TCViettelFC_API.Repositories.Interfaces;

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
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    // Cấu hình cho JWT Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập JWT Bearer token vào ô dưới đây (Ví dụ: Bearer {token})"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddDbContext<Sep490G53Context>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("value")));

builder.Services.AddScoped<IHelloWorldRepository, HelloWorldRepository>();
builder.Services.AddScoped<INewRepository, NewRepository>();
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
builder.Services.AddScoped<ITiketOrderRepository, TiketOrderRepository>();
builder.Services.AddScoped<ISupplementaryItemRepository, SupplementaryItemRepository>();
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
        builder.WithOrigins("https://localhost:7004")
               .AllowCredentials()
               .AllowAnyHeader()
               .AllowAnyMethod();
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
});
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.InstallerServicesInAssembly(builder.Configuration);

var app = builder.Build();

app.UseCors("AllowMvcClient");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseODataBatching();
app.MapControllers();

app.Run();
