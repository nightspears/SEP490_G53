using TCViettelFC_API.Repositories.Implementations;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Installers
{
    public class SystemInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAreaRepository, AreaRepository>();
            services.AddScoped<ITicketUtilRepository, TicketUtilRepository>();
            services.AddScoped<IAdminDashboardRepository, AdminDashboardRepository>();
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));



        }
    }
}
