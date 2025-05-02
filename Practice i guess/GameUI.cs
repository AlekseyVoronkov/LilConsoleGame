using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice_i_guess
{
    static class GameUI
    {
        private static readonly Dictionary<string, (int x, int y)> uiElements = new()
        {
            ["score"] = (8, 1),
            ["help1"] = (0, 15),
            ["help2"] = (0, 16)
        };

        public static void DrawUI(BufferConsole buffer, int score)
        {
            // Очистка предыдущих элементов
            ClearUI(buffer);

            // Отрисовка счёта
            buffer.DrawText(uiElements["score"].x, uiElements["score"].y, $"Score: {score}");

            // Отрисовка подсказок
            buffer.DrawText(uiElements["help1"].x, uiElements["help1"].y, "Respawn rock: [R]");
            buffer.DrawText(uiElements["help2"].x, uiElements["help2"].y, "Portals: [P]");
        }

        private static void ClearUI(BufferConsole buffer)
        {
            foreach (var element in uiElements.Values)
            {
                buffer.ClearArea(element.x, element.y, 30, 1); // Очищаем область под элемент
            }
        }
    }
}
