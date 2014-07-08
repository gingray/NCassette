using System;
using System.IO;

namespace NCassetteLib.Storage.FileStorage
{
    public class SpecificPathFileStorage:IStorage
    {
        private readonly string _folder;

        public SpecificPathFileStorage(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            _folder = folder;
        }
        public byte[] GetFromStorage(string id)
        {
            var filename = Path.Combine(_folder, id);
            if (!File.Exists(filename))
            {
                return null;
            }
            return File.ReadAllBytes(filename);
        }

        public void PutIntoStorage(byte[] data, string id)
        {
            var filename = Path.Combine(_folder, id);
            File.WriteAllBytes(filename, data);
        }

        public DateTime? LastChangedDate(string id)
        {
            var filename = Path.Combine(_folder, id);
            if (!File.Exists(filename))
            {
                return null;
            }
            return File.GetLastWriteTime(filename);
        }
    }
}
