using System;

namespace NCassetteLib.Common
{
    [Serializable]
    public class StoreItem<T>
    {
        public T MainObject { get; set; }
        public DateTime DateTime { get; set; }

        public StoreItem(T mainObject,DateTime dateTime)
        {
            MainObject = mainObject;
            DateTime = dateTime;
        }
    }
}
