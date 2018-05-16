using NUnit.Framework;
using DIContainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestFixture]
    public class DITests
    {
        [Test]
        public void SingletonInstanceCreationTest()
        {
            SimpleContainer container = new SimpleContainer();
            container.RegisterType<Foo>(true);

            Foo f1 = container.Resolve<Foo>();
            Foo f2 = container.Resolve<Foo>();

            Assert.AreSame(f1, f2);
        }

        [Test]
        public void NotSingletonInstanceCreationTest()
        {
            SimpleContainer container = new SimpleContainer();
            container.RegisterType<Foo>(false);

            Foo f1 = container.Resolve<Foo>();
            Foo f2 = container.Resolve<Foo>();

            Assert.AreNotSame(f1, f2);
        }

        [Test]
        public void InstanceCreationTest()
        {
            SimpleContainer container = new SimpleContainer();

            Foo f1 = container.Resolve<Foo>();

            Assert.IsInstanceOf<Foo>(f1);
        }

    }



    class Foo
    {

    }
}
