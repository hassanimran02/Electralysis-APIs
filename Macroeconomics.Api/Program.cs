
using Macroeconomics.Api;
using Serilog;

CreateHostBuilder(args).Build().Run();


static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureLogging(logging =>
               {
                   var logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(new ConfigurationBuilder()
                    .AddJsonFile("seri-log.config.json")
                    .Build())
                    .Enrich.FromLogContext()
                    .CreateLogger();
                   logging.ClearProviders();
                   logging.AddSerilog(logger);
               })
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   webBuilder.UseStartup<Startup>();
               });
