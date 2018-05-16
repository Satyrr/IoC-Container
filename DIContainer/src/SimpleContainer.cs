using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainer
{

    

    public class SimpleContainer
    {
        interface IInstanceCreator
        {
            object GetInstance();
        }

        class SingletonInstanceCreator : IInstanceCreator
        {
            object _instance;
            public SingletonInstanceCreator(Type type)
            {
               this._instance = Activator.CreateInstance(type);
            }

            public object GetInstance()
            {
                return this._instance;
            }
        }

        Dictionary<Type, IInstanceCreator> _registeredTypes = new Dictionary<Type, IInstanceCreator>();

        public void RegisterType<T>(bool Singleton) where T : class
        {
            if(Singleton)
            {
                _registeredTypes[typeof(T)] = new SingletonInstanceCreator(typeof(T));
            }
        }

        public void RegisterType<From, To>(bool Singleton) where To : From
        {
            throw new NotImplementedException();
        }

        public T Resolve<T>()
        {
            if(_registeredTypes.ContainsKey(typeof(T)))
            {
                return (T)_registeredTypes[typeof(T)].GetInstance();
            }

            return (T)Activator.CreateInstance(typeof(T)); 

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
