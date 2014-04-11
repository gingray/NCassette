using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCassette.Serialize
{
    public interface ISerialize<T>
    {
        T Deserialize(byte[] data);
        byte[] Serialize(T obj);
    }
}
