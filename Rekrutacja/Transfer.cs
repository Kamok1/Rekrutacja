using System;
using Abstractions;
using Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Models.Faire;

namespace Rekrutacja;
public class Transfer
{
    private readonly ILogger _logger;
    private readonly IBaselinkerService _baselinkerService;
    private readonly IFaireService _faireService;
    public Transfer(ILoggerFactory loggerFactory, IBaselinkerService baselinkerService,  IFaireService faireService)
    {
        _logger = loggerFactory.CreateLogger<Transfer>();
        _baselinkerService = baselinkerService;
        _faireService = faireService;
    }

    [Function("Transfer")]
    public async Task Run([TimerTrigger("0 */1 * * * *")] string info)
    {
        try
        {
            _logger.LogInformation($"Started: {DateTime.Now}");
            await DoWorkAsync();
            _logger.LogInformation($"Finished: {DateTime.Now}");
        }
        catch (Exception e)
        {
            ErrorHandler(e);
        }
    }
    private async Task DoWorkAsync()
    {
        var orders = await _faireService.GetOrdersAsync();

        var newOrders = orders.Select(order =>
        {
            var newOrder = Mapper.ToBaselinkerNewOrder(order);
            newOrder.OrderStatusId = "8069";
            newOrder.CustomSourceId = "1024";
            newOrder.ExtraField1 = order.Id;
            return newOrder;
        }).ToList();

        newOrders.ForEach(newOrder => _baselinkerService.AddOrderAsync(newOrder));
    }
    private void ErrorHandler(Exception exception)
    {
        _logger.LogError(exception.Message);
    }
}
