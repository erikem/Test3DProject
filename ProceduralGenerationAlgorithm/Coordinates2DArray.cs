using System;
using System.Collections.Generic;


/// <summary>
/// class that contains 2D array of floats specifically tailored for storing maps. Use this inseatd of float[,] wheverer possible
/// </summary>
public class Coordinates2DArray
{
    public static Random Rand = new Random();
    private float[,] coordinatesArray;
    public float[,] CoordinatesArray
    {
        get
        {
            return coordinatesArray;
        }
        set
        {
            coordinatesArray = value;
            nonZeroBlocks = CountBlocksNotEqualTo(0);
            oneOrHigherBlocks = CountBlocksHigherOrEqualThan(1);
        }
    }
    /// <summary>
    /// stored calcualted value of number of blocks that are not 0
    /// </summary>
    private int nonZeroBlocks = 0;
    public int NonZeroBlocks { get => nonZeroBlocks; }

    /// <summary>
    /// stored calcualted value of number of blocks that have avlue of 1 or higher
    /// </summary>
    private int oneOrHigherBlocks = 0;
    public int OneOrHigherBlocks { get => oneOrHigherBlocks; }

    /// <summary>
    /// constructore based on float[,]; deep copy by default
    /// </summary>
    public Coordinates2DArray(float[,] inputCoordinatesArray, bool shallow = false)
    {
        if (shallow)
        {
            coordinatesArray = inputCoordinatesArray;
        }
        else
        {
            coordinatesArray = new float[inputCoordinatesArray.GetLength(0), inputCoordinatesArray.GetLength(1)];
            for (int i = 0; i < this.coordinatesArray.GetLength(0); i++)
            {
                for (int j = 0; j < this.coordinatesArray.GetLength(1); j++)
                {
                    {
                        coordinatesArray[i, j] = inputCoordinatesArray[i, j];
                    }
                }
            }
        }
        nonZeroBlocks = CountBlocksNotEqualTo(0);
        oneOrHigherBlocks = CountBlocksHigherOrEqualThan(1);

    }

    /// <summary>
    /// constructore that builds empty aray size x size
    /// </summary>
    public Coordinates2DArray(int size)
    {
        coordinatesArray = new float[size, size];

    }

    /// <summary>
    /// constructore that builds empty aray sizeX x sizeY
    /// </summary>
    public Coordinates2DArray(int sizeX, int sizeY)
    {
        coordinatesArray = new float[sizeX, sizeY];
    }

    /// <summary>
    /// clone constructor; deep copy by default
    /// </summary>
    public Coordinates2DArray(Coordinates2DArray cloneable, bool shallow = false)
    {
        if (shallow)
        {
            coordinatesArray = cloneable.CoordinatesArray;
        }
        else
        {
            coordinatesArray = new float[cloneable.CoordinatesArray.GetLength(0), cloneable.CoordinatesArray.GetLength(1)];
            for (int i = 0; i < this.coordinatesArray.GetLength(0); i++)
            {
                for (int j = 0; j < this.coordinatesArray.GetLength(1); j++)
                {
                    {
                        coordinatesArray[i, j] = cloneable.CoordinatesArray[i, j];
                    }
                }
            }
        }
        nonZeroBlocks = CountBlocksNotEqualTo(0);
        oneOrHigherBlocks = CountBlocksHigherOrEqualThan(1);
    }

    /// <summary>
    /// generic function that calculates a number of blocks in a given area that match given condition. For example count of bloks that have value higher than 0,5 in a bottom right corner of 2D array
    /// </summary>
    public int CountBlocks(float cellCheckValue, string cellCheckCondition, int fromRow = 0, int toRow = -1, int fromColumn = 0, int toColumn = -1)
    {
        if (toRow == -1)
        {
            toRow = coordinatesArray.GetLength(0);
        }
        if (toColumn == -1)
        {
            toColumn = coordinatesArray.GetLength(1);
        }
        int count = 0;
        for (int i = fromRow; i < this.coordinatesArray.GetLength(0) && i <= toRow; i++)
        {
            for (int j = fromColumn; j < this.coordinatesArray.GetLength(1) && j <= toColumn; j++)
            {
                if (CheckPointCondition(new Coordinates2D(i, j), cellCheckValue, cellCheckCondition))
                {
                    count++;
                }
            }
        }
        return count;
    }

