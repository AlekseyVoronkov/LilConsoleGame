using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TestGame;

namespace Practice_i_guess
{
    internal class Shop
    {
        int shopWidth = 24;
        int shopHeight = 18;
        char wallChar = '$';

        internal static void TryToBuyItem(ref TestGame.MainCharacter mainCharacter, GameUI gameUI)
        {
            if (mainCharacter.positionX == 3 && mainCharacter.positionY == 4 && mainCharacter.score >= GameEconomy.PortalCost)
            {
                mainCharacter.score -= GameEconomy.PortalCost;
                mainCharacter.Abilities.Add(Ability.Portal);
                GameUI.ShowTempMessage("You bought a portal yupieee");
                ShopPool.portalUpgrade.isBought = true;
            }
            if (mainCharacter.positionX == 3 && mainCharacter.positionY == 4 && mainCharacter.score < GameEconomy.PortalCost)
            {
                GameUI.ShowTempMessage($"poor bastard, you need {GameEconomy.PortalCost - mainCharacter.score} more");
            }

            if ((mainCharacter.positionX == 6 && mainCharacter.positionY == 4) && mainCharacter.score >= GameEconomy.DashCost)
            {
                mainCharacter.score -= GameEconomy.DashCost;
                mainCharacter.Abilities.Add(Ability.Dash);
                GameUI.ShowTempMessage("You bought a dash yupieee");
                ShopPool.dashUpgrade.isBought = true;
            }
            if (mainCharacter.positionX == 3 && mainCharacter.positionY == 4 && mainCharacter.score < GameEconomy.DashCost)
            {
                GameUI.ShowTempMessage($"poor bastard, you need {GameEconomy.DashCost - mainCharacter.score} more");
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
            if (!ShopPool.portalUpgrade.isBought)
            {
                buffer.DrawToBuffer(3, 3, 'P', ConsoleColor.Magenta);
                buffer.DrawToBuffer(3, 4, '.', ConsoleColor.Yellow);
            }

            if (!ShopPool.dashUpgrade.isBought)
            {
                buffer.DrawToBuffer(6, 3, 'D', ConsoleColor.Gray);
                buffer.DrawToBuffer(6, 4, '.', ConsoleColor.Yellow);
            }
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

    public const int DashCost = 5;
    public const char DashSymbol = 'D';
}
