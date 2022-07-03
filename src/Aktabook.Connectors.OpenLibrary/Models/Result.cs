namespace Aktabook.Connectors.OpenLibrary.Models;

public class Result<T>
{
    public bool IsError { get; set; }

    public T? Value { get; set; }
}