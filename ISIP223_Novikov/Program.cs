using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversityManagement
{
    public abstract class Person
    {
        public Guid Id { get; }
        public string Name { get; private set; }
        public int Age { get; private set; }
        public string Contact { get; private set; }

        protected Person(string name, int age, string contact)
        {
            Id = Guid.NewGuid();
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
            return $"{Name} (Id: {Id.ToString().Substring(0, 8)}), Age: {Age}, Contact: {Contact}";
        }
    }

   