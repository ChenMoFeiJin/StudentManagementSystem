#region
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
#endregion

namespace StudentManagementSystem {
    [Serializable]
    public class DormitoryData : IData<DormitoryData> {
        #region Fields And Properties
        private string _Building;
        private string _Room;
        private List<StudentData> _Students;
        public string Building { get => _Building; set => _Building = value; }
        public string Room { get => _Room; set => _Room = value; }
        public List<StudentData> Students => _Students ??= new List<StudentData>();
        #endregion

        #region Constructors
        public DormitoryData() { }
        public DormitoryData(string building, string room) {
            Building = building;
            Room = room;
        }
        public DormitoryData(SerializationInfo info, StreamingContext context) {
            _Building = info.GetString(nameof(Building));
            _Room = info.GetString(nameof(Room));
        }
        #endregion

        #region IData<DormitoryData> Members
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue(nameof(Building), Building);
            info.AddValue(nameof(Room), Room);
        }
        public string ID => Building + Room;
        public bool Equals(DormitoryData other) =>
            other is not null && (ReferenceEquals(this, other) || _Building == other._Building && _Room == other._Room);
        #endregion

        public override string ToString() => $"Dormitory{{{Building}, {Room}}}";
        public override bool Equals(object obj) =>
            obj is not null && (ReferenceEquals(this, obj) || obj.GetType() == GetType() && Equals((DormitoryData) obj));
        public override int GetHashCode() => ID.GetHashCode();
    }
}
