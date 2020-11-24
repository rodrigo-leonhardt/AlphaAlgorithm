using System;
using System.Xml;

namespace AlphaMiner
{
    public class MXMLFile
    {
        public static EventLog LoadEventLog(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);
            var root = doc.DocumentElement;

            var process = root.GetChildNode("Process");
            var processId = process.GetAttributeString("id");
            var processDescription = process.GetAttributeString("description");

            var eventLog = new EventLog();
            eventLog.Process = processId ?? "";
            eventLog.Description = processDescription ?? "";

            var processInstances = process.GetChildNodes("ProcessInstance");

            foreach (XmlNode processInstance in processInstances)
            {
                var processInstanceId = processInstance.GetAttributeString("id");
                var processData = processInstance.GetChildNode("Data");
                var auditTrailEntries = processInstance.GetChildNodes("AuditTrailEntry");


                var eventLogCase = eventLog.AddCase();
                eventLogCase.Id = processInstanceId ?? Guid.NewGuid().ToString();

                foreach (XmlNode attributeData in processData.GetChildNodes("Attribute"))
                {
                    eventLogCase.Attributes.Add(attributeData.GetAttributeString("name"), attributeData.InnerText);
                }

                foreach (XmlNode auditTrailEntry in auditTrailEntries)
                {
                    var workflowModelElement = auditTrailEntry.GetChildNode("WorkflowModelElement")?.InnerText;
                    var eventType = auditTrailEntry.GetChildNode("EventType")?.InnerText;
                    var timestamp = auditTrailEntry.GetChildNode("Timestamp")?.InnerText;
                    var originator = auditTrailEntry.GetChildNode("Originator")?.InnerText;
                    var eventData = auditTrailEntry.GetChildNode("Data");

                    var eventLogCaseEvent = eventLogCase.AddEvent();
                    eventLogCaseEvent.Id = Guid.NewGuid().ToString();
                    eventLogCaseEvent.Activity = workflowModelElement ?? "";
                    eventLogCaseEvent.Type = (EventType)Enum.Parse(typeof(EventType), eventType ?? "unknown", true);
                    eventLogCaseEvent.Timestamp = XmlConvert.ToDateTime(timestamp, XmlDateTimeSerializationMode.Utc);

                    foreach (XmlNode attributeData in eventData.GetChildNodes("Attribute"))
                    {
                        eventLogCaseEvent.Attributes.Add(attributeData.GetAttributeString("name"), attributeData.InnerText);
                    }

                }

            }

            return eventLog;
        }

    }

    public static class XmlAttributeCollectionExtensions
    {
        public static string GetAttributeString(this XmlNode xmlNode, string name, string defaultValue = null)
        {
            var node = xmlNode.Attributes.GetNamedItem(name);

            if (node != null)
                return node.Value;

            return defaultValue;
        }

        public static bool GetAttributeBoolean(this XmlNode xmlNode, string name, bool defaultValue = false)
        {
            var node = xmlNode.Attributes.GetNamedItem(name);

            if (node != null)
                return node.Value.ToLower() == "true";

            return defaultValue;
        }

        public static int GetAttributeInt(this XmlNode xmlNode, string name, int defaultValue = 0)
        {
            var node = xmlNode.Attributes.GetNamedItem(name);

            if (node != null)
            {
                int.TryParse(node.Value, out int iResult);
                return iResult;
            }

            return defaultValue;
        }

        public static XmlNode GetChildNode(this XmlNode xmlNode, string name)
        {
            return xmlNode.SelectSingleNode($"child::*[local-name()='{name}']");
        }

        public static XmlNodeList GetChildNodes(this XmlNode xmlNode, string name)
        {
            return xmlNode.SelectNodes($"child::*[local-name()='{name}']");
        }
    }

}
