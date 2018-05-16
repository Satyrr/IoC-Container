using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainer
{
    public class SimpleContainer
    {
        public void RegisterType<T>(bool Singleton) where T : class
        {
            throw new NotImplementedException();
        }

        public void RegisterType<From, To>(bool Singleton) where To : From
        {
            throw new NotImplementedException();
        }

        public T Resolve<T>()
        {
            throw new NotImplementedException();
        }
    }

    public class InvalidResolveTypeException : Exception
    {
        public InvalidResolveTypeException()
        {
        }

        public InvalidResolveTypeException(string message)
            : base(message)
        {
        }

        public InvalidResolveTypeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
