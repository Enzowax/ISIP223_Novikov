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

