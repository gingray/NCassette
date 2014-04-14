namespace NCassetteLib.Storage
{
    public interface IStorage
    {
        byte[] GetFromStorage(string id);
        void PutIntoStorage(byte[] data, string id);
    }
}
