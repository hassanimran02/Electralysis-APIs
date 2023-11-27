using Macroeconomics.BLS.Helper;
using Macroeconomics.BLS.Services.Common;
using Macroeconomics.BLS.Services.EconomicIndicatorFields;
using Macroeconomics.BLS.Services.EconomicIndicatorGroups;
using Macroeconomics.BLS.Services.EconomicIndicatorValues;
using Macroeconomics.BLS.Services.EconomicIndicatorSources;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Macroeconomics.BLS.Services.MacroEconomics;
using Common.BLS.Services.FiscalPeriods;
using ChartsEconomics.BLS.Services.ChartsEconomics;
using Macroeconomics.BLS.Services.ChartsEconomics;
using ArgaamPlus.BLS.Services.MacroEconomics;
using Common.BLS.Services.Common;
using Common.BLS.Utils;
using Macroeconomics.BLS.Services.ArgaamEconomics;
using ArgaamPlus.BLS.Services.MeasuringUnits;

namespace Macroeconomics.BLS
{
    public static class ServiceRegistation
    {
        public static void AddMacroBLS(this IServiceCollection services)
        {
            //Common package usage
            services.AddTransient(typeof(ICommonGenericRepository<,>), typeof(CommonGenericRepository<,>));
            services.AddTransient<IFiscalPeriodService, FiscalPeriodService>();

            //Argaamplus package usage
            services.AddTransient<DatabaseExtention>();
            services.AddTransient<IArgaamPlusEconomicService, ArgaamPlusEconomicService>();
            services.AddTransient<IMeasuringUnitService, MeasuringUnitService>();

            services.AddAutoMapper(typeof(MappingProfile));
            services.AddTransient(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            services.AddTransient<IEconomicIndicatorGroupService, EconomicIndicatorGroupService>();
            services.AddTransient<IEconomicIndicatorFieldService, EconomicIndicatorFieldService>();
            services.AddTransient<IEconomicIndicatorSourceService, EconomicIndicatorSourceService>();
            services.AddTransient<IEconomicIndicatorValueService, EconomicIndicatorValueService>();
            services.AddTransient<IMacroEconomicsService, MacroEconomicsService>();
            services.AddTransient<IChartsEconomicsService, ChartsEconomicsService>();
            services.AddTransient<ITooslsEconomicsService, TooslsEconomicsService>();
            services.AddTransient<IArgaamEconomicsService, ArgaamEconomicsService>();
        }
    }
}
