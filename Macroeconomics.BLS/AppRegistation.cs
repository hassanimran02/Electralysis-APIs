
using Macroeconomics.Api.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
namespace Macroeconomics.BLS
{
    public static class AppRegistation
    {
        public static void AddMacroShared(this IApplicationBuilder app, IConfiguration Configuration)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
