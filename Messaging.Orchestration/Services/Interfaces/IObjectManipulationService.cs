using Messaging.Orchestration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Orchestration.Shared.Services.Interfaces
{
    public interface IObjectManipulationService
    {
        IObjectInformation<TVersion> GetObjectInformation<TVersion, TObject>(QueueTypeEnum queueType, Guid id,
            TVersion version);

        IDictionary<string, object> CreateDictionaryWithEmptyValues(IDictionary<string, string> dictionary);

        TObject InstantiateObject<TObject>(
            IDictionary<string, object> constructorKVP,
            IDictionary<string, object> publicPropertiesKVP);
    }
}
