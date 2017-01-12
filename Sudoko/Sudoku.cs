using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sudoko
{
    public class Sudoku
    {
        private static readonly int[] _1_9 = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        private SudokuCell[] data = new SudokuCell[81];

        public Sudoku(int[,] obj)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    data[(i * 9 + j)] = new SudokuCell(obj[i, j], i, j);
                }
            }
        }

        public void PrintToConsole()
        {
            Console.WriteLine();
            Console.WriteLine();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Console.Write("  ");
                    Console.ForegroundColor = ConsoleColor.White;
                    if (data[(i * 9 + j)].Value != 0 && data[(i * 9 + j)].IsPreFilled)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }
                    if (data[(i * 9 + j)].Value != 0 && !data[(i * 9 + j)].IsPreFilled)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }

                    Console.Write(data[(i * 9 + j)]);

                }
                Console.WriteLine();
                Console.WriteLine();
            }
        }

        public void Solve()
        {
            List<int> remainingNumbers = null;
            List<int> allNoInBox = null;
            List<int> possibleValues = null;
            int zero_count = data.Where(P => !P.IsFilled).Count();
            int previousCount = zero_count;
            int break_x = 8;
            int break_y = 2;
            while (zero_count > 0)
            {
                foreach (var item in data.Where(P => !P.IsFilled))
                {
                    if (item.XIndex == break_x && item.YIndex == break_y)
                    {
                        remainingNumbers = null;
                        allNoInBox = null;
                    }
                    remainingNumbers = null;
                    allNoInBox = null;

                    allNoInBox = GetAllNosInBox(item);
                    remainingNumbers = _1_9.Except(allNoInBox).ToList();

                    possibleValues = new List<int>();

                    foreach (var x in remainingNumbers)
                    {
                        if (!IsInRow(x, item) && !IsInColumn(x, item) && !IsInBox(x, item, allNoInBox))
                        {
                            possibleValues.Add(x);
                            if (possibleValues.Count > 1)
                            {
                                break;
                            }
                        }
                    }
                    if (possibleValues.Count != 1)
                    {
                        possibleValues = new List<int>();
                        foreach (var x in remainingNumbers)
                        {
                            if (IsInAdjacentCells(x, item) && !IsInRow(x, item) && !IsInColumn(x, item) && !IsInBox(x, item, allNoInBox))
                            {
                                possibleValues.Add(x);
                                if (possibleValues.Count > 1)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    if (possibleValues.Count == 1)
                    {
                        item.Value = possibleValues.FirstOrDefault();
                    }
                }
                zero_count = data.Where(P => !P.IsFilled).Count();

                if (previousCount == zero_count)
                {
                    break;
                }
                previousCount = zero_count;
            }
        }

        private bool IsInRow(int x, SudokuCell cell)
        {
            return data.Where(P => P.XIndex == cell.XIndex && P.Value == x).FirstOrDefault() != null;
        }

        private bool IsInColumn(int x, SudokuCell cell)
        {
            return data.Where(P => P.YIndex == cell.YIndex && P.Value == x).FirstOrDefault() != null;
        }

        private bool IsInBox(int x, SudokuCell cell, List<int> allNoOfBox)
        {
            return allNoOfBox.Contains(x);
        }

        private bool IsInAdjacentCells(int x, SudokuCell item)
        {
            var result = true;
            var getAdjacentRows = GetAdjacentRows(item.XIndex);
            var getAdjacentColumns = GetAdjacentColumns(item.YIndex);
            var rowCheck = true;
            var colCheck = true;

            //for testing
            if (item.XIndex == 1 && item.YIndex == 3)
            {
                result = true;
            }

            getAdjacentColumns = data.Where(P => P.XIndex == item.XIndex && getAdjacentColumns.Contains(P.YIndex) && P.Value == 0 && P.BoxNo==item.BoxNo).Select(P => P.YIndex).ToList();
            getAdjacentRows = data.Where(P => P.YIndex == item.YIndex && getAdjacentRows.Contains(P.XIndex) && P.Value == 0 && P.BoxNo==item.BoxNo).Select(P => P.XIndex).ToList();

            for (int i = 0; i < getAdjacentRows.Count; i++)
            {
                if (data.Where(P => P.XIndex == getAdjacentRows[i] && P.Value == x).FirstOrDefault() == null)
                {
                    //if (data.Where(P => P.XIndex == getAdjacentRows[i] && P.BoxNo == item.BoxNo).Count() < 3)
                    {
                        rowCheck = false;
                        result = false;
                        break;
                    }
                }
            }

            for (int i = 0; i < getAdjacentColumns.Count; i++)
            {
                if (data.Where(P => P.YIndex == getAdjacentColumns[i] && P.Value == x).FirstOrDefault() == null)
                {
                    //if (data.Where(P => P.YIndex == getAdjacentColumns[i] && P.BoxNo == item.BoxNo).Count() < 3)
                    {
                        colCheck = false;
                        result = false;
                        break;
                    }
                }
            }

            if (rowCheck)
            {
                result = true;
                getAdjacentColumns = data.Where(P => P.XIndex == item.XIndex && getAdjacentColumns.Contains(P.YIndex) && P.Value == 0).Select(P => P.YIndex).ToList();
                for (int i = 0; i < getAdjacentColumns.Count; i++)
                {
                    if (data.Where(P => P.YIndex == getAdjacentColumns[i] && P.Value == x).FirstOrDefault() == null)
                    {
                        result = false;
                        break;
                    }
                }
            }

            if (colCheck)
            {
                result = true;
                getAdjacentRows = data.Where(P => P.YIndex == item.YIndex && getAdjacentRows.Contains(P.XIndex) && P.Value == 0).Select(P => P.XIndex).ToList();
                for (int i = 0; i < getAdjacentRows.Count; i++)
                {
                    if (data.Where(P => P.XIndex == getAdjacentRows[i] && P.Value == x).FirstOrDefault() == null)
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }

        private List<int> GetAdjacentRows(int currentRowIndex)
        {
            List<int> result = new List<int>();
            switch (currentRowIndex)
            {
                case 0:
                case 3:
                case 6:
                    result.Add(currentRowIndex + 1);
                    result.Add(currentRowIndex + 2);
                    break;
                case 1:
                case 4:
                case 7:
                    result.Add(currentRowIndex - 1);
                    result.Add(currentRowIndex + 1);
                    break;
                case 2:
                case 5:
                case 8:
                    result.Add(currentRowIndex - 1);
                    result.Add(currentRowIndex - 2);
                    break;
                default:
                    break;
            }

            return result;
        }

        private List<int> GetAdjacentColumns(int currentColumnIndex)
        {
            List<int> result = new List<int>();
            switch (currentColumnIndex)
            {
                case 0:
                case 3:
                case 6:
                    result.Add(currentColumnIndex + 1);
                    result.Add(currentColumnIndex + 2);
                    break;
                case 1:
                case 4:
                case 7:
                    result.Add(currentColumnIndex - 1);
                    result.Add(currentColumnIndex + 1);
                    break;
                case 2:
                case 5:
                case 8:
                    result.Add(currentColumnIndex - 1);
                    result.Add(currentColumnIndex - 2);
                    break;
                default:
                    break;
            }

            return result;
        }

        private List<int> GetAllNosInBox(SudokuCell cell)
        {
            List<int> result = new List<int>();
            foreach (var item in GetIndexForAllNoOfBox(cell))
            {
                if (data[item].Value != 0)
                    result.Add(data[item].Value);
            }

            return result;
        }

        private List<int> GetIndexForAllNoOfBox(SudokuCell cell)
        {
            List<int> result = new List<int>();

            var n = 0;
            if ((cell.XIndex == 0 || cell.XIndex == 1 || cell.XIndex == 2)
                && (cell.YIndex == 0 || cell.YIndex == 1 || cell.YIndex == 2))
            {
                n = 0;
            }

            if ((cell.XIndex == 0 || cell.XIndex == 1 || cell.XIndex == 2)
                && (cell.YIndex == 3 || cell.YIndex == 4 || cell.YIndex == 5))
            {
                n = 1;
            }

            if ((cell.XIndex == 0 || cell.XIndex == 1 || cell.XIndex == 2)
               && (cell.YIndex == 6 || cell.YIndex == 7 || cell.YIndex == 8))
            {
                n = 2;
            }

            if ((cell.XIndex == 3 || cell.XIndex == 4 || cell.XIndex == 5)
                && (cell.YIndex == 0 || cell.YIndex == 1 || cell.YIndex == 2))
            {
                n = 3;
            }

            if ((cell.XIndex == 3 || cell.XIndex == 4 || cell.XIndex == 5)
                && (cell.YIndex == 3 || cell.YIndex == 4 || cell.YIndex == 5))
            {
                n = 4;
            }

            if ((cell.XIndex == 3 || cell.XIndex == 4 || cell.XIndex == 5)
               && (cell.YIndex == 6 || cell.YIndex == 7 || cell.YIndex == 8))
            {
                n = 5;
            }

            if ((cell.XIndex == 6 || cell.XIndex == 7 || cell.XIndex == 8)
                && (cell.YIndex == 0 || cell.YIndex == 1 || cell.YIndex == 2))
            {
                n = 6;
            }

            if ((cell.XIndex == 6 || cell.XIndex == 7 || cell.XIndex == 8)
                && (cell.YIndex == 3 || cell.YIndex == 4 || cell.YIndex == 5))
            {
                n = 7;
            }

            if ((cell.XIndex == 6 || cell.XIndex == 7 || cell.XIndex == 8)
               && (cell.YIndex == 6 || cell.YIndex == 7 || cell.YIndex == 8))
            {
                n = 8;
            }




            switch (n)
            {
                case 0:
                    result.Add(0);
                    result.Add(1);
                    result.Add(2);
                    break;
                case 1:
                    result.Add(3);
                    result.Add(4);
                    result.Add(5);
                    break;
                case 2:
                    result.Add(6);
                    result.Add(7);
                    result.Add(8);
                    break;
                case 3:
                    result.Add(27);
                    result.Add(28);
                    result.Add(29);
                    break;
                case 4:
                    result.Add(30);
                    result.Add(31);
                    result.Add(32);
                    break;
                case 5:
                    result.Add(33);
                    result.Add(34);
                    result.Add(35);
                    break;
                case 6:
                    result.Add(54);
                    result.Add(55);
                    result.Add(56);
                    break;
                case 7:
                    result.Add(57);
                    result.Add(58);
                    result.Add(59);
                    break;
                case 8:
                    result.Add(60);
                    result.Add(61);
                    result.Add(62);
                    break;
                default:
                    break;
            }

            result.Add(result[0] + 9);
            result.Add(result[1] + 9);
            result.Add(result[2] + 9);

            result.Add(result[3] + 9);
            result.Add(result[4] + 9);
            result.Add(result[5] + 9);


            return result;
        }

    }

    public class SudokuCell
    {
        public SudokuCell(int value, int x, int y)
        {
            Value = value;
            XIndex = x;
            YIndex = y;
            if (value != 0)
            {
                IsPreFilled = true;
            }
        }

        public SudokuCell()
        {
            Value = 0;
            XIndex = -1;
            YIndex = -1;
            IsPreFilled = false;
        }
        public bool IsFilled
        {
            get
            {
                return Value > 0;
            }
        }
        public int BoxNo
        {
            get
            {
                var result = 0;
                if ("012".Contains(this.XIndex.ToString()) && "012".Contains(this.YIndex.ToString()))
                {
                    result = 0;
                }
                if ("012".Contains(this.XIndex.ToString()) && "345".Contains(this.YIndex.ToString()))
                {
                    result = 1;
                }
                if ("012".Contains(this.XIndex.ToString()) && "678".Contains(this.YIndex.ToString()))
                {
                    result = 2;
                }
                if ("345".Contains(this.XIndex.ToString()) && "012".Contains(this.YIndex.ToString()))
                {
                    result = 3;
                }
                if ("345".Contains(this.XIndex.ToString()) && "345".Contains(this.YIndex.ToString()))
                {
                    result = 4;
                }
                if ("345".Contains(this.XIndex.ToString()) && "678".Contains(this.YIndex.ToString()))
                {
                    result = 5;
                }
                if ("678".Contains(this.XIndex.ToString()) && "012".Contains(this.YIndex.ToString()))
                {
                    result = 6;
                }
                if ("678".Contains(this.XIndex.ToString()) && "345".Contains(this.YIndex.ToString()))
                {
                    result = 7;
                }
                if ("678".Contains(this.XIndex.ToString()) && "678".Contains(this.YIndex.ToString()))
                {
                    result = 8;
                }
                return result;
            }
        }
        public int Value { get; set; }
        public int XIndex { get; set; }
        public int YIndex { get; set; }
        public bool IsPreFilled { get; set; }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
