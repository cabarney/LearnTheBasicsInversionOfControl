namespace SimpleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var sampleOrder = new Order { Id = 1, OrderNumber = "ABC-555-0001", ShippingMethodKey = "UPS" };
            var orderProcessor = new OrderProcessor();
            orderProcessor.ProcessOrder(sampleOrder);
        }   
    }

    public class OrderProcessor
    {
        private UpsShippingService _shippingService;

        public OrderProcessor()
        {
            _shippingService = new UpsShippingService();
        }

        public void ProcessOrder(Order order)
        {
            _shippingService.ShipOrder(order);
        }
    }

    public class UpsShippingService
    {
        public string Key { get { return "UPS"; } }
        public string Name { get { return "United Parcel Service"; } }

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
