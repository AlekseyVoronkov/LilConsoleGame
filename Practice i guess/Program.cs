using Practice_i_guess;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

class TestGame
{
    public static Random random = new();
    public static int score = 0;
    private const int MapWidth = 24;
    private const int MapHeight = 17;
    private const char wallChar = '|';
    private const char NoKeyMarker = '}';
    private static readonly char[] NoKeys = [NoKeyMarker, NoKeyMarker, NoKeyMarker, NoKeyMarker];
    private static readonly Dictionary<char[], (int deltaX, int deltaY)> controls = new()
    {
        [['a', 'A', 'ф', 'Ф']] = (-1, 0),
        [['d', 'D', 'в', 'В']] = (1, 0),
        [['w', 'W', 'ц', 'Ц']] = (0, -1),
        [['s', 'S', 'ы', 'Ы']] = (0, 1),
        [['r', 'R', 'к', 'К']] = (0, 0),
        [['p', 'P', 'з', 'З']] = (0, 0)
    };

    struct Portal
    {
        public int enterPositionX;
        public int enterPositionY;

        public int exitPositionX;
        public int exitPositionY;

        public bool isEnterSpawned = false;
        public bool isExitSpawned = false;
        public Portal()
        {
        }

        public void SpawnPortal(ref MainCharacter mainCharacter)
        {
            if(isEnterSpawned == false)
            {
                enterPositionX = mainCharacter.positionX;
                enterPositionY = mainCharacter.positionY;
                isEnterSpawned = true;
            }
            else if(isExitSpawned == false)
            {
                exitPositionX = mainCharacter.positionX;
                exitPositionY = mainCharacter.positionY;
                isExitSpawned = true;
            }
            else
            {
                isEnterSpawned = false;
                isExitSpawned = false;
            }
        }

        public void TeleportObject(ref MainCharacter mainCharater, ref Rock rock)
        {
            if (mainCharater.positionX == enterPositionX && mainCharater.positionY == enterPositionY)
            {
                mainCharater.positionX = exitPositionX;
                mainCharater.positionY = exitPositionY;
            }

            if (rock.positionX == enterPositionX && rock.positionY == enterPositionY)
            {
                rock.positionX = exitPositionX;
                rock.positionY = exitPositionY;
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

    static void HandleControll(ref MainCharacter mainCharacter, ref Rock rock, ref Portal portal)
    {
        if (Console.KeyAvailable)
        {
            var ch = Console.ReadKey(true).KeyChar;
            var keysPressed = GetPressedKeys(ch).Item1;
            var movement = GetPressedKeys(ch).Item2;
            MoveCharacter(movement.deltaX, movement.deltaY, ref mainCharacter, ref rock);

            if (keysPressed.Contains('r')) 
            {
                score -= 1;
                rock = new Rock();
            }

            if (keysPressed.Contains('p'))
            {
                HandlePortal('p', ref mainCharacter, ref portal);
            }
        }
        if(portal.isExitSpawned == true)
        {
            portal.TeleportObject(ref mainCharacter, ref rock);
        }

        ValidateRockPosition(ref rock);
    }
    
    private static (char[], (int deltaX, int deltaY)) GetPressedKeys(char input)
    {
        char[] keys = controls.FirstOrDefault(kvp => kvp.Key.Contains(input)).Key ?? NoKeys;
        (int, int) values = controls.FirstOrDefault(kvp => kvp.Key.Contains(input)).Value;
        
        return (keys, values);
    }
    private static void MoveCharacter(int deltaX, int deltaY, ref MainCharacter mainCharacter, ref Rock rock)
    {

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

    private static void HandlePortal(char ch, ref MainCharacter mainCharacter, ref Portal portal)
    {
        portal.SpawnPortal(ref mainCharacter);
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
            buffer.DrawToBuffer(i, 2, wallChar, ConsoleColor.DarkYellow);
            buffer.DrawToBuffer(i, 14, wallChar, ConsoleColor.DarkYellow);
            if(i < 14 && i > 2)
            {
                // vertical walls
                buffer.DrawToBuffer(0, i, wallChar, ConsoleColor.DarkYellow);
                buffer.DrawToBuffer(23, i, wallChar, ConsoleColor.DarkYellow);
            }
        }
    }

    public static void Main()
    {
        MainCharacter mainCharacter = new();
        Rock rock1 = new();
        PlaceForRock placeForRock = new();
        Portal portal = new();

        BufferConsole buffer = new();
        buffer.InitBuffer(MapWidth, MapHeight);

        Console.CursorVisible = false;

        while (true)
        {
            buffer.ClearBuffer();
            DrawWalls(buffer);
            buffer.DrawToBuffer(placeForRock.positionX, placeForRock.positionY, 'X', ConsoleColor.Red);
            if (portal.isEnterSpawned)
                buffer.DrawToBuffer(portal.enterPositionX, portal.enterPositionY, 'P', ConsoleColor.Blue);
            if (portal.isExitSpawned)
                buffer.DrawToBuffer(portal.exitPositionX, portal.exitPositionY, 'P', ConsoleColor.Magenta);
            buffer.DrawToBuffer(rock1.positionX, rock1.positionY, 'O', ConsoleColor.DarkGray);
            buffer.DrawToBuffer(mainCharacter.positionX, mainCharacter.positionY, 'A');

            // Вывод UI 
            GameUI.DrawUI(buffer, score);


            // Рендер буфера на экран
            buffer.RenderBuffer();

            if (rock1.positionX == placeForRock.positionX && rock1.positionY == placeForRock.positionY)
            {
                rock1 = new Rock();
                placeForRock = new PlaceForRock();
                score += 1;
                buffer.ClearBuffer();
            }

            HandleControll(ref mainCharacter, ref rock1, ref portal);
            GetMeOutOfThisRockPls(ref rock1, ref mainCharacter);

            Thread.Sleep(5);
        }
    }
}