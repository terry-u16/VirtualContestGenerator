using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VirtualContestGenerator.Data;
using VirtualContestGenerator.Services;

namespace VirtualContestGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                if (context.HostingEnvironment.EnvironmentName == "Production")
                {
                    var keyVaultEndPoint = "https://VirtualContestKeyVault.vault.azure.net";
                    var tokenProvider = new AzureServiceTokenProvider();
                    var client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(tokenProvider.KeyVaultTokenCallback));
                    config.AddAzureKeyVault(keyVaultEndPoint, client, new DefaultKeyVaultSecretManager());
                }
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddDbContext<AtCoderProblemsContext>(options => 
                    options.UseSqlServer(hostContext.Configuration.GetConnectionString("AtCoderProblemsContext")), ServiceLifetime.Singleton);
                services.AddSingleton<VirtualContestService>();
                services.AddSingleton<FetchingJsonService>();
                services.AddHostedService<MainWorkerService>();
            });
    }
}
