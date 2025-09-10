using System;
using System.Collections.Generic;
using System.Globalization;

namespace ExpenseTrackerStrict
{
    public class Expense
    {
        public string Name;
        public decimal Amount;
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

            int n = ReadIntInRange("Введите количество операций, которые будут записаны (2–40): ", 2, 40);

            var expenses = new List<Expense>(n);
            Console.WriteLine("\nВводите траты по шаблону: Название - Сумма");

            for (int i = 0; i < n; i++)
            {
                Expense e = ReadExpense(i + 1);
                expenses.Add(e);
            }


            while (true)
            {
                Console.WriteLine("\nМеню:");
                Console.WriteLine("1. Вывод данных");
                Console.WriteLine("2. Статистика (среднее, максимальное, минимальное, сумма)");
                Console.WriteLine("3. Сортировка по цене (пузырьковая сортировка)");
                Console.WriteLine("4. Конвертация валюты (пользователь вводит курс или выбирает из списка)");
                Console.WriteLine("5. Поиск по названию");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите пункт: ");

                string choice = (Console.ReadLine() ?? "").Trim();

                switch (choice)
                {
                    case "1":
                        PrintData(expenses);
                        break;

                    case "2":
                        PrintStats(expenses);
                        break;

                    case "3":
                        BubbleSort(expenses); 
                        Console.WriteLine("\nОтсортировано по возрастанию суммы.");
                        PrintData(expenses);
                        break;

                    case "4":
                        ConvertCurrency(expenses);
                        break;

                    case "5":
                        SearchByName(expenses);
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Неверный пункт. Повторите ввод.");
                        break;
                }
            }
        }

  
        static Expense ReadExpense(int index)
        {
            while (true)
            {
                Console.Write($"\nТрата #{index}: ");
                string line = (Console.ReadLine() ?? "").Trim();

                int sep = line.LastIndexOf('-'); 
                if (sep <= 0 || sep >= line.Length - 1)
                {
                    Console.WriteLine("Ошибка формата. Введите по образцу: Название - 123.45");
                    continue;
                }

                string name = line.Substring(0, sep).Trim();
                string amountStr = line.Substring(sep + 1).Trim().Replace(',', '.');

                decimal amount;
                if (string.IsNullOrWhiteSpace(name) ||
                    !decimal.TryParse(amountStr, NumberStyles.Number, CultureInfo.InvariantCulture, out amount) ||
                    amount < 0)
                {
                    Console.WriteLine("Ошибка. Название не должно быть пустым, сумма — неотрицательное число.");
                    continue;
                }

                return new Expense(name, amount);
            }
        }

        // --- Вывод данных ---
        static void PrintData(List<Expense> expenses)
        {
            Console.WriteLine("\nВсе траты (рубли):");
            for (int i = 0; i < expenses.Count; i++)
                Console.WriteLine($"{i + 1,2}. {expenses[i].Name} — {expenses[i].Amount:F2} руб.");
        }

        static void PrintStats(List<Expense> expenses)
        {
            decimal sum = 0m, min = decimal.MaxValue, max = decimal.MinValue;
            foreach (var e in expenses)
            {
                sum += e.Amount;
                if (e.Amount < min) min = e.Amount;
                if (e.Amount > max) max = e.Amount;
            }
            decimal avg = sum / expenses.Count;

            Console.WriteLine("\nСтатистика (рубли):");
            Console.WriteLine($"Сумма:       {sum:F2}");
            Console.WriteLine($"Среднее:     {avg:F2}");
            Console.WriteLine($"Максимальное:{max:F2}");
            Console.WriteLine($"Минимальное: {min:F2}");
        }

        static void BubbleSort(List<Expense> expenses)
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
            Console.WriteLine("\nКонвертация валюты:");
            Console.WriteLine("1. Ввести курс вручную");
            Console.WriteLine("2. Выбрать из списка (примерные множители от рубля)");
            Console.Write("Ваш выбор: ");
            string mode = (Console.ReadLine() ?? "").Trim();

            decimal rate;
            string code;

            if (mode == "2")
            {
                string[] codes = { "USD", "EUR", "KZT", "CNY" };
                string[] names = { "Доллар США", "Евро", "Тенге", "Юань" };
                decimal[] rates = { 0.011m, 0.010m, 5.000m, 0.079m };

                Console.WriteLine();
                for (int i = 0; i < codes.Length; i++)
                    Console.WriteLine($"{i + 1}. {names[i]} ({codes[i]}) — множитель {rates[i]}");

                int pick = ReadIntInRange("Выберите валюту: ", 1, codes.Length);
                rate = rates[pick - 1];
                code = codes[pick - 1];
            }
            else
            {
                rate = ReadDecimal("Введите курс (множитель, во что умножать рубли): ");
                Console.Write("Введите код валюты (например, USD): ");
                code = ((Console.ReadLine() ?? "CUR").Trim().ToUpperInvariant());
                if (string.IsNullOrWhiteSpace(code)) code = "CUR";
            }

            Console.WriteLine($"\nРезультат конвертации по курсу × {rate} ({code}):");
            for (int i = 0; i < expenses.Count; i++)
                Console.WriteLine($"{i + 1,2}. {expenses[i].Name} — {expenses[i].Amount * rate:F2} {code}");
        }

        static void SearchByName(List<Expense> expenses)
        {
            Console.Write("Введите строку для поиска: ");
            string needle = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

            bool any = false;
            for (int i = 0; i < expenses.Count; i++)
            {
                if (expenses[i].Name.ToLowerInvariant().Contains(needle))
                {
                    if (!any) Console.WriteLine("\nНайдено:");
                    any = true;
                    Console.WriteLine($"{i + 1,2}. {expenses[i].Name} — {expenses[i].Amount:F2} руб.");
                }
            }
            if (!any) Console.WriteLine("Совпадений не найдено.");
        }

        static int ReadIntInRange(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                string s = (Console.ReadLine() ?? "").Trim();
                int v;
                if (int.TryParse(s, out v) && v >= min && v <= max) return v;
                Console.WriteLine($"Ошибка: введите целое число от {min} до {max}.");
            }
        }

        static decimal ReadDecimal(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string s = (Console.ReadLine() ?? "").Trim().Replace(',', '.');
                decimal v;
                if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out v) && v >= 0)
                    return v;
                Console.WriteLine("Ошибка: введите неотрицательное число (пример: 123.45).");
            }
        }
    }
}
