using Microsoft.Extensions.DependencyInjection;
using ApexRestaurant.Repository.RCustomer;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ApexRestaurant.Repository
{
    public static class RepositoryModule
    {
        public static void Register(IServiceCollection services, string? connectionString)
        {
            services.AddDbContext<RestaurantContext>(options => options.UseSqlServer(
                connectionString,
                builder => builder.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name)
            ));

            services.AddTransient<ICustomerRepository, CustomerRepository>();
        }
    }
}