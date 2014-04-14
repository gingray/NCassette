namespace NCassetteLib.Serialize
{
    public interface ISerialize<T>
    {
        T Deserialize(byte[] data);
        byte[] Serialize(T obj);
    }
}
