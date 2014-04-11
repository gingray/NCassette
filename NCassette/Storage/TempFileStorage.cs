using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NCassette.Storage
{
    public class TempFileStorage : IStorage
    {
        private const string FOLDER_NAME = "NCassette";
        private readonly string _pathToTempFolder;
        public TempFileStorage()
        {
            _pathToTempFolder = Path.Combine(Path.GetTempPath(), FOLDER_NAME);
            if (!Directory.Exists(_pathToTempFolder))
            {
                Directory.CreateDirectory(_pathToTempFolder);
            }
        }
        public byte[] GetFromStorage(string id)
        {
            var filename = Path.Combine(_pathToTempFolder, id);
            if (!File.Exists(filename))
            {
                return null;
            }
            return File.ReadAllBytes(filename);
        }

        public void PutIntoStorage(byte[] data, string id)
        {
            var filename = Path.Combine(_pathToTempFolder, id);
            File.WriteAllBytes(filename, data);
        }
    }
}
