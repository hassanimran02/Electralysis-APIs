using Microsoft.Extensions.Diagnostics.HealthChecks;
using RestSharp;

namespace Macroeconomics.Api
{
    public class ApiHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            //var url = "https://api.chucknorris.io/jokes/categories";
            //var client = new RestClient();
            //var request = new RestRequest(url, Method.Get);
            //request.AddHeader("Accept", "application/json");
            //request.AddHeader("Content-Type", "application/json");

            //var response = client.Execute(request);

            //if (response.IsSuccessful)

            //    return Task.FromResult(HealthCheckResult.Healthy());
            //else

            //    return Task.FromResult(HealthCheckResult.Unhealthy());

            return Task.FromResult(HealthCheckResult.Healthy());
        }
    }
}
