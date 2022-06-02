using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;


public class MapGenerator
{
    public static Random Rand = new Random();
    private Coordinates2DArray[] layers;

    /// <summary>
    /// Layers: 0 = ground, 1 = walls, 2 = passable props, 3 = player, 4 = enemies, 5 = treasures
    /// </summary>
    public Coordinates2DArray[] Layers
    {
        get { return layers; }
    }

    /// <summary>
    /// on creation generates 6 layers (0 = ground, 1 = walls, 2 = passable props, 3 = player, 4 = enemies, 5 = treasures) of map generation accessible via Coordinates2DArray[] Layers. Uses provided settings but also has default values
    /// </summary>
    public MapGenerator(int mapSize = 10, bool shiftedAngle = false, float targetArea = 0.3f,
        float propsProbability = 0.01f, int propsMin = 5, int propsMax = 10,
        float enemiesProbability = 0.01f, int enemiesMin = 1, int enemiesMax = 2,
        float treasureProbability = 0.01f, int treasureMin = 3, int treasureMax = 5,
        int samplingCount = 3, string algorithm = "MaxFreeArea", bool newPointEverySample = false, int maxGenerationCycles = 100)
    {
        layers = new Coordinates2DArray[6];
        layers[0] = GenerateGroundMap(mapSize, targetArea, samplingCount, algorithm, newPointEverySample, maxGenerationCycles);
        layers[1] = GenerateWallsMap(layers[0], mapSize, shiftedAngle);
        layers[2] = GenerateGroundObjects(layers[0], mapSize, propsProbability, propsMin, propsMax);
        layers[3] = GenerateGroundObjects(layers[0], mapSize, 0.01f, 1, 1);
        //layers[3].ListOfOneCells().First().SurroundingCells(false, mapSize, mapSize) basically stands for all 8 cells surrounding player
        layers[4] = GenerateGroundObjects(layers[0], mapSize, enemiesProbability, enemiesMin, enemiesMax, layers[3].ListOfOneCells().First().SurroundingCells(false, mapSize, mapSize));
        layers[5] = GenerateGroundObjects(layers[0], mapSize, treasureProbability, treasureMin, treasureMax);

        //PrintMap(0.6f, false);
    }

