using Practice_i_guess;
using static TestGame;

class TestGame
{
    public static Random random = new();
    public enum Ability { None, Portal, Dash }
    public static bool inShop = false;
    public static bool inMainArea = true;
    private const int MapWidth = 24;
    private const int MapHeight = 17;
    private const char wallChar = '|';

    // default value for keys (in case user pressed unmentioned in controls dictionary key) 
    private const char NoKeyMarker = '}';
    private static readonly char[] NoKeys = [NoKeyMarker, NoKeyMarker, NoKeyMarker, NoKeyMarker];

    private static readonly Dictionary<char[], (int deltaX, int deltaY)> controls = new()
    {
        [['a', 'A', 'ф', 'Ф']] = (-1, 0),
        [['d', 'D', 'в', 'В']] = (1, 0), // wasd handling
        [['w', 'W', 'ц', 'Ц']] = (0, -1),
        [['s', 'S', 'ы', 'Ы']] = (0, 1),
        [['r', 'R', 'к', 'К']] = (0, 0), // respawn rock
        [['p', 'P', 'з', 'З']] = (0, 0), // poral handling
        [['e', 'E', 'у', 'У']] = (0, 0), // poral handling
        [['f', 'F', 'а', 'А']] = (0, 0) // dash handling
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
            if (isEnterSpawned == false)
            {
                enterPositionX = mainCharacter.positionX;
                enterPositionY = mainCharacter.positionY;
                isEnterSpawned = true;
            }
            else if (isExitSpawned == false)
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

    internal struct MainCharacter
    {
        public int positionX = 1;
        public int score = 10;
        public int positionY = 3;
        public List<Ability> Abilities { get; } = new();

        public MainCharacter()
        {
        }

        public bool HasAbility(Ability ability) => Abilities.Contains(ability);

    }

    static void HandleControll(ref MainCharacter mainCharacter, ref Rock rock, ref Portal portal, ref GameUI gameUI, bool inShop)
    {
        if (Console.KeyAvailable)
        {
            // getting pressed key and getting corresponding keys and values in controls dictionary 
            var ch = Console.ReadKey(true).KeyChar;
            var keysPressed = GetPressedKeys(ch).Item1;
            var movement = GetPressedKeys(ch).Item2;

            // handling controlls
            MoveCharacter(movement.deltaX, movement.deltaY, ref mainCharacter, ref rock);

            if (keysPressed.Contains('r'))
            {
                if (!inShop)
                {
                    mainCharacter.score -= 1;
                    rock = new Rock();
                }
            }

            if (keysPressed.Contains('p'))
            {
                HandlePortal('p', ref mainCharacter, ref portal);
            }

            if (keysPressed.Contains('f'))
            {
                HandleDash(ref movement.deltaX, ref movement.deltaY);
            }

            if (keysPressed.Contains('e') && inShop)
            {
                Shop.TryToBuyItem(ref mainCharacter, gameUI);
            }
        }

        if (portal.isExitSpawned == true)
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

        if (IsCollision(mainCharacter, rock, deltaX, deltaY))
        {
            rock.Move(deltaX, deltaY);
        }

        HandleMovementBetweenAreas(deltaX, deltaY, ref mainCharacter, ref rock);

        mainCharacter.positionX = Math.Clamp(mainCharacter.positionX + deltaX, 1, 22);
        mainCharacter.positionY = Math.Clamp(mainCharacter.positionY + deltaY, 3, 13);
    }

    private static void HandleMovementBetweenAreas(int deltaX, int deltaY, ref MainCharacter mainCharacter, ref Rock rock)
    {
        if (inMainArea)
        {
            if (mainCharacter.positionX == 22 && (mainCharacter.positionY == 6 || mainCharacter.positionY == 7) && deltaX == 1)
            {
                inShop = true;
                inMainArea = false;
                mainCharacter.positionX = 0;
            }
        }
        else
        {
            if (mainCharacter.positionX == 1 && (mainCharacter.positionY == 6 || mainCharacter.positionY == 7) && deltaX == -1)
            {
                inShop = false;
                inMainArea = true;
                mainCharacter.positionX = 23;
            }
        }
    }
    static void GetMeOutOfThisRockPls(ref Rock rock, ref MainCharacter mainCharacter)
    {
        // pushing mc out of rock depending on rock position
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

    private static bool IsCollision(MainCharacter mainCharacter, Rock rock, int deltaX, int deltaY)
    {
        return mainCharacter.positionX == rock.positionX + deltaX * -1 && mainCharacter.positionY == rock.positionY + deltaY * -1;
    }

    private static void HandlePortal(char ch, ref MainCharacter mainCharacter, ref Portal portal)
    {
        if (mainCharacter.HasAbility(Ability.Portal) == true)
        {
            portal.SpawnPortal(ref mainCharacter);
        }
    }

    private static void HandleDash(ref int deltaX, ref int deltaY)
    {
        deltaX *= 3;
        deltaY *= 3;
    }

    private static void ValidateRockPosition(ref Rock rock)
    {
        rock.positionX = Math.Clamp(rock.positionX, 1, 22);
        rock.positionY = Math.Clamp(rock.positionY, 3, 13);
    }

    private static void HandleRockOnCross(ref Rock rock, ref PlaceForRock placeForRock, ref MainCharacter mainCharacter)
    {

        if (rock.positionX == placeForRock.positionX && rock.positionY == placeForRock.positionY)
        {
            rock = new Rock();
            placeForRock = new PlaceForRock();
            mainCharacter.score += 1;

        }
    }

    private static void DrawWalls(BufferConsole buffer)
    {
        for (int i = 0; i < MapWidth; ++i)
        {
            // horizontal walls
            buffer.DrawToBuffer(i, 2, wallChar, ConsoleColor.DarkCyan);
            buffer.DrawToBuffer(i, 14, wallChar, ConsoleColor.DarkCyan);
            if (i < 14 && i > 2)
            {
                // vertical walls
                buffer.DrawToBuffer(0, i, wallChar, ConsoleColor.DarkCyan);
                buffer.DrawToBuffer(23, i, wallChar, ConsoleColor.DarkCyan);
            }
        }

        // overwriting some walls for door  
        buffer.DrawToBuffer(23, 6, '@', ConsoleColor.Black);
        buffer.DrawToBuffer(23, 7, '@', ConsoleColor.Black);
    }
    public static void Main()
    {
        MainCharacter mainCharacter = new();
        Rock rock1 = new();
        PlaceForRock placeForRock = new();
        Portal portal = new();
        Shop shop = new();
        GameUI gameUI = new();


        BufferConsole buffer = new();
        buffer.InitBuffer(60, 20);

        Console.CursorVisible = false;

        while (true)
        {
            buffer.ClearBuffer();
            if(inMainArea)
            {
                DrawWalls(buffer);
                buffer.DrawToBuffer(placeForRock.positionX, placeForRock.positionY, 'X', ConsoleColor.Red);
                if (portal.isEnterSpawned)
                    buffer.DrawToBuffer(portal.enterPositionX, portal.enterPositionY, 'P', ConsoleColor.Blue);
                if (portal.isExitSpawned)
                    buffer.DrawToBuffer(portal.exitPositionX, portal.exitPositionY, 'P', ConsoleColor.Magenta);
                buffer.DrawToBuffer(rock1.positionX, rock1.positionY, 'O', ConsoleColor.DarkGray);
                buffer.DrawToBuffer(mainCharacter.positionX, mainCharacter.positionY, 'A');

                HandleRockOnCross(ref rock1, ref placeForRock, ref mainCharacter);

                gameUI.DrawUI(buffer, mainCharacter.score, mainCharacter.positionX, mainCharacter.positionY, inShop);
            }
            if(inShop)
            {
                shop.DrawShop(buffer);
                buffer.DrawToBuffer(rock1.positionX, rock1.positionY, 'O', ConsoleColor.DarkGray);
                if (portal.isEnterSpawned)
                    buffer.DrawToBuffer(portal.enterPositionX, portal.enterPositionY, 'P', ConsoleColor.Blue);
                if (portal.isExitSpawned)
                    buffer.DrawToBuffer(portal.exitPositionX, portal.exitPositionY, 'P', ConsoleColor.Magenta);
                buffer.DrawToBuffer(mainCharacter.positionX, mainCharacter.positionY, 'A');
                gameUI.DrawUI(buffer, mainCharacter.score, mainCharacter.positionX, mainCharacter.positionY, inShop);
            }

            // Рендер буфера на экран
            buffer.RenderBuffer();

            HandleControll(ref mainCharacter, ref rock1, ref portal, ref gameUI, inShop);
            GetMeOutOfThisRockPls(ref rock1, ref mainCharacter);

            Thread.Sleep(5);
        }
    }
}