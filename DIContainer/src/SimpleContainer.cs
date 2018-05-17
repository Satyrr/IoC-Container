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

        class InstanceCreator : IInstanceCreator
        {
            Type _type;
            public InstanceCreator(Type type)
            {
                _type = type;
            }
            public object GetInstance()
            {
                return Activator.CreateInstance(_type);
            }
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
                _registeredTypes[typeof(T)] = new SingletonInstanceCreator(typeof(T));
        }

        public void RegisterType<From, To>(bool Singleton) where To : class, From
        {
            if (Singleton)
                _registeredTypes[typeof(From)] = new SingletonInstanceCreator(typeof(To));
            else
                _registeredTypes[typeof(From)] = new InstanceCreator(typeof(To));
        }

        public T Resolve<T>()
        {   
            if(_registeredTypes.ContainsKey(typeof(T)))
            {
                return (T)_registeredTypes[typeof(T)].GetInstance();
            }
            if (typeof(T).IsInterface)
                throw new InvalidResolveTypeException("This type of interface has not been registered.");
            return (T)Activator.CreateInstance(typeof(T)); 
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
