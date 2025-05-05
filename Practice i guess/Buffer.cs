using Practice_i_guess;


namespace Practice_i_guess
{
    internal class BufferConsole
    {
        private static char[,] buffer;
        private static ConsoleColor[,] colorBuffer; 
        private static int width, height;

        public BufferConsole()
        {

        }


        public void InitBuffer(int w, int h)
        {
            //setting buffers width and height 
            width = w;
            height = h;
            buffer = new char[width, height];
            colorBuffer = new ConsoleColor[width, height];

        }

        public void ClearBuffer()
        {
            // filling buffer with space char
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    buffer[x, y] = ' ';
        }

        public void DrawToBuffer(int x, int y, char c, ConsoleColor color = ConsoleColor.White)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                buffer[x, y] = c;
                colorBuffer[x, y] = color; // saving color of char at the same coords
            }
        }


        public void RenderBuffer()
        {
            Console.SetCursorPosition(0, 0);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Console.ForegroundColor = colorBuffer[x, y];
                    if (buffer[x, y] == '@')
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.Write(buffer[x, y]);
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    else if (buffer[x, y] == '.')
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.Write(buffer[x, y]);
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.Write(buffer[x, y]);
                    }
                }
                Console.WriteLine();
            }
            Console.ResetColor();
        }


    }
}

static class BufferExtensions
{
    public static void DrawText(this BufferConsole buffer, int x, int y, string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            buffer.DrawToBuffer(x + i, y, text[i]);
        }
    }
    public static void DrawText(this BufferConsole buffer, int x, int y, string text, ConsoleColor color)
    {
        for (int i = 0; i < text.Length; i++)
        {
            buffer.DrawToBuffer(x + i, y, text[i], color);
        }
    }

    public static void ClearArea(this BufferConsole buffer, int x, int y, int width, int height)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                buffer.DrawToBuffer(x + i, y + j, ' ');
            }
        }
    }
}

