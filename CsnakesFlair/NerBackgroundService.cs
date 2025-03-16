using CSnakes.Runtime;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CsnakesFlair;

internal sealed class NerBackgroundService : BackgroundService
{
   private readonly IPythonEnvironment _pythonEnvironment;
   private readonly ILogger<NerBackgroundService> _logger;

   public NerBackgroundService(IPythonEnvironment pythonEnvironment, ILogger<NerBackgroundService> logger)
   {
      _pythonEnvironment = pythonEnvironment;
      _logger = logger;
   }

   /// <summary>
   /// Executes when the service is ready to start.
   /// </summary>
   /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
   /// <returns><see cref="Task"/></returns>
   public override async Task StartAsync(CancellationToken cancellationToken)
   {
      _logger.LogInformation("Starting service {service}", nameof(NerBackgroundService));

      await base.StartAsync(cancellationToken);
   }

   protected override Task ExecuteAsync(CancellationToken stoppingToken)
   {
      var flairNer = _pythonEnvironment.FlairNer();

      var result = flairNer.Predict("George Washington est allé à Washington");

      foreach (var item in result)
      {
         _logger.LogInformation("{tag} = {text}", item.GetAttr("tag"), item.GetAttr("text"));
      }

      _logger.LogInformation("Exiting");

      return Task.CompletedTask;
   }

   private static string RequestUserInput()
   {
      Console.Write("Insert sentence (empty to exit): ");

      return Console.ReadLine() ?? string.Empty;
   }

   /// <summary>
   /// Executes when the service is performing a graceful shutdown.
   /// </summary>
   /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
   /// <returns><see cref="Task"/></returns>
   public override Task StopAsync(CancellationToken cancellationToken)
   {
      _logger.LogInformation("Stopping service {service}", nameof(NerBackgroundService));

      _pythonEnvironment?.Dispose();

      return base.StopAsync(cancellationToken);
   }
}
