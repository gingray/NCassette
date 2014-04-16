using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NCassetteLib.Exceptions;
using NCassetteLib.Serialize;
using NCassetteLib.Storage;
using NCassetteLib.Storage.FileStorage;

namespace NCassetteLib.Common
{
    public class NRecord<T>
    {
        private readonly Func<T> _createFunction;
        private ISerialize<T> _serializer;
        private IStorage _storage;
        private readonly List<object> _dependsObjects;
        private int _numToExecute = 1;
        private Action<Exception> _callbackOnFailExecution;
        private Func<T, bool> _needToUpdateObject;
        private bool _canWorkInReleaseMode;
        private static DebuggableAttribute _debuggableAttribute;
        private string _duplicateStorageKey;
        private static object _locker = new object();

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
            _serializer = serialize;
            return this;
        }

        public NRecord<T> SerializeWayBinary()
        {
            _serializer = new BinarySerializer<T>();
            return this;
        }

        public NRecord<T> SerializeWayJson()
        {
            _serializer = new JsonSerializer<T>();
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

        public NRecord<T> DuplicateStorageWithKey(string duplicateStorageKey)
        {
            _duplicateStorageKey = duplicateStorageKey;
            return this;
        }

        public NRecord<T> StorageInFolder(string path)
        {
            _storage = new SpecificPathFileStorage(path);
            return this;
        }

        public T Execute()
        {
            CheckMinimalRequirements();
            if (CheckDebugMode())
                return _createFunction();
            var mainName = string.Format("{0}.{1}.{2}", _createFunction.Method.DeclaringType.FullName, _createFunction.Method.Name, _serializer.GetType().ToString());
            var line = _dependsObjects.Aggregate(mainName, (acc, item) => acc + item.ToString());
            var hash = Hash.CalculateHash(line);
            byte[] data = null;
            lock (_locker)
            {
                data = _storage.GetFromStorage(hash);
            }
            bool needToReturnObject = true;
            if (data != null)
            {
                try
                {
                    var store = _serializer.Deserialize(data);
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

            var binData = _serializer.Serialize(result);
            lock (_locker)
            {
                _storage.PutIntoStorage(binData, hash);
                if (_duplicateStorageKey != null)
                {
                    _storage.PutIntoStorage(binData, _duplicateStorageKey);
                }
            }
            return result;
        }

        private bool CheckDebugMode()
        {
            if (_debuggableAttribute != null)
            {
                if (_debuggableAttribute.IsJITOptimizerDisabled)
                    return false;
                if (!_debuggableAttribute.IsJITOptimizerDisabled && _canWorkInReleaseMode)
                    return false;
            }
            return true;
        }

        private void CheckMinimalRequirements()
        {
            if (_serializer == null)
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
