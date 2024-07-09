using System;

//Дан двумерный массив.
//Вычислить сумму второй строки и произведение первого столбца. Вывести исходную матрицу и результаты вычислений. 

namespace millionDollarsCourses
{
    internal class Program
    {
        static void Main()
        {
            Random random = new Random();
            int minRandomValue = 1;
            int maxRandomValue = 9;

            int rows = 4;
            int columns = 4;
            int[,] matrix = new int[rows, columns];

            int firstColumn = 1;
            int secondRow = 2;
            int secondRowSummory = 0;
            int firstColumnMultiplication = 0;


            #region fill and display matrix
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    matrix[i, j] = random.Next(minRandomValue, maxRandomValue + 1);
                    Console.Write(matrix[i, j] + " ");
                }

                Console.WriteLine();
            }
            #endregion


            #region count summory and multiplication
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (matrix[i, j] == matrix[i, secondRow])
                        secondRowSummory += matrix[i, j];

                    if (matrix[i, j] == matrix[firstColumn, j])
                        secondRowSummory += matrix[i, j];
                }

                Console.WriteLine();
            }
            #endregion

            Console.WriteLine($"Second row summory: {secondRowSummory}\n" +
                $"First columb Multiplication: {firstColumnMultiplication}");
        }
    }
}
