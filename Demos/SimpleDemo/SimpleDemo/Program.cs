using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDemo
{
    class Program
    {
        private static Injector _injector;

        static void Main(string[] args)
        {
            _injector = new Injector();

            _injector.Register<IEnumerable<IShippingProvider>>(new List<IShippingProvider>
            {
                new FedExShipper(), new UpsShipper(), new UspsShipper()
            });
            _injector.Register<IShippingController,ShippingController>();


            var processor = _injector.Resolve<OrderProcessor>();
            var order = new Order() {Shipper = "FedEx"};
            processor.ProcessOrder(order);

            //var processor = new OrderProcessor().WithShippingController(shippingController);
            //processor.ShippingController = shippingController;
        }
    }
    
    public class OrderProcessor : INeedAShippingController
    {
        public IShippingController ShippingController { get; set; }

        //public OrderProcessor()
        //{
        //}

        public OrderProcessor(IShippingController shippingController)
        {
            ShippingController = shippingController;
        }

        public OrderProcessor WithShippingController(IShippingController controller)
        {
            ShippingController = controller;
            return this;
        }

        public void ProcessOrder(Order o)
        {
            ShippingController.ShipOrder(o);
        }
    }


    public interface INeedAShippingController
    {
        OrderProcessor WithShippingController(IShippingController controller);
    }


    public static class ShippingCControllerFactory
    {
        public static IShippingController CreateShippingController()
        {
            return new ShippingController(null);
        }
    }

    public static class ServiceLocator
    {
        public static T Locate<T>()
        {
            return Activator.CreateInstance<T>();
        }
    }

    public interface IShippingController
    {
        void ShipOrder(Order order);
    }

    public class ShippingController : IShippingController
    {
        public IEnumerable<IShippingProvider> ShippingProviders { get; set; }

        public ShippingController(IEnumerable<IShippingProvider> shippingProviders)
        {
            ShippingProviders = shippingProviders;
        }

        public void ShipOrder(Order order)
        {
            ShippingProviders.First(s=>s.Name == order.Shipper).Ship(order);
        }
    }

    public interface IShippingProvider
    {
        void Ship(Order o);
        string Name { get; }
    }

    public class Order
    {
        public string Shipper { get; set; }
    }

    public class UpsShipper : IShippingProvider
    {
        public void ShipAPackage()
        {
        }

        public void Ship(Order o)
        {
            Console.WriteLine("Shipped via " + Name );
        }

        public string Name { get { return "UPS"; } }
    }

    public class FedExShipper : IShippingProvider
    {
        public string GetATrackingNumber()
        {
            return "";
        }

        public void Ship(Order o)
        {
            Console.WriteLine("Shipped via " + Name);
        }

        public string Name { get { return "FedEx"; } }
    }

    public class UspsShipper : IShippingProvider
    {
        public void ShipSlower()
        {
            
        }

        public void Ship(Order o)
        {
            Console.WriteLine("Shipped via " + Name);
        }

        public string Name { get { return "USPS"; } }
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
}
