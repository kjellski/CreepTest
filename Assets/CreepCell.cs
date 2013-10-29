using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Xml.Serialization;
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class CreepCell : MonoBehaviour
{
    // Layout of a cell:
    // Cube with 8 Points for corners:
    // Directions: 
    // U = Up,   F = Forward, D = Down, 
    // L = Left, R = Right,   B = Back, 
    //
    //    4------5   0 = UBL = UpBackLeft
    //   /.  F  /|   1 = UBR = UpBackRight
    //  / . U  / |   2 = DBL = DownBackLeft
    // /R 6.../..7   3 = DBR = DownBackRight
    // 0-----1 L/    4 = UFR = UpForwardRight
    // | . D | /     5 = UFL = UpForward Left
    // |. B  |/      6 = DFR = DownForwardRight
    // 2-----3       7 = DFL = DownForwardLeft
    //
    //

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
    private float _fillingLevel = 1;

    private const int _verticesCount = _cellVerticesPerSide * _cellSideCount;
    // 3 points per vertice
    private const int _trianglesCount = _cellTrianglesPerSide * _cellSideCount;

    private readonly Vector3[] _vertices = new Vector3[_verticesCount];
    private readonly Vector3[] _normals = new Vector3[_verticesCount];
    private readonly Vector2[] _uv = new Vector2[_verticesCount];
    private readonly int[] _triangles = new int[_trianglesCount];

    public float FillingLevel
    {
        get
        {
            return _fillingLevel;
        }
        set
        {
            if (_fillingLevel == value)
                return;

            if (value > 1)
                _fillingLevel = 1;
            else if (value < 0)
                _fillingLevel = 0;
            else
                _fillingLevel = value;

            UpdateMesh();
        }
    }

    private MeshFilter _meshFilter;

    // Use this for initialization
    void Start()
    {
        UpdateMesh();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpdateMesh()
    {
        var verticesOffset = 0;
        var trianglesOffset = 0;
        // up side 
        GenerateUpSideOfMesh(verticesOffset, trianglesOffset);
        verticesOffset += _cellVerticesPerSide;
        trianglesOffset += _cellTrianglesPerSide;
        // forward side 
        GenerateForwardSideOfMesh(verticesOffset, trianglesOffset);
        verticesOffset += _cellVerticesPerSide;
        trianglesOffset += _cellTrianglesPerSide;
        // down side
        GenerateDownSideOfMesh(verticesOffset, trianglesOffset);
        verticesOffset += _cellVerticesPerSide;
        trianglesOffset += _cellTrianglesPerSide;
        // left side
        GenerateLeftSideOfMesh(verticesOffset, trianglesOffset);
        verticesOffset += _cellVerticesPerSide;
        trianglesOffset += _cellTrianglesPerSide;
        // right side
        GenerateRightSideOfMesh(verticesOffset, trianglesOffset);
        verticesOffset += _cellVerticesPerSide;
        trianglesOffset += _cellTrianglesPerSide;
        // backward side
        GenerateBackSideOfMesh(verticesOffset, trianglesOffset);

        SetMesh();
    }

    private float CalculateHighestSideEvelevation()
    {
        return _fillingLevel;
    }

    private void GenerateUpSideOfMesh(int verticesOffset, int trianglesOffset)
    {
        //Debug.Log("Generating UP with offsets: " + verticesOffset + " + " + trianglesOffset);
        _vertices[verticesOffset + 0] = new Vector3(0, _fillingLevel, 1);
        _vertices[verticesOffset + 1] = new Vector3(1, _fillingLevel, 1);

        _vertices[verticesOffset + 2] = UM;

        _vertices[verticesOffset + 3] = new Vector3(0, _fillingLevel, 0);
        _vertices[verticesOffset + 4] = new Vector3(1, _fillingLevel, 0);

        _triangles[trianglesOffset + 0] = verticesOffset + 0;
        _triangles[trianglesOffset + 1] = verticesOffset + 1;
        _triangles[trianglesOffset + 2] = verticesOffset + 2;

        _triangles[trianglesOffset + 3] = verticesOffset + 0;
        _triangles[trianglesOffset + 4] = verticesOffset + 2;
        _triangles[trianglesOffset + 5] = verticesOffset + 3;

        _triangles[trianglesOffset + 6] = verticesOffset + 2;
        _triangles[trianglesOffset + 7] = verticesOffset + 1;
        _triangles[trianglesOffset + 8] = verticesOffset + 4;

        _triangles[trianglesOffset + 9] = verticesOffset + 3;
        _triangles[trianglesOffset + 10] = verticesOffset + 2;
        _triangles[trianglesOffset + 11] = verticesOffset + 4;

        for (int i = verticesOffset; i < verticesOffset + _cellVerticesPerSide; i++)
            _uv[i] = Vector2.zero;

        for (int i = verticesOffset; i < verticesOffset + _cellVerticesPerSide; i++)
            _normals[i] = Vector3.up;
    }

    private void GenerateForwardSideOfMesh(int verticesOffset, int trianglesOffset)
    {
        //Debug.Log("Generating FORWARD with offsets: " + verticesOffset + " + " + trianglesOffset);
        _vertices[verticesOffset + 0] = new Vector3(0, _fillingLevel, 1);
        _vertices[verticesOffset + 1] = new Vector3(1, _fillingLevel, 1);

        _vertices[verticesOffset + 2] = FM;

        _vertices[verticesOffset + 3] = new Vector3(0, 0, 1);
        _vertices[verticesOffset + 4] = new Vector3(1, 0, 1);

        _triangles[trianglesOffset + 0] = verticesOffset + 2;
        _triangles[trianglesOffset + 1] = verticesOffset + 1;
        _triangles[trianglesOffset + 2] = verticesOffset + 0;

        _triangles[trianglesOffset + 3] = verticesOffset + 3;
        _triangles[trianglesOffset + 4] = verticesOffset + 2;
        _triangles[trianglesOffset + 5] = verticesOffset + 0;

        _triangles[trianglesOffset + 6] = verticesOffset + 4;
        _triangles[trianglesOffset + 7] = verticesOffset + 1;
        _triangles[trianglesOffset + 8] = verticesOffset + 3;

        _triangles[trianglesOffset + 9] = verticesOffset + 4;
        _triangles[trianglesOffset + 10] = verticesOffset + 2;
        _triangles[trianglesOffset + 11] = verticesOffset + 3;

        for (int i = verticesOffset; i < verticesOffset + _cellVerticesPerSide; i++)
            _uv[i] = Vector2.zero;

        for (int i = verticesOffset; i < verticesOffset + _cellVerticesPerSide; i++)
            _normals[i] = Vector3.forward;
    }

    private void GenerateDownSideOfMesh(int verticesOffset, int trianglesOffset)
    {
        Debug.Log("Generating DOWN with offsets: " + verticesOffset + " + " + trianglesOffset);
        // up, just flipped left to right
        _vertices[verticesOffset + 0] = new Vector3(0, 0, 1);
        _vertices[verticesOffset + 1] = new Vector3(1, 0, 1);
        _vertices[verticesOffset + 2] = DM;
        _vertices[verticesOffset + 3] = new Vector3(0, 0, 0);
        _vertices[verticesOffset + 4] = new Vector3(1, 0, 0);

        _triangles[trianglesOffset + 0] = verticesOffset + 1;
        _triangles[trianglesOffset + 1] = verticesOffset + 0;
        _triangles[trianglesOffset + 2] = verticesOffset + 2;

        _triangles[trianglesOffset + 3] = verticesOffset + 1;
        _triangles[trianglesOffset + 4] = verticesOffset + 2;
        _triangles[trianglesOffset + 5] = verticesOffset + 4;

        _triangles[trianglesOffset + 6] = verticesOffset + 2;
        _triangles[trianglesOffset + 7] = verticesOffset + 0;
        _triangles[trianglesOffset + 8] = verticesOffset + 3;

        _triangles[trianglesOffset + 9] = verticesOffset + 4;
        _triangles[trianglesOffset + 10] = verticesOffset + 2;
        _triangles[trianglesOffset + 11] = verticesOffset + 3;

        for (int i = verticesOffset; i < verticesOffset + _cellVerticesPerSide; i++)
            _uv[i] = Vector2.zero;

        for (int i = verticesOffset; i < verticesOffset + _cellVerticesPerSide; i++)
            _normals[i] = Vector3.down;
    }

    private void GenerateLeftSideOfMesh(int verticesOffset, int trianglesOffset)
    {
        //Debug.Log("Generating LEFT with offsets: " + verticesOffset + " + " + trianglesOffset);
        _vertices[verticesOffset + 0] = new Vector3(0, _fillingLevel, 1);
        _vertices[verticesOffset + 1] = new Vector3(0, _fillingLevel, 0);
        _vertices[verticesOffset + 2] = LM;
        _vertices[verticesOffset + 3] = new Vector3(0, 0, 1);
        _vertices[verticesOffset + 4] = new Vector3(0, 0, 0);

        _triangles[trianglesOffset + 0] = verticesOffset + 2;
        _triangles[trianglesOffset + 1] = verticesOffset + 0;
        _triangles[trianglesOffset + 2] = verticesOffset + 1;

        _triangles[trianglesOffset + 3] = verticesOffset + 4;
        _triangles[trianglesOffset + 4] = verticesOffset + 2;
        _triangles[trianglesOffset + 5] = verticesOffset + 1;

        _triangles[trianglesOffset + 6] = verticesOffset + 3;
        _triangles[trianglesOffset + 7] = verticesOffset + 0;
        _triangles[trianglesOffset + 8] = verticesOffset + 2;

        _triangles[trianglesOffset + 9] = verticesOffset + 3;
        _triangles[trianglesOffset + 10] = verticesOffset + 2;
        _triangles[trianglesOffset + 11] = verticesOffset + 4;

        for (int i = verticesOffset; i < verticesOffset + _cellVerticesPerSide; i++)
            _uv[i] = Vector2.zero;

        for (int i = verticesOffset; i < verticesOffset + _cellVerticesPerSide; i++)
            _normals[i] = Vector3.left;
    }

    private void GenerateRightSideOfMesh(int verticesOffset, int trianglesOffset)
    {
        //Debug.Log("Generating RIGHT with offsets: " + verticesOffset + " + " + trianglesOffset);
        _vertices[verticesOffset + 0] = new Vector3(1, _fillingLevel, 1);
        _vertices[verticesOffset + 1] = new Vector3(1, _fillingLevel, 0);
        _vertices[verticesOffset + 2] = RM;
        _vertices[verticesOffset + 3] = new Vector3(1, 0, 1);
        _vertices[verticesOffset + 4] = new Vector3(1, 0, 0);

        _triangles[trianglesOffset + 0] = verticesOffset + 1;
        _triangles[trianglesOffset + 1] = verticesOffset + 0;
        _triangles[trianglesOffset + 2] = verticesOffset + 2;

        _triangles[trianglesOffset + 3] = verticesOffset + 1;
        _triangles[trianglesOffset + 4] = verticesOffset + 2;
        _triangles[trianglesOffset + 5] = verticesOffset + 4;

        _triangles[trianglesOffset + 6] = verticesOffset + 2;
        _triangles[trianglesOffset + 7] = verticesOffset + 0;
        _triangles[trianglesOffset + 8] = verticesOffset + 3;

        _triangles[trianglesOffset + 9] = verticesOffset + 4;
        _triangles[trianglesOffset + 10] = verticesOffset + 2;
        _triangles[trianglesOffset + 11] = verticesOffset + 3;

        for (int i = verticesOffset; i < verticesOffset + _cellVerticesPerSide; i++)
            _uv[i] = Vector2.zero;

        for (int i = verticesOffset; i < verticesOffset + _cellVerticesPerSide; i++)
            _normals[i] = Vector3.right;
    }

    private void GenerateBackSideOfMesh(int verticesOffset, int trianglesOffset)
    {
        //Debug.Log("Generating BACK with offsets: " + verticesOffset + " + " + trianglesOffset);
        _vertices[verticesOffset + 0] = new Vector3(0, _fillingLevel, 0);
        _vertices[verticesOffset + 1] = new Vector3(1, _fillingLevel, 0);
        _vertices[verticesOffset + 2] = BM;
        _vertices[verticesOffset + 3] = new Vector3(0, 0, 0);
        _vertices[verticesOffset + 4] = new Vector3(1, 0, 0);

        _triangles[trianglesOffset + 0] = verticesOffset + 0;
        _triangles[trianglesOffset + 1] = verticesOffset + 1;
        _triangles[trianglesOffset + 2] = verticesOffset + 2;

        _triangles[trianglesOffset + 3] = verticesOffset + 0;
        _triangles[trianglesOffset + 4] = verticesOffset + 2;
        _triangles[trianglesOffset + 5] = verticesOffset + 3;

        _triangles[trianglesOffset + 6] = verticesOffset + 3;
        _triangles[trianglesOffset + 7] = verticesOffset + 1;
        _triangles[trianglesOffset + 8] = verticesOffset + 4;

        _triangles[trianglesOffset + 9] = verticesOffset + 3;
        _triangles[trianglesOffset + 10] = verticesOffset + 2;
        _triangles[trianglesOffset + 11] = verticesOffset + 4;

        for (int i = verticesOffset; i < verticesOffset + _cellVerticesPerSide; i++)
            _uv[i] = Vector2.zero;

        for (int i = verticesOffset; i < verticesOffset + _cellVerticesPerSide; i++)
            _normals[i] = Vector3.back;
    }

    #region MiddleProperties
    private Vector3 UM
    {
        get
        {
            return new Vector3(0.5f, _fillingLevel, 0.5f);
        }
    }

    private Vector3 FM
    {
        get
        {
            return new Vector3(0.5f, _fillingLevel / 2, 1);
        }
    }

    private Vector3 DM
    {
        get
        {
            return new Vector3(0.5f, 0, 0.5f);
        }
    }

    private Vector3 LM
    {
        get
        {
            return new Vector3(0, _fillingLevel / 2, 0.5f);
        }
    }

    private Vector3 RM
    {
        get
        {
            return new Vector3(1, _fillingLevel / 2, 0.5f);
        }
    }

    private Vector3 BM
    {
        get
        {
            return new Vector3(0.5f, _fillingLevel / 2, 0);
        }
    }

    #endregion

    #region CornerProperties

    private Vector3 UBL
    {
        get
        {
            return new Vector3();
        }
    }
    private Vector3 UBR
    {
        get
        {
            return new Vector3();
        }
    }
    private Vector3 DBL
    {
        get
        {
            return new Vector3();
        }
    }
    private Vector3 DBR
    {
        get
        {
            return new Vector3();
        }
    }
    private Vector3 UFR
    {
        get
        {
            return new Vector3();
        }
    }
    private Vector3 UFL
    {
        get
        {
            return new Vector3();
        }
    }
    private Vector3 DFR
    {
        get
        {
            return new Vector3();
        }
    }
    private Vector3 DFL
    {
        get
        {
            return new Vector3();
        }
    }

    #endregion

    private void SetMesh()
    {
        if (_meshFilter == null)
            _meshFilter = GetComponent<MeshFilter>();

        _meshFilter.mesh = new Mesh
        {
            vertices = _vertices,
            triangles = _triangles,
            normals = _normals,
            uv = _uv
        };
    }
}
