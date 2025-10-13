using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversityManagement
{
    public abstract class Person
    {
        private static int nextId = 1;
        public int Id { get; }
        public string Name { get; private set; }
        public int Age { get; private set; }
        public string Contact { get; private set; }

        protected Person(string name, int age, string contact)
        {
            Id = nextId++;
            SetName(name);
            SetAge(age);
            SetContact(contact);
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Имя не может быть пустым.");
            Name = name.Trim();
        }

        public void SetAge(int age)
        {
            if (age <= 0 || age > 120)
                throw new ArgumentException("Возраст должен быть в диапазоне 1..120.");
            Age = age;
        }

        public void SetContact(string contact)
        {
            Contact = contact?.Trim() ?? string.Empty;
        }

        public override string ToString()
        {
            return $"{Name} (Id: {Id}), Age: {Age}, Contact: {Contact}";
        }
    }

    public class Student : Person
    {
        private readonly List<int> courseIds = new List<int>();

        public Student(string name, int age, string contact) : base(name, age, contact) { }

        internal void Enroll(int courseId)
        {
            if (!courseIds.Contains(courseId))
                courseIds.Add(courseId);
        }

        internal void Unenroll(int courseId)
        {
            courseIds.Remove(courseId);
        }

        public IReadOnlyCollection<int> CourseIds => courseIds.AsReadOnly();

        public override string ToString()
        {
            return $"Student: {base.ToString()}";
        }
    }

    public class Teacher : Person
    {
        private readonly List<int> teachingCourseIds = new List<int>();

        public Teacher(string name, int age, string contact) : base(name, age, contact) { }

        internal void AssignCourse(int courseId)
        {
            if (!teachingCourseIds.Contains(courseId))
                teachingCourseIds.Add(courseId);
        }

        internal void UnassignCourse(int courseId)
        {
            teachingCourseIds.Remove(courseId);
        }

        public IReadOnlyCollection<int> TeachingCourseIds => teachingCourseIds.AsReadOnly();

        public override string ToString()
        {
            return $"Teacher: {base.ToString()}";
        }
    }

    public class Course
    {
        private static int nextId = 1;
        public int Id { get; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public int? TeacherId { get; private set; }
        private readonly List<int> studentIds = new List<int>();

        public Course(string title, string description)
        {
            Id = nextId++;
            SetTitle(title);
            SetDescription(description);
        }

        public void SetTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Название курса не может быть пустым.");
            Title = title.Trim();
        }

        public void SetDescription(string description)
        {
            Description = description?.Trim() ?? string.Empty;
        }

        internal void AssignTeacher(int teacherId)
        {
            TeacherId = teacherId;
        }

        internal void RemoveTeacher()
        {
            TeacherId = null;
        }

        internal void AddStudent(int studentId)
        {
            if (!studentIds.Contains(studentId))
                studentIds.Add(studentId);
        }

        internal void RemoveStudent(int studentId)
        {
            studentIds.Remove(studentId);
        }

        public IReadOnlyCollection<int> StudentIds => studentIds.AsReadOnly();

        public override string ToString()
        {
            var teacherPart = TeacherId.HasValue ? $"TeacherId: {TeacherId}" : "No teacher";
            return $"Course: {Title} (Id: {Id}) - {teacherPart} - {studentIds.Count} students";
        }
    }

    public class University
    {
        private readonly Dictionary<int, Student> students = new();
        private readonly Dictionary<int, Teacher> teachers = new();
        private readonly Dictionary<int, Course> courses = new();

        public Student AddStudent(string name, int age, string contact)
        {
            var s = new Student(name, age, contact);
            students[s.Id] = s;
            return s;
        }

        public IEnumerable<Student> GetAllStudents() => students.Values;

        public Student FindStudent(int id) => students.TryGetValue(id, out var s) ? s : null;

        public Teacher AddTeacher(string name, int age, string contact)
        {
            var t = new Teacher(name, age, contact);
            teachers[t.Id] = t;
            return t;
        }

        public IEnumerable<Teacher> GetAllTeachers() => teachers.Values;

        public Teacher FindTeacher(int id) => teachers.TryGetValue(id, out var t) ? t : null;

        public Course AddCourse(string title, string description)
        {
            var c = new Course(title, description);
            courses[c.Id] = c;
            return c;
        }

        public IEnumerable<Course> GetAllCourses() => courses.Values;

        public Course FindCourse(int id) => courses.TryGetValue(id, out var c) ? c : null;

        public bool AssignTeacherToCourse(int teacherId, int courseId)
        {
            var t = FindTeacher(teacherId);
            var c = FindCourse(courseId);
            if (t == null || c == null) return false;
            c.AssignTeacher(teacherId);
            t.AssignCourse(courseId);
            return true;
        }

        public bool EnrollStudentToCourse(int studentId, int courseId)
        {
            var s = FindStudent(studentId);
            var c = FindCourse(courseId);
            if (s == null || c == null) return false;
            c.AddStudent(studentId);
            s.Enroll(courseId);
            return true;
        }

        public IEnumerable<Student> GetStudentsInCourse(int courseId)
        {
            var c = FindCourse(courseId);
            if (c == null) yield break;
            foreach (var sid in c.StudentIds)
            {
                var s = FindStudent(sid);
                if (s != null) yield return s;
            }
        }

        public IEnumerable<Course> GetCoursesOfStudent(int studentId)
        {
            var s = FindStudent(studentId);
            if (s == null) yield break;
            foreach (var cid in s.CourseIds)
            {
                var c = FindCourse(cid);
                if (c != null) yield return c;
            }
        }
    }

    class Program
    {
        static University uni = new University();

        static void Main(string[] args)
        {
            SeedSampleData();
            RunMenu();
        }

        static void RunMenu()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Универ");
                Console.WriteLine("1. Добавить студента");
                Console.WriteLine("2. Просмотреть всех студентов");
                Console.WriteLine("3. Добавить преподавателя");
                Console.WriteLine("4. Просмотреть всех преподавателей");
                Console.WriteLine("5. Добавить курс");
                Console.WriteLine("6. Просмотреть все курсы");
                Console.WriteLine("7. Назначить преподавателя на курс");
                Console.WriteLine("8. Записать студента на курс");
                Console.WriteLine("9. Показать студентов на курсе");
                Console.WriteLine("10. Показать курсы студента");
                Console.WriteLine("11. Полные списки");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите пункт: ");
                var choice = Console.ReadLine()?.Trim();

                Console.WriteLine();
                try
                {
                    switch (choice)
                    {
                        case "1": AddStudent(); break;
                        case "2": ListStudents(); break;
                        case "3": AddTeacher(); break;
                        case "4": ListTeachers(); break;
                        case "5": AddCourse(); break;
                        case "6": ListCourses(); break;
                        case "7": AssignTeacher(); break;
                        case "8": EnrollStudent(); break;
                        case "9": ShowStudentsInCourse(); break;
                        case "10": ShowCoursesOfStudent(); break;
                        case "11": ShowAllLists(); break;
                        case "0": return;
                        default: Console.WriteLine("Неверный выбор."); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }
        }

        static void AddStudent()
        {
            Console.Write("Имя студента: ");
            var name = Console.ReadLine();
            var age = ReadInt("Возраст: ");
            Console.Write("Контакт: ");
            var contact = Console.ReadLine();

            var s = uni.AddStudent(name, age, contact);
            Console.WriteLine($"Добавлен студент: {s}.");
        }

        static void ListStudents()
        {
            var list = uni.GetAllStudents().ToList();
            if (!list.Any()) { Console.WriteLine("Студентов нет."); return; }
            Console.WriteLine("Список студентов:");
            foreach (var s in list)
                Console.WriteLine($"{s.Id}. {s.Name} | Age: {s.Age} | Contact: {s.Contact}");
        }

        static void AddTeacher()
        {
            Console.Write("Имя преподавателя: ");
            var name = Console.ReadLine();
            var age = ReadInt("Возраст: ");
            Console.Write("Контакт: ");
            var contact = Console.ReadLine();

            var t = uni.AddTeacher(name, age, contact);
            Console.WriteLine($"Добавлен преподаватель: {t}.");
        }

        static void ListTeachers()
        {
            var list = uni.GetAllTeachers().ToList();
            if (!list.Any()) { Console.WriteLine("Преподавателей нет."); return; }
            Console.WriteLine("Список преподавателей:");
            foreach (var t in list)
                Console.WriteLine($"{t.Id}. {t.Name} | Age: {t.Age} | Contact: {t.Contact}");
        }

        static void AddCourse()
        {
            Console.Write("Название курса: ");
            var title = Console.ReadLine();
            Console.Write("Описание курса: ");
            var desc = Console.ReadLine();

            var c = uni.AddCourse(title, desc);
            Console.WriteLine($"Добавлен курс: {c.Title} (Id: {c.Id})");
        }

        static void ListCourses()
        {
            var list = uni.GetAllCourses().ToList();
            if (!list.Any()) { Console.WriteLine("Курсов нет."); return; }
            Console.WriteLine("Список курсов:");
            foreach (var c in list)
            {
                var teacher = c.TeacherId.HasValue ? (uni.FindTeacher(c.TeacherId.Value)?.Name ?? "Unknown") : "не назначен";
                Console.WriteLine($"{c.Id}. {c.Title} | Teacher: {teacher} | Students: {c.StudentIds.Count}");
            }
        }

        static void AssignTeacher()
        {
            Console.WriteLine("Выберите преподавателя (Id):");
            ListTeachers();
            var tId = ReadInt("Id преподавателя: ");
            Console.WriteLine("Выберите курс (Id):");
            ListCourses();
            var cId = ReadInt("Id курса: ");

            var ok = uni.AssignTeacherToCourse(tId, cId);
            Console.WriteLine(ok ? "Преподаватель назначен." : "Не удалось назначить.");
        }

        static void EnrollStudent()
        {
            Console.WriteLine("Выберите студента (Id):");
            ListStudents();
            var sId = ReadInt("Id студента: ");
            Console.WriteLine("Выберите курс (Id):");
            ListCourses();
            var cId = ReadInt("Id курса: ");

            var ok = uni.EnrollStudentToCourse(sId, cId);
            Console.WriteLine(ok ? "Студент записан на курс." : "Не удалось записать.");
        }

        static void ShowStudentsInCourse()
        {
            Console.WriteLine("Выберите курс (Id):");
            ListCourses();
            var cId = ReadInt("Id курса: ");
            var students = uni.GetStudentsInCourse(cId).ToList();
            if (!students.Any()) { Console.WriteLine("На курсе нет студентов."); return; }
            Console.WriteLine("Студенты на курсе:");
            foreach (var s in students)
                Console.WriteLine($"{s.Id}. {s.Name} | Contact: {s.Contact}");
        }

        static void ShowCoursesOfStudent()
        {
            Console.WriteLine("Выберите студента (Id):");
            ListStudents();
            var sId = ReadInt("Id студента: ");
            var courses = uni.GetCoursesOfStudent(sId).ToList();
            if (!courses.Any()) { Console.WriteLine("У студента нет курсов."); return; }
            Console.WriteLine("Курсы студента:");
            foreach (var c in courses)
                Console.WriteLine($"{c.Id}. {c.Title} | Description: {c.Description}");
        }

        static void ShowAllLists()
        {
            Console.WriteLine("Все студенты");
            ListStudents();
            Console.WriteLine();
            Console.WriteLine("Все преподаватели");
            ListTeachers();
            Console.WriteLine();
            Console.WriteLine("Все курсы");
            ListCourses();
        }

        static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out var val)) return val;
                Console.WriteLine("Неверный ввод.");
            }
        }

        static void SeedSampleData()
        {
            var t1 = uni.AddTeacher("Иван Петров", 45, "ivan.petrov@example.com");
            var t2 = uni.AddTeacher("Мария Смирнова", 38, "m.smirnova@example.com");

            var c1 = uni.AddCourse("Алгоритмы и структуры данных", "Базовый курс по алгоритмам.");
            var c2 = uni.AddCourse("Базы данных", "Введение в реляционные СУБД и SQL.");
            var c3 = uni.AddCourse("Операционные системы", "Основы ОС и управление процессами.");

            uni.AssignTeacherToCourse(t1.Id, c1.Id);
            uni.AssignTeacherToCourse(t2.Id, c2.Id);

            var s1 = uni.AddStudent("Андрей Водяной", 20, "andrey@example.com");
            var s2 = uni.AddStudent("Ольга Иванова", 19, "olga@example.com");

            uni.EnrollStudentToCourse(s1.Id, c1.Id);
            uni.EnrollStudentToCourse(s1.Id, c2.Id);
            uni.EnrollStudentToCourse(s2.Id, c2.Id);
        }
    }
}