    /// <summary>
    ///counts number of all blocks in 2D array that have value higher than given one
    /// </summary>
    public int CountBlocksHigherThan(float threshold, int fromRow = 0, int toRow = -1, int fromColumn = 0, int toColumn = -1)
    {
        return CountBlocks(threshold, ">", fromRow, toRow, fromColumn, toColumn);
    }

    /// <summary>
    /// counts number of all blocks in 2D array that have value higher or equal than given one
    /// </summary>
    public int CountBlocksHigherOrEqualThan(float threshold, int fromRow = 0, int toRow = -1, int fromColumn = 0, int toColumn = -1)
    {
        return CountBlocks(threshold, ">=", fromRow, toRow, fromColumn, toColumn);
    }

    /// <summary>
    /// counts number of all blocks in 2D array that have value lower than given one
    /// </summary>
    public int CountBlocksLowerThan(float threshold, int fromRow = 0, int toRow = -1, int fromColumn = 0, int toColumn = -1)
    {
        return CountBlocks(threshold, "<", fromRow, toRow, fromColumn, toColumn);
    }

    /// <summary>
    /// counts number of all blocks in 2D array that have value lower or equal than given one
    /// </summary>
    public int CountBlocksLowerOrEqualThan(float threshold, int fromRow = 0, int toRow = -1, int fromColumn = 0, int toColumn = -1)
    {
        return CountBlocks(threshold, "<=", fromRow, toRow, fromColumn, toColumn);
    }

    /// <summary>
    /// counts number of all blocks in 2D array that have value equal to the given one
    /// </summary>
    public int CountBlocksEqualTo(float threshold, int fromRow = 0, int toRow = -1, int fromColumn = 0, int toColumn = -1)
    {
        return CountBlocks(threshold, "==", fromRow, toRow, fromColumn, toColumn);
    }

    /// <summary>
    /// counts number of all blocks in 2D array that have value not equal to the given one
    /// </summary>
    public int CountBlocksNotEqualTo(float threshold, int fromRow = 0, int toRow = -1, int fromColumn = 0, int toColumn = -1)
    {
        return CountBlocks(threshold, "!=", fromRow, toRow, fromColumn, toColumn);
    }

    /// <summary>
    /// prebuilt function based on CountBlocks() that counts number of blocks that have value of 1 or higher in square from point (inclusive) to top left corner
    /// </summary>
    public int CountOneOrHigherBlocksFromPointLessRowLessColumn(Coordinates2D point)
    {
        return CountBlocks(1, ">=", 0, point.Coordinates[0], 0, point.Coordinates[1]);
    }

    /// <summary>
    /// prebuilt function based on CountBlocks() that counts number of blocks that have value of 1 or higher in square from point (inclusive) to top right corner
    /// </summary>
    public int CountOneOrHigherBlocksFromPointLessRowMoreColumn(Coordinates2D point)
    {
        return CountBlocks(1, ">=", 0, point.Coordinates[0], point.Coordinates[1], coordinatesArray.GetLength(1) - 1);
    }

    /// <summary>
    /// prebuilt function based on CountBlocks() that counts number of blocks that have value of 1 or higher in square from point (inclusive) to bottom left corner
    /// </summary>
    public int CountOneOrHigherBlocksFromPointMoreRowLessColumn(Coordinates2D point)
    {
        return CountBlocks(1, ">=", point.Coordinates[0], coordinatesArray.GetLength(0) - 1, 0, point.Coordinates[1]);
    }

    /// <summary>
    /// prebuilt function based on CountBlocks() that counts number of blocks that have value of 1 or higher in square from point (inclusive) to bottom right corner
    /// </summary>
    public int CountOneOrHigherBlocksFromPointMoreRowMoreColumn(Coordinates2D point)
    {
        return CountBlocks(1, ">=", point.Coordinates[0], coordinatesArray.GetLength(0) - 1, point.Coordinates[1], coordinatesArray.GetLength(1) - 1);
    }

