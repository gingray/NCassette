using System;
using System.IO;

namespace NCassetteLib.Storage.FileStorage
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

        public DateTime? LastChangedDate(string id)
        {
            var filename = Path.Combine(_pathToTempFolder, id);
            if (!File.Exists(filename))
            {
                return null;
            }
            return File.GetLastWriteTime(filename);
        }
    }
}
