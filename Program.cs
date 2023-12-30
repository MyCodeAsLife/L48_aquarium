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
        private int _capacity;
        private int _delimeterLenght = 75;

        private List<GlowFish> _fishes = new List<GlowFish>();
        private FishCreater _fishCreater = new FishCreater();
        private char _delimeter = '=';

        public Aquarium(int capacity)
        {
            _capacity = capacity;
            Fill();
        }

        private enum Menu
        {
            ShowStatusAllFishes = 1,
            ShowStatusFish = 2,
            AddNewFish = 3,
            RemoveFish = 4,
            NextCycle = 5,
            Exit = 6,
        }

        public void Run()
        {
            bool isOpen = true;

            while (isOpen)
            {
                ShowLive();
                ShowMenu();

                Console.Write("Выберите пункт: ");

                if (int.TryParse(Console.ReadLine(), out int number))
                {
                    Console.Clear();

                    switch ((Menu)number)
                    {
                        case Menu.ShowStatusAllFishes:
                            ShowStatusAllFishes();
                            break;

                        case Menu.ShowStatusFish:
                            ShowStatusFish();
                            break;

                        case Menu.AddNewFish:
                            AddNewFish();
                            break;

                        case Menu.RemoveFish:
                            RemoveFish();
                            break;

                        case Menu.NextCycle:
                            GrowUp();
                            break;

                        case Menu.Exit:
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

        private void ShowMenu()
        {
            Console.WriteLine(new string(_delimeter, _delimeterLenght) + $"\n{(int)Menu.ShowStatusAllFishes} - Проверить состояние всех рыб.\n" +
                              $"{(int)Menu.ShowStatusFish} - Проверить состояние определенной рыбы.\n{(int)Menu.AddNewFish} - Добавить новую " +
                              $"рыбу в аквариум.\n{(int)Menu.RemoveFish} - Удалить рыбу из аквариума.\n{(int)Menu.NextCycle} - Следующий цикл." +
                              $"\n{(int)Menu.Exit} - Выход из игры.\n");
        }

        private bool IsCorrectIndex(int index) => (index < _fishes.Count && index >= 0);

        private void GrowUp()
        {
            foreach (var fish in _fishes)
                fish.GrowUp();
        }

        private void ShowLive()
        {
            Console.Clear();
            Console.WriteLine(new string(_delimeter, _delimeterLenght));

            for (int i = 0; i < _fishes.Count; i++)
            {
                Console.Write("Рыба " + (i + 1) + " - Вид " + _fishes[i].Type + ".\t");
                _fishes[i].ShowLive();
            }
        }

        private void CheckStatusFish(GlowFish fish)
        {
            Console.WriteLine($"Вид: {fish.Type}. Состояние: {fish.Status}. Возраст: {fish.Age}.");
        }

        private void ShowStatusFish()
        {
            Console.WriteLine("Введите номер рыбы: ");

            if (int.TryParse(Console.ReadLine(), out int index))
            {
                index--;

                if (IsCorrectIndex(index))
                    CheckStatusFish(_fishes[index]);
                else
                    ShowError();
            }
            else
            {
                ShowError();
            }
        }

        private void ShowStatusAllFishes()
        {
            for (int i = 0; i < _fishes.Count; i++)
                CheckStatusFish(_fishes[i]);
        }

        private void AddNewFish()
        {
            Console.Clear();

            if (_fishes.Count < _capacity)
            {
                bool checkCreation = false;
                Console.Write($"Какую из рыб вы хотите добавить в аквариум:");

                foreach (var fishType in _fishCreater.FishList)
                    Console.Write($"\t{fishType}");

                Console.Write("\nНапишите тип рыбы: ");
                string userInput = Console.ReadLine();

                foreach (var fishType in _fishCreater.FishList)
                {
                    if (userInput.ToLower() == fishType.ToLower())
                    {
                        _fishes.Add(_fishCreater.Create(fishType));
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
            {
                index--;

                if (IsCorrectIndex(index))
                    _fishes.Remove(_fishes[index]);
                else
                    ShowError();
            }
        }

        private void Fill()
        {
            int startFishesCount = RandomGenerator.GetRandomNumber(_capacity);

            for (int i = 0; i < startFishesCount; i++)
            {
                int randomFish = RandomGenerator.GetRandomNumber(_fishCreater.FishList.Count);
                _fishes.Add(_fishCreater.Create(_fishCreater.FishList[randomFish]));
            }
        }

        private void ShowError()
        {
            Console.WriteLine("Введено некорректное значение.");
        }
    }

    class GlowFish
    {
        private const string SwimBehaviorHealthy = "активно";
        private const string SwimBehaviorMalaise = "вяло";
        private const string SwimBehaviorDisease = "странно";
        private const string GlowBehaviorHealthy = "яркое";
        private const string GlowBehaviorMalaise = "мерцающее";
        private const string GlowBehaviorDisease = "тусклое";

        private int _maxChance;
        private int _chanceDeathFromAge;
        private int _chanceDeathFromStatus;

        public GlowFish(string type, HealthStatus status, int age)
        {
            Type = type;
            Age = age;
            Status = status;
            _chanceDeathFromAge = age;
            _chanceDeathFromStatus = (int)Status;
            _maxChance = 100;
        }

        public string Type { get; private set; }
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
                    Glow(GlowBehaviorHealthy);
                }
                else if (Status == HealthStatus.Malaise)
                {
                    Swim(SwimBehaviorMalaise);
                    Glow(GlowBehaviorMalaise);
                }
                else
                {
                    Swim(SwimBehaviorDisease);
                    Glow(GlowBehaviorDisease);
                }
            }
        }

        public void GrowUp()
        {
            Age++;
            _chanceDeathFromAge += Age;
            Status = (HealthStatus)RandomGenerator.GetRandomNumber((int)HealthStatus.Healthy, (int)HealthStatus.Disease + 1);
            _chanceDeathFromStatus = (int)Status;
            int chanceDead = _chanceDeathFromStatus * _chanceDeathFromAge;

            if (RandomGenerator.GetRandomNumber(_maxChance) <= chanceDead)
                Status = HealthStatus.Dead;
        }

        protected void Swim(string swimBehavior)
        {
            Console.Write($"Она плавает {swimBehavior} и ");
        }

        protected void Glow(string glowBehavior)
        {
            Console.WriteLine($"ее свечение {glowBehavior}.");
        }
    }

    class FishCreater
    {
        private int _maxFishAge = 6;
        private List<string> _fishList = new List<string>() { "Тетра", "Данио", "Барб" };

        public IReadOnlyList<string> FishList => _fishList;

        public GlowFish Create(string fishType) => new GlowFish(fishType, HealthStatus.Healthy, RandomGenerator.GetRandomNumber(_maxFishAge));
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
