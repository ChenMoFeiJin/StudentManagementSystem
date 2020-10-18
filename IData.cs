#region
using System;
using System.Runtime.Serialization;
#endregion

namespace StudentManagementSystem {
    public interface IData<TData> : ISerializable, IEquatable<TData> where TData : IData<TData> {
        #region Fields And Properties
        public string ID { get; }
        #endregion
    }
}
