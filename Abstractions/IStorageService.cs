namespace Abstractions;

public interface IStorageService
{
    T Get<T>(string name);
    void Set<T>(string name, T context);
    bool Exists(string name);
}