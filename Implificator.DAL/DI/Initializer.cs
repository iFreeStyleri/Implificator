using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Implificator.DAL.Entities;
using Implificator.DAL.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Implificator.DAL.DI
{
    public static class Initializer
    {
        public static IServiceCollection ConfigureDAL(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<UserContext>(ServiceLifetime.Transient);
            services.AddTransient<IBaseRepository<User>, BaseRepository<User>>();
            services.AddTransient<IBaseRepository<VIP>, BaseRepository<VIP>>();
            return services;
        }
    }
}
