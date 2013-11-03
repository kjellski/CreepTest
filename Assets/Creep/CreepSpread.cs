using System;
using UnityEngine;

public class CreepSpread
{

    public int ZSize
    {
        get;
        private set;
    }

    public int XSize
    {
        get;
        private set;
    }

    public IFillable[,] Field { get; private set; }

    public CreepSpread(int xSize, int zSize, IFillable[,] field)
    {
        XSize = xSize;
        ZSize = zSize;
        Field = field;
    }

    /// <summary>
    /// Spreads from the given (x,z) position to size fields in a circle. Adds "level" to fields hit by spread.
    /// </summary>
    /// <remarks>
    /// In a 3x3 field, given size = 1, x = 1, z = 1, level = 0.2, these fields would
    /// get +0.2 to their filling: [(1,0), (0,1), (1,1), (2,1), (1,2)] = 
    /// 0    +.2  0
    /// +.2  +.2  +.2
    /// 0    +.2  0 
    /// </remarks>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="size"></param>
    /// <param name="level"></param>
    public void SpreadFrom(int xpos, int zpos, int size, float level)
    {
        if (xpos < 0 || xpos >= XSize) throw new ArgumentOutOfRangeException();
        if (zpos < 0 || zpos >= ZSize) throw new ArgumentOutOfRangeException();
        if (size < 0) throw new ArgumentOutOfRangeException();

        var maxValue = Mathf.CeilToInt(size / 2);
        for (int z = -size; z <= +size; z++)
        {
            for (int x = -size; x <= +size; x++)
            {
                var absX = x + xpos;
                var absZ = z + zpos;
                if (absX < 0 || absZ < 0 || absX >= XSize || absZ >= ZSize)
                    continue;

                if (Mathf.Abs(x) == size && z == 0)
                    Field[absX, absZ].FillingHeight += level;
                if (Mathf.Abs(z) == size && x == 0)
                    Field[absX, absZ].FillingHeight += level;

                if (Mathf.Abs(x) <= maxValue && Mathf.Abs(z) <= maxValue)
                    Field[absX, absZ].FillingHeight += level;
            }
        }
    }
}

