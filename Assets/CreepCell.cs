using System;
using UnityEngine;

// Layout of a cell:
// Cube with 8 Points for corners:
// Directions: 
// U = Up,   F = Forward, D = Down, 
// L = Left, R = Right,   B = Back, 
// M = Middle
//    0------1  UpForwardRight   = UFR = 0  
//   /.  F  /|  UpForward Left   = UFL = 1  
//  / . U  / |  UpBackLeft       = UBL = 2  
// /R 4.../..5  UpBackRight      = UBR = 3  
// 2-----3 L/   DownForwardRight = DFR = 4  
// | . D | /    DownForwardLeft  = DFL = 5  
// |. B  |/     DownBackLeft     = DBL = 6  
// 6-----7      DownBackRight    = DBR = 7  
[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class CreepCell : MonoBehaviour, IFillable
{
    public static float Height
    {
        get
        {
            return 1.0f;
        }
    }

    enum PIC // PointInCell
    {
        UBL = 0,
        UBR = 1,
        DBL = 2,
        DBR = 3,
        UFL = 4,
        UFR = 5,
        DFL = 6,
        DFR = 7,
        UM = 8,
        FM = 9,
        DM = 10,
        LM = 11,
        RM = 12,
        BM = 13
    }

    // 6 sides 
    private const int _cellSideCount = 6;
    // each side like this: 
    // 5 vertices
    // 0-----1
    // | \ / |
    // |  n  |  n = after all corners, in UFBLRD order
    // | / \ |
    // 3-----4
    private const int _cellVerticesPerSide = 5;
    // 4 triangles
    // /-----\
    // | \0/ |
    // |1 X 2|
    // | /3\ |
    // \-----/
    private const int _cellTrianglesPerSide = 4 * 3;
    // With filling, the 2 will rise first 
    private float _fillingHeight = 1;

    private const int _verticesCount = 14;
    // 3 points per vertice
    private const int _trianglesCount = _cellTrianglesPerSide * _cellSideCount;

    private readonly Vector3[] _vertices = new Vector3[_verticesCount];
    private readonly Vector3[] _normals = new Vector3[_verticesCount];
    private readonly Vector2[] _uv = new Vector2[_verticesCount];
    private readonly int[] _triangles = new int[_trianglesCount];

    public float FillingHeight
    {
        get
        {
            return _fillingHeight;
        }
        set
        {
            if (_fillingHeight == value)
                return;

            if (value > 1)
                _fillingHeight = 1;
            else if (value < 0)
                _fillingHeight = 0;
            else
                _fillingHeight = value;

            UpdateMesh();
        }
    }

    // Use this for initialization
    void Start()
    {
        UpdateMesh();
    }

    private Boolean _updateDirection = true;
    // Update is called once per frame
    void Update()
    {
        if (_updateDirection)
        {
            if (_fillingHeight < 1.0f)
                _fillingHeight += 0.01f;

            if (_fillingHeight >= 0.98f)
                _updateDirection = !_updateDirection;
        }
        else
        {
            if (_fillingHeight > 0.0f)
                _fillingHeight -= 0.01f;

            if (_fillingHeight <= 0.02f)
                _updateDirection = !_updateDirection;
        }
        UpdateMesh();
    }

    /// <summary>
    /// Creates the initial Mesh, should later set the 
    /// fixed points and then exclude them from calculation
    /// </summary>
    void InitializeMesh()
    {
        UpdateMesh();
    }

    public void UpdateMesh()
    {
        _vertices[(int)PIC.UBL] = P(PIC.UBL);
        _vertices[(int)PIC.UBR] = P(PIC.UBR);
        _vertices[(int)PIC.DBL] = P(PIC.DBL);
        _vertices[(int)PIC.DBR] = P(PIC.DBR);

        _vertices[(int)PIC.UFL] = P(PIC.UFL);
        _vertices[(int)PIC.UFR] = P(PIC.UFR);
        _vertices[(int)PIC.DFL] = P(PIC.DFL);
        _vertices[(int)PIC.DFR] = P(PIC.DFR);

        _vertices[(int)PIC.UM] = P(PIC.UM);
        _vertices[(int)PIC.FM] = P(PIC.FM);
        _vertices[(int)PIC.DM] = P(PIC.DM);
        _vertices[(int)PIC.LM] = P(PIC.LM);
        _vertices[(int)PIC.RM] = P(PIC.RM);
        _vertices[(int)PIC.BM] = P(PIC.BM);

        _normals[(int)PIC.UBL] = new Vector3(-1, 1, -1);
        _normals[(int)PIC.UBR] = new Vector3(1, 1, -1);
        _normals[(int)PIC.DBL] = new Vector3(-1, -1, -1);
        _normals[(int)PIC.DBR] = new Vector3(1, -1, -1);

        _normals[(int)PIC.UFL] = new Vector3(-1, 1, 1);
        _normals[(int)PIC.UFR] = new Vector3(1, 1, 1);
        _normals[(int)PIC.DFL] = new Vector3(-1, -1, 1);
        _normals[(int)PIC.DFR] = new Vector3(1, -1, 1);

        _normals[(int)PIC.UM] = Vector3.up;
        _normals[(int)PIC.FM] = Vector3.forward;
        _normals[(int)PIC.DM] = Vector3.down;
        _normals[(int)PIC.LM] = Vector3.left;
        _normals[(int)PIC.RM] = Vector3.right;
        _normals[(int)PIC.BM] = Vector3.back;

        // zero out the material
        for (int i = 0; i < _verticesCount; i++)
            _uv[i] = Vector2.zero;

        var triangleOffset = 0;
        SetTrianglesForSide(triangleOffset, (int)PIC.UFL, (int)PIC.UFR, (int)PIC.UM, (int)PIC.UBL, (int)PIC.UBR); // UP
        triangleOffset += _cellTrianglesPerSide;
        SetTrianglesForSide(triangleOffset, (int)PIC.UFR, (int)PIC.UFL, (int)PIC.FM, (int)PIC.DFR, (int)PIC.DFL); // FORWARD
        triangleOffset += _cellTrianglesPerSide;
        SetTrianglesForSide(triangleOffset, (int)PIC.DFR, (int)PIC.DFL, (int)PIC.DM, (int)PIC.DBR, (int)PIC.DBL); // DOWN
        triangleOffset += _cellTrianglesPerSide;
        SetTrianglesForSide(triangleOffset, (int)PIC.UFL, (int)PIC.UBL, (int)PIC.LM, (int)PIC.DFL, (int)PIC.DBL); // LEFT
        triangleOffset += _cellTrianglesPerSide;
        SetTrianglesForSide(triangleOffset, (int)PIC.UBR, (int)PIC.UFR, (int)PIC.RM, (int)PIC.DBR, (int)PIC.DFR); // RIGHT
        triangleOffset += _cellTrianglesPerSide;
        SetTrianglesForSide(triangleOffset, (int)PIC.UBL, (int)PIC.UBR, (int)PIC.BM, (int)PIC.DBL, (int)PIC.DBR); // BACK

        SetMesh();
    }

    /// <summary>
    /// This method, sets all the triangles between the parameters to generate 4 triangles in total like so:
    /// UL---UR
    /// |\   /|
    /// | \0/ |
    /// |1 M 2|
    /// | /3\ |
    /// |/   \|
    /// DL---DR
    /// </summary>
    /// <param name="trianglesOffset">offset from where on to set the triangles in <see cref="_triangles"/></param>
    /// <param name="UL">Upper Left corner of the side</param>
    /// <param name="UR">Upper Right corner of the side</param>
    /// <param name="M">Middle of the side</param>
    /// <param name="DL">Downer Left corner of the side</param>
    /// <param name="DR">Downer Right corner of the side</param>
    private void SetTrianglesForSide(int trianglesOffset, int UL, int UR, int M, int DL, int DR)
    {
        // upper
        _triangles[trianglesOffset + 0] = UL;
        _triangles[trianglesOffset + 1] = UR;
        _triangles[trianglesOffset + 2] = M;
        // left
        _triangles[trianglesOffset + 3] = UL;
        _triangles[trianglesOffset + 4] = M;
        _triangles[trianglesOffset + 5] = DL;
        // right
        _triangles[trianglesOffset + 6] = M;
        _triangles[trianglesOffset + 7] = UR;
        _triangles[trianglesOffset + 8] = DR;
        // lower
        _triangles[trianglesOffset + 9] = DL;
        _triangles[trianglesOffset + 10] = M;
        _triangles[trianglesOffset + 11] = DR;
    }

    private Vector3 P(PIC p)
    {
        var firstHalfOfCircle = Mathf.Sin(_fillingHeight * Mathf.PI) / 3;

        switch (p)
        {
            ///* Up 4 corners*/
            case PIC.UFR: return new Vector3(1 - firstHalfOfCircle, FillingHeight, 1 - firstHalfOfCircle);
            case PIC.UFL: return new Vector3(0 + firstHalfOfCircle, FillingHeight, 1 - firstHalfOfCircle);
            case PIC.UBL: return new Vector3(0 + firstHalfOfCircle, FillingHeight, 0 + firstHalfOfCircle);
            case PIC.UBR: return new Vector3(1 - firstHalfOfCircle, FillingHeight, 0 + firstHalfOfCircle);
            /* Down 4 Corners*/
            case PIC.DFR: return new Vector3(1, 0, 1);
            case PIC.DFL: return new Vector3(0, 0, 1);
            case PIC.DBL: return new Vector3(0, 0, 0);
            case PIC.DBR: return new Vector3(1, 0, 0);
            /* Middles */
            case PIC.UM: return new Vector3(0.5f, FillingHeight, 0.5f);
            case PIC.FM: return new Vector3(0.5f, FillingHeight / 2, 1.0f);
            case PIC.DM: return new Vector3(0.5f, 0, 0.5f);
            case PIC.LM: return new Vector3(0, FillingHeight / 2, 0.5f);
            case PIC.RM: return new Vector3(1, FillingHeight / 2, 0.5f);
            case PIC.BM: return new Vector3(0.5f, FillingHeight / 2, 0);
            default:
                throw new ArgumentOutOfRangeException("p");
        }
    }

    private void SetMesh()
    {


        GetComponent<MeshFilter>().mesh = new Mesh
        {
            vertices = _vertices,
            triangles = _triangles,
            normals = _normals,
            uv = _uv
        };
    }
}
