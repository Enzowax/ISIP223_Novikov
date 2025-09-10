using System;
using System.Collections.Generic;
using System.Globalization;

namespace ExpenseApp
{
    public class Expense
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public Expense(string name, decimal amount)
        {
            Name = name;
            Amount = amount;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var expenses = new List<Expense>();

            int n = ReadIntInRange("Введите количество операций (от 2 до 40): ", 2, 40);

            for (int i = 0; i < n; i++)
            {
                Console.WriteLine($"\nОперация #{i + 1}");
                Console.Write("Введите название товара/услуги: ");
                string name = (Console.ReadLine() ?? "").Trim();
                decimal amount = ReadDecimal("Введите сумму в рублях: ");
                expenses.Add(new Expense(name, amount));
            }

            while (true)
            {
                Console.WriteLine("\nМеню:");
                Console.WriteLine("1. Вывод данных");
                Console.WriteLine("2. Статистика (среднее, максимум, минимум, сумма)");
                Console.WriteLine("3. Сортировка по цене (пузырьковая сортировка)");
                Console.WriteLine("4. Конвертация валюты (курс вводит пользователь или выбирает из списка)");
                Console.WriteLine("5. Поиск по названию");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите пункт: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        PrintExpenses(expenses);
                        break;

                    case "2":
                        PrintStats(expenses);
                        break;

                    case "3":
                        BubbleSortByPrice(expenses);
                        Console.WriteLine("\nОтсортировано по возрастанию цены:");
                        PrintExpenses(expenses);
                        break;

                    case "4":
                        ConvertCurrency(expenses);
                        break;

                    case "5":
                        DoSearch(expenses);  
                        break;

                    case "0":
                        Console.WriteLine("До свидания!");
                        return;

                    default:
                        Console.WriteLine("Неверный пункт меню. Повторите ввод.");
                        break;
                }
            }
        }

        static int ReadIntInRange(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                string s = Console.ReadLine();
                int val;
                if (int.TryParse(s, out val) && val >= min && val <= max)
                    return val;
                Console.WriteLine($"Ошибка: введите целое число от {min} до {max}.");
            }
        }

        static decimal ReadDecimal(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string s = (Console.ReadLine() ?? "").Trim();
                s = s.Replace(',', '.'); 
                decimal v;
                if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out v))
                    return v;
                Console.WriteLine("Ошибка: введите число (например, 123.45).");
            }
        }

        static void PrintExpenses(List<Expense> expenses)
        {
            if (expenses.Count == 0)
            {
                Console.WriteLine("Список пуст.");
                return;
            }

            Console.WriteLine("\nВаши расходы (руб.):");
            int i = 1;
            foreach (var e in expenses)
                Console.WriteLine($"{i++,2}. {e.Name} — {e.Amount:F2} ₽");
        }

     
        static void PrintStats(List<Expense> expenses)
        {
            if (expenses.Count == 0)
            {
                Console.WriteLine("Нет данных для статистики.");
                return;
            }

            decimal sum = 0m, min = decimal.MaxValue, max = decimal.MinValue;
            foreach (var e in expenses)
            {
                sum += e.Amount;
                if (e.Amount < min) min = e.Amount;
                if (e.Amount > max) max = e.Amount;
            }
            decimal avg = sum / expenses.Count;

            Console.WriteLine("\nСтатистика (руб.):");
            Console.WriteLine($"Сумма:    {sum:F2} ₽");
            Console.WriteLine($"Среднее:  {avg:F2} ₽");
            Console.WriteLine($"Максимум: {max:F2} ₽");
            Console.WriteLine($"Минимум:  {min:F2} ₽");
        }

        static void BubbleSortByPrice(List<Expense> expenses)
        {
            int n = expenses.Count;
            for (int i = 0; i < n; i++)
            {
                bool swapped = false;
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (expenses[j].Amount > expenses[j + 1].Amount)
                    {
                        var tmp = expenses[j];
                        expenses[j] = expenses[j + 1];
                        expenses[j + 1] = tmp;
                        swapped = true;
                    }
                }
                if (!swapped) break;
            }
        }

        static void ConvertCurrency(List<Expense> expenses)
        {
            if (expenses.Count == 0)
            {
                Console.WriteLine("Нет данных для конвертации.");
                return;
            }

            Console.WriteLine("\nВыберите вариант:");
            Console.WriteLine("1. Ввести курс вручную");
            Console.WriteLine("2. Выбрать из списка (примерные курсы)");
            Console.Write("Ваш выбор: ");
            string c = Console.ReadLine();

            decimal rate;
            string suffix;

            if (c == "2")
            {
                string[] names = { "Доллар США (USD)", "Евро (EUR)", "Тенге (KZT)", "Юань (CNY)" };
                decimal[] rates = { 0.011m, 0.010m, 5.00m, 0.079m };

                for (int i = 0; i < names.Length; i++)
                    Console.WriteLine($"{i + 1}. {names[i]} — множитель {rates[i]}");

                int pick = ReadIntInRange("Выберите валюту: ", 1, names.Length);
                rate = rates[pick - 1];
                suffix = new[] { "USD", "EUR", "KZT", "CNY" }[pick - 1];
            }
            else
            {
                rate = ReadDecimal("Введите курс (множитель, во что умножать рубли): ");
                Console.Write("Введите код валюты (например, USD): ");
                suffix = (Console.ReadLine() ?? "CUR").Trim().ToUpperInvariant();
                if (string.IsNullOrWhiteSpace(suffix)) suffix = "CUR";
            }

            Console.WriteLine($"\nКонвертация по курсу × {rate} ({suffix}):");
            int i2 = 1;
            foreach (var e in expenses)
                Console.WriteLine($"{i2++,2}. {e.Name} — {(e.Amount * rate):F2} {suffix}");
        }

        // ----- Поиск -----
        static void DoSearch(List<Expense> expenses)
        {
            Console.Write("Введите строку для поиска по названию: ");
            string needle = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

            var found = new List<Tuple<int, Expense>>();
            for (int i = 0; i < expenses.Count; i++)
                if (!string.IsNullOrEmpty(expenses[i].Name) &&
                    expenses[i].Name.ToLowerInvariant().Contains(needle))
                    found.Add(Tuple.Create(i, expenses[i]));

            if (found.Count == 0)
            {
                Console.WriteLine("Совпадений не найдено.");
                return;
            }

            Console.WriteLine("\nНайдено:");
            foreach (var item in found)
                Console.WriteLine($"{item.Item1 + 1,2}. {item.Item2.Name} — {item.Item2.Amount:F2} ₽");
        }
    }
}
