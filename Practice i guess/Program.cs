using Practice_i_guess;
using static TestGame;
using System;

class TestGame
{
    public static Random random = new();
    public enum Ability { None, Portal, Dash }
    public static bool inShop = false;
    public static bool inRaceRoom = false;
    public static bool inMainArea = true;
    private const int MapWidth = 24;
    private const int MapHeight = 17;
    private const char wallChar = '|';

    // default value for keys (in case user pressed unmentioned in controls dictionary key) 
    private const char NoKeyMarker = '}';
    private static readonly char[] NoKeys = [NoKeyMarker, NoKeyMarker, NoKeyMarker, NoKeyMarker];

    private static readonly Dictionary<char[], (int deltaX, int deltaY)> controls = new()
    {
        [['a', 'A', 'ф', 'Ф']] = (-1, 0),   // 
        [['d', 'D', 'в', 'В']] = (1, 0),    // wasd handling
        [['w', 'W', 'ц', 'Ц']] = (0, -1),   // 
        [['s', 'S', 'ы', 'Ы']] = (0, 1),    //
        [['r', 'R', 'к', 'К']] = (0, 0),    // respawn rock
        [['p', 'P', 'з', 'З']] = (0, 0),    // poral handling
        [['e', 'E', 'у', 'У']] = (0, 0),    // item buying handling
        [['f', 'F', 'а', 'А']] = (0, 0)     // dash handling
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
                // third call dispawn portals 
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
        public (int, int) movement = (0, 0); 
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
            int prevDeltaX = mainCharacter.movement.Item1;
            int prevDeltaY = mainCharacter.movement.Item2;

            mainCharacter.movement = GetPressedKeys(ch).Item2;

            // handling controlls
            MoveCharacter(mainCharacter.movement.Item1, mainCharacter.movement.Item2, ref mainCharacter, ref rock);

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
                mainCharacter.movement.Item1 = prevDeltaX;
                mainCharacter.movement.Item2 = prevDeltaY;

                HandleDash(prevDeltaX, prevDeltaY, ref mainCharacter, ref rock, gameUI);
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
            // from MainArea to Shop
            if (mainCharacter.positionX == 22 && (mainCharacter.positionY == 6 || mainCharacter.positionY == 7) && deltaX == 1)
            {
                inShop = true;
                inMainArea = false;
                inRaceRoom = false;
                mainCharacter.positionX = 0;
            }
        }
        else if(inShop) 
        {
            // from Shop to MainArea
            if (mainCharacter.positionX == 1 && (mainCharacter.positionY == 6 || mainCharacter.positionY == 7) && deltaX == -1)
            {
                inShop = false;
                inMainArea = true;
                inRaceRoom = false;
                mainCharacter.positionX = 23;
            }

            // from Shop to RaceRoom
            if (mainCharacter.positionY == 13 && (mainCharacter.positionX == 11 || mainCharacter.positionX == 12) && deltaY == 1)
            {
                inShop = false;
                inMainArea = false;
                inRaceRoom = true;
                mainCharacter.positionY = 2;
            }
        }
        else if (inRaceRoom)
        {
            // from RaceRoom to Shop
            if (mainCharacter.positionY == 3 && (mainCharacter.positionX == 11 || mainCharacter.positionX == 12) && deltaY == -1)
            {
                inShop = true;
                inMainArea = false;
                inRaceRoom = false;
                mainCharacter.positionY = 13;
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

    private static void HandleDash(int deltaX, int deltaY, ref MainCharacter mainCharacter, ref Rock rock, GameUI gameUI)
    {
        if (mainCharacter.HasAbility(Ability.Dash) == true)
        {
            // getting dash direction + range
            deltaX = Math.Sign(deltaX) * 3; // -3, 0 or 3
            deltaY = Math.Sign(deltaY) * 3;

            (int startX, int startY) = (mainCharacter.positionX, mainCharacter.positionY);

            MoveCharacter(deltaX, deltaY, ref mainCharacter, ref rock);

            // drawing dash trail
            if (deltaX != 0 || deltaY != 0)
            {
                for (int i = 1; i < 3; ++i)
                {
                    int trailX = startX + deltaX * i / 3;
                    int trailY = startY + deltaY * i / 3;

                    gameUI.ShowTempMessage(",", (Math.Clamp(trailX, 1, 22), Math.Clamp(trailY, 3, 13)), ConsoleColor.Cyan, durationMs: 350 * i);
                    gameUI.ShowTempMessage("o", (Math.Clamp(trailX, 1, 22), Math.Clamp(trailY, 3, 13)), ConsoleColor.Cyan, durationMs: 250 * i);
                    gameUI.ShowTempMessage("O", (Math.Clamp(trailX, 1, 22), Math.Clamp(trailY, 3, 13)), ConsoleColor.Cyan, durationMs: 150 * i);
                }
            }
        }
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
            if (i <  MapHeight - 3 && i > 2)
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

    private static void DrawMainArea(BufferConsole buffer, ref PlaceForRock placeForRock, ref Rock rock1, ref MainCharacter mainCharacter, Portal portal, GameUI gameUI)
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
        gameUI.DrawUI(buffer, mainCharacter.score, mainCharacter.positionX, mainCharacter.positionY, inShop, inMainArea, inRaceRoom);
    }
    private static void DrawShopArea(Shop shop, BufferConsole buffer, Rock rock1, Portal portal, MainCharacter mainCharacter, GameUI gameUI)
    {
        shop.DrawShop(buffer);
        buffer.DrawToBuffer(rock1.positionX, rock1.positionY, 'O', ConsoleColor.DarkGray);

        if (portal.isEnterSpawned)
            buffer.DrawToBuffer(portal.enterPositionX, portal.enterPositionY, 'P', ConsoleColor.Blue);
        if (portal.isExitSpawned)
            buffer.DrawToBuffer(portal.exitPositionX, portal.exitPositionY, 'P', ConsoleColor.Magenta);

        buffer.DrawToBuffer(mainCharacter.positionX, mainCharacter.positionY, 'A');
        gameUI.DrawUI(buffer, mainCharacter.score, mainCharacter.positionX, mainCharacter.positionY, inShop, inMainArea, inRaceRoom);
    }
    private static void DrawRaceArea(RaceRoom raceRoom, BufferConsole buffer, Portal portal, MainCharacter mainCharacter, GameUI gameUI)
    {
        raceRoom.DrawRaceRoom(buffer, ref mainCharacter);

        if (portal.isEnterSpawned)
            buffer.DrawToBuffer(portal.enterPositionX, portal.enterPositionY, 'P', ConsoleColor.Blue);
        if (portal.isExitSpawned)
            buffer.DrawToBuffer(portal.exitPositionX, portal.exitPositionY, 'P', ConsoleColor.Magenta);

        buffer.DrawToBuffer(mainCharacter.positionX, mainCharacter.positionY, 'A');
        gameUI.DrawUI(buffer, mainCharacter.score, mainCharacter.positionX, mainCharacter.positionY, inShop, inMainArea, inRaceRoom);
    }
    public static void Main()
    {
        MainCharacter mainCharacter = new();
        Rock rock1 = new();
        PlaceForRock placeForRock = new();
        Portal portal = new();

        Shop shop = new();
        GameUI gameUI = new();
        RaceRoom raceRoom = new();


        BufferConsole buffer = new();
        buffer.InitBuffer(60, 20);

        Console.CursorVisible = false;

        while (true)
        {
            buffer.ClearBuffer();
            if(inMainArea)
            {
                DrawMainArea(buffer, ref placeForRock, ref rock1, ref mainCharacter, portal, gameUI);
            }
            if(inShop)
            {
                DrawShopArea(shop, buffer, rock1, portal, mainCharacter, gameUI);
            }
            if(inRaceRoom)
            {
                DrawRaceArea(raceRoom, buffer, portal, mainCharacter, gameUI);
            }
            
            // Рендер буфера на экран
            buffer.RenderBuffer();

            HandleControll(ref mainCharacter, ref rock1, ref portal, ref gameUI, inShop);
            GetMeOutOfThisRockPls(ref rock1, ref mainCharacter);

            Thread.Sleep(5);
        }
    }
}