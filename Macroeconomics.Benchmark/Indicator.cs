using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Order;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Macroeconomics.Benchmark
{
    [SimpleJob(RunStrategy.Throughput)]
    [MemoryDiagnoser]
    [KeepBenchmarkFiles(false)]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByMethod)]
    [Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
    [HtmlExporter]
    public class Indicator
    {
        private readonly RestClient _restClient = new RestClient();
        //[Benchmark]
        //public async Task IndicatorsGroups_Next()
        //{
        //    await _restClient.IndicatorsGroups_Next();
        //}
        //[Benchmark]
        //public async Task IndicatorsGroups()
        //{
        //    await _restClient.IndicatorsGroups();
        //}
        //[Benchmark]
        //public async Task GetIndicatorFieldDataForFormula_Next()
        //{
        //    await _restClient.GetIndicatorFieldDataForFormula_Next();
        //}
        //[Benchmark]
        //public async Task GetIndicatorFieldDataForFormula()
        //{
        //    await _restClient.GetIndicatorFieldDataForFormula();
        //}
        //[Benchmark]
        //public async Task GetMacroComparableIndicatorsFieldData_Next()
        //{
        //    await _restClient.GetMacroComparableIndicatorsFieldData_Next();
        //}
        //[Benchmark]
        //public async Task GetMacroComparableIndicatorsFieldData()
        //{
        //    await _restClient.GetMacroComparableIndicatorsFieldData();
        //}
        //[Benchmark]
        //public async Task GetIndicatorsFieldValues_Next()
        //{
        //    await _restClient.GetIndicatorsFieldValues_Next();
        //}
        //[Benchmark]
        //public async Task GetIndicatorsFieldValues()
        //{
        //    await _restClient.GetIndicatorsFieldValues();
        //}
        //[Benchmark]
        //public async Task GetMacroComparableIndicatorsFieldData2_Next()
        //{
        //    await _restClient.GetMacroComparableIndicatorsFieldData2_Next();
        //}
        //[Benchmark]
        //public async Task GetMacroComparableIndicatorsFieldData2()
        //{
        //    await _restClient.GetMacroComparableIndicatorsFieldData2();
        //}
        //[Benchmark]
        //public async Task GetMacroComparableIndicatorsFieldData2PieData_Next()
        //{
        //    await _restClient.GetMacroComparableIndicatorsFieldData2PieData_Next();
        //}
        //[Benchmark]
        //public async Task GetMacroComparableIndicatorsFieldData2PieData()
        //{
        //    await _restClient.GetMacroComparableIndicatorsFieldData2PieData();
        //}
        [Benchmark]
        public async Task MacroEconomicData_Next()
        {
            await _restClient.MacroEconomicData_Next();
        }
        [Benchmark]
        public async Task MacroEconomicData()
        {
            await _restClient.MacroEconomicData();
        }
    }

    public class RestClient
    {
        private static readonly HttpClient client = new HttpClient();
        public async Task<object?> IndicatorsGroups_Next()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return await client.GetFromJsonAsync<object>($"https://localhost:8001/api/v1/json/macro/indicators-groups");
        }
        public async Task<object?> IndicatorsGroups()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return await client.GetFromJsonAsync<object>($"https://localhost:44374/api/v1/json/macro/indicators-groups");
        }
        public async Task<object?> GetMacroComparableIndicatorsFieldData_Next()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return await client.GetFromJsonAsync<object>($"https://localhost:8001/api/v1/json/macro/indicator-data/2/1993/2023/302/4");
        }
        public async Task<object?> GetMacroComparableIndicatorsFieldData()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return await client.GetFromJsonAsync<object>($"https://localhost:44374/api/v1/json/macro/indicator-data/2/1993/2023/302/4");
        }
        public async Task<object?> GetIndicatorFieldDataForFormula_Next()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return await client.GetFromJsonAsync<object>($"https://localhost:8001/api/v1/json/macro/indicator-formula-data/2/1993/2023/35/4");
        }
        public async Task<object?> GetIndicatorFieldDataForFormula()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return await client.GetFromJsonAsync<object>($"https://localhost:44374/api/v1/json/macro/indicator-formula-data/2/1993/2023/35/4");
        }
        public async Task<object?> GetIndicatorsFieldValues_Next()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return await client.GetFromJsonAsync<object>($"https://localhost:8001/api/v1/json/macro/indicator-values/213/2013/4/1");
        }
        public async Task<object?> GetIndicatorsFieldValues()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return await client.GetFromJsonAsync<object>($"https://localhost:44374/api/v1/json/macro/indicator-values/213/2013/4/1");
        }
        public async Task<object?> GetMacroComparableIndicatorsFieldData2_Next()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return await client.GetFromJsonAsync<object>($"https://localhost:8001/api/v1/json/macro/indicator-data-2/2/1993/2013/302/4?usePie=false");
        }
        public async Task<object?> GetMacroComparableIndicatorsFieldData2()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return await client.GetFromJsonAsync<object>($"https://localhost:44374/api/v1/json/macro/indicator-data-2/2/1993/2013/302/4?usePie=false");
        }
        public async Task<object?> GetMacroComparableIndicatorsFieldData2PieData_Next()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return await client.GetFromJsonAsync<object>($"https://localhost:8001/api/v1/json/macro/indicator-data-2/2/1993/2013/302/4?usePie=true");
        }
        public async Task<object?> GetMacroComparableIndicatorsFieldData2PieData()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return await client.GetFromJsonAsync<object>($"https://localhost:44374/api/v1/json/macro/indicator-data-2/2/1993/2013/302/4?usePie=true");
        } 
        public async Task<object?> MacroEconomicData_Next()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return await client.GetFromJsonAsync<object>($"http://localhost:19322/api/v1/json/charts/ui/macro-economic-data/302/4/1993/2023/2/33");
        }
        public async Task<object?> MacroEconomicData()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return await client.GetFromJsonAsync<object>($"https://localhost:44327/api/v1/json/charts/ui/macro-economic-data/302/4/1993/2023/2/33");
        }
    }
}
