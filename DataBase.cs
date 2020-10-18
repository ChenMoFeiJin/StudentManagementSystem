#region
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
#endregion

namespace StudentManagementSystem {
    [Serializable]
    public class DataBase<TData> : ISerializable, IDictionary<string, TData> where TData : IData<TData> {
        #region Fields And Properties
        private bool _Changed;
        private SortedDictionary<string, TData> _Data;
        public bool Changed => _Changed;
        public SortedDictionary<string, TData> Data => _Data ??= new SortedDictionary<string, TData>();
        #endregion

        #region Constructors
        public DataBase() { }
        public DataBase(IEnumerable<TData> enumerable) {
            var list = enumerable as List<TData> ?? enumerable.ToList();
            if (list.Select(item => item.ID).Distinct().Count() == list.Count)
                _Data = new SortedDictionary<string, TData>(list.ToDictionary(item => item.ID, item => item));
            else throw new ArgumentException($"There Cannot Be Data With Duplicate ID In {nameof(list)}", nameof(list));
        }
        public DataBase(IEnumerable<KeyValuePair<string, TData>> enumerable) =>
            _Data = new SortedDictionary<string, TData>(enumerable.ToDictionary(item => item.Key, item => item.Value));
        public DataBase(string path) {
            if (File.Exists(path)) {
                using var stream = File.OpenRead(path);
                _Data = (new BinaryFormatter().Deserialize(stream) as DataBase<TData>)?.Data;
            } else Save(path);
        }
        public DataBase(SerializationInfo info, StreamingContext context) => _Data =
            info.GetValue(nameof(Data), Data.GetType()) as SortedDictionary<string, TData>;
        #endregion

        #region IDictionary<string,TData> Members
        public bool ContainsKey(string key) => Data.ContainsKey(key);
        public void Add(string key, TData value) => Data.Add(key, value);
        public bool Remove(string key) => Data.Remove(key);
        public bool TryGetValue(string key, out TData value) => Data.TryGetValue(key, out value);
        public TData this[string index] { get => Data[index]; set => Data[index] = value; }
        public ICollection<string> Keys => Data.Keys;
        public ICollection<TData> Values => Data.Values;
        public void CopyTo(KeyValuePair<string, TData>[] array, int index) => Data.ToArray().CopyTo(array, index);
        public bool Remove(KeyValuePair<string, TData> item) => Data.Remove(item.Key);
        public int Count => Data.Count;
        public bool IsReadOnly => false;
        public void Add(KeyValuePair<string, TData> item) => Data.Add(item.Key, item.Value);
        public void Clear() => Data.Clear();
        public bool Contains(KeyValuePair<string, TData> item) => Data.Contains(item);
        public IEnumerator<KeyValuePair<string, TData>> GetEnumerator() => Data.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        #region ISerializable Members
        public void GetObjectData(SerializationInfo info, StreamingContext context) => info.AddValue(nameof(Data), Data);
        #endregion

        public void Add(TData item) => Data.Add(item.ID, item);
        public bool Save(string path) {
            try {
                using var stream = File.Create(path);
                new BinaryFormatter().Serialize(stream, this);
                _Changed = false;
            } catch (Exception e) {
                Console.WriteLine(e);
                return false;
            }
            return true;
        }
    }
}
