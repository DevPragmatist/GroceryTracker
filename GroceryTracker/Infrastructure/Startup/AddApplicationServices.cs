using GroceryTracker.Domain;
using GroceryTracker.Infrastructure.Authentication;
using System.Reflection;

namespace GroceryTracker.Infrastructure.Startup
{
    public static class AddApplicationServicesExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            // Register application services here
            services.AddScoped<IUnitOfWork, GroceryContext>();
            services.AddScoped<IEmailSender, SmtpEmailSender>();
            AddGroceryItemServices(services, assembly);
        }

        private static void AddGroceryItemServices(IServiceCollection services, Assembly assembly)
        {
            var appServices = assembly.GetTypes()
                .Where(t => t.Name.EndsWith("Service"))
                .Where(t => t is { IsClass: true, IsAbstract: false })
                .Select(t => new
                {
                    Interface = t.GetInterfaces().Single(),
                    Implementation = t
                });

            foreach (var service in appServices)
            {
                services.AddScoped(service.Interface, service.Implementation);
            }
        }
    }
}
