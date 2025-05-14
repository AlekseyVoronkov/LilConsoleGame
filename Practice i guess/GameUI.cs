using System;

namespace Practice_i_guess
{
    internal class GameUI
    {
        private readonly List<TempMessage> _tempMessages = new();

        // Класс для хранения данных сообщения
        private class TempMessage
        {
            public string Text { get; }
            public (int x, int y) Position { get; }
            public ConsoleColor Color { get; }
            public DateTime ExpireTime { get; }

            public TempMessage(string text, (int x, int y) pos,
                             ConsoleColor color, int durationMs)
            {
                Text = text;
                Position = pos;
                Color = color;
                ExpireTime = DateTime.Now.AddMilliseconds(durationMs);
            }

            public bool IsExpired => DateTime.Now >= ExpireTime;
        }

        private static readonly Dictionary<string, (int x, int y)> uiElements = new()
        {
            // text cords
            ["score"] = (8, 1),
            ["help1"] = (0, 15),
            ["help2"] = (0, 16),
            ["thats shop"] = (25, 5),
            ["help3"] = (0, 17),
            ["s_portal"] = (25, 5),
            ["s_dash"] = (25, 5)

        };

        public void ShowTempMessage(string text,
                              (int x, int y)? position = null,
                              ConsoleColor? color = null,
                              int durationMs = 3000)
        {
            var pos = position ?? (0, 18); // Позиция по умолчанию
            var col = color ?? ConsoleColor.Red; // Цвет по умолчанию

            _tempMessages.Add(new TempMessage(text, pos, col, durationMs));
        }

        // Обновление состояний сообщений (вызывать каждый кадр)
        public void Update()
        {
            _tempMessages.RemoveAll(m => m.IsExpired);
        }

        // Отрисовка всех активных сообщений
        private void DrawTempMessages(BufferConsole buffer)
        {
            foreach (var msg in _tempMessages)
            {
                buffer.DrawText(msg.Position.x, msg.Position.y, msg.Text, msg.Color);
            }
        }


        public void DrawUI(BufferConsole buffer, int score, int positionX, int positionY, bool inSnop)
        {
            // Clear previous elements
            ClearUI(buffer);

            // debug slop 
            buffer.DrawText(28, 9, $"x{positionX}, y{positionY}");
            buffer.DrawText(28, 10, $"p_isBought: {ShopPool.portalUpgrade.isBought}");
            buffer.DrawText(28, 11, $"d_isBought: {ShopPool.dashUpgrade.isBought}");
            // draw score and controll hints
            buffer.DrawText(uiElements["score"].x, uiElements["score"].y, $"Score: {score}");
            buffer.DrawText(uiElements["help1"].x, uiElements["help1"].y, "Respawn rock: [R]");
            buffer.DrawText(uiElements["help2"].x, uiElements["help2"].y, "Portals: [P]");
            buffer.DrawText(0, 17, $"x{positionX}, y{positionY}");

            Update();
            DrawTempMessages(buffer);

            if (inSnop)
            {
                buffer.DrawText(uiElements["help3"].x, uiElements["help3"].y, "Buy an item [E]");

                if (!ShopPool.portalUpgrade.isBought)
                {
                    if (positionX == 3 && positionY == 4)
                    {
                        buffer.DrawText(uiElements["s_portal"].x, uiElements["s_portal"].y, $"Portal costs {GameEconomy.DashCost}$");
                    }
                }

                if (!ShopPool.dashUpgrade.isBought)
                {
                    if (positionX == 6 && positionY == 4)
                    {
                        buffer.DrawText(uiElements["s_dash"].x, uiElements["s_dash"].y, $"Dash costs {GameEconomy.DashCost}$");
                    }
                }
            }

            if (positionX == 22 && (positionY == 6 || positionY == 7))
            {
                buffer.DrawText(uiElements["thats shop"].x, uiElements["thats shop"].y, "Shop doors");
            }
        }

        private static void ClearUI(BufferConsole buffer)
        {
            foreach (var element in uiElements.Values)
            { 
                buffer.ClearArea(element.x, element.y, 30, 1); // clearing specified area
            }
        }
    }
}
