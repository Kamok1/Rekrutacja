using System.Globalization;
using Abstractions;
using Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Models;
using Models.Faire;

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
        if (_storageService.Exists("lastUpdatedDate") == false)
            _storageService.Set("lastUpdatedDate", DateTimeOffset.MinValue);

        var orders = await _faireService.GetOrdersAsync(50, 1,
            _storageService.Get<DateTimeOffset>("lastUpdatedDate"));

        _storageService.Set("lastUpdatedDate", orders.Last().UpdatedAt);
        _logger.LogInformation("Retrieved orders from Faire and updated the last updated date in the storage service.");


        await RemoveOrdersThatAlreadyExistsAsync(orders);
        _logger.LogInformation("Removed existing orders.");

        var newOrders = orders.Select(order =>
        {
            var newOrder = Mapper.ToBaselinkerNewOrder(order);
            newOrder.OrderStatusId = _baselinkerSettings.DefaultStatusId;
            newOrder.CustomSourceId = _baselinkerSettings.DefaultSourceId;
            newOrder.ExtraField1 = order.Id;
            return newOrder;
        }).ToList();

        newOrders.ForEach(newOrder => _baselinkerService.AddOrderAsync(newOrder));
        _logger.LogInformation("Added new orders to the Baselinker system.");
    }

    /// <summary>
    /// Select orders where the create date is different from the update date.
    /// Then, retrieves orders that was added at the same time, or later.
    /// If any field contains an ID from the Faire system that matches order's Id,
    /// remove that order from the list to prevent uploading it multiple times.
    /// </summary>
    /// <param name="orders"></param>
    /// <returns></returns>
    private async Task RemoveOrdersThatAlreadyExistsAsync(ICollection<FaireOrder> orders)
    {
        var updatedOrders = orders
            .Where(order => order.UpdatedAt != order.CreatedAt)
            .OrderBy(order => order.CreatedAt).ToList();
        var i = 0;
        var numberOfUpdatedOrders = updatedOrders.Count;
        while (i < numberOfUpdatedOrders)
        {
            // Retrieve orders added after the minimum date
            var baselinkerOffers = await _baselinkerService.GetOrdersAsync(updatedOrders.First().CreatedAt);
            var lastDate = baselinkerOffers.Last().DateAdd;

            // Filter updated orders within the range of minimum and last date
            var updatedOrdersInRange = updatedOrders.Where(updatedOrder =>
                updatedOrder.CreatedAt < lastDate);


            foreach (var updatedOrderInRange in updatedOrdersInRange)
            {
                // Remove the processed updated order from the list
                updatedOrders.Remove(updatedOrderInRange);

                // Check if the updated order already exists in the Baselinker's System
                if (baselinkerOffers.Any(addedOrder => addedOrder.ExtraField1 == updatedOrderInRange.Id))
                    orders.Remove(updatedOrderInRange);
                i++;
            }
        }
    }
    private void ErrorHandler(Exception exception)
    {
        _logger.LogError(exception.Message);
    }
}
