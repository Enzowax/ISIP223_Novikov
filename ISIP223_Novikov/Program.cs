using System;
using System.Collections.Generic;
using System.Linq;

namespace StoreInventory
{
    // Перечисление категорий товаров 
    enum Category
    {
        Еда,
        Электроника,
        Одежда
    }

    // Класс описывающий товар
    class Product
    {
        private static int counter = 1; // Счётчик для генерации уникального кода
        public int Code { get; private set; }        // Уникальный код товара
        public string Name { get; private set; }     // Название товара
        public decimal Price { get; private set; }   // Цена
        public int Quantity { get; private set; }    // Количество на складе
        public bool InStock => Quantity > 0;         // Есть ли товар в наличии
        public Category Category { get; private set; } // Категория товара

        // Конструктор
        public Product(string name, decimal price, int quantity, Category category)
        {
            // Проверка на корректность данных
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название товара не может быть пустым!");
            if (price < 0)
                throw new ArgumentException("Цена не может быть отрицательной!");
            if (quantity < 0)
                throw new ArgumentException("Количество не может быть отрицательным!");

            Code = counter++; // Автоматическая генерация кода
            Name = name;
            Price = price;
            Quantity = quantity;
            Category = category;
        }

        // Метод для пополнения склада
        public void AddStock(int amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine("Количество должно быть положительным!");
                return;
            }
            Quantity += amount;
        }

        // Метод для продажи товара
        public bool Sell(int amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine("Количество должно быть положительным!");
                return false;
            }
            if (Quantity >= amount)
            {
                Quantity -= amount;
                return true;
            }
            else
            {
                Console.WriteLine("Недостаточно товара на складе!");
                return false;
            }
        }

        // Переопределение ToString() для красивого вывода
        public override string ToString()
        {
            return $"Код: {Code}, Название: {Name}, Цена: {Price} руб, " +
                   $"Количество: {Quantity}, В наличии: {(InStock ? "Да" : "Нет")}, " +
                   $"Категория: {Category}";
        }
    }

    class Program
    {
        static List<Product> products = new List<Product>(); // Список всех товаров

        static void Main()
        {
            // Добавляем 5 тестовых товаров
            products.Add(new Product("Хлеб", 30, 10, Category.Еда));
            products.Add(new Product("Телефон", 15000, 3, Category.Электроника));
            products.Add(new Product("Джинсы", 2500, 5, Category.Одежда));
            products.Add(new Product("Молоко", 60, 8, Category.Еда));
            products.Add(new Product("Наушники", 2000, 2, Category.Электроника));

            // Основное меню программы
            while (true)
            {
                Console.WriteLine("\n--- Учёт товаров в магазине ---");
                Console.WriteLine("1. Добавить товар");
                Console.WriteLine("2. Удалить товар");
                Console.WriteLine("3. Заказать поставку");
                Console.WriteLine("4. Продать товар");
                Console.WriteLine("5. Поиск товара");
                Console.WriteLine("6. Показать все товары");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите действие: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": AddProduct(); break;
                    case "2": DeleteProduct(); break;
                    case "3": OrderProduct(); break;
                    case "4": SellProduct(); break;
                    case "5": SearchProduct(); break;
                    case "6": ShowAllProducts(); break;
                    case "0": return;
                    default: Console.WriteLine("Неверный ввод!"); break;
                }
            }
        }

        // Добавление товара
        static void AddProduct()
        {
            Console.Write("Введите название: ");
            string name = Console.ReadLine();

            Console.Write("Введите цену: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price < 0)
            {
                Console.WriteLine("Некорректная цена!");
                return;
            }

            Console.Write("Введите количество: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity < 0)
            {
                Console.WriteLine("Некорректное количество!");
                return;
            }

            Console.WriteLine("Выберите категорию: 0 - Еда, 1 - Электроника, 2 - Одежда");
            if (!Enum.TryParse(Console.ReadLine(), out Category category))
            {
                Console.WriteLine("Некорректная категория!");
                return;
            }

            try
            {
                products.Add(new Product(name, price, quantity, category));
                Console.WriteLine("Товар добавлен!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        // Удаление товара
        static void DeleteProduct()
        {
            Console.Write("Введите код товара для удаления: ");
            if (int.TryParse(Console.ReadLine(), out int code))
            {
                var product = products.FirstOrDefault(p => p.Code == code);
                if (product != null)
                {
                    products.Remove(product);
                    Console.WriteLine("Товар удалён.");
                }
                else Console.WriteLine("Товар не найден!");
            }
        }
        // Заказ поставки (увеличение количества товара)
        static void OrderProduct()
        {
            Console.Write("Введите код товара для поставки: ");
            if (int.TryParse(Console.ReadLine(), out int code))
            {
                var product = products.FirstOrDefault(p => p.Code == code);
                if (product != null)
                {
                    Console.Write("Введите количество: ");
                    if (int.TryParse(Console.ReadLine(), out int amount) && amount > 0)
                    {
                        product.AddStock(amount);
                        Console.WriteLine("Поставка добавлена!");
                    }
                }
                else Console.WriteLine("Товар не найден!");
            }
        }

        // Продажа товара
        static void SellProduct()
        {
            Console.Write("Введите код товара для продажи: ");
            if (int.TryParse(Console.ReadLine(), out int code))
            {
                var product = products.FirstOrDefault(p => p.Code == code);
                if (product != null)
                {
                    Console.Write("Введите количество: ");
                    if (int.TryParse(Console.ReadLine(), out int amount) && amount > 0)
                    {
                        if (product.Sell(amount))
                            Console.WriteLine("Продажа выполнена!");
                    }
                }
                else Console.WriteLine("Товар не найден!");
            }
        }

        // Поиск товара по коду, названию или категории
        static void SearchProduct()
        {
            Console.WriteLine("Введите параметр поиска (код/название/категория): ");
            string query = Console.ReadLine().ToLower();

            var results = products.Where(p =>
                p.Code.ToString() == query ||
                p.Name.ToLower().Contains(query) ||
                p.Category.ToString().ToLower() == query);

            foreach (var product in results)
                Console.WriteLine(product);

            if (!results.Any())
                Console.WriteLine("Ничего не найдено!");
        }

        // Показать все товары
        static void ShowAllProducts()
        {
            foreach (var product in products)
                Console.WriteLine(product);
        }
    }
}