    /// <summary>
    /// prebuilt function based on CountBlocks() that counts number of blocks that have value less than 1 in square from point (inclusive) to top left corner
    /// </summary>
    public int CountLessThanOneBlocksFromPointLessRowLessColumn(Coordinates2D point)
    {
        return CountBlocks(1, "<", 0, point.Coordinates[0], 0, point.Coordinates[1]);
    }

    /// <summary>
    /// prebuilt function based on CountBlocks() that counts number of blocks that have value less than 1 in square from point (inclusive) to top right corner
    /// </summary>
    public int CountLessThanOneBlocksFromPointLessRowMoreColumn(Coordinates2D point)
    {
        return CountBlocks(1, "<", 0, point.Coordinates[0], point.Coordinates[1], coordinatesArray.GetLength(1) - 1);
    }

    /// <summary>
    /// prebuilt function based on CountBlocks() that counts number of blocks that have value less than 1 in square from point (inclusive) to bottom left corner
    /// </summary>
    public int CountLessThanOneBlocksFromPointMoreRowLessColumn(Coordinates2D point)
    {
        return CountBlocks(1, "<", point.Coordinates[0], coordinatesArray.GetLength(0) - 1, 0, point.Coordinates[1]);
    }

    /// <summary>
    /// prebuilt function based on CountBlocks() that counts number of blocks that have value less than 1 in square from point (inclusive) to bottom right corner
    /// </summary>
    public int CountLessThanOneBlocksFromPointMoreRowMoreColumn(Coordinates2D point)
    {
        return CountBlocks(1, "<", point.Coordinates[0], coordinatesArray.GetLength(0) - 1, point.Coordinates[1], coordinatesArray.GetLength(1) - 1);
    }

    /// <summary>
    /// prebuilt function based on CountBlocks() % of populated area at top left corner
    /// </summary>
    public float CountPopulatedAreaByOnesFromPointLessRowLessColumn(Coordinates2D point)
    {
        int ones = CountOneOrHigherBlocksFromPointLessRowLessColumn(point);
        int lessThanOnes = CountLessThanOneBlocksFromPointLessRowLessColumn(point);
        return (float)ones / ((float)ones + (float)lessThanOnes);
    }

    /// <summary>
    /// prebuilt function based on CountBlocks() % of populated area at top right corner
    /// </summary>
    public float CountPopulatedAreaByOnesFromPointLessRowMoreColumn(Coordinates2D point)
    {
        int ones = CountOneOrHigherBlocksFromPointLessRowMoreColumn(point);
        int lessThanOnes = CountLessThanOneBlocksFromPointLessRowMoreColumn(point);

        return (float)ones / ((float)ones + (float)lessThanOnes);
    }

    /// <summary>
    /// prebuilt function based on CountBlocks() % of populated area at bottom left corner
    /// </summary>
    public float CountPopulatedAreaByOnesFromPointMoreRowLessColumn(Coordinates2D point)
    {
        int ones = CountOneOrHigherBlocksFromPointMoreRowLessColumn(point);
        int lessThanOnes = CountLessThanOneBlocksFromPointMoreRowLessColumn(point);
        return (float)ones / ((float)ones + (float)lessThanOnes);
    }

    /// <summary>
    /// prebuilt function based on CountBlocks() % of populated area at bottom right corner
    /// </summary>
    public float CountPopulatedAreaByOnesFromPointMoreRowMoreColumn(Coordinates2D point)
    {
        int ones = CountOneOrHigherBlocksFromPointMoreRowMoreColumn(point);
        int lessThanOnes = CountLessThanOneBlocksFromPointMoreRowMoreColumn(point);
        return (float)ones / ((float)ones + (float)lessThanOnes);
    }

