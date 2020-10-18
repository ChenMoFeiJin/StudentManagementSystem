#region
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static StudentManagementSystem.ChineseCharacters;
#endregion

namespace StudentManagementSystem {
    public static class Program {
        #region Static Member
        public static DataBase<DormitoryData> Dormitories = new DataBase<DormitoryData>("Dormitories.dat");
        public static DataBase<StudentData> Students = new DataBase<StudentData>("Students.dat");
        private static readonly char[] _Separator = {' '};
        private static void Main(string[] args) {
            #if DEBUG
            if (!Students.Any())
                for (var (i, s) = (1000, ""); i-- > 0;) {
                    while (Students.ContainsKey(s = $"20{Rnd.Next(100000, 209999)}")) ;
                    Students.Add(new StudentData(s, GetChineseCharacters(Rnd.Next(2, 11)),
                        $"{Rnd.Next(1, 5)}{(Rnd.Next() % 2 == 0 ? "N" : "S")}", $"{Rnd.Next(1, 3)}{Rnd.Next(1, 20):00}"));
                }
            ;
            Students = new DataBase<StudentData>(Students.OrderBy(item => (item.Value.Dormitory.Building, item.Value.Dormitory.Room)));
            ;
            #endif
            while (true) {
                PrintMenu();
                switch (Console.ReadKey().Key) {
                    case ConsoleKey.A:
                        PrintAllData();
                        break;
                    case ConsoleKey.B:
                        Save();
                        break;
                    case ConsoleKey.C:
                        AddStudent();
                        break;
                    case ConsoleKey.D:
                        RemoveStudent();
                        break;
                    case ConsoleKey.E:
                        ChangeDormitory();
                        break;
                    case ConsoleKey.F:
                        MoveDormitory();
                        break;
                    case ConsoleKey.G:
                        AddDuties();
                        break;
                    case ConsoleKey.H:
                        RemoveDuties();
                        break;
                    case ConsoleKey.I:
                        CheckStudent();
                        break;
                    case ConsoleKey.J:
                        CheckDuty();
                        break;
                    case ConsoleKey.K:
                        CheckDormitory();
                        break;
                    case ConsoleKey.Delete:
                        Clear();
                        break;
                    case ConsoleKey.Escape: return;
                    default:
                        Console.Write("\n\nUnknown Key.\n");
                        break;
                }
                Console.Write("\nPress Any Key To Continue.\n");
                Console.ReadKey();
                Console.Clear();
            }
        }
        private static void PrintMenu() {
            Console.Write("Menu:\n\n");
            Console.Write("A) Print All Data\t");
            Console.Write("B) Save\n");
            Console.Write("C) Add Student\t\t");
            Console.Write("D) Remove Student\n");
            Console.Write("E) Change Dormitory\t");
            Console.Write("F) Move Dormitory\n");
            Console.Write("G) Add Duties\t\t");
            Console.Write("H) Remove Duties\n");
            Console.Write("I) Check Student\t");
            Console.Write("J) Check Duty\n");
            Console.Write("K) Check Dormitory\n");
            Console.Write("\nDel) Clear\t\t");
            Console.Write("Esc) Exit\n");
            Console.Write("\nPlease Select An Action:");
        }
        private static void PrintAllData() {
            if (Students.Any()) {
                var (len1, len2, len3, len4) = (Math.Max(Students.Select(item => item.Value.ID.LengthExtra()).Max(), 2) + 2,
                    Math.Max(Students.Select(item => item.Value.Name.LengthExtra()).Max(), 4) + 2,
                    Math.Max(Students.Select(item => item.Value.Dormitory.Building.LengthExtra()).Max(), 8) + 2,
                    Math.Max(Students.Select(item => item.Value.Dormitory.Room.LengthExtra()).Max(), 4) + 2);
                Console.Write(
                    $"\n\n{"ID".PadRightExtra(len1)}{"Name".PadRightExtra(len2)}{"Building".PadRightExtra(len3)}{"Room".PadRightExtra(len4)}\n");
                foreach (var student in Students)
                    Console.Write(
                        $"{student.Value.ID.PadRightExtra(len1)}{student.Value.Name.PadRightExtra(len2)}{student.Value.Dormitory.Building.PadRightExtra(len3)}{student.Value.Dormitory.Room.PadRightExtra(len4)}\n");
            } else Console.WriteLine("\n\nThere Is No Student Data.");
        }
        private static void Save() {
            Console.Write(Students.Save($"{nameof(Students)}.dat") ? $"\n\n{nameof(Students)} Save Successfully.\n"
                : $"\n\n{nameof(Students)} Save Failed.\n");
            Console.Write(Dormitories.Save($"{nameof(Dormitories)}.dat") ? $"{nameof(Dormitories)} Save Successfully.\n"
                : $"{nameof(Dormitories)} Save Failed.\n");
        }
        private static void AddStudent() {
            Console.Write("\n\nPlease Enter Four Parameters (Separated By Spaces), The Order Is ID Name Building Room:\n");
            var s = Console.ReadLine().MySplit();
            if (s.Length != 4) {
                Console.Write("\nParameter Error.\n");
                return;
            }
            if (Students.ContainsKey(s[0])) {
                Console.Write("\nID Already Exists.\n");
                return;
            }
            Students.Add(new StudentData(s[0], s[1], s[2], s[3]));
            Console.Write($"\nAdd One Data{{{s[0]}, {s[1]}, {s[2]}, {s[3]}}} Successfully.\n");
        }
        private static void RemoveStudent() {
            Console.Write("\n\nPlease Enter The IDs You Want Delete (Separated By Spaces):");
            var ids = Console.ReadLine().MySplit().Distinct().Where(id => Students.Remove(id)).ToList();
            Console.Write(ids.Any()
                ? $"Remove IDs{{{ids.Aggregate(new StringBuilder(), (sb, next) => sb.Append(next).Append(", "), sb => sb.Remove(sb.Length - 2, 2).ToString())}}} Successfully.\n"
                : "\nDo Not Exist Such IDs\n");
        }
        private static void ChangeDormitory() {
            var input = ReadLine("\n\nPlease Enter Target Building And Room (Separated By Spaces):");
            if (input.Length != 2) {
                Console.Write("\nParameter Error.\n");
                return;
            }
            if (!Dormitories.ContainsKey(input[0] + input[1])) Dormitories.Add(new DormitoryData(input[0], input[1]));
            var dormitory = Dormitories[input[0] + input[1]];
            input = ReadLine("Please Enter The IDs You Want Change (Separated By Spaces):");
            var ids = input.Distinct().Where(item => Students.ContainsKey(item)).ToArray();
            foreach (var item in ids) {
                Students[item].Dormitory.Students.Remove(Students[item]);
                if (!Students[item].Dormitory.Students.Any()) Dormitories.Remove(Students[item].Dormitory.ID);
                Students[item].Dormitory = dormitory;
                dormitory.Students.Add(Students[item]);
            }
            Console.Write(ids.Any()
                ? $"\nStudents{{{ids.Aggregate(new StringBuilder(), (sb, next) => sb.Append(next).Append(", "), sb => sb.Remove(sb.Length - 2, 2).ToString())}}} Changed Successfully.\n"
                : "\nNo Valid ID.\n");
        }
        private static void MoveDormitory() {
            var input = ReadLine(
                "\n\nPlease Enter From And Target Dormitory (Separated By Spaces), The Order Is Building Room Building Room:\n");
            if (input.Length != 4) {
                Console.Write("\nParameter Error.\n");
                return;
            }
            if (input[0] == input[2] && input[1] == input[3]) {
                Console.Write("\nTarget Is Same As Form.\n");
                return;
            }
            if (Dormitories.ContainsKey(input[0] + input[1])) {
                if (!Dormitories.ContainsKey(input[2] + input[3])) Dormitories.Add(new DormitoryData(input[2], input[3]));
                var dormitories = Dormitories[input[2] + input[3]];
                Dormitories[input[0] + input[1]].Students.ForEach(item => {
                    item.Dormitory = dormitories;
                    dormitories.Students.Add(item);
                });
                Dormitories.Remove(input[0] + input[1]);
                Console.Write($"\nMove Dormitory {{{input[0]}, {input[1]}}} To Dormitory {{{input[2]}, {input[3]}}} Successfully.\n");
            } else Console.Write($"\nDo Not Exist Dormitory{{{input[0]}, {input[1]}}}\n");
        }
        private static void AddDuties() {
            var inputs = ReadLine("\n\nPlease Enter ID You Want Add Duties:");
            if (inputs.Length != 1) {
                Console.Write("\nParameter Error.\n");
                return;
            }
            if (!Students.ContainsKey(inputs[0])) {
                Console.Write("\nID Do Not Exist.\n");
                return;
            }
            var student = Students[inputs[0]];
            inputs = ReadLine("\nPlease Enter Duties You Want Add:\n");
            var valids = new List<string>();
            foreach (var input in inputs)
                if (DateTime.TryParse(input, out var date)) {
                    if (student.Duties.Contains(date)) continue;
                    student.Duties.Add(date);
                    valids.Add(input);
                }
            Console.Write(valids.Any()
                ? $"\nAdd Duties {valids.Aggregate(new StringBuilder(), (sb, next) => sb.Append(next).Append(", "), sb => sb.Remove(sb.Length - 2, 2).ToString())} Successfully.\n"
                : "\nNo Valid Date.\n");
        }
        private static void RemoveDuties() {
            var inputs = ReadLine("\n\nPlease Enter ID You Want Remove Duties:");
            if (inputs.Length != 1) {
                Console.Write("\nParameter Error.\n");
                return;
            }
            if (!Students.ContainsKey(inputs[0])) {
                Console.Write("\nID Do Not Exist.\n");
                return;
            }
            var student = Students[inputs[0]];
            inputs = ReadLine("\nPlease Enter Duties You Want Remove:\n");
            var valids = new List<string>();
            foreach (var input in inputs) {
                DateTime.TryParse(input, out var date);
                if (student.Duties.Remove(date)) valids.Add(input);
            }
            Console.Write(valids.Any()
                ? $"\nRemove Duties {valids.Aggregate(new StringBuilder(), (sb, next) => sb.Append(next).Append(", "), sb => sb.Remove(sb.Length - 2, 2).ToString())} Successfully.\n"
                : "\nNo Valid Date.\n");
        }
        private static void CheckStudent() {
            var inputs = ReadLine("\n\nPlease Enter ID You Want Check:");
            if (inputs.Length != 1) {
                Console.Write("\nParameter Error.\n");
                return;
            }
            if (!Students.ContainsKey(inputs[0])) {
                Console.Write("\nID Do Not Exist.\n");
                return;
            }
            var student = Students[inputs[0]];
            Console.Write(
                $"\nID: {student.ID}\tName: {student.Name}\tDormitory: {student.Dormitory.ID}\nRoommates: {student.Dormitory.Students.Aggregate(new StringBuilder(), (sb, next) => sb.Append(next.ID).Append(", "), sb => sb.Remove(sb.Length - 2, 2).ToString())}\n");
        }
        private static void CheckDuty() {
            var inputs = ReadLine("\n\nPlease Enter Dormitory And Date (Separated By Spaces), The Order Is Building Room Date:");
            if (inputs.Length != 3) {
                Console.Write("\nParameter Error.\n");
                return;
            }
            if (!Dormitories.ContainsKey(inputs[0] + inputs[1])) {
                Console.Write("\nDormitory Do Not Exist.\n");
                return;
            }
            if (DateTime.TryParse(inputs[2], out var date)) {
                var students = Dormitories[inputs[0] + inputs[1]].Students.Where(item => item.Duties.Contains(date)).ToArray();
                Console.Write(students.Any()
                    ? $"\nStudents {students.Aggregate(new StringBuilder(), (sb, next) => sb.Append(next.ID).Append(", "), sb => sb.Remove(sb.Length - 2, 2).ToString())} Are On Duty.\n"
                    : "\nNo One Is On Duty.\n");
            } else Console.Write("\nDate Invalid.\n");
        }
        private static void CheckDormitory() {
            var input = ReadLine("\n\nPlease Enter Target Building And Room (Separated By Spaces):");
            if (input.Length != 2) {
                Console.Write("\nParameter Error.\n");
                return;
            }
            if (!Dormitories.ContainsKey(input[0] + input[1])) {
                Console.Write("\nDormitory Do Not Exist.\n");
                return;
            }
            var dormitory = Dormitories[input[0] + input[1]];
            Console.Write(
                $"\nBuilding: {dormitory.Building}\tRoom: {dormitory.Room}\nRoommates: {dormitory.Students.Aggregate(new StringBuilder(), (sb, next) => sb.Append(next.ID).Append(", "), sb => sb.Remove(sb.Length - 2, 2).ToString())}\n");
        }
        private static void Clear() {
            Students.Data.Clear();
            Dormitories.Data.Clear();
            Console.Write("\n\nClear All Data Successfully.\n");
        }
        private static string[] ReadLine(string inform) {
            Console.Write(inform);
            return Console.ReadLine().MySplit();
        }
        private static string[] MySplit(this string @this) => @this.Split(_Separator, StringSplitOptions.RemoveEmptyEntries);
        #endregion
    }
}
