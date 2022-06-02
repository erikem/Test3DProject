using System;
using System.Collections.Generic;



/// <summary>
/// class stores predifined templates that can be used during generation of map. They can be of any size but squares are the best. For proper functioning you need to specify coordinates of all corners for every template. This will ensure that templates are properly connected and that there are no gaps
/// </summary>
public class MapTemplate
{
    public static List<MapTemplate> AllTemplates = new List<MapTemplate>();
    private Coordinates2DArray template;
    public Coordinates2DArray Template
    {
        get { return template; }
    }
    //think in rows and columns instead of x and y coordinates here. So horizontal is actually columns and vertical is rows
    public Coordinates2D TopLeft;
    public Coordinates2D TopRight;
    public Coordinates2D BottomLeft;
    public Coordinates2D BottomRight;
    public int ActiveCells;
    /// <summary>
    /// just your averagre constructor. Resulting array is a deep copy of the one provided in comstructor
    /// </summary>
    public MapTemplate(float[,] templateArray, Coordinates2D topLeft, Coordinates2D topRight, Coordinates2D bottomLeft, Coordinates2D bottomRight)
    {
        this.template = new Coordinates2DArray(templateArray) ?? throw new ArgumentNullException(nameof(templateArray));
        ActiveCells = template.NonZeroBlocks;
        TopLeft = topLeft ?? new Coordinates2D();
        TopRight = topRight ?? new Coordinates2D();
        BottomLeft = bottomLeft ?? new Coordinates2D();
        BottomRight = bottomRight ?? new Coordinates2D();
    }

    /// <summary>
    /// static method that creates a new template and adds it to a static List of templates AllTemplates
    /// </summary>
    public static MapTemplate AddTemplate(float[,] templateArray, Coordinates2D topLeft, Coordinates2D topRight, Coordinates2D bottomLeft, Coordinates2D bottomRight)
    {
        var mapTemplate = new MapTemplate(templateArray, topLeft, topRight, bottomLeft, bottomRight);
        AllTemplates.Add(mapTemplate);
        return mapTemplate;
    }

    /// <summary>
    /// translates this template onto a bigger space (space of whole map presumably) into provided coordinate using anchor point (anchor point will be places in provided coordinate). Anchor point should be one of predifined corners ("TopLeft", "TopRight", "BottomLeft" or "BottomRight") but can be set to any point if you want some chaos
    /// </summary>
    public Coordinates2DArray PutIntoContext(int size, Coordinates2D insertionPoint, string templateAnchorPoint = "TopLeft", Coordinates2D customAnchorPoint = null)
    {
        if (customAnchorPoint == null)
        {
            if (templateAnchorPoint == "TopLeft")
            {
                customAnchorPoint = TopLeft;
            }
            else if (templateAnchorPoint == "TopRight")
            {
                customAnchorPoint = TopRight;
            }
            else if (templateAnchorPoint == "BottomLeft")
            {
                customAnchorPoint = BottomLeft;
            }
            else if (templateAnchorPoint == "BottomRight")
            {
                customAnchorPoint = BottomRight;
            }
        }
        if (customAnchorPoint == null)
        {
            return null;
        }

        float[,] templateArray = new float[size, size];
        int startX = insertionPoint.Coordinates[0] - customAnchorPoint.Coordinates[0];
        int startY = insertionPoint.Coordinates[1] - customAnchorPoint.Coordinates[1];
        for (int i = 0; i < this.template.CoordinatesArray.GetLength(0); i++)
        {
            for (int j = 0; j < this.template.CoordinatesArray.GetLength(1); j++)
            {
                int insertionCoordinateX = startX + i;
                int insertionCoordinateY = startY + j;
                if (insertionCoordinateX >= 0
                    && insertionCoordinateX < size
                    && insertionCoordinateY >= 0
                    && insertionCoordinateY < size
                    )
                {
                    templateArray[insertionCoordinateX, insertionCoordinateY] = this.template.CoordinatesArray[i, j];
                }
            }
        }
        return new Coordinates2DArray(templateArray);
    }

    public static MapTemplate GetRandomTemplate()
    {
        if (AllTemplates.Count > 0)
        {
            return AllTemplates[Coordinates2DArray.Rand.Next(AllTemplates.Count)];
        }
        else
        {
            return null;
        }
    }
}
