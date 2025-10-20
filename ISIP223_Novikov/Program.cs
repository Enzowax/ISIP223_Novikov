using System;
using System.Collections.Generic;

namespace TextRoguelike
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Run();
        }
    }

    public class Item
    {
        public string Name { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }

        public Item(string name, int attack = 0, int defense = 0)
        {
            Name = name;
            Attack = attack;
            Defense = defense;
        }
    }

    public class Enemy
    {
        public string Name { get; set; }
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public string Type { get; set; }
        public bool IsBoss { get; set; }
        public int CritChancePercent { get; set; }
        public int FreezeChancePercent { get; set; }

        public Enemy(string name, int hp, int attack, int defense, string type, bool isBoss = false)
        {
            Name = name;
            HP = hp;
            MaxHP = hp;
            Attack = attack;
            Defense = defense;
            Type = type;
            IsBoss = isBoss;
        }
    }

    public class Player
    {
        public int MaxHP { get; set; } = 100;
        public int HP { get; set; } = 100;
        public Item Weapon { get; set; }
        public Item Armor { get; set; }
        public bool Frozen { get; set; }
        public bool DefendingThisTurn { get; set; }

        public Player()
        {
            Weapon = new Item("Ничего", 1, 0);
            Armor = new Item("Семейники деда", 0, 1);
        }

        public int TotalAttack => Weapon.Attack;
        public int TotalDefense => Armor.Defense;
    }

    public class Game
    {
        private Player player;
        private Random rnd;
        private int turn;

        private readonly Enemy goblinBase = new Enemy("Гоблин", 20, 8, 3, "goblin") { CritChancePercent = 20 };
        private readonly Enemy skeletonBase = new Enemy("Скелет", 25, 7, 2, "skeleton");
        private readonly Enemy mageBase = new Enemy("Маг", 15, 10, 1, "mage") { FreezeChancePercent = 20 };

        public Game()
        {
            player = new Player();
            rnd = new Random();
            turn = 1;
        }

        public void Run()
        {
            Console.WriteLine("Вы отправляетесь в опасное подземелье...");

            while (player.HP > 0)
            {
                Console.WriteLine($"Ход {turn}");
                player.DefendingThisTurn = false;
                DisplayPlayerStatus();

                Console.WriteLine("Нажмите Enter для продолжения...");
                Console.ReadLine();

                if (rnd.Next(2) == 0)
                {
                    Enemy enemy = CreateEnemy();
                    if (enemy.IsBoss)
                        Console.WriteLine($"\n!!! Появился босс: {enemy.Name} !!!");
                    else
                        Console.WriteLine($"\nПоявился враг: {enemy.Name}");

                    if (!Battle(enemy)) break;
                }
                else
                {
                    OpenChest();
                }

                turn++;
            }

            if (player.HP <= 0)
            {
                Console.WriteLine("\nВы погибли...");
                Console.WriteLine($"Вы продержались {turn} ходов");
            }
            else
            {
                Console.WriteLine("\nВы решили вернуться домой...");
                Console.WriteLine($"Итог: {turn} ходов, {player.HP} HP");
            }
        }

        private void DisplayPlayerStatus()
        {
            Console.WriteLine($"HP: {HPBar(player.HP, player.MaxHP)}  {player.HP}/{player.MaxHP}");
            Console.WriteLine($"Оружие: {player.Weapon.Name} (+{player.Weapon.Attack} атаки)");
            Console.WriteLine($"Броня: {player.Armor.Name} (+{player.Armor.Defense} защиты)");
        }

        private string HPBar(int current, int max, int length = 20)
        {
            if (max <= 0) max = 1;
            current = Math.Max(0, Math.Min(current, max));
            double fraction = (double)current / max;
            int filled = (int)Math.Round(fraction * length);

            string filledPart = new string('#', filled);
            string emptyPart = new string(' ', length - filled);

            return $"[{filledPart}{emptyPart}]";
        }

        private Enemy CreateEnemy()
        {
            if (turn % 10 == 0) return CreateBoss();

            int pick = rnd.Next(3);
            if (pick == 0) return CloneEnemy(goblinBase);
            if (pick == 1) return CloneEnemy(skeletonBase);
            return CloneEnemy(mageBase);
        }

        private Enemy CreateBoss()
        {
            var bosses = new List<Enemy>();

            var vvg = CloneEnemy(goblinBase);
            vvg.Name = "ВВГ";
            vvg.IsBoss = true;
            vvg.MaxHP = (int)(goblinBase.MaxHP * 2.0);
            vvg.HP = vvg.MaxHP;
            vvg.Attack = (int)Math.Round(goblinBase.Attack * 1.5);
            vvg.Defense = (int)Math.Round(goblinBase.Defense * 1.2);
            vvg.CritChancePercent = goblinBase.CritChancePercent + 10;
            bosses.Add(vvg);

            var kov = CloneEnemy(skeletonBase);
            kov.Name = "Ковальский";
            kov.IsBoss = true;
            kov.MaxHP = (int)(skeletonBase.MaxHP * 2.5);
            kov.HP = kov.MaxHP;
            kov.Attack = (int)Math.Round(skeletonBase.Attack * 1.3);
            kov.Defense = (int)Math.Round(skeletonBase.Defense * 1.4);
            bosses.Add(kov);

            var archmage = CloneEnemy(mageBase);
            archmage.Name = "Архимаг C++";
            archmage.IsBoss = true;
            archmage.MaxHP = (int)(mageBase.MaxHP * 1.8);
            archmage.HP = archmage.MaxHP;
            archmage.Attack = (int)Math.Round(mageBase.Attack * 1.6);
            archmage.Defense = (int)Math.Round(mageBase.Defense * 1.1);
            archmage.FreezeChancePercent = mageBase.FreezeChancePercent + 10;
            bosses.Add(archmage);

            var pestov = CloneEnemy(skeletonBase);
            pestov.Name = "Пестов С--";
            pestov.IsBoss = true;
            pestov.MaxHP = (int)(skeletonBase.MaxHP * 1.3);
            pestov.HP = pestov.MaxHP;
            pestov.Attack = (int)Math.Round(skeletonBase.Attack * 1.8);
            pestov.Defense = (int)Math.Round(skeletonBase.Defense * 0.6);
            pestov.FreezeChancePercent = mageBase.FreezeChancePercent + 15;
            bosses.Add(pestov);

            return bosses[rnd.Next(bosses.Count)];
        }

        private Enemy CloneEnemy(Enemy baseEnemy)
        {
            return new Enemy(baseEnemy.Name, baseEnemy.MaxHP, baseEnemy.Attack, baseEnemy.Defense, baseEnemy.Type, baseEnemy.IsBoss)
            {
                CritChancePercent = baseEnemy.CritChancePercent,
                FreezeChancePercent = baseEnemy.FreezeChancePercent
            };
        }

        private bool Battle(Enemy enemy)
        {
            Console.WriteLine($"Бой с {enemy.Name}");
            Console.WriteLine($"Ваше HP:  {HPBar(player.HP, player.MaxHP)}  {player.HP}/{player.MaxHP}");
            Console.WriteLine($"HP врага: {HPBar(enemy.HP, enemy.MaxHP)}  {enemy.HP}/{enemy.MaxHP}");

            while (player.HP > 0 && enemy.HP > 0)
            {
                if (player.Frozen)
                {
                    Console.WriteLine("Вы заморожены и пропускаете ход!");
                    player.Frozen = false;
                }
                else
                {
                    Console.WriteLine("\nВыберите действие:");
                    Console.WriteLine("1 - Атака");
                    Console.WriteLine("2 - Защита");
                    string choice = Console.ReadLine();

                    if (choice == "1") PlayerAttack(enemy);
                    else if (choice == "2") player.DefendingThisTurn = true;
                }

                if (enemy.HP <= 0)
                {
                    Console.WriteLine($"{enemy.Name} побеждён!");
                    break;
                }

                EnemyAttack(enemy);

                Console.WriteLine($"\nВаше HP:  {HPBar(player.HP, player.MaxHP)}  {player.HP}/{player.MaxHP}");
                Console.WriteLine($"HP врага: {HPBar(enemy.HP, enemy.MaxHP)}  {enemy.HP}/{enemy.MaxHP}");
            }

            return player.HP > 0;
        }

        private void PlayerAttack(Enemy enemy)
        {
            int damage = player.TotalAttack;
            enemy.HP -= damage;
            Console.WriteLine($"Вы наносите {damage} урона.");
        }

        private void EnemyAttack(Enemy enemy)
        {
            int baseDamage = enemy.Attack;

            if (enemy.CritChancePercent > 0 && rnd.Next(100) < enemy.CritChancePercent)
            {
                baseDamage *= 2;
                Console.WriteLine("Критический удар врага!");
            }

            if (player.DefendingThisTurn && rnd.Next(100) < 40)
            {
                Console.WriteLine("Вы успешно увернулись от атаки!");
                player.DefendingThisTurn = false;
                return;
            }

            if (enemy.Type == "skeleton")
            {
                player.HP -= baseDamage;
                Console.WriteLine($"Скелет наносит {baseDamage} урона (игнорирует броню).");
            }
            else
            {
                int blockedAmount = (int)(player.TotalDefense * (rnd.Next(70, 101) / 100.0));
                int finalDamage = Math.Max(0, baseDamage - blockedAmount);
                player.HP -= finalDamage;
                Console.WriteLine($"Враг наносит {baseDamage} урона. Блокировано: {blockedAmount}. Вы получаете: {finalDamage} урона.");
            }

            if (enemy.FreezeChancePercent > 0 && rnd.Next(100) < enemy.FreezeChancePercent)
            {
                player.Frozen = true;
                Console.WriteLine("Враг наложил заморозку — вы пропустите следующий ход!");
            }

            player.DefendingThisTurn = false;
        }

        private void OpenChest()
        {
            var items = new List<Item>
            {
                new Item("Лечебное зелье"),
                new Item("Меч воина", 15, 0),
                new Item("Кинжал разбойника", 12, 0),
                new Item("Посох мага", 10, 0),
                new Item("Кожаная броня", 0, 8),
                new Item("Кольчуга", 0, 12),
                new Item("Мантия мага", 0, 6)
            };

            Item drop = items[rnd.Next(items.Count)];
            Console.WriteLine($"\nВы открыли сундук и нашли: {drop.Name}");

            if (drop.Name == "Лечебное зелье")
            {
                player.HP = player.MaxHP;
                Console.WriteLine("Зелье восстанавливает здоровье до максимума!");
                return;
            }

            if (drop.Attack > 0)
            {
                Console.WriteLine($"Оружие: Атака +{drop.Attack}");
                Console.WriteLine($"Текущее оружие: {player.Weapon.Name} (Атака +{player.Weapon.Attack})");
            }
            else
            {
                Console.WriteLine($"Доспехи: Защита +{drop.Defense}");
                Console.WriteLine($"Текущие доспехи: {player.Armor.Name} (Защита +{player.Armor.Defense})");
            }

            Console.WriteLine("1 - Взять предмет");
            Console.WriteLine("2 - Выбросить предмет");

            if (Console.ReadLine() == "1")
            {
                if (drop.Attack > 0) player.Weapon = drop;
                else player.Armor = drop;
                Console.WriteLine("Предмет экипирован.");
            }
        }
    }
}