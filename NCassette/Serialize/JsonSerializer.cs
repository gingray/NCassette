using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace NCassette.Serialize
{
    public class JsonSerializer<T> : ISerialize<T>
    {
        public T Deserialize(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(ms);
            }
        }

        public byte[] Serialize(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(ms, obj);
                return ms.ToArray();
            }
        }
    }
}
