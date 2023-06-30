using Abstractions;
using Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Models;
using Order = Models.Faire.Order;

namespace Functions;
public class Transfer
{
    private readonly ILogger _logger;
    private readonly IBaselinkerService _baselinkerService;
    private readonly IFaireService _faireService;
    private readonly IStorageService _storageService;
    private readonly BaselinkerSettings _baselinkerSettings;
    public Transfer(ILoggerFactory loggerFactory, IBaselinkerService baselinkerService,  
        IFaireService faireService, IStorageService storageService, BaselinkerSettings baselinkerSettings)
    {
        _logger = loggerFactory.CreateLogger<Transfer>();
        _baselinkerService = baselinkerService;
        _faireService = faireService;
        _storageService = storageService;
        _baselinkerSettings = baselinkerSettings;
    }

    /// <summary>
    /// Transfers data from Faire to Baselinker.
    /// Runs every 10 minutes, transfer 50 records.
    /// </summary>
    /// <param name="info">Additional information or parameters for the method.</param>

    [Function("Transfer")]
    public async Task Run([TimerTrigger("0 */10 * * * *")] string info)
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
        if(_storageService.Exists("lastUpdatedDate") == false)
            _storageService.Set("lastUpdatedDate", DateTimeOffset.MinValue);

        var orders = await _faireService.GetOrdersAsync(50,1,
            _storageService.Get<DateTimeOffset>("lastUpdatedDate"));

        _storageService.Set("lastUpdatedDate",  DateTimeOffset.Parse(orders.Last().UpdatedAt));

        await RemoveOrdersThatAlreadyExistsAsync(orders);


        var newOrders = orders.Select(order =>
        {
            var newOrder = Mapper.ToBaselinkerNewOrder(order);
            newOrder.OrderStatusId = _baselinkerSettings.DefaultStatusId;
            newOrder.CustomSourceId = _baselinkerSettings.DefaultSourceId;
            newOrder.ExtraField1 = order.Id;
            return newOrder;
        }).ToList();

        newOrders.ForEach(newOrder => _baselinkerService.AddOrderAsync(newOrder));
    }

    /// <summary>
    /// Select orders where the create date is different from the update date.
    /// Then, retrieves orders that was added at the same time, or later.
    /// If any field contains an ID from the Faire system that matches order's Id,
    /// remove that order from the list to prevent uploading it multiple times.
    /// </summary>
    /// <param name="orders"></param>
    /// <returns></returns>
    private async Task RemoveOrdersThatAlreadyExistsAsync(ICollection<Order> orders)
    {
        var updatedOrders = orders.Where(order => order.UpdatedAt != order.CreatedAt).OrderBy(order => order.CreatedAt).ToList();
        var i = 0;
        if (i < updatedOrders.Count)
        {
            var ordersAdded = await _baselinkerService.GetOrdersAsync(DateTimeOffset.Parse(updatedOrders.First().CreatedAt));
            var lastDate = ordersAdded.Last().DateAdd;
            var updatedOrdersInRange = updatedOrders.Where(order =>
                DateTimeOffset.Parse(order.CreatedAt) < DateTimeOffset.Parse(lastDate));

            foreach (var order in updatedOrdersInRange)
            {
                if (ordersAdded.Any(addedOrder => addedOrder.ExtraField1 == order.Id))
                    orders.Remove(order);
                i++;
            }
        }
    }
    private void ErrorHandler(Exception exception)
    {
        _logger.LogError(exception.Message);
    }
}