    /// <summary>
    /// this method finds the best (most free) quadrant to populate a template from selected cell based one of available algorithms. This ensures that we don't randomly overpuplate already populated areas
    /// </summary>
    private string DetermineBestAnchorPoint(Coordinates2D point, string algorithm, Coordinates2DArray groundMap)
    {
        #region this block finds the best (most free) quadrant to populate from selected cell. This ensures that we don't randomly overpuplate already populated areas
        string bestAnchorPoint = "";
        #region MinPopulated counts te number of populated (1 and higher) blocks in zone and returns the quadrant with the smallest number
        if (algorithm == "MinPopulated")
        {
            int bestDirectionCountFullCells = Int32.MaxValue;
            int currentDirectionCountFullCells = 0;

            currentDirectionCountFullCells = groundMap.CountOneOrHigherBlocksFromPointLessRowLessColumn(point);
            //Console.WriteLine("Count of occupied cells on TOP LEFT from " + point.ToStr() + ": " + currentDirectionCountFullCells.ToString());
            if (currentDirectionCountFullCells < bestDirectionCountFullCells)
            {
                bestDirectionCountFullCells = currentDirectionCountFullCells;
                bestAnchorPoint = "BottomRight";
            }

            currentDirectionCountFullCells = groundMap.CountOneOrHigherBlocksFromPointLessRowMoreColumn(point);
            //Console.WriteLine("Count of occupied cells on TOP RIGHT from " + point.ToStr() + ": " + currentDirectionCountFullCells.ToString());
            if (currentDirectionCountFullCells < bestDirectionCountFullCells)
            {
                bestDirectionCountFullCells = currentDirectionCountFullCells;
                bestAnchorPoint = "BottomLeft";
            }

            currentDirectionCountFullCells = groundMap.CountOneOrHigherBlocksFromPointMoreRowLessColumn(point);
            //Console.WriteLine("Count of occupied cells on BOTTOM LEFT from " + point.ToStr() + ": " + currentDirectionCountFullCells.ToString());
            if (currentDirectionCountFullCells < bestDirectionCountFullCells)
            {
                bestDirectionCountFullCells = currentDirectionCountFullCells;
                bestAnchorPoint = "TopRight";
            }

            currentDirectionCountFullCells = groundMap.CountOneOrHigherBlocksFromPointMoreRowMoreColumn(point);
            //Console.WriteLine("Count of occupied cells on BOTTOM RIGHT from " + point.ToStr() + ": " + currentDirectionCountFullCells.ToString());
            if (currentDirectionCountFullCells < bestDirectionCountFullCells)
            {
                bestDirectionCountFullCells = currentDirectionCountFullCells;
                bestAnchorPoint = "TopLeft";
            }
        }
        #endregion
        #region MaxFree counts the number of "free" cells (vallue less than 1) in an area and retunrs the area with the maximum value
        if (algorithm == "MaxFree")
        {
            int bestDirectionCountFreeCells = -1;
            int currentDirectionCountFreeCells = 0;

            currentDirectionCountFreeCells = groundMap.CountLessThanOneBlocksFromPointLessRowLessColumn(point);
            //Console.WriteLine("Count of free cells on TOP LEFT from " + point.ToStr() + ": " + currentDirectionCountFreeCells.ToString());
            if (currentDirectionCountFreeCells > bestDirectionCountFreeCells)
            {
                bestDirectionCountFreeCells = currentDirectionCountFreeCells;
                bestAnchorPoint = "BottomRight";
            }

            currentDirectionCountFreeCells = groundMap.CountLessThanOneBlocksFromPointLessRowMoreColumn(point);
            //Console.WriteLine("Count of free cells on TOP RIGHT from " + point.ToStr() + ": " + currentDirectionCountFreeCells.ToString());
            if (currentDirectionCountFreeCells > bestDirectionCountFreeCells)
            {
                bestDirectionCountFreeCells = currentDirectionCountFreeCells;
                bestAnchorPoint = "BottomLeft";
            }

            currentDirectionCountFreeCells = groundMap.CountLessThanOneBlocksFromPointMoreRowLessColumn(point);
            //Console.WriteLine("Count of free cells on BOTTOM LEFT from " + point.ToStr() + ": " + currentDirectionCountFreeCells.ToString());
            if (currentDirectionCountFreeCells > bestDirectionCountFreeCells)
            {
                bestDirectionCountFreeCells = currentDirectionCountFreeCells;
                bestAnchorPoint = "TopRight";
            }

            currentDirectionCountFreeCells = groundMap.CountLessThanOneBlocksFromPointMoreRowMoreColumn(point);
            //Console.WriteLine("Count of free cells on BOTTOM RIGHT from " + point.ToStr() + ": " + currentDirectionCountFreeCells.ToString());
            if (currentDirectionCountFreeCells > bestDirectionCountFreeCells)
            {
                bestDirectionCountFreeCells = currentDirectionCountFreeCells;
                bestAnchorPoint = "TopLeft";
            }
        }
        #endregion
        #region MaxFreeArea checks the population density of areas (1 and higher/all) and returns the area with the smallest density
        if (algorithm == "MaxFreeArea")
        {
            float bestDirectionFreeArea = 1f;
            float currentDirectionFreeArea = 1f;

            currentDirectionFreeArea = groundMap.CountPopulatedAreaByOnesFromPointLessRowLessColumn(point);
            //Console.WriteLine("% of occupied area on TOP LEFT from " + point.ToStr() + ": " + currentDirectionFreeArea.ToString());
            if (currentDirectionFreeArea < bestDirectionFreeArea)
            {
                bestDirectionFreeArea = currentDirectionFreeArea;
                bestAnchorPoint = "BottomRight";
            }

            currentDirectionFreeArea = groundMap.CountPopulatedAreaByOnesFromPointLessRowMoreColumn(point);
            //Console.WriteLine("% of occupied area on TOP RIGHT from " + point.ToStr() + ": " + currentDirectionFreeArea.ToString());
            if (currentDirectionFreeArea < bestDirectionFreeArea)
            {
                bestDirectionFreeArea = currentDirectionFreeArea;
                bestAnchorPoint = "BottomLeft";
            }

            currentDirectionFreeArea = groundMap.CountPopulatedAreaByOnesFromPointMoreRowLessColumn(point);
            //Console.WriteLine("% of occupied area on BOTTOM LEFT from " + point.ToStr() + ": " + currentDirectionFreeArea.ToString());
            if (currentDirectionFreeArea < bestDirectionFreeArea)
            {
                bestDirectionFreeArea = currentDirectionFreeArea;
                bestAnchorPoint = "TopRight";
            }

            currentDirectionFreeArea = groundMap.CountPopulatedAreaByOnesFromPointMoreRowMoreColumn(point);
            //Console.WriteLine("% of occupied area on BOTTOM RIGHT from " + point.ToStr() + ": " + currentDirectionFreeArea.ToString());
            if (currentDirectionFreeArea < bestDirectionFreeArea)
            {
                bestDirectionFreeArea = currentDirectionFreeArea;
                bestAnchorPoint = "TopLeft";
            }
        }
        #endregion

        //Console.WriteLine("Best anchor point: " + bestAnchorPoint);
        #endregion
        return bestAnchorPoint;

    }

