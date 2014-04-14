using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NCassetteLib.Exceptions;
using NCassetteLib.Serialize;
using NCassetteLib.Storage;

namespace NCassetteLib.Common
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
        private bool _canWorkInReleaseMode;
        private static DebuggableAttribute _debuggableAttribute;

        static NRecord()
        {
            var ret = Assembly.GetCallingAssembly().GetCustomAttributes(typeof(DebuggableAttribute), true);
            if (ret.Length > 0)
            {
                _debuggableAttribute = (DebuggableAttribute)ret[0];
            }
        }

        public NRecord(Func<T> createFunction)
        {
            _createFunction = createFunction;
            _canWorkInReleaseMode = false;
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

        public NRecord<T> WorkInReleaseMode()
        {
            _canWorkInReleaseMode = true;
            return this;
        } 

        public T Execute()
        {
            CheckMinimalRequirements();
            CheckDebugMode();
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

        private void CheckDebugMode()
        {
            if (_debuggableAttribute != null)
            {
                if(_debuggableAttribute.IsJITTrackingEnabled)
                    return;
                if(!_debuggableAttribute.IsJITTrackingEnabled && _canWorkInReleaseMode)
                    return;
            }
            throw new WorkInReleaseModeException("You use NCassette in Realese assembly without explicit permisions");
        }

        private void CheckMinimalRequirements()
        {
            if (_serialize == null)
            {
                throw new NCassetteConfigureException("You need to set serialization way in NCassette it's required");
            }

            if (_storage == null)
            {
                throw new NCassetteConfigureException("You need to set storage way in NCassette it's required");
            }
        }
    }
}
