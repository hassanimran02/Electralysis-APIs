using ArgaamPlus.BLS.Services.MacroEconomics;
using ArgaamPlus.DAL.Entities;
using Common.BLS.Services.FiscalPeriods;
using Common.DAL.Entities;
using Consul;
using Macroeconomics.BLS.Models.ToolsEconomics;
using Macroeconomics.BLS.Services.ChartsEconomics;
using Macroeconomics.BLS.Services.MacroEconomics;
using Macroeconomics.DAL.Entities;
using System;
using System.Net;
using System.Reflection;
using static System.Net.WebRequestMethods;

namespace Macroeconomics.Api
{
    public static class TestMethods
    {
        public static IApplicationBuilder UseTestMethods(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            scope.ServiceProvider.GetRequiredService<ArgaamNext_IndicatorContext>();
            var _tooslsEconomicsService = scope.ServiceProvider.GetRequiredService<ITooslsEconomicsService>();
            //var abc = _tooslsEconomicsService.ArgaamToolsPageQuery(ArgaamToolsPageQueryEnum.FuelPrices).Result;

            return app;
        }
    }
}
