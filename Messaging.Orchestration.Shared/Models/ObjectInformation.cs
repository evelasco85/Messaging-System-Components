using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Orchestration.Shared.Models
{
    public interface IObjectInformation<TVersion>
    {
        IVersionInfo<TVersion> VersionInfo { get; set; }
        IList<IDictionary<string, string>> ConstructorParameters { get; set; }
        IDictionary<string, string> Properties { get; set; }
    }

    public class ObjectInformationInformation<TObject, TVersion> : IObjectInformation<TVersion>
        where TObject: IVersionInfo<TVersion>
    {
        public IVersionInfo<TVersion> VersionInfo { get; set; }
        public IList<IDictionary<string, string>> ConstructorParameters { get; set; }
        public IDictionary<string, string> Properties { get; set; }

        public ObjectInformationInformation(TObject obj)
        {
            if(obj == null)
                throw  new ArgumentNullException("'obj' constructor parameter requires a value");

            this.VersionInfo = obj;
            ConstructorParameters = new List<IDictionary<string, string>>();
            Properties = new Dictionary<string, string>();
        }
    }
}
