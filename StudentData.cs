#region
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
#endregion

namespace StudentManagementSystem {
    [Serializable]
    public class StudentData : IData<StudentData> {
        #region Fields And Properties
        private string _ID;
        private string _Name;
        private DormitoryData _Dormitory;
        private HashSet<DateTime> _Duties;
        public string Name { get => _Name; set => _Name = value; }
        public DormitoryData Dormitory { get => _Dormitory; set => _Dormitory = value; }
        public HashSet<DateTime> Duties => _Duties ??= new HashSet<DateTime>();
        #endregion

        #region Constructors
        public StudentData(string id, string name, string building, string room) {
            _ID = id;
            _Name = name;
            if (!Program.Dormitories.ContainsKey(building + room)) Program.Dormitories.Add(new DormitoryData(building, room));
            _Dormitory = Program.Dormitories[building + room];
            Program.Dormitories[building + room].Students.Add(this);
        }
        public StudentData(string id, string name, DormitoryData dormitory) {
            _ID = id;
            _Name = name;
            if (!Program.Dormitories.ContainsKey(dormitory.ID)) Program.Dormitories.Add(dormitory);
            _Dormitory = Program.Dormitories[dormitory.ID];
            Program.Dormitories[dormitory.ID].Students.Add(this);
        }
        public StudentData(SerializationInfo info, StreamingContext context) {
            _ID = info.GetString(nameof(ID));
            _Name = info.GetString(nameof(Name));
            var dormitory = info.GetValue(nameof(Dormitory), typeof(DormitoryData)) as DormitoryData;
            if (!Program.Dormitories.ContainsKey(dormitory!.ID)) Program.Dormitories.Add(dormitory);
            _Dormitory = Program.Dormitories[dormitory.ID];
            Program.Dormitories[dormitory.ID].Students.Add(this);
            _Duties = info.GetValue(nameof(Duties), typeof(HashSet<DateTime>)) as HashSet<DateTime>;
        }
        #endregion

        #region IData<StudentData> Members
        public string ID => _ID;
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue(nameof(ID), ID);
            info.AddValue(nameof(Name), Name);
            info.AddValue(nameof(Dormitory), Dormitory);
            info.AddValue(nameof(Duties), Duties);
        }
        public bool Equals(StudentData other) => other is not null && (ReferenceEquals(this, other) || _ID == other._ID);
        #endregion

        public override bool Equals(object obj) => obj is not null
            && (ReferenceEquals(this, obj) || obj.GetType() == GetType() && Equals((StudentData) obj));
        public override int GetHashCode() => ID.GetHashCode();
    }
}
