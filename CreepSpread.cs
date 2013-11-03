
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class CreepSpread<T> where T : IFillable
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

    public T[,] Field { get; private set; }

    public CreepSpread(int xSize, int zSize, T[,] field)
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
    public void SpreadFrom(int x, int z, int size, float level)
    {
        if (x < 0 || x >= XSize) throw new ArgumentOutOfRangeException();
        if (z < 0 || z >= ZSize) throw new ArgumentOutOfRangeException();
        if (size < 0) throw new ArgumentOutOfRangeException();

        var bla = IsSpreadPositions(size);

        //for (int s = -size; s <= size; s++)
        //{
        //    var xBoundary = x - s;
        //    var zBoundary = z - s;

        //    // go from min of pos - size
        //    for (int zd = 0; zd < z - size; zd++)
        //    {
        //        for (int xd = 0; xd < x - size; xd++)
        //        {
        //            var xpos = x - Mathf.Abs(s);
        //            var zpos = x - Mathf.Abs(s);

        //            if (xd - xpos == 0 && zd - zpos == 0)
        //            {
        //                Field[x, z].FillingHeight = level;
        //            }
        //        }
        //    }
        //}
    }

    private List<Pos> IsSpreadPositions(int size)
    {
        var resultSize = 2*size + 1;
        var result = new List<Pos>();

        var maxValue = Mathf.CeilToInt(size/2);

        for (int z = -size; z <= +size; z++)
        {
            for (int x = -size; x <= +size; x++)
            {
                if (Mathf.Abs(x) == size && z == 0)
                    result.Add(new Pos(x,z));
                if (Mathf.Abs(z) == size && x == 0)
                    result.Add(new Pos(x, z));

                if (Mathf.Abs(x) <= maxValue && Mathf.Abs(z) <= maxValue)
                    result.Add(new Pos(x, z));
            }
        }
        return result;
    }

    public class Pos
    {
        public Pos(int x, int z)
        {
            Z = z;
            X = x;
        }

        public int X { get; private set; }
        public int Z { get; private set; }
    }
}

