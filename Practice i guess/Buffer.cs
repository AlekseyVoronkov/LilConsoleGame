using Practice_i_guess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice_i_guess
{
    class BufferConsole
    {
        private static char[,] buffer;
        private static ConsoleColor[,] colorBuffer; // Новый буфер для цветов
        private static int width, height;

        public BufferConsole()
        {

        }


        public void InitBuffer(int w, int h)
        {
            width = w;
            height = h;
            buffer = new char[width, height];
            colorBuffer = new ConsoleColor[width, height];

        }

        public void ClearBuffer()
        {
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    buffer[x, y] = ' ';
        }

        public void DrawToBuffer(int x, int y, char c, ConsoleColor color = ConsoleColor.White)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                buffer[x, y] = c;
                colorBuffer[x, y] = color; // Сохраняем цвет
            }
        }


        public void RenderBuffer()
        {
            Console.SetCursorPosition(0, 0);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Console.ForegroundColor = colorBuffer[x, y]; // Устанавливаем цвет
                    Console.Write(buffer[x, y]);
                }
                Console.WriteLine();
            }
            Console.ResetColor(); // Сбрасываем цвет после отрисовки
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

