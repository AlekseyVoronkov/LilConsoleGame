using Practice_i_guess;

class TestGame
{
    public static Random random = new();
    public static int score = 0;
    private const int MapWidth = 24;
    private const int MapHeight = 15;

    struct Portal
    {
        public int positionX;
        public int positionY;

        public bool isEnterSpawned = false;
        public bool isExitSpawned = false;
        public Portal()
        {
        }

        public void SpawnPortal(ref MainCharacter mainCharacter, ref Portal enterPortal, ref Portal exitPortal)
        {
            if(isEnterSpawned == false)
            {
                enterPortal.positionX = mainCharacter.positionX;
                enterPortal.positionY = mainCharacter.positionY;
                isEnterSpawned = true;
            }
            else if(isEnterSpawned == true)
            {
                exitPortal.positionX = mainCharacter.positionX;
                exitPortal.positionY = mainCharacter.positionY;
                isExitSpawned = true;
            }
            else if(isEnterSpawned && isExitSpawned)
            {
                isEnterSpawned = false;
                isExitSpawned = false;
            }

        }

        public void TeleportObject(ref MainCharacter mainCharater, ref Rock rock, Portal exitPortal)
        {
            if (mainCharater.positionX == this.positionX && mainCharater.positionY == this.positionY)
            {
                mainCharater.positionX = exitPortal.positionX;
                mainCharater.positionY = exitPortal.positionY;
            }

            if (rock.positionX == this.positionX && rock.positionY == this.positionY)
            {
                rock.positionX = exitPortal.positionX;
                rock.positionY = exitPortal.positionY;
            }
        }
    }
    struct PlaceForRock
    {
        public int positionX;
        public int positionY;

        public PlaceForRock()
        {
            positionX = random.Next(1, 23);
            positionY = random.Next(3, 12);
        }
    }
    struct Rock
    {
        public int positionX;
        public int positionY;
        public Rock()
        {
            positionX = random.Next(2, 22);
            positionY = random.Next(4, 11);
        }

        public void Move(int deltaX, int deltaY)
        {
            positionX += deltaX;
            positionY += deltaY;
        }
    }
    struct MainCharacter
    {
        public int positionX = 1;
        public int positionY = 3;

        public MainCharacter()
        {
        }
    }

    static void TryToMoveRock(ref MainCharacter mainCharacter, ref Rock rock, ref Portal enterPortal, ref Portal exitPortal)
    {
        if (Console.KeyAvailable)
        {
            var ch = Console.ReadKey(true).KeyChar;
            MoveCharacter(ch, ref mainCharacter, ref rock);

            if (ch == 'p')
            {
                HandlePortal('p', ref mainCharacter, ref enterPortal, ref exitPortal);
            }

            if (ch == 'r')
            {
                score -= 1;
                rock = new Rock();
            }
        }

        enterPortal.TeleportObject(ref mainCharacter, ref rock, exitPortal);
        ValidateRockPosition(ref rock);
    }

    private static void MoveCharacter(char ch, ref MainCharacter mainCharacter, ref Rock rock)
    {
        int deltaX = 0, deltaY = 0;

        switch (ch)
        {
            case 'w':
                deltaY = -1;
                break;

            case 'a':
                deltaX = -1;
                break;

            case 's':
                deltaY = 1;
                break;

            case 'd':
                deltaX = 1;
                break;
        }

        if(IsCollision(mainCharacter, rock, deltaX, deltaY))
        {
            rock.Move(deltaX, deltaY);
        }

        mainCharacter.positionX = Math.Clamp(mainCharacter.positionX + deltaX, 1, 22);
        mainCharacter.positionY = Math.Clamp(mainCharacter.positionY + deltaY, 3, 13);
    }

    static void GetMeOutOfThisRockPls(ref Rock rock, ref MainCharacter mainCharacter)
    {
        if (rock.positionY == mainCharacter.positionY && rock.positionX == mainCharacter.positionX)
        {
            if (rock.positionX == 1)
            {
                mainCharacter.positionX += 1;
            }
            if (rock.positionX == 22)
            {
                mainCharacter.positionX += -1;
            }

            if (rock.positionY == 3)
            {
                mainCharacter.positionY += 1;
            }
            if (rock.positionY == 13)
            {
                mainCharacter.positionY += -1;
            }
        }
    }

