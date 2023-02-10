using Microsoft.Extensions.DependencyInjection;
using ApexRestaurant.Repository.RCustomer;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ApexRestaurant.Repository
{
    public static class RepositoryModule
    {
        public static void Register(IServiceCollection services, string connection, string migrationsAssembly)
        {
            services.AddDbContext<RestaurantContext>(options => options.UseSqlServer(
                connection,
                builder => builder.MigrationsAssembly(migrationsAssembly)
            ));

            services.AddTransient<ICustomerRepository, CustomerRepository>();
        }
    }
}