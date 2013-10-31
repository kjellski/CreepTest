using System;
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
    // M = Middle
    //    0------1  UpForwardRight   = UFR = 0  
    //   /.  F  /|  UpForward Left   = UFL = 1  
    //  / . U  / |  UpBackLeft       = UBL = 2  
    // /R 4.../..5  UpBackRight      = UBR = 3  
    // 2-----3 L/   DownForwardRight = DFR = 4  
    // | . D | /    DownForwardLeft  = DFL = 5  
    // |. B  |/     DownBackLeft     = DBL = 6  
    // 6-----7      DownBackRight    = DBR = 7  
    //

    enum PIC // PointInCell
    {
        UFR = 0,
        UFL = 1,
        UBL = 2,
        UBR = 3,
        DFR = 4,
        DFL = 5,
        DBL = 6,
        DBR = 7,
        UM = 9,
        FM = 10,
        DM = 11,
        LM = 12,
        RM = 13,
        BM = 14
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

    private Boolean _updateDirection = true;
    // Update is called once per frame
    void Update()
    {
        if (_updateDirection)
        {
            if (_fillingLevel < 1.0f)
                _fillingLevel += 0.01f;

            if (_fillingLevel >= 0.98f)
                _updateDirection = !_updateDirection;
        }
        else
        {
            if (_fillingLevel > 0.0f)
                _fillingLevel -= 0.01f;

            if (_fillingLevel <= 0.02f)
                _updateDirection = !_updateDirection;
        }
        UpdateMesh();

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
        _vertices[verticesOffset + 0] = P(PIC.UFL);
        _vertices[verticesOffset + 1] = P(PIC.UFR);

        _vertices[verticesOffset + 2] = P(PIC.UM);

        _vertices[verticesOffset + 3] = P(PIC.UBL);
        _vertices[verticesOffset + 4] = P(PIC.UBR);

        SetTrianglesForSide(verticesOffset, trianglesOffset);

        for (int i = verticesOffset; i < verticesOffset + _cellVerticesPerSide; i++)
            _uv[i] = Vector2.zero;

        for (int i = verticesOffset; i < verticesOffset + _cellVerticesPerSide; i++)
            _normals[i] = Vector3.up;
    }

    private void GenerateForwardSideOfMesh(int verticesOffset, int trianglesOffset)
    {
        //Debug.Log("Generating FORWARD with offsets: " + verticesOffset + " + " + trianglesOffset);
        _vertices[verticesOffset + 0] = P(PIC.UFR);
        _vertices[verticesOffset + 1] = P(PIC.UFL);

        _vertices[verticesOffset + 2] = P(PIC.FM);

        _vertices[verticesOffset + 3] = P(PIC.DFR);
        _vertices[verticesOffset + 4] = P(PIC.DFL);

        SetTrianglesForSide(verticesOffset, trianglesOffset);

        for (int i = verticesOffset; i < verticesOffset + _cellVerticesPerSide; i++)
            _uv[i] = Vector2.zero;

        for (int i = verticesOffset; i < verticesOffset + _cellVerticesPerSide; i++)
            _normals[i] = Vector3.forward;
    }

    private void GenerateDownSideOfMesh(int verticesOffset, int trianglesOffset)
    {
        //Debug.Log("Generating DOWN with offsets: " + verticesOffset + " + " + trianglesOffset);
        // up, just flipped left to right
        _vertices[verticesOffset + 0] = P(PIC.DFR);
        _vertices[verticesOffset + 1] = P(PIC.DFL);
        _vertices[verticesOffset + 2] = P(PIC.DM);
        _vertices[verticesOffset + 3] = P(PIC.DBR);
        _vertices[verticesOffset + 4] = P(PIC.DBL);

        SetTrianglesForSide(verticesOffset, trianglesOffset);

        for (int i = verticesOffset; i < verticesOffset + _cellVerticesPerSide; i++)
            _uv[i] = Vector2.zero;

        for (int i = verticesOffset; i < verticesOffset + _cellVerticesPerSide; i++)
            _normals[i] = Vector3.down;
    }

    private void GenerateLeftSideOfMesh(int verticesOffset, int trianglesOffset)
    {
        //Debug.Log("Generating LEFT with offsets: " + verticesOffset + " + " + trianglesOffset);
        _vertices[verticesOffset + 0] = P(PIC.UFL);
        _vertices[verticesOffset + 1] = P(PIC.UBL);
        _vertices[verticesOffset + 2] = P(PIC.LM);
        _vertices[verticesOffset + 3] = P(PIC.DFL);
        _vertices[verticesOffset + 4] = P(PIC.DBL);

        SetTrianglesForSide(verticesOffset, trianglesOffset);

        for (int i = verticesOffset; i < verticesOffset + _cellVerticesPerSide; i++)
            _uv[i] = Vector2.zero;

        for (int i = verticesOffset; i < verticesOffset + _cellVerticesPerSide; i++)
            _normals[i] = Vector3.left;
    }

    private void GenerateRightSideOfMesh(int verticesOffset, int trianglesOffset)
    {
        //Debug.Log("Generating RIGHT with offsets: " + verticesOffset + " + " + trianglesOffset);
        _vertices[verticesOffset + 0] = P(PIC.UBR);
        _vertices[verticesOffset + 1] = P(PIC.UFR);
        _vertices[verticesOffset + 2] = P(PIC.RM);
        _vertices[verticesOffset + 3] = P(PIC.DBR);
        _vertices[verticesOffset + 4] = P(PIC.DFR);

        SetTrianglesForSide(verticesOffset, trianglesOffset);

        for (int i = verticesOffset; i < verticesOffset + _cellVerticesPerSide; i++)
            _uv[i] = Vector2.zero;

        for (int i = verticesOffset; i < verticesOffset + _cellVerticesPerSide; i++)
            _normals[i] = Vector3.right;
    }

    private void GenerateBackSideOfMesh(int verticesOffset, int trianglesOffset)
    {
        //Debug.Log("Generating BACK with offsets: " + verticesOffset + " + " + trianglesOffset);
        _vertices[verticesOffset + 0] = P(PIC.UBL);
        _vertices[verticesOffset + 1] = P(PIC.UBR);
        _vertices[verticesOffset + 2] = P(PIC.BM);
        _vertices[verticesOffset + 3] = P(PIC.DBL);
        _vertices[verticesOffset + 4] = P(PIC.DBR);

        SetTrianglesForSide(verticesOffset, trianglesOffset);

        for (int i = verticesOffset; i < verticesOffset + _cellVerticesPerSide; i++)
            _uv[i] = Vector2.zero;

        for (int i = verticesOffset; i < verticesOffset + _cellVerticesPerSide; i++)
            _normals[i] = Vector3.back;
    }

    private void SetTrianglesForSide(int verticesOffset, int trianglesOffset)
    {
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
    }

    private Vector3 P(PIC p)
    {
        var firstHalfOfCircle = Mathf.Sin(_fillingLevel * Mathf.PI) / 3;

        switch (p)
        {
            /* Up 4 corners*/
            case PIC.UFR: return new Vector3(1 - firstHalfOfCircle, FillingLevel, 1 - firstHalfOfCircle);
            case PIC.UFL: return new Vector3(0 + firstHalfOfCircle, FillingLevel, 1 - firstHalfOfCircle);
            case PIC.UBL: return new Vector3(0 + firstHalfOfCircle, FillingLevel, 0 + firstHalfOfCircle);
            case PIC.UBR: return new Vector3(1 - firstHalfOfCircle, FillingLevel, 0 + firstHalfOfCircle);
            /* Down 4 Corners*/
            case PIC.DFR: return new Vector3(1, 0, 1);
            case PIC.DFL: return new Vector3(0, 0, 1);
            case PIC.DBL: return new Vector3(0, 0, 0);
            case PIC.DBR: return new Vector3(1, 0, 0);
            case PIC.UM: return new Vector3(0.5f, FillingLevel, 0.5f);
            case PIC.FM: return new Vector3(0.5f, FillingLevel / 2, 1);
            case PIC.DM: return new Vector3(0.5f, 0, 0.5f);
            case PIC.LM: return new Vector3(0, FillingLevel / 2, 0.5f);
            case PIC.RM: return new Vector3(1, FillingLevel / 2, 0.5f);
            case PIC.BM: return new Vector3(0.5f, FillingLevel / 2, 0);
            default:
                throw new ArgumentOutOfRangeException("p");
        }
    }

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
