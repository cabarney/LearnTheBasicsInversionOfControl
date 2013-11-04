using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var sampleOrder = new Order { Id = 1, OrderNumber = "ABC-555-0001", ShippingMethodKey = "UPS" };

            var ioc = new Injector();
            ioc.Register<IEnumerable<IShippingService>>(new List<IShippingService>
            {
                new UpsShippingService(),
                new FedExShippingService()
            });
            ioc.Register<IOrderProcessor, OrderProcessor>();

            var processor = ioc.Resolve<IOrderProcessor>();
            processor.ProcessOrder(sampleOrder);
        }   
    }

    public class Foo
    {
    }

    public class Injector
    {
        private readonly Dictionary<Type, Func<object>> _providers = new Dictionary<Type, Func<object>>();

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        private object Resolve(Type type)
        {
            Func<object> provider;
            if (_providers.TryGetValue(type, out provider))
                return provider();
            return ResolveByType(type);
        }

        private object ResolveByType(Type type)
        {
            var ctor = type.GetConstructors().SingleOrDefault();
            if (ctor == null)
                return Activator.CreateInstance(type);
            var args = ctor.GetParameters().Select(p => Resolve(p.ParameterType)).ToArray();
            return ctor.Invoke(args);
        }


        public void Register<T, TImpl>() where TImpl : T
        {
            _providers[typeof(T)] = () => ResolveByType(typeof(TImpl));
        }

        public void Register<T>(T instance)
        {
            _providers[typeof(T)] = () => instance;
        }
    }



    public interface IOrderProcessor
    {
        void ProcessOrder(Order order);
    }

    public class OrderProcessor : IOrderProcessor
    {
        private IEnumerable<IShippingService> _shippingServices;

        public OrderProcessor(IEnumerable<IShippingService> shippingServices, Foo bar)
        {
            _shippingServices = shippingServices;
        }

        public void ProcessOrder(Order order)
        {
            _shippingServices.Single(x=>x.Key==order.ShippingMethodKey).ShipOrder(order);
        }
    }

    public interface IShippingService
    {
        string Key { get; }
        string Name { get; }
        void ShipOrder(Order order);
    }

    public class UpsShippingService : IShippingService
    {
        public string Key { get { return "UPS"; } }
        public string Name { get { return "United Parcel Service"; } }

        public void ShipOrder(Order order)
        {
            // Do shipping stuff
        }
    }
    public class FedExShippingService : IShippingService
    {
        public string Key { get { return "FedEx"; } }
        public string Name { get { return "Federal Express"; } }

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