    private static bool IsCollision(MainCharacter mainCharacter,  Rock rock, int deltaX, int deltaY)
    {
        return mainCharacter.positionX == rock.positionX + deltaX * -1 && mainCharacter.positionY == rock.positionY + deltaY * -1;
    }

    private static void HandlePortal(char ch, ref MainCharacter mainCharacter, ref Portal enterPortal, ref Portal exitPortal)
    {
        if (ch == 'p')
        {
            enterPortal.SpawnPortal(ref mainCharacter, ref enterPortal, ref exitPortal);
        }
    }

    private static void ValidateRockPosition(ref Rock rock)
    {
        rock.positionX = Math.Clamp(rock.positionX, 1, 22);
        rock.positionY = Math.Clamp(rock.positionY, 3, 13);
    }

    private static void DrawWalls(BufferConsole buffer)
    {
        for (int i = 0; i < MapWidth; ++i)
        {
            // horizontal walls
            buffer.DrawToBuffer(i, 2, '|', ConsoleColor.DarkYellow);
            buffer.DrawToBuffer(i, 14, '|', ConsoleColor.DarkYellow);
            if(i < 14 && i > 2)
            {
                // vertical walls
                buffer.DrawToBuffer(0, i, '|', ConsoleColor.DarkYellow);
                buffer.DrawToBuffer(23, i, '|', ConsoleColor.DarkYellow);
            }
        }
    }


    protected static int origRow;
    protected static int origCol;

    //protected static void WriteAt(string s, int x, int y)
    //{
    //    if (x >= 0 && x < Console.WindowWidth && y >= 0 && y < Console.WindowHeight)
    //    {
    //        Console.SetCursorPosition(origCol + x, origRow + y);
    //        Console.Write(s);
    //    }
    //}

    //public static void ClearConsole()
    //{
    //    Console.SetCursorPosition(0, 0);
    //    Console.CursorVisible = false;
    //    for (int y = 0; y < Console.WindowHeight; ++y)
    //        Console.Write(new String(' ', Console.WindowWidth));
    //    Console.SetCursorPosition(0, 0);
    //}

    public static void Main()
    {
        MainCharacter mainCharacter = new();
        Rock rock1 = new();
        PlaceForRock placeForRock = new();
        Portal portal = new();
        Portal enterPortal = new();
        Portal exitPortal = new();

        BufferConsole buffer = new();
        buffer.InitBuffer(MapWidth, MapHeight);

        Console.CursorVisible = false;

        while (true)
        {
            buffer.ClearBuffer();
            DrawWalls(buffer);
            buffer.DrawToBuffer(mainCharacter.positionX, mainCharacter.positionY, 'A');
            buffer.DrawToBuffer(rock1.positionX, rock1.positionY, 'O', ConsoleColor.DarkGray);
            buffer.DrawToBuffer(placeForRock.positionX, placeForRock.positionY, 'X', ConsoleColor.Red);
            if (enterPortal.isEnterSpawned)
                buffer.DrawToBuffer(portal.positionX, portal.positionY, 'P', ConsoleColor.Blue);
            if (exitPortal.isExitSpawned)
                buffer.DrawToBuffer(portal.positionX, portal.positionY, 'P', ConsoleColor.Magenta);

            // Вывод UI (счёт)
            string scoreText = $"Score: {score}";
            for (int i = 0; i < scoreText.Length; i++)
                buffer.DrawToBuffer(8 + i, 1, scoreText[i]);

            // Рендер буфера на экран
            buffer.RenderBuffer();

            if (rock1.positionX == placeForRock.positionX && rock1.positionY == placeForRock.positionY)
            {
                rock1 = new Rock();
                placeForRock = new PlaceForRock();
                score += 1;
                buffer.ClearBuffer();
            }

            TryToMoveRock(ref mainCharacter, ref rock1, ref enterPortal, ref exitPortal);
            GetMeOutOfThisRockPls(ref rock1, ref mainCharacter);

            Thread.Sleep(5);
        }
    }
}