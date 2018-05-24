using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DIContainer
{
    public class SimpleContainer
    {
        interface IInstanceGetter
        {
            object GetInstance(HashSet<Type> resolvedTypes);
        }

        class RegisteredInstanceGetter : IInstanceGetter
        {
            object _instance;
            public RegisteredInstanceGetter(object instance)
            {
                _instance = instance;
            }
            public object GetInstance(HashSet<Type> resolvedTypes)
            {
                return _instance;
            }
        }

        class BasicInstanceGetter : IInstanceGetter
        {
            SimpleContainer _sc;
            Type _type;
            public BasicInstanceGetter(SimpleContainer sc, Type type)
            {
                _sc = sc;
                _type = type;
            }
            public object GetInstance(HashSet<Type> resolvedTypes)
            {
                return _sc.CreateInstance(_type, resolvedTypes);
            }
        }

        class SingletonInstanceGetter : IInstanceGetter
        {
            SimpleContainer _sc;
            Type _type;
            object _instance;
            public SingletonInstanceGetter(SimpleContainer sc, Type type)
            {
                _sc = sc;
                _type = type;
            }
            public object GetInstance(HashSet<Type> resolvedTypes)
            {
                if (this._instance == null)
                {
                    this._instance = _sc.CreateInstance(_type, resolvedTypes);
                }
                return this._instance;
            }
        }

        Dictionary<Type, IInstanceGetter> _registeredTypes = new Dictionary<Type, IInstanceGetter>();

        public void RegisterType<T>(bool Singleton) where T : class
        {
            if(Singleton)
                _registeredTypes[typeof(T)] = new SingletonInstanceGetter(this, typeof(T));
        }

        public void RegisterType<From, To>(bool Singleton) where To : class, From
        {
            if (Singleton)
                _registeredTypes[typeof(From)] = new SingletonInstanceGetter(this, typeof(To));
            else
                _registeredTypes[typeof(From)] = new BasicInstanceGetter(this, typeof(To));
        }

        public void RegisterInstance<T>(T Instance)
        {
            _registeredTypes[typeof(T)] = new RegisteredInstanceGetter(Instance);
        }

        public T Resolve<T>()
        {
            return Resolve<T>(new HashSet<Type>());
        }

        T Resolve<T>(HashSet<Type> resolvedTypes)
        {
            try
            {
                if (_registeredTypes.ContainsKey(typeof(T)))
                {
                    return (T)_registeredTypes[typeof(T)].GetInstance(resolvedTypes);
                }
                if (typeof(T).IsInterface)
                    throw new InvalidResolveTypeException("This type of interface has not been registered.");
                return (T)CreateInstance(typeof(T), resolvedTypes);
            }
            catch(Exception e)
            {
                throw new InvalidResolveTypeException("Unable to resolve\n" + e.ToString());
            }
        }

        public object CreateInstance(Type type, HashSet<Type> resolvedTypes)
        {
            ConstructorInfo[] constructors = type.GetConstructors();
            var maxConstructors = constructors.Where(ci => ci.GetParameters().Count() ==
                    constructors.Max(x => x.GetParameters().Count()));
            if (maxConstructors.Count() > 1)
                throw new Exception("There is more than one constructor with the maximum number of parameters.");
            var constructor = maxConstructors.First();

            return InvokeConstructor(type, constructor, resolvedTypes);
        }
        public object InvokeConstructor(Type type, ConstructorInfo constructor, HashSet<Type> resolvedTypes)
        {
            return constructor.Invoke(
                constructor.GetParameters()
                .Aggregate(  // iterate over parameters, resolve each one, add resolved to List
                    new List<object>(),
                    (acc, param) => {
                        Type paramType = param.ParameterType;
                        if (resolvedTypes.Contains(paramType))
                            throw new Exception("There is a cycle in a resolving tree.");
                        else
                        {
                            HashSet<Type> childResolvedTypes = new HashSet<Type>(resolvedTypes);
                            childResolvedTypes.Add(type);
                            acc.Add(
                                this
                                .GetType()
                                .GetMethod("Resolve", BindingFlags.NonPublic | BindingFlags.Instance)
                                .MakeGenericMethod(paramType)
                                .Invoke(this, new object[] { childResolvedTypes })  
                            );
                            return acc;
                        }
                    }
                ).ToArray()
            );
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
