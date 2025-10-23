using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OLTPSystem.BLL;
using OLTPSystem.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OLTPSystem
{
    public static class OLTPExtension
    {
        public static void OLTPDependencies(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
        {
            services.AddDbContext<OLTPContext>(options);

            //Add services
            services.AddScoped<CustomerService>((ServiceProvider) =>
            {
                var context = ServiceProvider.GetService<OLTPContext>();
                return context == null ?
                    throw new InvalidOperationException("OLTPContext is not registered.")
                    : new CustomerService(context);
            });

            services.AddScoped<LookupService>((ServiceProvider) =>
            {
                var context = ServiceProvider.GetService<OLTPContext>();
                return context == null ?
                    throw new InvalidOperationException("OLTPContext is not registered.")
                    : new LookupService(context);
            });
        }
    }
}
