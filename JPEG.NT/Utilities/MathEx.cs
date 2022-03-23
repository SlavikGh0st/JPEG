using System.Collections.Generic;
using System.Linq;

namespace JPEG.NT.Utilities
{
    public static class MathEx
    {
        public static T[] GetRow<T>(this T[,] matrix, int rowNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(1))
                .Select(x => matrix[rowNumber, x])
                .ToArray();
        }

        public static void SetRow<T>(this T[,] matrix, int rowNumber, T[] row)
        {
            var length = row.Length;
            for (var i = 0; i < length; i++)
                matrix[rowNumber, i] = row[i];
        }

        public static T[] GetColumn<T>(this T[,] matrix, int columnNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(0))
                .Select(x => matrix[x, columnNumber])
                .ToArray();
        }

        public static void SetColumn<T>(this T[,] matrix, int columnNumber, T[] column)
        {
            var length = column.Length;
            for (var i = 0; i < length; i++)
                matrix[i, columnNumber] = column[i];
        }

        public static IEnumerable<T> ToEnumerable<T>(this T[,] matrix)
        {
            var height = matrix.GetLength(0);
            var width = matrix.GetLength(1);

            for (var j = 0; j < height; j++)
            for (var i = 0; i < width; i++)
                yield return matrix[j, i];
        }
    }
}