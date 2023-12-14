using System;
using System.Collections.Generic;

namespace L48_aquarium
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const int CommandShowStatusAllFishes = 1;
            const int CommandShowStatusFish = 2;
            const int CommandAddNewFish = 3;
            const int CommandRemoveFish = 4;
            const int CommandNextCycle = 5;
            const int CommandExit = 6;

            Random random = new Random();
            Aquarium aquarium = new Aquarium(10, random);

            int delimeterLenght = 75;

            char delimeter = '=';

            bool isOpen = true;

            while (isOpen)
            {
                Console.Clear();
                Console.WriteLine(new string(delimeter, delimeterLenght));
                aquarium.ShowLive();
                Console.WriteLine(new string(delimeter, delimeterLenght) + $"\n{CommandShowStatusAllFishes} - Проверить состояние всех рыб.\n" +
                                  $"{CommandShowStatusFish} - Проверить состояние определенной рыбы.\n{CommandAddNewFish} - Добавить новую " +
                                  $"рыбу в аквариум.\n{CommandRemoveFish} - Удалить рыбу из аквариума.\n{CommandNextCycle} - Следующий цикл." +
                                  $"\n{CommandExit} - Выход из игры.");

                Console.Write("Выберите пункт: ");

                if (int.TryParse(Console.ReadLine(), out int numberMenu))
                {
                    Console.Clear();

                    switch (numberMenu)
                    {
                        case CommandShowStatusAllFishes:
                            aquarium.ShowStatusAllFishes();
                            break;

                        case CommandShowStatusFish:
                            aquarium.ShowStatusFish();
                            break;

                        case CommandAddNewFish:
                            aquarium.AddNewFish();
                            break;

                        case CommandRemoveFish:
                            aquarium.RemoveFish();
                            break;

                        case CommandNextCycle:
                            aquarium.NextCycle();
                            break;

                        case CommandExit:
                            isOpen = false;
                            continue;

                        default:
                            Error.Show();
                            break;
                    }
                }
                else
                {
                    Error.Show();
                }

                Console.WriteLine("Для продолжения нажмите любую кнопку...");
                Console.ReadKey(true);
            }
        }
    }

    class Error
    {
        public static void Show()
        {
            Console.WriteLine("Введено некорректное значение.");
        }
    }

    class Aquarium
    {
        const string TypeTetra = "Тетра";
        const string TypeDanio = "Данио";
        const string TypeBarb = "Барб";

        private Random _random;
        private List<GlowFish> _fishes = new List<GlowFish>();
        private int _capacity;
        private int _maxFishAge = 6;

        public Aquarium(int capacity, Random random)
        {
            _random = random;
            _capacity = capacity;

            FillAquarium();
        }

        public void NextCycle()
        {
            for (int i = 0; i < _fishes.Count; i++)
            {
                _fishes[i].NextCycle();
            }
        }

        public void ShowLive()
        {
            for (int i = 0; i < _fishes.Count; i++)
            {
                Console.Write("Рыба " + (i + 1) + " - Вид " + _fishes[i].GetFishType() + ".\t");
                _fishes[i].ShowLive();
            }
        }

        public void CheckStatusFish(int index)
        {
            if (IsCorrectIndex(index))
                Console.WriteLine($"{index + 1} - Вид: {_fishes[index].GetFishType()}. Состояние: {_fishes[index].Status}. Возраст: {_fishes[index].Age}.");
            else
                Error.Show();
        }

        public void ShowStatusFish()
        {
            Console.WriteLine("Введите номер рыбы: ");

            if (int.TryParse(Console.ReadLine(), out int index))
                CheckStatusFish(index - 1);
        }

        public void ShowStatusAllFishes()
        {
            for (int i = 0; i < _fishes.Count; i++)
                CheckStatusFish(i);
        }

        public void AddNewFish()
        {
            Console.Clear();
            Console.WriteLine($"Какую из рыб вы хотите добавить в аквариум:\t{TypeTetra}\t{TypeDanio}\t{TypeBarb}");
            Console.Write("Напишите тип рыбы: ");
            string userInput = Console.ReadLine();

            if (userInput.ToLower() == TypeTetra.ToLower())
                AddFish(new Tetra(_random, HealthStatus.Healthy, _random.Next(_maxFishAge)));
            else if (userInput.ToLower() == TypeDanio.ToLower())
                AddFish(new Danio(_random, HealthStatus.Healthy, _random.Next(_maxFishAge)));
            else if (userInput.ToLower() == TypeBarb.ToLower())
                AddFish(new Barb(_random, HealthStatus.Healthy, _random.Next(_maxFishAge)));
            else
                Error.Show();
        }

        public void RemoveFish()
        {
            Console.WriteLine("Введите номер удаляемой рыбы: ");

            if (int.TryParse(Console.ReadLine(), out int index))
                if (IsCorrectIndex(index))
                    _fishes.RemoveAt(index);
                else
                    Error.Show();
        }

        private void AddFish(GlowFish fish)
        {
            if (_fishes.Count < _capacity)
                _fishes.Add(fish);
            else
                Console.WriteLine("В аквариуме нет места для еще одной рыбы.");
        }

        private bool IsCorrectIndex(int index)
        {
            if (index < _fishes.Count || index >= 0)
                return true;
            else
                return false;
        }

        private void FillAquarium()
        {
            int startCountFish = _random.Next(_capacity);

            if (startCountFish > 3)
            {
                for (int i = 0; i < startCountFish / 3; i++)
                    AddFish(new Tetra(_random, HealthStatus.Healthy, _random.Next(_maxFishAge)));

                for (int i = 0; i < startCountFish / 3; i++)
                    AddFish(new Danio(_random, HealthStatus.Healthy, _random.Next(_maxFishAge)));

                for (int i = 0; i < startCountFish / 3; i++)
                    AddFish(new Barb(_random, HealthStatus.Healthy, _random.Next(_maxFishAge)));
            }
            else
            {
                AddFish(new Tetra(_random, HealthStatus.Healthy, _random.Next(_maxFishAge)));
                AddFish(new Danio(_random, HealthStatus.Healthy, _random.Next(_maxFishAge)));
                AddFish(new Barb(_random, HealthStatus.Healthy, _random.Next(_maxFishAge)));
            }
        }
    }

    abstract class GlowFish
    {
        const string SwimBehaviorHealthy = "активно";
        const string SwimBehaviorMalaise = "вяло";
        const string SwimBehaviorDisease = "странно";
        const string GLowBehaviorHealthy = "яркое";
        const string GLowBehaviorMalaise = "мерцающее";
        const string GlowBehaviorDisease = "тусклое";

        private Random _random;

        private int _chanceDeathFromAge;
        private int _chanceDeathFromStatus;

        public GlowFish(Random random, HealthStatus status, int age)
        {
            _random = random;
            Age = age;
            Status = status;
            _chanceDeathFromAge = age;
            _chanceDeathFromStatus = (int)Status;
        }

        public HealthStatus Status { get; private set; }

        public int Age { get; private set; }

        public virtual void ShowLive()
        {
            if (Status == HealthStatus.Dead)
            {
                Console.WriteLine("Рыбка умерла от старости или болезни.");
            }
            else
            {
                Console.Write($"Ей испольнилось: {Age}.\t");

                if (Status == HealthStatus.Healthy)
                {
                    Swim(SwimBehaviorHealthy);
                    Glow(GLowBehaviorHealthy);
                }
                else if (Status == HealthStatus.Malaise)
                {
                    Swim(SwimBehaviorMalaise);
                    Glow(GLowBehaviorMalaise);
                }
                else
                {
                    Swim(SwimBehaviorDisease);
                    Glow(GlowBehaviorDisease);
                }
            }
        }

        public void NextCycle()
        {
            Age++;
            _chanceDeathFromAge += Age;
            Status = (HealthStatus)_random.Next((int)HealthStatus.Healthy, (int)HealthStatus.Disease + 1);
            _chanceDeathFromStatus = (int)Status;
            int chanceDead = _chanceDeathFromStatus * _chanceDeathFromAge;

            if (_random.Next(100) <= chanceDead)
                Status = HealthStatus.Dead;
        }

        public abstract string GetFishType();

        protected void Swim(string swimBehavior)
        {
            Console.Write($"Она плавает {swimBehavior} и ");
        }

        protected void Glow(string glowBehavior)
        {
            Console.WriteLine($"ее свечение {glowBehavior}.");
        }
    }

    class Tetra : GlowFish
    {
        public Tetra(Random random, HealthStatus status, int age) : base(random, status, age) { }

        public override string GetFishType()
        {
            return "Тетра";
        }
    }

    class Danio : GlowFish
    {
        public Danio(Random random, HealthStatus status, int age) : base(random, status, age) { }

        public override string GetFishType()
        {
            return "Данио";
        }
    }

    class Barb : GlowFish
    {
        public Barb(Random random, HealthStatus status, int age) : base(random, status, age) { }

        public override string GetFishType()
        {
            return "Барб";
        }
    }

    enum HealthStatus
    {
        Dead,
        Healthy,
        Malaise,
        Disease,
    }
}
