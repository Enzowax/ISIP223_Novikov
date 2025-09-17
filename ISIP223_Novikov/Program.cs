using System;
using System.Collections.Generic;
using System.Linq;

namespace StoreInventory
{
    // Перечисление категорий товаров (на русском языке)
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


