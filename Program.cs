﻿using System;
using System.Collections.Generic;

namespace L48_aquarium
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Room room = new Room();

            room.Run();
        }
    }

    class Room
    {
        private Aquarium _aquarium = new Aquarium();
        private int _delimeterLenght = 75;
        private char _delimeterSymbol = '=';
        private string _delimeter;

        public Room()
        {
            _delimeter = new string(_delimeterSymbol, _delimeterLenght);
        }

        private enum Menu
        {
            ShowStatusAllFishes = 1,
            ShowFishStatus = 2,
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
                ShowFishStatus();
                ShowMenu();

                Console.Write("Выберите пункт: ");

                if (int.TryParse(Console.ReadLine(), out int number))
                {
                    Console.Clear();

                    switch ((Menu)number)
                    {
                        case Menu.ShowStatusAllFishes:
                            _aquarium.ShowStatusAllFishes();
                            break;

                        case Menu.ShowFishStatus:
                            _aquarium.ShowFishStatus();
                            break;

                        case Menu.AddNewFish:
                            _aquarium.AddNewFish();
                            break;

                        case Menu.RemoveFish:
                            _aquarium.RemoveFish();
                            break;

                        case Menu.NextCycle:
                            _aquarium.GrowUp();
                            break;

                        case Menu.Exit:
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

        private void ShowMenu()
        {
            Console.WriteLine(_delimeter + $"\n{(int)Menu.ShowStatusAllFishes} - Проверить состояние всех рыб.\n" +
                              $"{(int)Menu.ShowFishStatus} - Проверить состояние определенной рыбы.\n{(int)Menu.AddNewFish} - Добавить новую " +
                              $"рыбу в аквариум.\n{(int)Menu.RemoveFish} - Удалить рыбу из аквариума.\n{(int)Menu.NextCycle} - Следующий цикл." +
                              $"\n{(int)Menu.Exit} - Выход из игры.\n");
        }

        private void ShowFishStatus()
        {
            Console.Clear();
            Console.WriteLine(_delimeter);

            for (int i = 0; i < _aquarium.Fishes.Count; i++)
            {
                Console.Write("Рыба " + (i + 1) + " - Вид " + _aquarium.Fishes[i].Type + ".\t");
                _aquarium.Fishes[i].ShowVitalStatus();
            }
        }
    }

    class Aquarium
    {
        private int _capacity;

        private List<GlowFish> _fishes = new List<GlowFish>();
        private FishCreater _fishCreater = new FishCreater();

        public Aquarium()
        {
            _capacity = 10;
            int startFishesCount = RandomGenerator.Generate(_capacity);

            for (int i = 0; i < startFishesCount; i++)
            {
                int randomIndex = RandomGenerator.Generate(_fishCreater.FishesList.Count);
                _fishes.Add(_fishCreater.Create(_fishCreater.FishesList[randomIndex]));
            }
        }

        public IReadOnlyList<GlowFish> Fishes => _fishes;

        public void RemoveFish()
        {
            Console.WriteLine("Введите номер удаляемой рыбы: ");

            if (int.TryParse(Console.ReadLine(), out int index))
            {
                index--;

                if (IsCorrectIndex(index))
                    _fishes.Remove(_fishes[index]);
                else
                    Error.Show();
            }
        }

        public void ShowFishStatus()
        {
            Console.WriteLine("Введите номер рыбы: ");

            if (int.TryParse(Console.ReadLine(), out int index))
            {
                index--;

                if (IsCorrectIndex(index))
                    ShowStatus(_fishes[index]);
                else
                    Error.Show();
            }
            else
            {
                Error.Show();
            }
        }

        public void ShowStatusAllFishes()
        {
            for (int i = 0; i < _fishes.Count; i++)
                ShowStatus(_fishes[i]);
        }

        public void GrowUp()
        {
            foreach (var fish in _fishes)
                fish.GrowUp();
        }

        public void AddNewFish()
        {
            Console.Clear();

            if (_fishes.Count < _capacity)
            {
                bool isCreation = false;
                Console.Write($"Какую из рыб вы хотите добавить в аквариум:");

                foreach (var fishType in _fishCreater.FishesList)
                    Console.Write($"\t{fishType}");

                Console.Write("\nНапишите тип рыбы: ");
                string userInput = Console.ReadLine();

                foreach (var fishType in _fishCreater.FishesList)
                {
                    if (userInput.ToLower() == fishType.ToLower())
                    {
                        _fishes.Add(_fishCreater.Create(fishType));
                        isCreation = true;
                        break;
                    }
                }

                if (isCreation)
                    Console.WriteLine("Рыба успешно добавлена.");
                else
                    Error.Show();
            }
            else
            {
                Console.WriteLine("В аквариуме нет места для еще одной рыбы.");
            }
        }

        private bool IsCorrectIndex(int index) => (index < _fishes.Count && index >= 0);

        private void ShowStatus(GlowFish fish)
        {
            Console.WriteLine($"Вид: {fish.Type}. Состояние: {fish.Status}. Возраст: {fish.Age}.");
        }
    }

    class GlowFish
    {
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

        public void ShowVitalStatus()
        {
            const string SwimBehaviorHealthy = "активно";
            const string SwimBehaviorMalaise = "вяло";
            const string SwimBehaviorDisease = "странно";
            const string GlowBehaviorHealthy = "яркое";
            const string GlowBehaviorMalaise = "мерцающее";
            const string GlowBehaviorDisease = "тусклое";

            switch (Status)
            {
                case HealthStatus.Healthy:
                    Console.WriteLine($"Она плавает {SwimBehaviorHealthy} и ее свечение {GlowBehaviorHealthy}.");
                    break;

                case HealthStatus.Malaise:
                    Console.WriteLine($"Она плавает {SwimBehaviorMalaise} и ее свечение {GlowBehaviorMalaise}.");
                    break;

                case HealthStatus.Disease:
                    Console.WriteLine($"Она плавает {SwimBehaviorDisease} и ее свечение {GlowBehaviorDisease}.");
                    break;

                default:
                    Console.WriteLine("Рыбка умерла от старости или болезни.");
                    break;
            }
        }

        public void GrowUp()
        {
            if (Status != HealthStatus.Dead)
            {
                Age++;
                _chanceDeathFromAge += Age;
                Status = (HealthStatus)RandomGenerator.Generate((int)HealthStatus.Healthy, (int)HealthStatus.Disease + 1);
                _chanceDeathFromStatus = (int)Status;
                int chanceDead = _chanceDeathFromStatus * _chanceDeathFromAge;

                if (RandomGenerator.Generate(_maxChance) <= chanceDead)
                    Status = HealthStatus.Dead;
            }
        }
    }

    class FishCreater
    {
        private int _maxFishAge = 6;
        private List<string> _fishesList = new List<string>() { "Тетра", "Данио", "Барб" };

        public IReadOnlyList<string> FishesList => _fishesList;

        public GlowFish Create(string fishType) => new GlowFish(fishType, HealthStatus.Healthy, RandomGenerator.Generate(_maxFishAge));
    }

    static class RandomGenerator
    {
        private static Random s_random = new Random();

        public static int Generate(int minValue, int maxValue) => s_random.Next(minValue, maxValue);

        public static int Generate(int maxValue) => s_random.Next(maxValue);
    }

    static class Error
    {
        public static void Show()
        {
            Console.WriteLine("Введено некорректное значение.");
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
