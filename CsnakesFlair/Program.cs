using CSnakes.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CsnakesFlair;

internal class Program
{
   static async Task Main(string[] args)
   {
      using var cancellationTokenSource = new CancellationTokenSource();
      var cancellationToken = cancellationTokenSource.Token;

      var builder = Host.CreateApplicationBuilder(args);

      var home = Path.Join(Environment.CurrentDirectory, "..", "..", "..", "..", "CsnakesFlairPython");
      var venv = Path.Join(home, ".venv");

      builder.Services
         .WithPython()
         .WithHome(home)
         .FromRedistributable(CSnakes.Runtime.Locators.RedistributablePythonVersion.Python3_12)
         // .FromEnvironmentVariable("Python3_ROOT_DIR", "3.12")
         .WithVirtualEnvironment(venv, ensureEnvironment: true)
         .WithUvInstaller(requirementsPath: "requirements.txt");

      builder.Logging.ClearProviders();

      builder.Services.AddSerilog(config => config.ReadFrom.Configuration(builder.Configuration));

      if (args is { Length: > 0 })
      {
         builder.Configuration.AddCommandLine(args);
      }

      builder.Services.AddHostedService<NerBackgroundService>();

      var host = builder.Build();

      Console.CancelKeyPress += (sender, e) =>
      {
         e.Cancel = false;
         cancellationTokenSource.Cancel();
      };

      await host.RunAsync(cancellationToken);
   }
}
