using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice_i_guess
{
    internal class BufferConsole
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
