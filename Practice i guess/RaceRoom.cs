using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static TestGame;

namespace Practice_i_guess
{
    internal class RaceRoom
    {
        private readonly int raceWidth = 24;
        private readonly int raceHeight = 18;
        private readonly char wallChar = '#';
        private string currentRoom; // Текущая комната (сохраняется между кадрами)
        private static readonly Random random = new(); // Один экземпляр Random
        static string[] roomPatterns = [   
                                    "##########@@############" +
                                    "#room1                 #" +
                                    "#   ####################" +
                                    "#                      #" +
                                    "#   ####################" +
                                    "#     #    #   #       #" +
                                    "##### #   #  # #=======#" +
                                    "#   # #  #  #  #       #" +
                                    "# #   # #  #   #       #" +
                                    "# ######  #    #       #" +
                                    "# #   #  #     #       #" +
                                    "#   #   #              #" +
                                    "########################",

                                    "##########@@############" +
                                    "#room2                 #" +
                                    "#                      #" +
                                    "#                      #" +
                                    "#                      #" +
                                    "#                      #" +
                                    "#                      #" +
                                    "#                      #" +
                                    "#                      #" +
                                    "#                      #" +
                                    "#======================#" +
                                    "#                      #" +
                                    "########################"];

        public RaceRoom()
        {
            currentRoom = ChooseRandomRoom();
        }
        internal void DrawRaceRoom(BufferConsole buffer, ref MainCharacter mainCharacter)
        {
            int mainCharacterPosition = (mainCharacter.positionY - 2) * raceWidth + mainCharacter.positionX;

            int x = 0;
            int y = 2;
            foreach ( var tile in currentRoom)
            {
                // draw walls and finish line
                // scary code for alternating wall color between gray and white. it must be possible to optimize this thing a lot.

                if (tile == '#' && (x % 2 == 0 && y % 2 == 0 || (x % 2 != 0 && y % 2 != 0)))
                {
                    buffer.DrawToBuffer(x, y, tile, ConsoleColor.DarkGray);
                }
                else if (tile == '=')
                {
                    buffer.DrawToBuffer(x, y, tile, ConsoleColor.Green);
                } 
                else
                {
                    buffer.DrawToBuffer(x, y, tile);
                }
                // again, probably suboptimal way to get x and y, but it works.
                if(x < raceWidth - 1)
                {
                    ++x;
                } else
                {
                    x = 0;
                    ++y;
                }
            }
            // character movement handling, time to suffer.
            // detect if mc reached finish line -> give him points -> move him to start pos -> roll new room
            // kinda broken rn :(
            if (currentRoom[mainCharacterPosition] == '=')
            {
                mainCharacter.score += 1;
                mainCharacter.positionX = 11;
                mainCharacter.positionY = 2;
                currentRoom = ChooseRandomRoom();
            }
        }

        private string ChooseRandomRoom()
        {
            return roomPatterns[random.Next(roomPatterns.Length)];
        }
    }
}
