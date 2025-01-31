using System;
using System.Collections.Generic;

class ConsoleRPG
{
    private int roomCount; 
    private int currentRoom; 
    private int attemptsLeft; 
    private int experience;
    private char[] correctDirections;
    private int bonusRoomVisitCount; 
    private Random random;

    private Dictionary<char, char> layoutMap = new Dictionary<char, char>
    {
        { 'Ц', 'W' }, { 'ц', 'W' }, 
        { 'Ф', 'A' }, { 'ф', 'A' }, 
        { 'В', 'D' }, { 'в', 'D' } 
    };

    public ConsoleRPG(int roomCount)
    {
        this.roomCount = roomCount;
        this.currentRoom = 0;
        this.attemptsLeft = 5;
        this.experience = 0;
        this.bonusRoomVisitCount = 0;
        this.random = new Random();
        this.correctDirections = new char[roomCount];

        GenerateCorrectDirections();
    }

    private void GenerateCorrectDirections()
    {
        char[] directions = { 'W', 'A', 'D' }; 
        for (int i = 0; i < roomCount; i++)
        {
            correctDirections[i] = directions[random.Next(directions.Length)]; 
        }
    }

    public void StartGame()
    {
        Console.WriteLine($"Игра началась! У вас есть {attemptsLeft} попыток, чтобы пройти лабиринт.");
        Play();
    }

    private void Play()
    {
        while (currentRoom < roomCount && attemptsLeft > 0)
        {
            Console.WriteLine($"\nВы в комнате {currentRoom + 1}. Опыт: {experience}, попыток осталось: {attemptsLeft}");

            if ((currentRoom + 1) % 6 == 0) 
            {
                HandleBossRoom();
                currentRoom++; 
            }
            else if ((currentRoom + 1) % 3 == 0) 
            {
                HandleMathRoom();
                currentRoom++; 
            }
            else if (random.Next(10) == 0) 
            {
                HandleBonusRoom();
                currentRoom++; 
            }
            else 
            {
                HandleRegularRoom();
            }

            if (attemptsLeft <= 0)
            {
                Console.WriteLine("Попытки закончились. Игра начинается заново.");
                ResetGame();
                return;
            }
        }

        if (currentRoom == roomCount)
        {
            Console.WriteLine("Поздравляем! Вы прошли лабиринт!");
        }
    }

    private void HandleRegularRoom()
    {
        while (true) 
        {
            Console.WriteLine("Куда вы хотите пойти? (W - вперед, A - влево, D - вправо)");
            char direction = char.ToUpper(Console.ReadKey().KeyChar); 
            Console.WriteLine();

            if (layoutMap.ContainsKey(direction))
            {
                direction = layoutMap[direction];
            }

            if (CheckDirection(direction))
            {
                Console.WriteLine("Правильный выбор! Переходим в следующую комнату.");
                currentRoom++; 
                break;
            }
            else
            {
                attemptsLeft--;
                Console.WriteLine($"Неверный выбор! Осталось попыток: {attemptsLeft}");

                if (attemptsLeft <= 0)
                {
                    Console.WriteLine("Попытки закончились. Игра начинается заново.");
                    ResetGame();
                    return;
                }
            }
        }
    }

    private void HandleMathRoom()
    {
        Console.WriteLine("Вы попали в арифметическую комнату! Решите пример, чтобы продолжить.");

        int num1 = random.Next(10, 100);
        int num2 = random.Next(10, 100);
        int num3 = random.Next(10, 100);
        char[] operators = { '+', '-', '*' };
        char op1 = operators[random.Next(operators.Length)];
        char op2 = operators[random.Next(operators.Length)];

        string expression = $"{num1} {op1} {num2} {op2} {num3}";
        int correctAnswer = CalculateExpression(expression);

        Console.WriteLine($"Решите пример: {expression}");
        int userAnswer = int.Parse(Console.ReadLine());

        if (userAnswer == correctAnswer)
        {
            Console.WriteLine("Правильно! Вы получаете 3 очка опыта и 1 дополнительную попытку.");
            experience += 3;
            attemptsLeft += 1;
        }
        else
        {
            Console.WriteLine($"Неверно! Правильный ответ: {correctAnswer}. Вы теряете 1 попытку.");
            attemptsLeft--;
        }
    }

    private void HandleBossRoom()
    {
        Console.WriteLine("Вы попали в комнату с боссом!");
        int bossHealth = random.Next(3, 16);
        Console.WriteLine($"У босса {bossHealth} жизней. Ваш опыт: {experience}.");

        if (experience > bossHealth)
        {
            Console.WriteLine("Ваш опыт больше, чем жизней у босса! Вы побеждаете и получаете 3 очка опыта и 2 дополнительные попытки.");
            experience += 3;
            attemptsLeft += 2;
        }
        else
        {
            Console.WriteLine("Ваш опыт меньше, чем жизней у босса. Вы теряете 2 попытки.");
            attemptsLeft -= 2;
        }
    }

    private void HandleBonusRoom()
    {
        bonusRoomVisitCount++; 
        int bonusAttempts = bonusRoomVisitCount; 
        int bonusExperience = bonusRoomVisitCount; 

        Console.WriteLine($"Вы попали в бонусную комнату! Это ваше {bonusRoomVisitCount} посещение.");
        Console.WriteLine($"Вы получаете {bonusAttempts} попыток и {bonusExperience} очков опыта.");
        attemptsLeft += bonusAttempts;
        experience += bonusExperience;

        Console.WriteLine("Автоматически переходим в следующую комнату.");
    }

    private bool CheckDirection(char direction)
    {
        return direction == correctDirections[currentRoom];
    }

    private int CalculateExpression(string expression)
    {
        var dataTable = new System.Data.DataTable();
        var result = dataTable.Compute(expression, null);
        return Convert.ToInt32(result);
    }

    private void ResetGame()
    {
        currentRoom = 0;
        attemptsLeft = 5;
        experience = 0;
        bonusRoomVisitCount = 0;
        GenerateCorrectDirections();
        StartGame();
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Введите количество комнат в лабиринте:");
        int roomCount = int.Parse(Console.ReadLine());

        ConsoleRPG game = new ConsoleRPG(roomCount);
        game.StartGame();
    }
}