    /// <summary>
    /// checks if value of a given point satisfies condition
    /// </summary>
    public bool CheckPointCondition(Coordinates2D point, float pointCheckValue, string pointCheckCondition)
    {
        if (point.Row < 0
            || point.Row >= coordinatesArray.GetLength(0)
            || point.Column < 0
            || point.Column >= coordinatesArray.GetLength(1)
            || !BoolOperationFromString.Compare(coordinatesArray[point.Row, point.Column], pointCheckValue, pointCheckCondition))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// generic method to surround a block taht satisfies condition with values provided surrounding cells also satisfy given conditions. For example you want to replace all 0 around 1 with 0.5; recalculate=false can be used to skip nonZeroBlocks and oneOrHigherBlocks recalculation (use with caution); by default prcoesses 4 adjacent cells, set surroundMode="max" to process all 8 cells
    /// </summary>
    public bool SurroundBlock(Coordinates2D point, float pointCheckValue, string pointCheckCondition, float surroundCheckValue, string surroundCheckCondition, float surroundWith, bool recalculate = true, string surroundMode = "")
    {
        if (!CheckPointCondition(point, pointCheckValue, pointCheckCondition))
        {
            return false;
        }

        if (CheckPointCondition(new Coordinates2D(point.Row + 1, point.Column), surroundCheckValue, surroundCheckCondition))
        {
            coordinatesArray[point.Row + 1, point.Column] = surroundWith;
        }
        if (CheckPointCondition(new Coordinates2D(point.Row - 1, point.Column), surroundCheckValue, surroundCheckCondition))
        {
            coordinatesArray[point.Row - 1, point.Column] = surroundWith;
        }
        if (CheckPointCondition(new Coordinates2D(point.Row, point.Column + 1), surroundCheckValue, surroundCheckCondition))
        {
            coordinatesArray[point.Row, point.Column + 1] = surroundWith;
        }
        if (CheckPointCondition(new Coordinates2D(point.Row, point.Column - 1), surroundCheckValue, surroundCheckCondition))
        {
            coordinatesArray[point.Row, point.Column - 1] = surroundWith;
        }
        if (surroundMode == "max")
        {
            if (CheckPointCondition(new Coordinates2D(point.Row + 1, point.Column + 1), surroundCheckValue, surroundCheckCondition))
            {
                coordinatesArray[point.Row + 1, point.Column + 1] = surroundWith;
            }
            if (CheckPointCondition(new Coordinates2D(point.Row - 1, point.Column + 1), surroundCheckValue, surroundCheckCondition))
            {
                coordinatesArray[point.Row - 1, point.Column + 1] = surroundWith;
            }
            if (CheckPointCondition(new Coordinates2D(point.Row + 1, point.Column - 1), surroundCheckValue, surroundCheckCondition))
            {
                coordinatesArray[point.Row + 1, point.Column - 1] = surroundWith;
            }
            if (CheckPointCondition(new Coordinates2D(point.Row - 1, point.Column - 1), surroundCheckValue, surroundCheckCondition))
            {
                coordinatesArray[point.Row - 1, point.Column - 1] = surroundWith;
            }
        }

        if (recalculate)
        {
            nonZeroBlocks = CountBlocksNotEqualTo(0);
            oneOrHigherBlocks = CountBlocksHigherOrEqualThan(1);
        }
        return true;
    }

    /// <summary>
    /// predefined method that uses SurroundBlock() that goes through the whole array and changes all 0s around 1s into 0.5s
    /// </summary>
    public void SurroundOnesWithHalves()
    {
        for (int i = 0; i < this.coordinatesArray.GetLength(0); i++)
        {
            for (int j = 0; j < this.coordinatesArray.GetLength(1); j++)
            {
                {
                    SurroundBlock(new Coordinates2D(i, j), 1f, "==", 0f, "==", 0.5f, false);
                }
            }
        }
        nonZeroBlocks = CountBlocksNotEqualTo(0);
        oneOrHigherBlocks = CountBlocksHigherOrEqualThan(1);
    }

    /// <summary>
    /// prints coordinatesArray to console, skipping 0 as whitespaces
    /// </summary>
    public void PrintArray(float minValue = 0.001f, bool formatted = true)
    {
        Console.Write("   ");
        for (int i = 0; i < coordinatesArray.GetLength(1); i++)
        {
            if (formatted)
            {

                if (i >= 10)
                {
                    Console.Write(" " + i.ToString() + "  ");
                }
                else
                {
                    Console.Write(" " + i.ToString() + "   ");
                }
            }
            else
            {
                if (i >= 10)
                {
                    Console.Write(i.ToString() + " ");
                }
                else
                {
                    Console.Write(i.ToString() + "  ");
                }

            }

        }
        Console.WriteLine();

        for (int i = 0; i < coordinatesArray.GetLength(1); i++)
        {
            if (i < 10)
            {
                Console.Write(" ");
            }
            Console.Write(i.ToString() + " ");
            for (int j = 0; j < coordinatesArray.GetLength(0); j++)
            {
                if (coordinatesArray[i, j] < minValue)
                {
                    if (formatted)
                    {
                        Console.Write("     ");
                    }
                    else
                    {
                        Console.Write("   ");
                    }

                }
                else
                {
                    if (formatted)
                    {
                        Console.Write(coordinatesArray[i, j].ToString("N1") + "  ");
                    }
                    else
                    {
                        Console.Write(coordinatesArray[i, j].ToString() + "  ");
                    }

                }

            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    /// <summary>
    /// generic method for various array operations like adding 2 arrays or selecting max values from each array. Returns new Coordinates2DArray and DOES NOT change either this or input Coordinates2DArray
    /// </summary>
    public Coordinates2DArray ArrayOperation(Coordinates2DArray operand, string operation)
    {
        if (coordinatesArray.GetLength(0) != operand.CoordinatesArray.GetLength(0)
            || coordinatesArray.GetLength(1) != operand.CoordinatesArray.GetLength(1))
        {
            return null;
        }
        float[,] resultArray = new float[coordinatesArray.GetLength(0), coordinatesArray.GetLength(1)];
        for (int i = 0; i < this.coordinatesArray.GetLength(0); i++)
        {
            for (int j = 0; j < this.coordinatesArray.GetLength(1); j++)
            {
                switch (operation)
                {
                    case "+":
                        resultArray[i, j] = coordinatesArray[i, j] + operand.CoordinatesArray[i, j];
                        break;
                    case "-":
                        resultArray[i, j] = coordinatesArray[i, j] - operand.CoordinatesArray[i, j];
                        break;
                    case "==":
                        if (coordinatesArray[i, j] == operand.CoordinatesArray[i, j])
                        {
                            resultArray[i, j] = 1;
                        }
                        else
                        {
                            resultArray[i, j] = 0;
                        }
                        break;
                    case "!=":
                        if (coordinatesArray[i, j] != operand.CoordinatesArray[i, j])
                        {
                            resultArray[i, j] = 1;
                        }
                        else
                        {
                            resultArray[i, j] = 0;
                        }
                        break;
                    case ">":
                        if (coordinatesArray[i, j] > operand.CoordinatesArray[i, j])
                        {
                            resultArray[i, j] = 1;
                        }
                        else
                        {
                            resultArray[i, j] = 0;
                        }
                        break;
                    case ">=":
                        if (coordinatesArray[i, j] >= operand.CoordinatesArray[i, j])
                        {
                            resultArray[i, j] = 1;
                        }
                        else
                        {
                            resultArray[i, j] = 0;
                        }
                        break;
                    case "<":
                        if (coordinatesArray[i, j] < operand.CoordinatesArray[i, j])
                        {
                            resultArray[i, j] = 1;
                        }
                        else
                        {
                            resultArray[i, j] = 0;
                        }
                        break;
                    case "<=":
                        if (coordinatesArray[i, j] <= operand.CoordinatesArray[i, j])
                        {
                            resultArray[i, j] = 1;
                        }
                        else
                        {
                            resultArray[i, j] = 0;
                        }
                        break;
                    case "Diff":
                        resultArray[i, j] = Math.Abs(coordinatesArray[i, j] - operand.CoordinatesArray[i, j]);
                        break;
                    case "MergeMax":
                        resultArray[i, j] = Math.Max(coordinatesArray[i, j], operand.CoordinatesArray[i, j]);
                        break;
                    case "MergeMin":
                        resultArray[i, j] = Math.Min(coordinatesArray[i, j], operand.CoordinatesArray[i, j]);
                        break;
                }
            }
        }
        return new Coordinates2DArray(resultArray);
    }

    /// <summary>
    /// predefined method based on ArrayOperation() that adds all values of 2 given arrays
    /// </summary>
    public Coordinates2DArray Add(Coordinates2DArray operand)
    {
        return ArrayOperation(operand, "+");
    }

    /// <summary>
    /// predefined method based on ArrayOperation() that returns an array that has Max() of each cell of two arrays
    /// </summary>
    public Coordinates2DArray MergeMax(Coordinates2DArray operand)
    {
        return ArrayOperation(operand, "MergeMax");
    }

    /// <summary>
    /// returns a List of Coordinates2D of that match provided condition. For example a list of all cells that have value greater than 10
    /// </summary>
    public List<Coordinates2D> CellsThatMatchCondition(float cellCheckValue, string cellCheckCondition)
    {
        List<Coordinates2D> result = new List<Coordinates2D>();
        for (int i = 0; i < this.coordinatesArray.GetLength(0); i++)
        {
            for (int j = 0; j < this.coordinatesArray.GetLength(1); j++)
            {
                Coordinates2D cell = new Coordinates2D(i, j);
                if (CheckPointCondition(cell, cellCheckValue, cellCheckCondition))
                {
                    result.Add(cell);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// predefined method using CellsThatMatchCondition() that returns a List of all cells that have value=0
    /// </summary>
    public List<Coordinates2D> ListOfZeroCells()
    {
        return CellsThatMatchCondition(0, "==");
    }

    /// <summary>
    /// predefined method using CellsThatMatchCondition() that returns a List of all cells that have value=1
    /// </summary>
    public List<Coordinates2D> ListOfOneCells()
    {
        return CellsThatMatchCondition(1, "==");
    }

    /// <summary>
    /// predefined method using CellsThatMatchCondition() that returns a List of all cells that have value >=1
    /// </summary>
    public List<Coordinates2D> ListOfOneOrGreaterCells()
    {
        return CellsThatMatchCondition(1, ">=");
    }

    /// <summary>
    /// predefined method using CellsThatMatchCondition() that returns a List of all cells that have value=0.5
    /// </summary>
    public List<Coordinates2D> ListOfHalfCells()
    {
        return CellsThatMatchCondition(0.5f, "==");
    }

    /// <summary>
    /// predefined method using CellsThatMatchCondition() that returns a List of all cells that have value<=1
    /// </summary>
    public List<Coordinates2D> ListOfLessThanOneCells()
    {
        return CellsThatMatchCondition(1f, "<");
    }

    /// <summary>
    /// gets Coordinates2D of a random cell that matches condition
    /// </summary>
    public Coordinates2D RandomCellThatMatchesCondition(float cellCheckValue, string cellCheckCondition)
    {
        var list = CellsThatMatchCondition(cellCheckValue, cellCheckCondition);
        if (list.Count > 0)
        {
            return list[Rand.Next(list.Count)];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// predefined method using RandomCellThatMatchesCondition() that returns random cell that has value equal to 0
    /// </summary>
    public Coordinates2D RandomZeroCell()
    {
        return RandomCellThatMatchesCondition(0, "==");
    }

    /// <summary>
    /// predefined method using RandomCellThatMatchesCondition() that returns random cell that has value equal to 1
    /// </summary>
    public Coordinates2D RandomOneCell()
    {
        return RandomCellThatMatchesCondition(1, "==");
    }

    /// <summary>
    /// predefined method using RandomCellThatMatchesCondition() that returns random cell that has value equal to 0.5
    /// </summary>
    public Coordinates2D RandomHalfCell()
    {
        return RandomCellThatMatchesCondition(0.5f, "==");
    }
}
