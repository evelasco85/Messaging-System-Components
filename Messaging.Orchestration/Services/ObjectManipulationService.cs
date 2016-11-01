using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Messaging.Orchestration.Models;
using Messaging.Utilities;
using Messaging.Orchestration.Shared.Services.Interfaces;

namespace Messaging.Orchestration.Services
{
    public class ObjectManipulationService : Singleton<ObjectManipulationService, IObjectManipulationService>, IObjectManipulationService
    {
        private ObjectManipulationService()
        {
        }

        public IObjectInformation<TVersion> GetObjectInformation<TVersion, TObject>(QueueTypeEnum queueType, Guid id, TVersion version)
        {
            PropertyInfo[] properties = GetProperties<TObject>();
            ConstructorInfo[] constructorInfos = GetConstructorParameters<TObject>();
            IDictionary<string, string> propertyDescriptions = ConvertToFriendlyPropertyDictionary(properties);
            IList<IDictionary<string, string>> constructorsParams = ConvertToFriendlyConstructorParamDictionary(constructorInfos);

            IObjectInformation<TVersion> objectInformation = new ObjectInformation<TVersion>(
                new VersionInfo<TVersion>(queueType, id, version)
                )
            {
                ConstructorParameters = constructorsParams,
                Properties = propertyDescriptions
            };

            return objectInformation;
        }

        PropertyInfo[] GetProperties<T>()
        {
            Type anyObjectType = typeof(T);

            PropertyInfo[] properties = anyObjectType
                .GetProperties(BindingFlags.Instance |BindingFlags.Public)
                .Where(prop => (prop.CanRead && prop.CanWrite))
                .ToArray();

            return properties;
        }

        ConstructorInfo[] GetConstructorParameters<T>()
        {
            Type concreteType = typeof(T);

            ConstructorInfo[] constructorInfoList = concreteType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

            return constructorInfoList;
        }

        IDictionary<string, string> ConvertToFriendlyPropertyDictionary(PropertyInfo[] properties)
        {
            IDictionary<string, string> propertyDescriptions = new Dictionary<string, string>();

            if (properties == null)
                return propertyDescriptions;

            propertyDescriptions = properties
                .ToDictionary(kvp => kvp.Name, kvp => kvp.PropertyType.FullName);
            
            return propertyDescriptions;
        }

        IList<IDictionary<string, string>> ConvertToFriendlyConstructorParamDictionary(ConstructorInfo[] constructorInfos)
        {
            IList<IDictionary<string, string>> constructorDescriptions = new List<IDictionary<string, string>>();

            if (constructorInfos == null)
                return constructorDescriptions;

            constructorDescriptions = constructorInfos
                .Select(info =>
                    (IDictionary<string, string>)info
                        .GetParameters()
                        .ToDictionary(param => param.Name, param => param.ParameterType.FullName)
                )
                .ToList();

            return constructorDescriptions;
        }

        public IDictionary<string, object> CreateDictionaryWithEmptyValues(IDictionary<string, string> dictionary)
        {
            IDictionary<string, object> emptyValueDictionary = dictionary
                .ToDictionary(kvp => kvp.Key, kvp => default(object));

            return emptyValueDictionary;
        }

        public TObject InstantiateObject<TObject>(
            IDictionary<string, object> constructorKVP,
            IDictionary<string, object> publicPropertiesKVP)
        {
            ConstructorInfo[] constInfoArray = GetConstructorParameters<TObject>();
            ConstructorInfo matchedConstInfo = constInfoArray
                .Where(constInfo =>
                    (constInfo.GetParameters().Length == constructorKVP.Count) &&
                    (constInfo.GetParameters().Any(x => constructorKVP.ContainsKey(x.Name)))
                    )
                .DefaultIfEmpty(null)
                .FirstOrDefault();

            if (matchedConstInfo == null)
                return default(TObject);

            object[] constructorParameters = matchedConstInfo
                .GetParameters()
                .Select(param => constructorKVP[param.Name])
                .ToArray();

            TObject instance = (TObject) matchedConstInfo.Invoke(
                (BindingFlags.Instance | BindingFlags.Public),
                null,
                constructorParameters,
                null);

            PropertyInfo[] properties = GetProperties<TObject>();

            for (int index = 0; index < properties.Length; index++)
            {
                PropertyInfo property = properties[index];

                if(property == null)
                    continue;

                object value = publicPropertiesKVP[property.Name];

                property.SetValue(instance, value);
            }

            return instance;
        }
    }
}
