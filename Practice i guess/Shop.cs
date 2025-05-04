using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice_i_guess
{
    internal class Shop
    {
        int shopWidth = 24;
        int shopHeight = 18;
        char wallChar = '$';

        struct portalUpgrade
        {
            bool isBought = false;
            char visual = 'P';
            
            public portalUpgrade()
            {
                isBought = true;

            }
        }

        internal void DrawShop(BufferConsole buffer)
        {
            for (int i = 0; i < shopWidth - 1; ++i)
            {
                // horizontal walls
                buffer.DrawToBuffer(i, 2, wallChar, ConsoleColor.DarkYellow);
                buffer.DrawToBuffer(i, 14, wallChar, ConsoleColor.DarkYellow);

                if (i < 16 && i > 2)
                {
                    // vertical walls
                    buffer.DrawToBuffer(0, i, wallChar, ConsoleColor.DarkYellow);
                    buffer.DrawToBuffer(23, i, wallChar, ConsoleColor.DarkYellow);
                }
            }

            buffer.DrawToBuffer(3, 3, 'P', ConsoleColor.Magenta);
            buffer.DrawToBuffer(3, 4, '.', ConsoleColor.Yellow);

            // overwriting some walls for door  
            buffer.DrawToBuffer(0, 6, '@', ConsoleColor.Black);
            buffer.DrawToBuffer(0, 7, '@', ConsoleColor.Black);
        }
    }
}

internal class GameEconomy
{
    public const int PortalCost = 5;
    public const char PortalSymbol = 'P';
}
