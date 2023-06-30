using Models.Baselinker.Request;
using Models.Faire;
using System.Collections.Generic;

namespace Helpers
{
    public static class Mapper
    {
        public static NewOrder ToBaselinkerNewOrder(Order order)
        {
            var newOrder = new NewOrder
            {
                
                DateAdd = DateTimeOffset.Parse(order.CreatedAt).ToUnixTimeSeconds().ToString(),
                
                DeliveryAddress = order.Address.Address1 + order.Address.Address2,
                Phone = order.Address.PhoneNumber,
                DeliveryFullname = order.Address.Name,
                DeliveryCompany = order.Address.CompanyName,
                Currency = "USD",
                DeliveryPostcode = order.Address.PostalCode,
                DeliveryCity = order.Address.City,
                DeliveryState = order.Address.State,
                DeliveryCountryCode = order.Address.CountryCode,
                
                InvoiceAddress = order.Address.Address1 + order.Address.Address2,
                InvoiceCity = order.Address.City,
                InvoiceState = order.Address.State,
                InvoiceCompany = order.Address.CompanyName,
                InvoicePostcode = order.Address.PostalCode,
                InvoiceCountryCode = order.Address.CountryCode,
                InvoiceFullname = order.Address.Name,
                Products = new List<Product>()
            };

            order.Items.ForEach(item =>
            {
                newOrder.Products.Add(new Product
                {
                    Name = item.ProductName,
                    PriceBrutto = item.PriceCents,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Sku = item.Sku
                });
            });
            return newOrder;
        }
    }
}