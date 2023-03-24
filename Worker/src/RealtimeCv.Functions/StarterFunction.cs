using RealtimeCv.Core.Interfaces;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Extensions.Abstractions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace RealtimeCv.Functions;

public class StarterFunction
{
  private readonly ILoggerAdapter<StarterFunction> _logger;

  public StarterFunction(
    ILoggerAdapter<StarterFunction> logger
  )
  {
    _logger = logger;

  }

  [Function("StarterFunction")]
  public async Task Run([TimerTrigger("0 */5 * * * *", RunOnStartup = true)] TimerInfo myTimer)
  {
    _logger.LogInformation($"StarterFunction Timer trigger function executed at: {DateTime.Now}");

    await Task.CompletedTask;
    // // Example code to retrieve all ToDoItems and email "noreply@noreply.com" when time trigger is hit.
    // ToDoItemGetAllSpecification specification = new ToDoItemGetAllSpecification(false);
    // System.Collections.Generic.IEnumerable<Core.Entities.ToDoItem> entities = await _repo.GetItemsAsync(specification);
    //
    // string messageBody = JsonSerializer.Serialize(entities);
    //
    // await _emailService.SendEmailAsync("noreply@noreply.com", "No Reply", "Todo Item Summary", messageBody);
  }
        
        
}
