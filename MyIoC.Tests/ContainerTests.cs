using System;
using NUnit.Framework;
using System.Reflection;
using MyIoC.Tests.TestEntities;

namespace MyIoC.Tests
{
    [TestFixture]
    public class ContainerTests
    {
        private Container container;

        [SetUp]
        public void TestInit()
        {
            container = new Container(new DefaultActivator());
        }

        [Test]
        public void GenericCreateInstance_ConstructorInjectionTest()
        {
            container.AddAssembly(Assembly.GetExecutingAssembly());

            var customerBll = container.CreateInstance<CustomerBLL>();

            Assert.IsNotNull(customerBll);
            Assert.IsTrue(customerBll.GetType() == typeof(CustomerBLL));
        }

        [Test]
        public void GenericCreateInstance_PropertiesInjectionTest()
        {
            container.AddAssembly(Assembly.GetExecutingAssembly());

            var customerBll = container.CreateInstance<CustomerBLL2>();

            Assert.IsNotNull(customerBll);
            Assert.IsTrue(customerBll.GetType() == typeof(CustomerBLL2));
            Assert.IsNotNull(customerBll.CustomerDAL);
            Assert.IsNotNull(customerBll.CustomerDAL.GetType() == typeof(CustomerDAL));
            Assert.IsNotNull(customerBll.Logger);
            Assert.IsNotNull(customerBll.Logger.GetType() == typeof(Logger));
        }

        [Test]
        public void Not_GenericCreateInstance_ConstructorInjectionTest()
        {
            container.AddType(typeof(CustomerBLL));
            container.AddType(typeof(Logger));
            container.AddType(typeof(CustomerDAL), typeof(ICustomerDAL));

            var customerBll = (CustomerBLL)container.CreateInstance(typeof(CustomerBLL));

            Assert.IsNotNull(customerBll);
            Assert.IsTrue(customerBll.GetType() == typeof(CustomerBLL));
        }

        [Test]
        public void Not_GenericCreateInstance_PropertiesInjectionTest()
        {
            container.AddAssembly(Assembly.GetExecutingAssembly());

            var customerBll = container.CreateInstance<CustomerBLL2>();

            Assert.IsNotNull(customerBll);
            Assert.IsTrue(customerBll.GetType() == typeof(CustomerBLL2));
            Assert.IsNotNull(customerBll.CustomerDAL);
            Assert.IsNotNull(customerBll.CustomerDAL.GetType() == typeof(CustomerDAL));
            Assert.IsNotNull(customerBll.Logger);
            Assert.IsNotNull(customerBll.Logger.GetType() == typeof(Logger));
        }

        [Test]
        public void EmitActivator_ConstructorInjectionTest()
        {
            container = new Container(new EmitActivator());
            container.AddType(typeof(CustomerBLL));
            container.AddType(typeof(Logger));
            container.AddType(typeof(CustomerDAL), typeof(ICustomerDAL));

            var customerBll = container.CreateInstance<CustomerBLL>();

            Assert.IsNotNull(customerBll);
            Assert.IsTrue(customerBll.GetType() == typeof(CustomerBLL));
        }
    }
}
