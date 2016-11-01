using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Messaging.Orchestration.Shared
{
    public interface IConfigurationLoader
    {
        void LoadXml(string xml);
        IList<Tuple<string, string>> GetConfiguration(string clientId);
    }

    public class ConfigurationLoader : IConfigurationLoader
    {
        XmlDocument _configuration = new XmlDocument();

        public ConfigurationLoader()
        {
        }

        public ConfigurationLoader(string xmlPath)
        {
            _configuration.Load(xmlPath);
        }

        public void LoadXml(string xml)
        {
            _configuration.LoadXml(xml);
        }

        public IList<Tuple<string, string>> GetConfiguration(string clientId)
        {
            IList<Tuple<string, string>> configurations = new List<Tuple<string, string>>();
            string configurationPath =
                string.Format("//clients/client[@id='{0}']/group/config", clientId);

            XmlNodeList configNodes = _configuration.SelectNodes(configurationPath);

            for(int index = 0; index < configNodes.Count; index++)
            {
                XmlNode config = configNodes[index];

                if (config == null)
                    continue;
                
                Tuple<string, string> configuration = new Tuple<string, string>(
                    GetAttributeValue(config.Attributes, "name"),
                    GetAttributeValue(config.Attributes, "value")
                    );

                configurations.Add(configuration);
            }

            return configurations;
        }

        string GetAttributeValue(XmlAttributeCollection attributes, string attributeName)
        {
            string value = string.Empty;

            if (attributes[attributeName] != null)
                value = attributes[attributeName].Value;

            return value;
        }
    }
}
