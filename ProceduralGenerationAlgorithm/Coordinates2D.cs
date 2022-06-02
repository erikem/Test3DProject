using System;
using System.Collections.Generic;

/// <summary>
/// kind of basic Vector2 class that simply stores 2 coordinates
/// </summary>
public class Coordinates2D
{
    public int[] Coordinates = new int[2];
    public int Row
    {
        get
        {
            return Coordinates[0];
        }
    }
    public int Column
    {
        get
        {
            return Coordinates[1];
        }
    }
    public void Print()
    {
        Console.WriteLine(ToStr());
    }

    public string ToStr()
    {
        return "(Row: " + Row + ", Column: " + Column + ")";
    }

    public Coordinates2D(int row, int column)
    {
        this.Coordinates[0] = row;
        this.Coordinates[1] = column;
    }
    public Coordinates2D()
    {
        this.Coordinates[0] = 0;
        this.Coordinates[1] = 0;
    }
    public Coordinates2D(Coordinates2D clonable)
    {
        this.Coordinates[0] = clonable.Coordinates[0];
        this.Coordinates[1] = clonable.Coordinates[1];
    }

    public Coordinates2D FixCell(Coordinates2D cell, int arraySizeRows = 0, int arraySizeColumns = 0)
    {
        Coordinates2D newCell = new Coordinates2D(cell);
        if (newCell.Row < 0)
        {
            newCell.Coordinates[0] = 0;
        }
        if (newCell.Column < 0)
        {
            newCell.Coordinates[1] = 0;
        }
        if (arraySizeRows > 0 && newCell.Row >= arraySizeRows)
        {
            newCell.Coordinates[0] = arraySizeRows - 1;
        }
        if (arraySizeColumns > 0 && newCell.Column >= arraySizeColumns)
        {
            newCell.Coordinates[1] = arraySizeColumns - 1;
        }
        return newCell;
    }

    public List<Coordinates2D> SurroundingCells(bool min = true, int arraySizeRows = 0, int arraySizeColumns = 0)
    {
        List<Coordinates2D> surroundingCells = new List<Coordinates2D>();

        surroundingCells.Add(FixCell(new Coordinates2D(Row + 1, Column), arraySizeRows, arraySizeColumns));
        surroundingCells.Add(FixCell(new Coordinates2D(Row - 1, Column), arraySizeRows, arraySizeColumns));
        surroundingCells.Add(FixCell(new Coordinates2D(Row, Column + 1), arraySizeRows, arraySizeColumns));
        surroundingCells.Add(FixCell(new Coordinates2D(Row, Column - 1), arraySizeRows, arraySizeColumns));

        if (!min)
        {
            surroundingCells.Add(FixCell(new Coordinates2D(Row + 1, Column + 1), arraySizeRows, arraySizeColumns));
            surroundingCells.Add(FixCell(new Coordinates2D(Row - 1, Column - 1), arraySizeRows, arraySizeColumns));
            surroundingCells.Add(FixCell(new Coordinates2D(Row - 1, Column + 1), arraySizeRows, arraySizeColumns));
            surroundingCells.Add(FixCell(new Coordinates2D(Row + 1, Column - 1), arraySizeRows, arraySizeColumns));
        }

        return surroundingCells;
    }

    /*public static bool operator ==(Coordinates2D obj1, Coordinates2D obj2)
    {
        if (obj1.Row == obj2.Row
            && obj1.Column == obj2.Column)
        {
            return true;
        }

        return false;
    }

    public static bool operator !=(Coordinates2D obj1, Coordinates2D obj2)
    {
        if (obj1.Row == obj2.Row
            && obj1.Column == obj2.Column)
        {
            return false;
        }
        return true;
    }*/
}