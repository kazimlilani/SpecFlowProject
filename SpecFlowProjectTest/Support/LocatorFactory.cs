using System.Xml.Linq;
using OpenQA.Selenium;

namespace SpecFlowProjectTest.Support
{
    public static class LocatorFactory
    {
        private static readonly string xmlPath = "ElementFactory.xml";
        private static readonly XElement root = XElement.Load(xmlPath);

        public static By GetLocator(string elementName, params object[] args)
        {
            var element = root.Elements("Element")
                .FirstOrDefault(e => e.Attribute("name")?.Value == elementName);

            if (element == null)
                throw new Exception($"Locator '{elementName}' not found in XML.");

            var type = element.Attribute("type")?.Value;
            var value = element.Value;

            if (args.Length > 0)
                value = string.Format(value, args);

            return type switch
            {
                "CssSelector" => By.CssSelector(value),
                "XPath" => By.XPath(value),
                "Id" => By.Id(value),
                "ClassName" => By.ClassName(value),
                _ => throw new Exception($"Locator type '{type}' not supported.")
            };
        }
    }
}