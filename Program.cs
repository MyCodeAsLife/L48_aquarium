using System;
using System.Collections.Generic;

namespace L48_aquarium
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int aquariumCapacity = 10;
            Aquarium aquarium = new Aquarium(aquariumCapacity);

            aquarium.Run();
        }
    }

    class Aquarium
    {
        private const int CommandShowStatusAllFishes = 1;
        private const int CommandShowStatusFish = 2;
        private const int CommandAddNewFish = 3;
        private const int CommandRemoveFish = 4;
        private const int CommandNextCycle = 5;
        private const int CommandExit = 6;

        private int _capacity;
        private int delimeterLenght = 75;

        private List<FishCreater> _fishList = new List<FishCreater>();
        private List<GlowFish> _fishes = new List<GlowFish>();
        private char delimeter = '=';

        public Aquarium(int capacity)
        {
            _capacity = capacity;
            _fishList = new List<FishCreater> { new FishCreaterTetra(),
                                                new FishCreaterDanio(),
                                                new FishCreaterBarb()};

            Fill();
        }

        public void Run()
        {
            bool isOpen = true;

            while (isOpen)
            {
                Console.Clear();
                Console.WriteLine(new string(delimeter, delimeterLenght));
                ShowLive();
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
                            ShowStatusAllFishes();
                            break;

                        case CommandShowStatusFish:
                            ShowStatusFish();
                            break;

                        case CommandAddNewFish:
                            AddNewFish();
                            break;

                        case CommandRemoveFish:
                            RemoveFish();
                            break;

                        case CommandNextCycle:
                            NextCycle();
                            break;

                        case CommandExit:
                            isOpen = false;
                            continue;

                        default:
                            ShowError();
                            break;
                    }
                }
                else
                {
                    ShowError();
                }

                Console.WriteLine("Для продолжения нажмите любую кнопку...");
                Console.ReadKey(true);
            }
        }

        private bool IsCorrectIndex(int index) => (index < _fishes.Count || index >= 0);

        private void NextCycle()
        {
            foreach (var fish in _fishes)
                fish.LiveThroughCycle();
        }

        private void ShowLive()
        {
            for (int i = 0; i < _fishes.Count; i++)
            {
                Console.Write("Рыба " + (i + 1) + " - Вид " + _fishes[i].GetFishType() + ".\t");
                _fishes[i].ShowLive();
            }
        }

        private void CheckStatusFish(int index)
        {
            if (IsCorrectIndex(index))
                Console.WriteLine($"{index + 1} - Вид: {_fishes[index].GetFishType()}. Состояние:" +
                                  $" {_fishes[index].Status}. Возраст: {_fishes[index].Age}.");
            else
                ShowError();
        }

        private void ShowStatusFish()
        {
            Console.WriteLine("Введите номер рыбы: ");

            if (int.TryParse(Console.ReadLine(), out int index))
                CheckStatusFish(index - 1);
            else
                ShowError();
        }

        private void ShowStatusAllFishes()
        {
            for (int i = 0; i < _fishes.Count; i++)
                CheckStatusFish(i);
        }

        private void AddNewFish()
        {
            Console.Clear();

            if (_fishes.Count < _capacity)
            {
                bool checkCreation = false;

                Console.Write($"Какую из рыб вы хотите добавить в аквариум:");

                foreach (var fish in _fishList)
                    Console.Write($"\t{fish.GetFishType()}");

                Console.Write("\nНапишите тип рыбы: ");
                string userInput = Console.ReadLine();

                foreach (var fish in _fishList)
                {
                    if (userInput.ToLower() == fish.GetFishType().ToLower())
                    {
                        _fishes.Add(fish.Create());
                        checkCreation = true;
                        break;
                    }
                }

                if (checkCreation)
                    Console.WriteLine("Рыба успешно добавлена.");
                else
                    ShowError();
            }
            else
            {
                Console.WriteLine("В аквариуме нет места для еще одной рыбы.");
            }
        }

        private void RemoveFish()
        {
            Console.WriteLine("Введите номер удаляемой рыбы: ");

            if (int.TryParse(Console.ReadLine(), out int index))
                if (IsCorrectIndex(index))
                    _fishes.RemoveAt(index);
                else
                    ShowError();
        }

        private void Fill()
        {
            int startFishesCount = RandomGenerator.GetRandomNumber(_capacity);

            for (int i = 0; i < startFishesCount; i++)
            {
                int randomFish = RandomGenerator.GetRandomNumber(_fishList.Count);
                _fishes.Add(_fishList[randomFish].Create());
            }
        }

        private void ShowError()
        {
            Console.WriteLine("Введено некорректное значение.");
        }
    }

    abstract class GlowFish
    {
        private const string SwimBehaviorHealthy = "активно";
        private const string SwimBehaviorMalaise = "вяло";
        private const string SwimBehaviorDisease = "странно";
        private const string GLowBehaviorHealthy = "яркое";
        private const string GLowBehaviorMalaise = "мерцающее";
        private const string GlowBehaviorDisease = "тусклое";

        private int _maxChance;
        private int _chanceDeathFromAge;
        private int _chanceDeathFromStatus;

        public GlowFish(HealthStatus status, int age)
        {
            Age = age;
            Status = status;
            _chanceDeathFromAge = age;
            _chanceDeathFromStatus = (int)Status;
            _maxChance = 100;
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

        public void LiveThroughCycle()
        {
            Age++;
            _chanceDeathFromAge += Age;
            Status = (HealthStatus)RandomGenerator.GetRandomNumber((int)HealthStatus.Healthy, (int)HealthStatus.Disease + 1);
            _chanceDeathFromStatus = (int)Status;
            int chanceDead = _chanceDeathFromStatus * _chanceDeathFromAge;

            if (RandomGenerator.GetRandomNumber(_maxChance) <= chanceDead)
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
        public Tetra(HealthStatus status, int age) : base(status, age) { }

        public override string GetFishType() => "Тетра";
    }

    class Danio : GlowFish
    {
        public Danio(HealthStatus status, int age) : base(status, age) { }

        public override string GetFishType() => "Данио";
    }

    class Barb : GlowFish
    {
        public Barb(HealthStatus status, int age) : base(status, age) { }

        public override string GetFishType() => "Барб";
    }

    abstract class FishCreater
    {
        protected int MaxFishAge = 6;

        public abstract GlowFish Create();

        public abstract string GetFishType();
    }

    class FishCreaterTetra : FishCreater
    {
        public override GlowFish Create() => new Tetra(HealthStatus.Healthy, RandomGenerator.GetRandomNumber(MaxFishAge));

        public override string GetFishType() => "Тетра";
    }

    class FishCreaterDanio : FishCreater
    {
        public override GlowFish Create() => new Danio(HealthStatus.Healthy, RandomGenerator.GetRandomNumber(MaxFishAge));

        public override string GetFishType() => "Данио";
    }

    class FishCreaterBarb : FishCreater
    {
        public override GlowFish Create() => new Barb(HealthStatus.Healthy, RandomGenerator.GetRandomNumber(MaxFishAge));

        public override string GetFishType() => "Барб";
    }

    static class RandomGenerator
    {
        private static Random s_random = new Random();

        public static int GetRandomNumber(int minValue, int maxValue) => s_random.Next(minValue, maxValue);

        public static int GetRandomNumber(int maxValue) => s_random.Next(maxValue);
    }

    enum HealthStatus
    {
        Dead,
        Healthy,
        Malaise,
        Disease,
    }
}
