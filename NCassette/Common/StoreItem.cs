using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCassette.Common
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
