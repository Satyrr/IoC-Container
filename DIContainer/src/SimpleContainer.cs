using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainer
{
    public class SimpleContainer
    {
        interface IInstanceGetter
        {
            object GetInstance();
        }

        class RegisteredInstanceGetter : IInstanceGetter
        {
            object _instance;
            public RegisteredInstanceGetter(object instance)
            {
                _instance = instance;
            }
            public object GetInstance()
            {
                return _instance;
            }
        }

        class BasicInstanceGetter : IInstanceGetter
        {
            Type _type;
            public BasicInstanceGetter(Type type)
            {
                _type = type;
            }
            public object GetInstance()
            {
                return Activator.CreateInstance(_type);
            }
        }

        class SingletonInstanceGetter : IInstanceGetter
        {
            object _instance;
            public SingletonInstanceGetter(Type type)
            {
               this._instance = Activator.CreateInstance(type);
            }

            public object GetInstance()
            {
                return this._instance;
            }
        }

        Dictionary<Type, IInstanceGetter> _registeredTypes = new Dictionary<Type, IInstanceGetter>();

        public void RegisterType<T>(bool Singleton) where T : class
        {
            if(Singleton)
                _registeredTypes[typeof(T)] = new SingletonInstanceGetter(typeof(T));
        }

        public void RegisterType<From, To>(bool Singleton) where To : class, From
        {
            if (Singleton)
                _registeredTypes[typeof(From)] = new SingletonInstanceGetter(typeof(To));
            else
                _registeredTypes[typeof(From)] = new BasicInstanceGetter(typeof(To));
        }

        public void RegisterInstance<T>(T Instance)
        {
            _registeredTypes[typeof(T)] = new RegisteredInstanceGetter(Instance);
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
