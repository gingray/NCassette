using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NCassette.Serialize;
using NCassette.Storage;

namespace NCassette.Common
{
    public class NRecord<T>
    {
        private readonly Func<T> _createFunction;
        private ISerialize<T> _serialize;
        private IStorage _storage;
        private readonly List<object> _dependsObjects;
        private int _numToExecute = 1;
        private Action<Exception> _callbackOnFailExecution;
        private Func<T, bool> _needToUpdateObject;

        public NRecord(Func<T> createFunction)
        {
            _createFunction = createFunction;
            _dependsObjects = new List<object>();
        }

        public NRecord<T> SerializeWay(ISerialize<T> serialize)
        {
            _serialize = serialize;
            return this;
        }

        public NRecord<T> SerializeWayBinary()
        {
            _serialize = new BinarySerializer<T>();
            return this;
        }

        public NRecord<T> SerializeWayJson()
        {
            _serialize = new JsonSerializer<T>();
            return this;
        }

        public NRecord<T> StorageStrategy(IStorage storage)
        {
            _storage = storage;
            return this;
        }

        public NRecord<T> StorageInTempFiles()
        {
            _storage = new TempFileStorage();
            return this;
        }

        public NRecord<T> DependsOn(params object[] objects)
        {
            _dependsObjects.AddRange(objects);
            return this;
        }

        public NRecord<T> TriesToExecute(int numToExecute, Action<Exception> callbackOnFailExecution)
        {
            _numToExecute = numToExecute;
            _callbackOnFailExecution = callbackOnFailExecution;
            return this;
        }

        public NRecord<T> SetLifeTime(Func<T, bool> needToUpdateObject)
        {
            _needToUpdateObject = needToUpdateObject;
            return this;
        }

        public T Execute()
        {
            var mainName = string.Format("{0}.{1}.{2}", _createFunction.Method.DeclaringType.FullName, _createFunction.Method.Name, _serialize.GetType().ToString());
            var line = _dependsObjects.Aggregate(mainName, (acc, item) => acc + item.ToString());
            var hash = Hash.CalculateHash(line);
            var data = _storage.GetFromStorage(hash);
            bool needToReturnObject = true;
            if (data != null)
            {
                try
                {
                    var store = _serialize.Deserialize(data);
                    if (_needToUpdateObject != null)
                    {
                        if (_needToUpdateObject(store))
                        {
                            needToReturnObject = false;
                        }
                    }
                    if (needToReturnObject)
                        return store;
                }
                catch (InvalidCastException ex)
                {

                }

            }
            T result = default(T);
            Exception lastException = null;
            for (int i = 0; i < _numToExecute; i++)
            {
                try
                {
                    result = _createFunction();
                    lastException = null;
                }
                catch (Exception ex)
                {
                    _callbackOnFailExecution(ex);
                    lastException = ex;
                }
            }

            if (lastException != null)
                throw lastException;

            var binData = _serialize.Serialize(result);
            _storage.PutIntoStorage(binData, hash);
            return result;
        }
    }
}