    /// <summary>
    /// generates Coordinates2DArray of size X size dimensions with 1 for ground tiles and 0.5 for wall tiles adjacent to ground
    /// </summary>
    public Coordinates2DArray GenerateGroundMap(int mapSize = 10, float targetArea = 0.3f, int samplingCount = 3, string algorithm = "MaxFreeArea", bool newPointEverySample = false, int maxGenerationCycles = 100)
    {
        Coordinates2DArray groundMap = new Coordinates2DArray(mapSize);
        //selecting the very first template
        var initialTemplate = MapTemplate.GetRandomTemplate();
        //putting this first template into teh center of the map
        groundMap = MapTemplateIntoMap(initialTemplate, new Coordinates2D(mapSize / 2, mapSize / 2), mapSize);
        //surrounding all 1 (floor) with 0.5 (wall)
        groundMap.SurroundOnesWithHalves();
        //initing counter to make sure we don't end up in infinite loop
        int countGenerationCycles = 0;
        do
        {
            //incrementing loops count
            countGenerationCycles++;
            //finding random 0.5 (wall) cell

            Coordinates2D randomHalfCell = null;
            string bestAnchorPoint = "";

            if (!newPointEverySample)
            {
                randomHalfCell = groundMap.RandomHalfCell();
                //randomHalfCell.Print();
                //Console.WriteLine("Value in cell: " + groundMap.CoordinatesArray[randomHalfCell.Row, randomHalfCell.Column]);
                bestAnchorPoint = DetermineBestAnchorPoint(randomHalfCell, algorithm, groundMap);
            }
            //initing variable for efficiency (efficiecny is the value of how big % of the template was populated into 0.5 and 0 cells)
            float addedEfficiency = 0f;
            //inititng variable for addedSample - this will be the winner sample after we do SamplingCount rounds of sampling. Winner means that this sampel has the best efficiency
            Coordinates2DArray addedSample = null;
            for (int i = 0; i < samplingCount; i++)
            {
                if (newPointEverySample)
                {
                    randomHalfCell = groundMap.RandomHalfCell();
                    //randomHalfCell.Print();
                    //Console.WriteLine("Value in cell: " + groundMap.CoordinatesArray[randomHalfCell.Row, randomHalfCell.Column]);
                    bestAnchorPoint = DetermineBestAnchorPoint(randomHalfCell, algorithm, groundMap);
                }
                //selecting random sample from the collection of samples
                var randomTemplate = MapTemplate.GetRandomTemplate();
                var mappedRandomTemplate = MapTemplateIntoMap(randomTemplate, randomHalfCell, mapSize, bestAnchorPoint);
                var currentSample = groundMap.MergeMax(mappedRandomTemplate);
                float currentEfficiency = ((float)currentSample.OneOrHigherBlocks - (float)groundMap.OneOrHigherBlocks) / (float)randomTemplate.ActiveCells;
                if (currentEfficiency > addedEfficiency)
                {
                    addedEfficiency = currentEfficiency;
                    addedSample = currentSample;
                    //mappedRandomTemplate.PrintArray(0.6f, false);
                }
            }
            if (addedSample != null)
            {
                groundMap = addedSample;
                //PrintMap();
                groundMap.SurroundOnesWithHalves();
                //PrintMap(0.6f, false);
            }
        }
        while (countGenerationCycles < maxGenerationCycles && ((float)groundMap.OneOrHigherBlocks / (float)(mapSize * mapSize) < targetArea));
        return groundMap;
    }

