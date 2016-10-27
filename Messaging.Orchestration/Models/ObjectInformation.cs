using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Orchestration.Models
{
    public interface IObjectInformation<TVersion>
    {
        IVersionInfo<TVersion> VersionInfo { get; set; }
        IList<IDictionary<string, string>> ConstructorParameters { get; set; }
        IDictionary<string, string> Properties { get; set; }
    }

    public class ObjectInformationInformation<TObject, TVersion> : IObjectInformation<TVersion>
    {
        public IVersionInfo<TVersion> VersionInfo { get; set; }
        public IList<IDictionary<string, string>> ConstructorParameters { get; set; }
        public IDictionary<string, string> Properties { get; set; }

        public ObjectInformationInformation(IVersionInfo<TVersion> versionInfo)
        {
            VersionInfo = versionInfo;
            ConstructorParameters = new List<IDictionary<string, string>>();
            Properties = new Dictionary<string, string>();
        }
    }
}
