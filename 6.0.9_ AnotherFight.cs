namespace millionDollarsCourses
{
    using System;
    using System.Collections.Generic;

    //Создать 5 классов, пользователь выбирает 2 воина и они сражаются друг с другом до смерти. У каждого класса могут быть свои статы.
    //Каждый класс должен иметь особую способность для атаки, которая свойственна только его типу класса!
    //Если можно выбрать одинаковых бойцов, то это не должна быть одна и та же ссылка на одного бойца, чтобы он не атаковал сам себя.
    //Пример, что может быть уникальное у бойцов. Кто-то каждый 3 удар наносит удвоенный урон, другой имеет 30% увернуться от полученного урона,
    //кто-то при получении урона немного себя лечит. Будут новые поля у наследников. У кого-то может быть мана и это только его особенность.

    //todo:
    //think about public method takeDamage - is it safe? no! - so watch store from the video how we take away the money
    //special abilities dualist and next

    internal class Program
    {
        static void Main()
        {
            BattleSystem battleSystem = new BattleSystem();

            battleSystem.SelectFighter();
            battleSystem.Fight();
        }

        class BattleSystem
        {
            private int _round = 0;
            private Fighter _fighter1;
            private Fighter _fighter2;
            List<Fighter> fighters = new List<Fighter>
                {
                    new Knight(),
                    new Thief(),
                    new Dualist(),
                    new Summoner(),
                    new BloodHunter()
                };

            public BattleSystem()
            {

            }

            public void SelectFighter()
            {
                int fighterNumOne;
                int fighterNumTwo;
                bool isUniqueFighter = false;

                DisplayFighters();
                fighterNumOne = Custom.GetUserNumberInRange("Select a fighter: ", fighters.Count);
                _fighter1 = fighters[fighterNumOne - 1];

                while (!isUniqueFighter)
                {
                    fighterNumTwo = Custom.GetUserNumberInRange($"Select opponent for {_fighter1.GetType().Name}:", fighters.Count);

                    if (fighterNumTwo != fighterNumOne)
                    {
                        _fighter2 = fighters[fighterNumTwo - 1];
                        isUniqueFighter = true;
                    }
                    else
                    {
                        Custom.WriteLineInColor($"{_fighter1.GetType().Name} can't fight with himself, can't he?");
                        Custom.PressAnythingToContinue();
                        DisplayFighters();
                    }
                }

                Custom.WriteLineInColor($"\n{_fighter1.GetType().Name} vs {_fighter2.GetType().Name}", ConsoleColor.Blue);

                Custom.PressAnythingToContinue();
            }

            public void DisplayFighters()
            {
                Custom.WriteLineInColor("Fighters:\n", ConsoleColor.DarkGray);

                int count = 1;

                foreach (Fighter fighter in fighters)
                {
                    Console.Write(count + ". ");
                    count++;
                    fighter.DisplayStats();
                    Console.WriteLine();
                }
            }

            public void Fight()
            {
                while (_fighter1.IsAlive() && _fighter2.IsAlive())
                {
                    RoundStart();

                    if (!_fighter2.Defend(_fighter1))
                        _fighter1.Attack(_fighter2);

                    if (!_fighter1.Defend(_fighter2))
                        _fighter2.Attack(_fighter1);

                    Custom.PressAnythingToContinue(ConsoleColor.DarkYellow, false, 0, 26, "press anything to continue", false);
                }

                DisplayWinner();
            }

            private void RoundStart()
            {
                _round++;
                Console.WriteLine($"\nRound {_round}");
            }

            private void DisplayWinner()
            {
                if (_fighter1.IsAlive())
                    Custom.WriteLineInColor($"\n{_fighter1.GetType().Name} is Victorious! 🚩😎👌🔥\n", ConsoleColor.Green);
                else if (_fighter2.IsAlive())
                    Custom.WriteLineInColor($"\n{_fighter2.GetType().Name} is Victorious 🚩😎👌🔥!\n", ConsoleColor.Green);
                else
                    Custom.WriteLineInColor($"\nIt's just two corpses. Bummer\n", ConsoleColor.Red);
            }
        }

        abstract class Fighter
        {
            public int Health { get; protected set; }
            public int Damage { get; protected set; }

            public Fighter()
            {
                Health = 30;
                Damage = 2;
            }

            public Fighter(int health, int damage)
            {
                Health = health;
                Damage = damage;
            }

            public virtual void DisplayStats()
            {
                Custom.WriteLineInColor($"{GetType().Name}", ConsoleColor.Blue);
                Console.WriteLine($"HP: {Health}\n" +
                    $"Damage: {Damage}");
            }

            public virtual void Attack(Fighter target)
            {
                target.TakeDamage(Damage);
                Console.Write($"[❤️{Health}] {GetType().Name} ⚔︎{Damage} attacks ");
                Custom.WriteLineInColor($"{target.GetType().Name} [❤️{target.Health}⬇]", ConsoleColor.Red);
            }

            public virtual void TakeDamage (int damage)
            {
                Health -= damage;
            }

            public virtual bool Defend(Fighter attacker)
            {
                return false;
            }

            public bool IsAlive()
            {
                return Health > 0;
            }
        }


        class Knight : Fighter
        {
            private int _block;
            private int _attackedCount;

            public Knight() : base(40, 1)
            {
                _block = 2; //every [*] attack is blocked
                _attackedCount = 0;
            }

            public override void DisplayStats()
            {
                base.DisplayStats();
                Console.WriteLine($"Block: every {_block} hit");
            }

            public override bool Defend(Fighter attacker)
            {
                _attackedCount++;

                if (_attackedCount != _block)
                {
                    return false;
                }
                else
                {
                    _attackedCount = 0;
                    Console.Write($"[❤️{attacker.Health}] {attacker.GetType().Name} ⚔︎ attacks but ");
                    Custom.WriteLineInColor($"[❤️{Health}] {GetType().Name} ⛊ blocks all the damage! Wow...", ConsoleColor.Blue);
                    return true;
                }
            }
        }

        class Thief : Fighter
        {
            private int _crit;
            private Random _random = new Random();
            private int _diceRoll;

            public Thief() : base(25, 2)
            {
                _crit = 25;
            }

            public override void DisplayStats()
            {
                base.DisplayStats();
                Console.WriteLine($"Critical Chance: {_crit}%");
            }

            public override void Attack(Fighter target)
            {
                _diceRoll = _random.Next(0, 101);

                if (_diceRoll <= _crit)
                {
                    target.TakeDamage(Damage * 2);
                    Console.Write($"[❤️{Health}] {GetType().Name} ");
                    Custom.WriteLineInColor($"🏹{Damage * 2} critically strikes {target.GetType().Name} [❤️{target.Health}⬇]", ConsoleColor.DarkRed);
                }
                else
                {
                    target.TakeDamage(Damage);
                    Console.Write($"[❤️{Health}] {GetType().Name} ⚔︎{Damage} attacks ");
                    Custom.WriteLineInColor($"{target.GetType().Name} [❤️{target.Health}⬇]", ConsoleColor.Red);
                }
            }
        }


        class Dualist : Fighter
        {
            private int _parry;

            public Dualist() : base()
            {
                _parry = 4; //every fourth attack is parried
            }
            public override void DisplayStats()
            {
                base.DisplayStats();
                Console.WriteLine($"Parry: Every {_parry} strike");
            }
        }

        class Summoner : Fighter
        {
            private int _petDamage;
            private int _petHealth;

            public Summoner() : base(20, 1)
            {
                _petDamage = 2;
                _petHealth = 15;
            }

            public override void DisplayStats()
            {
                base.DisplayStats();
                Console.WriteLine($"Pet Health: {_petHealth}");
                Console.WriteLine($"Pet Damage: {_petDamage}");
            }
        }

        class BloodHunter : Fighter
        {
            private int _vamp;

            public BloodHunter() : base(20, 1)
            {
                _vamp = 100;
            }

            public override void DisplayStats()
            {
                base.DisplayStats();
                Console.WriteLine($"Vamp: {_vamp}%");
            }
        }
    }

    class Custom
    {
        public static void WriteLineInColor(string text, ConsoleColor color = ConsoleColor.DarkRed, bool customPos = false, int xPos = 0, int YPos = 0)
        {
            if (customPos)
                Console.SetCursorPosition(xPos, YPos);

            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public static void WriteInColor(string text, ConsoleColor color = ConsoleColor.DarkRed, bool customPos = false, int xPos = 0, int YPos = 0)
        {
            if (customPos)
                Console.SetCursorPosition(xPos, YPos);

            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }

        public static void PressAnythingToContinue(ConsoleColor color = ConsoleColor.DarkYellow, bool customPos = false, int xPos = 0, int YPos = 0, string text = "press anything to continue", bool consoleClear = true)
        {
            if (customPos)
                Console.SetCursorPosition(xPos, YPos);

            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
            Console.ReadKey(true);

            if (consoleClear)
                Console.Clear();
        }

        public static void WriteFilled(string text, ConsoleColor color = ConsoleColor.DarkYellow, bool customPos = false, int xPos = 0, int yPos = 0)
        {
            int borderLength = text.Length + 2;
            string filler = new string('═', borderLength);
            string topBorder = "╔" + filler + "╗";
            string line = $"║ {text} ║";
            string bottomBorder = "╚" + filler + "╝";

            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = color;

            WriteAtPosition(xPos, yPos, topBorder);
            WriteAtPosition(xPos, yPos + 1, line);
            WriteAtPosition(xPos, yPos + 2, bottomBorder);

            Console.ResetColor();
        }

        public static void WriteAtPosition(int xPos, int yPos, string text)
        {
            Console.SetCursorPosition(xPos, yPos);
            Console.WriteLine(text);
        }

        public static int GetUserNumberInRange(string startMessage = "Select Number: ", int maxInput = 100)
        {
            int userInput = 0;
            bool isValidInput = false;

            Console.WriteLine(startMessage);

            while (!isValidInput)
            {
                if (int.TryParse(Console.ReadLine(), out userInput))
                {
                    if (userInput > 0 && userInput <= maxInput)
                        isValidInput = true;
                    else
                        Custom.WriteLineInColor($"\nPlease enter a number between 1 and {maxInput}:", ConsoleColor.Red);

                }
                else
                {
                    Custom.WriteLineInColor("\nInvalid input. Please enter a number instead:", ConsoleColor.Red);
                }
            }

            return userInput;
        }
    }
}