    /// <summary>
    /// generates Coordinates2DArray of size X size dimensions with 1 for normal full-height walls, 2 for front-screen walls (have to have transparent feeling to them, 3 for "internal walls" that will be pools/pits, 4 for "internal walls" that can be considered for pillars
    /// </summary>
    public Coordinates2DArray GenerateWallsMap(Coordinates2DArray groundMap, int mapSize, bool shiftedAngle)
    {
        var wallsMapArray = new float[mapSize, mapSize];
        var nonGroundCells = groundMap.ListOfLessThanOneCells();

        foreach (var cell in nonGroundCells)
        {
            int countGroundRowsAbove = groundMap.CountBlocks(1, ">=", 0, cell.Row, cell.Column, cell.Column);
            int countGroundRowsBelow = groundMap.CountBlocks(1, ">=", cell.Row, mapSize - 1, cell.Column, cell.Column);
            int countGroundColumnsLeft = groundMap.CountBlocks(1, ">=", cell.Row, cell.Row, 0, cell.Column);
            int countGroundColumnsRight = groundMap.CountBlocks(1, ">=", cell.Row, cell.Row, cell.Column, mapSize - 1);
            int countGroundRowsAboveColumnsLeftExcludingCurrent = groundMap.CountBlocks(1, ">=", 0, cell.Row - 1, 0, cell.Column - 1);

            int groundAdjacent = 0;
            groundAdjacent += groundMap.CountBlocks(1, ">=", cell.Row + 1, cell.Row + 1, cell.Column, cell.Column);
            groundAdjacent += groundMap.CountBlocks(1, ">=", cell.Row - 1, cell.Row - 1, cell.Column, cell.Column);
            groundAdjacent += groundMap.CountBlocks(1, ">=", cell.Row, cell.Row, cell.Column + 1, cell.Column + 1);
            groundAdjacent += groundMap.CountBlocks(1, ">=", cell.Row, cell.Row, cell.Column - 1, cell.Column - 1);
            if (shiftedAngle)
            {
                if (countGroundRowsAboveColumnsLeftExcludingCurrent == 0)
                {
                    wallsMapArray[cell.Row, cell.Column] = 1;
                }
                else if (countGroundRowsBelow == 0 || countGroundColumnsRight == 0)
                {
                    wallsMapArray[cell.Row, cell.Column] = 2;
                }
                else if (groundAdjacent >= 2)
                {
                    wallsMapArray[cell.Row, cell.Column] = 4;
                }
                else
                {
                    wallsMapArray[cell.Row, cell.Column] = 3;
                }
            }
            else
            {
                if (countGroundRowsAbove == 0)
                {
                    wallsMapArray[cell.Row, cell.Column] = 1;
                }
                else if (countGroundRowsBelow == 0)
                {
                    wallsMapArray[cell.Row, cell.Column] = 2;
                }
                else if (groundAdjacent >= 2)
                {
                    wallsMapArray[cell.Row, cell.Column] = 4;
                }
                else
                {
                    wallsMapArray[cell.Row, cell.Column] = 3;
                }
            }

        }
        return new Coordinates2DArray(wallsMapArray);
    }

    /// <summary>
    /// generates Coordinates2DArray of size X size dimensions with 1 for generated groudn objects (e.g. props, enemies, treasures, traps, player, etc.) ensuring amount of objects between min and max (both inclusive). Skips any cells added to restrictedPoints List
    /// </summary>
    public Coordinates2DArray GenerateGroundObjects(Coordinates2DArray groundMap, int mapSize, float probability, int minAmount, int maxAmount, List<Coordinates2D> restrictedPoints = null)
    {
        var groundObjectsArray = new float[mapSize, mapSize];
        var groundCells = groundMap.ListOfOneOrGreaterCells();
        maxAmount = Math.Min(maxAmount, groundCells.Count);
        minAmount = Math.Min(Math.Max(minAmount, 0), maxAmount);
        int targetAmount = Rand.Next(minAmount, maxAmount + 1);
        int insertedObjects = 0;
        int maxLoops = 1000;
        int currentLoop = 0;
        while (insertedObjects < targetAmount && currentLoop < maxLoops)
        {
            currentLoop++;
            foreach (var cell in groundCells)
            {
                if (Rand.NextDouble() < probability)
                {
                    if (restrictedPoints != null && restrictedPoints.Any(x => x.Row == cell.Row && x.Column == cell.Column))
                    {
                        continue;
                    }
                    groundObjectsArray[cell.Row, cell.Column] = 1;
                    insertedObjects++;
                }
                if (insertedObjects >= targetAmount)
                {
                    break;
                }
            }
        }
        return new Coordinates2DArray(groundObjectsArray);
    }

    /// <summary>
    /// places selected MapTemplate into Coordinates2DArray that matches the size of the map at insertionPoint mathcing coordinates of insertionPoint with coordinates of templateAnchorPoint
    /// </summary>
    public Coordinates2DArray MapTemplateIntoMap(MapTemplate template, Coordinates2D insertionPoint, int mapSize, string templateAnchorPoint = "TopLeft", Coordinates2D customAnchorPoint = null)
    {
        var templateInContext = template.PutIntoContext(mapSize, insertionPoint, templateAnchorPoint, customAnchorPoint);
        //templateInContext.PrintArray();
        return templateInContext;
    }

    public void PrintMap(float minValue = 0.001f, bool formatted = true)
    {
        Console.WriteLine("Ground Layer:");
        if (layers[0] != null)
        {
            layers[0].PrintArray(minValue, formatted);
        }
        Console.WriteLine("Walls Layer:");
        if (layers[1] != null)
        {
            layers[1].PrintArray(minValue, formatted);
        }
        Console.WriteLine("Passable props Layer:");
        if (layers[2] != null)
        {
            layers[2].PrintArray(minValue, formatted);
        }
        Console.WriteLine("Player Layer:");
        if (layers[3] != null)
        {
            layers[3].PrintArray(minValue, formatted);
        }
        Console.WriteLine("Enemies Layer:");
        if (layers[4] != null)
        {
            layers[4].PrintArray(minValue, formatted);
        }
        Console.WriteLine("Treasure Layer:");
        if (layers[5] != null)
        {
            layers[5].PrintArray(minValue, formatted);
        }

    }

}
