using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace SimpleDemo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var sampleOrder = new Order {Id = 1, OrderNumber = "ABC-555-0001", ShippingMethodKey = "UPS"};

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

    public class OrderProcessor : IOrderProcessor
    {
        private readonly IEnumerable<IShippingService> _shippers;

        public OrderProcessor(IEnumerable<IShippingService> shippers)
        {
            _shippers = shippers;
        }

        public void ProcessOrder(Order order)
        {
            _shippers.Single(x=>x.Key == order.ShippingMethodKey).ShipOrder(order);
        }
    }

    public static class Factory
    {
        public static IShippingService CreateUpsShippingService()
        {
            return new UpsShippingService();
        }
        public static IShippingService CreateFedExShippingService()
        {
            return new FedExShippingService();
        }

        public static IOrderProcessor CreatOrderProcessor()
        {
            var shippers = new List<IShippingService>
            {
                CreateUpsShippingService(),
                CreateFedExShippingService()
            };
            return new OrderProcessor(shippers);
        }
    }

    //public static class ServiceLocator
    //{
    //    public static T GetService<T>() where T: class
    //    {
    //        switch (typeof (T).Name)
    //        {
    //            case "IShippingService":
    //                return Factory.CreateUpsShippingService() as T;
    //            case "IOrderProcessor":
    //                return new OrderProcessor() as T;
    //        }
    //        return null;
    //    }
    //}


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
   

    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string ShippingMethodKey { get; set; }

        // More order-related stuff
    }
}
