using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryApp
{
    class Program
    {
        static void Main()
        {
            var library = new List<Book>
            {
                new Book("Война и мир", "Лев Толстой", "Роман", 1869, 500),
                new Book("Преступление и наказание", "Фёдор Достоевский", "Роман", 1866, 450),
                new Book("Мастер и Маргарита", "Михаил Булгаков", "Фантастика", 1967, 600),
                new Book("Три мушкетёра", "Александр Дюма", "Приключения", 1844, 300),
                new Book("Отцы и дети", "Иван Тургенев", "Роман", 1862, 400)
            };

            while (true)
            {
                Console.WriteLine("\nМеню:");
                Console.WriteLine("1 - Добавить книгу");
                Console.WriteLine("2 - Удалить книгу по Id");
                Console.WriteLine("3 - Найти книги");
                Console.WriteLine("4 - Отсортировать книги по названию");
                Console.WriteLine("5 - Отсортировать книги по году");
                Console.WriteLine("6 - Вывести самую дорогую и самую дешёвую книгу");
                Console.WriteLine("7 - Сгруппировать книги по авторам");
                Console.WriteLine("0 - Выход");
                Console.Write("Выберите команду: ");

                if (!int.TryParse(Console.ReadLine(), out int cmd)) continue;
                if (cmd == 0) break;

                switch (cmd)
                {
                    case 1:
                        AddBook(library);
                        break;
                    case 2:
                        RemoveBook(library);
                        break;
                    case 3:
                        SearchBooks(library);
                        break;
                    case 4:
                        var byTitle = library.OrderBy(b => b.Title);
                        PrintBooks(byTitle);
                        break;
                    case 5:
                        var byYear = library.OrderBy(b => b.Year);
                        PrintBooks(byYear);
                        break;
                    case 6:
                        var max = library.OrderByDescending(b => b.Price).First();
                        var min = library.OrderBy(b => b.Price).First();
                        Console.WriteLine($"Дорогая: {max}");
                        Console.WriteLine($"Дешёвая: {min}");
                        break;
                    case 7:
                        var grouped = library.GroupBy(b => b.Author);
                        foreach (var g in grouped)
                            Console.WriteLine($"{g.Key} -> {g.Count()} книг");
                        break;
                }
            }
        }

        static void AddBook(List<Book> library)
        {
            Console.Write("Название: ");
            string title = Console.ReadLine();

            Console.Write("Автор: ");
            string author = Console.ReadLine();

            Console.Write("Жанр (Роман/Фантастика/Приключения): ");
            string genre = Console.ReadLine();

            Console.Write("Год издания: ");
            if (!int.TryParse(Console.ReadLine(), out int year) || year < 0)
            {
                Console.WriteLine("Некорректный год!");
                return;
            }

            Console.Write("Цена: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price <= 0)
            {
                Console.WriteLine("Некорректная цена!");
                return;
            }

            library.Add(new Book(title, author, genre, year, price));
            Console.WriteLine("Книга добавлена!");
        }

        static void RemoveBook(List<Book> library)
        {
            Console.Write("Введите Id для удаления: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;
            var book = library.FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                library.Remove(book);
                Console.WriteLine("Книга удалена.");
            }
            else Console.WriteLine("Книга не найдена.");
        }

        static void SearchBooks(List<Book> library)
        {
            Console.WriteLine("Поиск: 1 - по названию, 2 - по автору, 3 - по жанру");
            if (!int.TryParse(Console.ReadLine(), out int type)) return;

            Console.Write("Введите значение: ");
            string val = Console.ReadLine();

            IEnumerable<Book> result = Enumerable.Empty<Book>();

            if (type == 1)
                result = library.Where(b => b.Title.Contains(val, StringComparison.OrdinalIgnoreCase));
            else if (type == 2)
                result = library.Where(b => b.Author.Contains(val, StringComparison.OrdinalIgnoreCase));
            else if (type == 3)
                result = library.Where(b => b.Genre.Contains(val, StringComparison.OrdinalIgnoreCase));

            PrintBooks(result);
        }

        static void PrintBooks(IEnumerable<Book> books)
        {
            foreach (var b in books) Console.WriteLine(b);
        }
    }

    class Book
    {
        private static int counter = 1;
        public int Id { get; }
        public string Title { get; }
        public string Author { get; }
        public string Genre { get; }
        public int Year { get; }
        public decimal Price { get; }

        public Book(string title, string author, string genre, int year, decimal price)
        {
            Id = counter++;
            Title = title;
            Author = author;
            Genre = genre;
            Year = year;
            Price = price;
        }

        public override string ToString()
        {
            return $"[{Id}] {Title}, {Author}, {Genre}, {Year}, {Price} руб.";
        }
    }
}
