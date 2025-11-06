// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.Diagnostics.CodeAnalysis;
// using System.IO;
// using System.Linq;
// using System.Runtime.CompilerServices;
// using System.Runtime.InteropServices;
// using System.Text.Json;
// using IPC2581A;
// using IPC2581B;
// using IPC2581B1;
// using IPC2581C;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace IPC2581Serializer;

public class Functions
{
    public static void Main()
    {
        // Enter Path to IPC-2581 XML file

        string AskForPath()
        {
            Console.Write("Enter path to IPC-2581 XML file: ");
            var xsdPath = Console.ReadLine();
            if (string.IsNullOrEmpty(xsdPath) || !File.Exists(xsdPath))
            {
                Console.WriteLine("Invalid file path.");
                return AskForPath();
            }
            return xsdPath;
        }
        string xmlFilePath = AskForPath();

        string xmlString = File.ReadAllText(xmlFilePath);

        string AskForVersion()
        {
            // Ask user for version to validate
            Console.WriteLine("Select version to validate:");
            Console.WriteLine("1. Rev A");
            Console.WriteLine("2. Rev B");
            Console.WriteLine("3. Rev B1");
            Console.WriteLine("4. Rev C");
            Console.Write("Enter choice (1-4): ");
            var choice = Console.ReadLine();
            if (string.IsNullOrEmpty(choice) || !"1234".Contains(choice))
            {
                Console.WriteLine("Invalid choice.");
                return AskForVersion();
            }
            string version = choice switch
            {
                "1" => "A",
                "2" => "B",
                "3" => "B1",
                "4" => "C",
                _ => throw new InvalidOperationException("Invalid choice"),
            };
            // return version;
            return version switch
            {
                "A" => IPC2581Schema.RevA,
                "B" => IPC2581Schema.RevB,
                "B1" => IPC2581Schema.RevB1,
                "C" => IPC2581Schema.RevC,
                _ => throw new InvalidOperationException("Invalid version"),
            };
        }
        string xsdString = AskForVersion();
        try
        {
            XmlReaderValidate(xmlString, xsdString, IPC2581Schema.NameSpace);
            // XDocumentValidate(xmlString, xsdString, IPC2581Schema.NameSpace);
            // var a = XmlSerializer<IPC2581C.IPC2581Type>(xmlString, xsdString, IPC2581Schema.NameSpace);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Validation failed: {ex.Message}");
        }
    }

    public static void XmlReaderValidate(string XMLString, string XSDString, string XMLNamespace)
    {
        XmlReaderSettings settings = new() { };

        settings.Schemas.Add(XMLNamespace, XmlReader.Create(new StringReader(XSDString)));
        settings.ValidationType = ValidationType.Schema;
        settings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings;
        int ValidationErrorCount = 0;
        settings.ValidationEventHandler += (sender, e) =>
        {
            ValidationErrorCount++;
            Console.WriteLine(
                $"{e.Severity} | Line: {e.Exception.LineNumber} Position: {e.Exception.LinePosition} | {e.Message}"
            );
        };

        XmlReader reader = XmlReader.Create(new StringReader(XMLString), settings);
        XDocument xdoc = XDocument.Load(reader);

        if (ValidationErrorCount > 0)
        {
            throw new InvalidOperationException("XML Validation Failed during Deserialization");
        }
        Console.WriteLine("XML Validation Succeeded.");
    }

    public static void XDocumentValidate(string XMLString, string XSDString, string XMLNamespace)
    {
        XmlSchemaSet schemas = new();
        schemas.Add(XMLNamespace, XmlReader.Create(new StringReader(XSDString)));
        XDocument xdoc = XDocument.Parse(
            XMLString,
            LoadOptions.SetBaseUri | LoadOptions.SetLineInfo
        );
        int ValidationErrorCount = 0;
        xdoc.Validate(
            schemas,
            (o, e) =>
            {
                ValidationErrorCount++;
                Console.WriteLine(
                    $"{e.Severity} | Line: {e.Exception.LineNumber} Position: {e.Exception.LinePosition} | {e.Message}"
                );
            }
        );
        if (ValidationErrorCount > 0)
        {
            throw new InvalidOperationException("XML Validation Failed during Deserialization");
        }
        Console.WriteLine("XML Validation Succeeded.");
    }

    public static T XmlSerializer<T>(string XMLString, string XSDString, string XMLNamespace)
        where T : notnull
    {
        XmlReaderSettings xmlReaderSettings = new() { };
        int ValidationErrorCount = 0;
        xmlReaderSettings.Schemas.Add(XMLNamespace, XmlReader.Create(new StringReader(XSDString)));
        xmlReaderSettings.ValidationType = ValidationType.Schema;
        xmlReaderSettings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings;
        xmlReaderSettings.ValidationEventHandler += (sender, e) =>
        {
            ValidationErrorCount++;
            Console.WriteLine(
                $"{e.Severity} | Line: {e.Exception.LineNumber} Position: {e.Exception.LinePosition} | {e.Message}"
            );
        };
        // Set up deserialization events to capture unknown elements, attributes, nodes, and unreferenced objects
        // events will stop the deserialization process from failing on unknowns
        XmlDeserializationEvents xmlDeserializationEvents = new()
        {
            // OnUnknownElement = (sender, e) =>
            // {
            //   Console.WriteLine(
            //     $"Unknown Element: {e.Element.Name} | Line: {((IXmlLineInfo)e.Element).LineNumber} Position: {((IXmlLineInfo)e.Element).LinePosition}"
            //   );
            // },
            // OnUnknownAttribute = (sender, e) =>
            // {
            //   Console.WriteLine(
            //     $"Unknown Attribute: {e.Attr.Name} | Line: {((IXmlLineInfo)e.Attr).LineNumber} Position: {((IXmlLineInfo)e.Attr).LinePosition}"
            //   );
            // },
            // OnUnknownNode = (sender, e) =>
            // {
            //   Console.WriteLine(
            //     $"Unknown Node: {e.Name} | Line: {e.LineNumber} Position: {e.LinePosition}"
            //   );
            // },
            // OnUnreferencedObject = (sender, e) =>
            // {
            //   Console.WriteLine($"Unreferenced Object: {e.UnreferencedObject}");
            // },
        };

        XmlReader reader = XmlReader.Create(new StringReader(XMLString), xmlReaderSettings);
        XmlSerializer serializer = new(typeof(T));

        object? deserialized =
            serializer.Deserialize(reader, xmlDeserializationEvents)
            ?? throw new InvalidOperationException("Deserialization returned null.");

        if (ValidationErrorCount > 0)
        {
            throw new InvalidOperationException("XML Validation Failed during Deserialization");
        }
        Console.WriteLine("XML Validation Succeeded.");

        var ClassObject = (T)deserialized;

        return ClassObject;
    }
}
