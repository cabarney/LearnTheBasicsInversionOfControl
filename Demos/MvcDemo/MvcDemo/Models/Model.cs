using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcDemo.Models
{
    public class OrderProcessor : IOrderProcessor
    {
        private readonly IEnumerable<IShippingService> _shippers;

        public OrderProcessor(IEnumerable<IShippingService> shippers)
        {
            _shippers = shippers;
        }

        public void ProcessOrder(Order order)
        {
            _shippers.Single(x => x.Key == order.ShippingMethodKey).ShipOrder(order);
        }
    }

    public interface IOrderProcessor
    {
        void ProcessOrder(Order order);
    }

    public interface IShippingService
    {
        void ShipOrder(Order order);
        string Key { get; set; }
        string Name { get; set; }
    }

    public class UpsShippingService : IShippingService
    {
        public UpsShippingService()
        {
            Name = "UPS";
            Key = "UPS";
        }

        public string Key { get; set; }
        public string Name { get; set; }

        public void ShipOrder(Order order)
        {
            // Do shipping stuff
        }
    }
    public class FedExShippingService : IShippingService
    {
        public FedExShippingService()
        {
            Name = "FedEx";
            Key = "FedEx";
        }
        public string Key { get; set; }
        public string Name { get; set; }

        public void ShipOrder(Order order)
        {
            // Do shipping stuff
        }
    }

    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string ShippingMethodKey { get; set; }

        // More order-related stuff
    }
}