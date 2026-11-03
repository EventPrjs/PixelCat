using System;

namespace Schoolmakerdays
{
    // Column 0 is the leftmost bit Q6; row 0 is the oldest/topmost row.
    public sealed class PixelGridModel
    {
        public const int Rows = 10;
        public const int Columns = 7;
        public readonly bool[] InputLine = new bool[Columns];
        public readonly bool[,] Grid = new bool[Rows, Columns];

        public void Commit()
        {
            for (int row = 0; row < Rows - 1; row++)
                for (int column = 0; column < Columns; column++)
                    Grid[row, column] = Grid[row + 1, column];
            for (int column = 0; column < Columns; column++)
                Grid[Rows - 1, column] = InputLine[column];
            // Live signals remain active until their contacts are released.
        }

        public void ResetGrid() => Array.Clear(Grid, 0, Grid.Length);
        public void ClearInput() => Array.Clear(InputLine, 0, InputLine.Length);
    }
}
