using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Messaging.Orchestration.Shared.Models;

namespace Messaging.Orchestration.Shared
{
    public interface IConfigurationLoader
    {
        void LoadXml(string xml);
        IList<Tuple<string, string>> GetConfiguration(string clientId, string groupId);
        void SetClientInfo(string clientId, string groupId, ref ServerMessage response);
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

        XmlNode GetClientNode(string clientId)
        {
            string configurationPath =
                string.Format("//clients/client[@id='{0}']", clientId);

            XmlNode clientNode = _configuration.SelectSingleNode(configurationPath);

            return clientNode;
        }

        XmlNode GetClientNode(string clientId, string groupId)
        {
            //string configurationPath =
            //    string.Format("//clients/client[@id='{0}']/group[@id='{1}']", clientId, groupId);

            //XmlNode clientNode = _configuration.SelectSingleNode(configurationPath);

            string configurationPath = string.Format("group[@id='{0}']", groupId);
            XmlNode clientNode = GetClientNode(clientId);
            XmlNode groupNode = clientNode.SelectSingleNode(configurationPath);

            //return clientNode;
            return groupNode;
        }

        public void SetClientInfo(string clientId, string groupId, ref ServerMessage response)
        {
            XmlNode clientNode = GetClientNode(clientId);

            if ((string.IsNullOrEmpty(clientId)) || (clientNode == null) || (response == null))
                return;

            response.ClientName = GetAttributeValue(clientNode.Attributes, "name");
        }

        public IList<Tuple<string, string>> GetConfiguration(string clientId, string groupId)
        {
            IList<Tuple<string, string>> configurations = new List<Tuple<string, string>>();
            XmlNode clientNode = GetClientNode(clientId, groupId);
            string configurationPath = "config";
            XmlNodeList configNodes = clientNode.SelectNodes(configurationPath);

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
