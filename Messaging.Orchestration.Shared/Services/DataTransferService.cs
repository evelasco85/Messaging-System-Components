using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Messaging.Orchestration.Shared.Models;
using Messaging.Utilities;

namespace Messaging.Orchestration.Shared.Services
{
    public interface IDataTransferService
    {
        IObjectInformation<TVersion> GetObjectInformation<TVersion, TObject>(QueueTypeEnum queueType, Guid id,
            TVersion version);
    }

    public class DataTransferService : Singleton<DataTransferService, IDataTransferService>, IDataTransferService
    {
        private DataTransferService()
        {
        }

        public IObjectInformation<TVersion> GetObjectInformation<TVersion, TObject>(QueueTypeEnum queueType, Guid id, TVersion version)
        {
            PropertyInfo[] properties = GetProperties<TObject>();
            ConstructorInfo[] constructorInfos = GetConstructorParameters<TObject>();
            IDictionary<string, string> propertyDescriptions = ConvertToFriendlyPropertyDictionary(properties);
            IList<IDictionary<string, string>> constructorsParams = ConvertToFriendlyConstructorParamDictionary(constructorInfos);

            IObjectInformation<TVersion> objectInformation = new ObjectInformationInformation<TObject, TVersion>(
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

            for (int index = 0; index < properties.Count(); index++)
            {
                PropertyInfo property = properties[index];
                propertyDescriptions.Add(property.Name, property.PropertyType.FullName);
            }

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
    }
}
