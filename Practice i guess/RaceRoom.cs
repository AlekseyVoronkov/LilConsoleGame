using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice_i_guess
{
    internal class RaceRoom
    {
        int raceWidth = 24;
        int raceHeight = 18;
        char wallChar = '#';

        internal void DrawRaceRoom(BufferConsole buffer)
        {
            for (int i = 0; i < raceWidth - 1; ++i)
            {
                // horizontal walls
                buffer.DrawToBuffer(i, 2, wallChar, i % 2 == 0 ? ConsoleColor.DarkGray: ConsoleColor.White);
                buffer.DrawToBuffer(i, 14, wallChar, i % 2 == 0 ? ConsoleColor.DarkGray : ConsoleColor.White);

                if (i < raceHeight - 2 && i > 1)
                {
                    // vertical walls
                    buffer.DrawToBuffer(0, i, wallChar, i % 2 == 0 ? ConsoleColor.DarkGray : ConsoleColor.White);
                    buffer.DrawToBuffer(23, i, wallChar, i % 2 == 0 ? ConsoleColor.DarkGray : ConsoleColor.White);
                }
            }

            // overwriting some walls for door  
            buffer.DrawToBuffer(11, 2, '@', ConsoleColor.Black);
            buffer.DrawToBuffer(12, 2, '@', ConsoleColor.Black);
        }
    }

}
