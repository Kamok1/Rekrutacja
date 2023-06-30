using Abstractions;
using System.Xml.Serialization;

namespace Implementations;

public class XmlService : IStorageService
{
    private readonly string _mainFolder = Path.Combine( Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "transfer");

    public XmlService()
    {
        Directory.CreateDirectory(_mainFolder);
    }

    public T Get<T>(string name)
    {
        var filePath = GetFilePath(name);

        if (!File.Exists(filePath))
            throw new FileNotFoundException("File not found.", filePath);

        var xmlContent = File.ReadAllText(filePath);
        return DeserializeFromXml<T>(xmlContent);
    }

    public void Set<T>(string name, T context)
    {
        var filePath = GetFilePath(name);
        var xmlContent = SerializeToXml(context);
        File.WriteAllText(filePath, xmlContent);
    }

    public bool Exists(string name)
    {
        return File.Exists(GetFilePath(name));
    }

    private string GetFilePath(string name)
    {
        var fileName = $"{name}.xml";
        return Path.Combine(_mainFolder, fileName);
    }

    private static string SerializeToXml<T>(T data)
    {
        var serializer = new XmlSerializer(typeof(T));
        using var writer = new StringWriter();
        serializer.Serialize(writer, data);
        return writer.ToString();
    }

    private T DeserializeFromXml<T>(string xmlContent)
    {
        var serializer = new XmlSerializer(typeof(T));
        using var reader = new StringReader(xmlContent);
        return (T)serializer.Deserialize(reader) ?? throw new InvalidOperationException();
    }
}