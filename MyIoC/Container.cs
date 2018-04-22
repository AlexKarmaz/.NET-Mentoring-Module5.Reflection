using MyIoC.Attributes;
using MyIoC.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyIoC
{
    public class Container
    {
        private readonly IDictionary<Type, Type> typesDictionary;
        private readonly IActivator activator;

        public Container(IActivator activator)
        {
            this.activator = activator;
            this.typesDictionary = new Dictionary<Type, Type>();
        }

        public void AddAssembly(Assembly assembly)
        {
            var types = assembly.ExportedTypes;
            foreach (var type in types)
            {
                var constructorImportAttribute = type.GetCustomAttribute<ImportConstructorAttribute>();
                if (constructorImportAttribute != null || HasImportProperties(type))
                {
                    typesDictionary.Add(type, type);
                }

                var exportAttributes = type.GetCustomAttributes<ExportAttribute>();
                foreach (var exportAttribute in exportAttributes)
                {
                    typesDictionary.Add(exportAttribute.Type ?? type, type);
                }
            }
        }

        public void AddType(Type type)
        {
            typesDictionary.Add(type, type);
        }

        public void AddType(Type type, Type baseType)
        {
            typesDictionary.Add(baseType, type);
        }

        public object CreateInstance(Type type)
        {
            var instance = ConstructInstanceOfType(type);
            return instance;
        }

        public T CreateInstance<T>()
        {
            var type = typeof(T);
            var instance = (T)ConstructInstanceOfType(type);
            return instance;
        }

        private bool HasImportProperties(Type type)
        {
            var propertiesInfo = type.GetProperties().Where(p => p.GetCustomAttribute<ImportAttribute>() != null);
            return propertiesInfo.Any();
        }

        private object ConstructInstanceOfType(Type type)
        {
            if (!typesDictionary.ContainsKey(type))
            {
                throw new IoCException($"Cannot create instance of {type.FullName}.");
            }

            Type dependendType = typesDictionary[type];
            ConstructorInfo constructorInfo = GetConstructor(dependendType);
            object instance = CreateFromConstructor(dependendType, constructorInfo);

            if (dependendType.GetCustomAttribute<ImportConstructorAttribute>() != null)
            {
                return instance;
            }

            ResolveProperties(dependendType, instance);
            return instance;
        }

        private ConstructorInfo GetConstructor(Type type)
        {
            ConstructorInfo[] constructors = type.GetConstructors();

            if (constructors.Length == 0)
            {
                throw new IoCException($"There are no public constructors for type {type.FullName}");
            }

            return constructors.First();
        }

        private object CreateFromConstructor(Type type, ConstructorInfo constructorInfo)
        {
            ParameterInfo[] parameters = constructorInfo.GetParameters();
            List<object> parametersInstances = new List<object>(parameters.Length);
            Array.ForEach(parameters, p => parametersInstances.Add(ConstructInstanceOfType(p.ParameterType)));

            object instance = activator.CreateInstance(type, parametersInstances.ToArray());
            return instance;
        }

        private void ResolveProperties(Type type, object instance)
        {
            var propertiesInfo = type.GetProperties().Where(p => p.GetCustomAttribute<ImportAttribute>() != null);
            foreach (var property in propertiesInfo)
            {
                var resolvedProperty = ConstructInstanceOfType(property.PropertyType);
                property.SetValue(instance, resolvedProperty);
            }
        }
    }
}
