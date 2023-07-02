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
    private readonly DefaultValues _defaultValues;
    public Transfer(ILoggerFactory loggerFactory, IBaselinkerService baselinkerService,
        IFaireService faireService, IStorageService storageService, DefaultValues defaultValues)
    {
        _logger = loggerFactory.CreateLogger<Transfer>();
        _baselinkerService = baselinkerService;
        _faireService = faireService;
        _storageService = storageService;
        _defaultValues = defaultValues;
    }

    /// <summary>
    /// Transfers data from Faire to Baselinker.
    /// Runs every 10 minutes, transfer 50 records.
    /// It starts with the orders that have been updated the earliest
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

        var faireOrders = await _faireService.GetOrdersAsync(50, 1,
            _storageService.Get<DateTimeOffset>("lastUpdatedDate"));

        _storageService.Set("lastUpdatedDate", faireOrders.Last().UpdatedAt);
        _logger.LogInformation("Retrieved orders from Faire and updated the last updated date in the storage.");


        await RemoveOrdersThatAlreadyExistsAsync(faireOrders);
        _logger.LogInformation("Removed existing orders.");

        var newBaselinkerOrders = faireOrders.Select(order =>
        {
            var newOrder = Mapper.ToBaselinkerNewOrder(order);
            newOrder.OrderStatusId = _defaultValues.StatusId;
            newOrder.CustomSourceId = _defaultValues.SourceId;
            newOrder.ExtraField1 = order.Id;
            return newOrder;
        }).ToList();

        newBaselinkerOrders.ForEach(newOrder => _baselinkerService.AddOrderAsync(newOrder));
        _logger.LogInformation("Added new orders to the Baselinker system.");
    }

    /// <summary>
    /// Removes orders that are already in Baselinker's system.
    /// </summary>
    /// <param name="faireOrders">The collection of Faire orders to process.</param>
    private async Task RemoveOrdersThatAlreadyExistsAsync(ICollection<FaireOrder> faireOrders)
    {
        // Select orders where the create date is different from the update date
        var updatedOrders = faireOrders
            .Where(order => order.UpdatedAt != order.CreatedAt)
            .OrderBy(order => order.CreatedAt)
            .ToList();

        while (updatedOrders.Any())
        {
            //Retrieve orders added after the minimum date of the remaining updated orders
            var baselinkerOffers = await _baselinkerService.GetOrdersAsync(updatedOrders.First().CreatedAt,
                _defaultValues.StatusId, _defaultValues.SourceId);

            //Filter the updated orders to match the range of fetched offers from Baselinker
            //According to documentation, baselinker should return orders sorted by date_from
            var updatedOrdersInRange = updatedOrders.Where(updatedOrder =>
                updatedOrder.CreatedAt <= baselinkerOffers.Last().DateAdd);

            foreach (var updatedOrderInRange in updatedOrdersInRange)
            {
                //Remove the processed order from the list
                updatedOrders.Remove(updatedOrderInRange);

                //Check if the order already exists in the Baselinker system
                if (baselinkerOffers.Any(baselinkerOffer => baselinkerOffer.ExtraField1 == updatedOrderInRange.Id))
                    faireOrders.Remove(updatedOrderInRange);
            }
        }
    }

    private void ErrorHandler(Exception exception)
    {
        _logger.LogError(exception.Message);
        throw exception;
    }
}
