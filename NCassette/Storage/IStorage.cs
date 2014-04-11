using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCassette.Storage
{
    public interface IStorage
    {
        byte[] GetFromStorage(string id);
        void PutIntoStorage(byte[] data, string id);
    }
}
