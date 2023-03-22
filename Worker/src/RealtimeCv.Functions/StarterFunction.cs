using RealtimeCv.Core.Interfaces;
// using RealtimeCv.Core.Specifications;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace CleanArchitectureCosmosDB.AzureFunctions
{
    public class StarterFunction
    {
        private readonly ILogger<StarterFunction> _log;
        private readonly IEmailService _emailService;
        private readonly IToDoItemRepository _repo;


        public StarterFunction(ILogger<StarterFunction> log,
                               IEmailService emailService,
                               IToDoItemRepository repo)
        {
            this._log = log ?? throw new ArgumentNullException(nameof(log));
            this._emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this._repo = repo ?? throw new ArgumentNullException(nameof(repo));

        }

        [FunctionName("StarterFunction")]
        public async Task Run([TimerTrigger("0 */5 * * * *", RunOnStartup = true)] TimerInfo myTimer)
        {
            _log.LogInformation($"StarterFunction Timer trigger function executed at: {DateTime.Now}");

            // Example code to retrieve all ToDoItems and email "noreply@noreply.com" when time trigger is hit.
            ToDoItemGetAllSpecification specification = new ToDoItemGetAllSpecification(false);
            System.Collections.Generic.IEnumerable<Core.Entities.ToDoItem> entities = await _repo.GetItemsAsync(specification);

            string messageBody = JsonSerializer.Serialize(entities);

            await _emailService.SendEmailAsync("noreply@noreply.com", "No Reply", "Todo Item Summary", messageBody);
        }
    }
}
