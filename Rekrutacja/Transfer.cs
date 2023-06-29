using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Rekrutacja;
public class Transfer
{
    private readonly ILogger _logger;

    public Transfer(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<Transfer>();
    }

    [Function("Transfer")]
    public void Run([TimerTrigger("0 */1 * * * *")] string info)
    {
        _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
    }
}
