using ArgaamPlus.DAL.Entities;
using Common.DAL.Entities;
using Macroeconomics.DAL.Entities;
using Macroeconomics.DAL.Petapoco;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
namespace Macroeconomics.DAL
{
    public static class ServiceRegistation
    {
        public static void AddMacroDal(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddDbContext<ArgaamNext_IndicatorContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("indicatorDbConnection")));

            services.AddDbContext<ArgaamPlusContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("argaamPlusConnection")));

            services.AddDbContext<ArgaamNext_CommonContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("commonConnection")));

            services.AddTransient<PetapocoServices>();
        }
    }
}
