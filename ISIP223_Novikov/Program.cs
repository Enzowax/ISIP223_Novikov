using System;
using System.Collections.Generic;

class TextStatistics
{
    public string ShortestWord;
    public string LongestWord;
    public int WordCount;
    public int SentenceCount;
    public int VowelCount;
    public int ConsonantCount;
    public Dictionary<char, int> LetterFrequency = new Dictionary<char, int>();

    // Вывод статистики по тексту
    public void Print()
    {
        Console.WriteLine("=== Статистика текста ===");
        Console.WriteLine($"Количество слов: {WordCount}");
        Console.WriteLine($"Количество предложений: {SentenceCount}");
        Console.WriteLine($"Самое короткое слово: {ShortestWord}");
        Console.WriteLine($"Самое длинное слово: {LongestWord}");
        Console.WriteLine($"Гласных букв: {VowelCount}");
        Console.WriteLine($"Согласных букв: {ConsonantCount}");
        Console.WriteLine("Частота букв:");
        foreach (var pair in LetterFrequency)
        {
            Console.WriteLine($"{pair.Key}: {pair.Value}");
        }
    }
}

class Program
{
    // Проверка, является ли символ гласной
    static bool IsVowel(char c)
    {
        char lower = Char.ToLower(c);
        return "аеёиоуыэюя".IndexOf(lower) >= 0;
    }

    static TextStatistics AnalyzeText(string text)
    {
        TextStatistics stats = new TextStatistics();

        string[] words = text.Split(new char[] { ' ', ',', '.', '!', '?', ';', ':', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        stats.WordCount = words.Length;

        // Поиск самого короткого и длинного слова
        stats.ShortestWord = words[0];
        stats.LongestWord = words[0];

        for (int i = 0; i < words.Length; i++)
        {
            string w = words[i];
            if (w.Length < stats.ShortestWord.Length) stats.ShortestWord = w;
            if (w.Length > stats.LongestWord.Length) stats.LongestWord = w;
        }

        // Подсчёт предложений
        string[] sentences = text.Split(new char[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
        stats.SentenceCount = sentences.Length;

        // Подсчёт гласных и согласных + статистика по буквам
        foreach (char c in text)
        {
            if (Char.IsLetter(c))
            {
                char lower = Char.ToLower(c);

                if (IsVowel(lower)) stats.VowelCount++;
                else stats.ConsonantCount++;

                if (!stats.LetterFrequency.ContainsKey(lower))
                    stats.LetterFrequency[lower] = 0;
                stats.LetterFrequency[lower]++;
            }
        }

        return stats;
    }

    static void Main()
    {
        List<TextStatistics> history = new List<TextStatistics>();

        while (true)
        {
            Console.WriteLine("\nВведите текст (минимум 100 символов):");
            string input = Console.ReadLine();

            if (input.Length < 100)
            {
                Console.WriteLine("Ошибка: текст слишком короткий!");
                continue;
            }

            TextStatistics result = AnalyzeText(input);
            history.Add(result);

            result.Print();

        }
    }
}
