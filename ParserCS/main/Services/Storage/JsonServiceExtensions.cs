using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using main.Interfaces;
using main.Models;
using Microsoft.Extensions.DependencyInjection;

namespace main.Services.Storage
{
    public static class JsonServiceExtensions
    {
        public static IServiceCollection AddJsonService(this IServiceCollection services)
        {
            services.AddSingleton<JsonService<List<Product>>>();
            services.AddSingleton<IExporter<List<Product>>>(provider =>
                provider.GetRequiredService<JsonService<List<Product>>>());
            services.AddSingleton<IImporter<List<Product>>>(provider =>
                provider.GetRequiredService<JsonService<List<Product>>>());

            return services;
        }
    }
}
