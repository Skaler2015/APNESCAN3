namespace ApneScan.Serialization;

public interface ISerializer<T>
{
    void Serialize(Stream stream, T? obj);

    T? Deserialize(Stream stream);
}