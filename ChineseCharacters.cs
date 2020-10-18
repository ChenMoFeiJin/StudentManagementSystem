#region
using System;
using System.Linq;
using System.Text;
#endregion

namespace StudentManagementSystem {
    public static class ChineseCharacters {
        #region Static Member
        public static string GetChineseCharacter() =>
            GB2312.GetString(new[] {Convert.ToByte(Rnd.Next(0xB0, 0xF7)), Convert.ToByte(Rnd.Next(0xA0, 0xFE))});
        public static string GetChineseCharacters(int length) {
            string result = default;
            while (length-- > 0) result += GetChineseCharacter();
            return result;
        }
        public static int LengthExtra(this string @this) => GB2312.GetByteCount(@this);
        public static string PadLeftExtra(this string @this, int totalWidth) =>
            @this.PadLeft(totalWidth - @this.LengthExtra() + @this.Length);
        public static string PadRightExtra(this string @this, int totalWidth) =>
            @this.PadRight(totalWidth - @this.LengthExtra() + @this.Length);
        public static Encoding GB2312 => _GB2312 ??= Encoding.GetEncoding("gb2312");
        public static Random Rnd => _Rnd ??= new Random();
        public static string[] Bases => _Bases ??= "0123456789abcdef".Select(@char => @char.ToString()).ToArray();
        private static Encoding _GB2312;
        private static Random _Rnd;
        private static string[] _Bases;
        #endregion
    }
}
