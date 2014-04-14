using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NCassetteLib.Serialize
{
    public class BinarySerializer<T> :ISerialize<T>
    {
        public T Deserialize(byte[] data)
        {
            using (var  memoryStream = new MemoryStream(data))
            {
                var binaryFormatter = new BinaryFormatter();
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }

        public byte[] Serialize(T obj)
        {
            using (var memoryStream = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }
    }
}
