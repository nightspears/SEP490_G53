using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TCViettelFC_API.Mapper;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Implementations;
using TCViettelFC_API.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<Sep490G53Context>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("value")));

builder.Services.AddScoped<IHelloWorldRepository, HelloWorldRepository>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddAuthorization();
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
builder.Services.AddAuthorizationBuilder().AddPolicy("admin", p =>
{
    p.RequireClaim("RoleId", "2");
});
builder.Services.AddAutoMapper(typeof(MappingProfile));
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

app.MapControllers();

app.Run();
