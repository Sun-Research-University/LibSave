﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;

namespace LibSave.Types
{
    //Follow normal dictionary specs
    public class DictionarySaveFile<T, TK> : SaveFile<Dictionary<T, TK>>, IDictionary<T, TK>, IReadOnlyDictionary<T, TK>, IDictionary, IDeserializationCallback, ISerializable where T : notnull
    {
        [NonSerialized]
        private object _syncRoot;
        [NonSerialized]
        private readonly Action<ulong, Dictionary<T, TK>> _cleanupAction;

        public DictionarySaveFile(string name) : base(name)
        {
            if (typeof(T) != typeof(ulong))
                throw new Exception("Default constructor should only be used if T is ulong and being used for discord guilds.");

            _cleanupAction = delegate (ulong guild, Dictionary<T, TK> dictionary) { (dictionary as Dictionary<ulong, TK>).Remove(guild); };
        }
        public DictionarySaveFile(string name, Action<ulong, Dictionary<T, TK>> cleanUp) : base(name) => _cleanupAction = cleanUp;

        public override void CleanUp(ulong id) => _cleanupAction?.Invoke(id, _data);

        public void Set(Dictionary<T, TK> newDictionary) => _data = newDictionary;

        public TK this[T key] { get => _data[key]; set => _data[key] = value; }
        public object this[object key] { get => _data[(T)key]; set => _data[(T)key] = (TK)value; }

        public ICollection<T> Keys => _data.Keys;

        public ICollection<TK> Values => _data.Values;

        public int Count => _data.Count;

        public bool IsReadOnly => false;

        public bool IsFixedSize => false;

        public bool IsSynchronized => false;

        public object SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                    Interlocked.CompareExchange<object>(ref _syncRoot, new object(), null);

                return _syncRoot;
            }
        }

        IEnumerable<T> IReadOnlyDictionary<T, TK>.Keys => _data.Keys;

        ICollection IDictionary.Keys => _data.Keys;

        IEnumerable<TK> IReadOnlyDictionary<T, TK>.Values => _data.Values;

        ICollection IDictionary.Values => _data.Values;

        public void Add(T key, TK value) => _data.Add(key, value);

        public void Add(KeyValuePair<T, TK> item) => _data.Add(item.Key, item.Value);

        public void Add(object key, object value) => _data.Add((T)key, (TK)value);

        public void Clear() => _data.Clear();

        public bool Contains(KeyValuePair<T, TK> item) => _data.ContainsKey(item.Key) && _data.ContainsValue(item.Value);

        public bool Contains(object key) => _data.ContainsKey((T)key);

        public bool ContainsKey(T key) => _data.ContainsKey(key);

        public void CopyTo(KeyValuePair<T, TK>[] array, int arrayIndex) => _data.ToArray().CopyTo(array, arrayIndex);

        public void CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentException();
            }

            if (array.Rank != 1)
            {
                throw new RankException();
            }

            if (array.GetLowerBound(0) != 0)
            {
                //??
                throw new Exception();
            }

            if (index < 0 || index > array.Length)
            {
                throw new IndexOutOfRangeException();
            }

            if (array.Length - index < _data.Count)
            {
                throw new Exception();
            }

            if (array is T[] keys)
            {
                CopyTo(keys, index);
            }
            else
            {
                if (!(array is object[] objects))
                {
                    throw new Exception();
                }

                int count = _data.Count;
                for (int i = 0; i < count; i++)
                    if (_data.ElementAt(i).GetHashCode() >= 0) objects[index++] = _data.ElementAt(i).Key;
            }
        }

        public IEnumerator<KeyValuePair<T, TK>> GetEnumerator() => _data.GetEnumerator();

        public void GetObjectData(SerializationInfo info, StreamingContext context) => _data.GetObjectData(info, context);

        public void OnDeserialization(object sender) => _data.OnDeserialization(sender);

        public bool Remove(T key) => _data.Remove(key);

        public bool Remove(KeyValuePair<T, TK> item) => _data.Remove(item.Key);

        public void Remove(object key) => _data.Remove((T)key);

        public bool TryGetValue(T key, [MaybeNullWhen(false)] out TK value) => _data.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => _data.GetEnumerator();

        IDictionaryEnumerator IDictionary.GetEnumerator() => _data.GetEnumerator();
    }
}
