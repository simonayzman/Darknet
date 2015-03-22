using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CreateMesh : MonoBehaviour
{
    public int TileWidth = 16;
    public int TileHeight = 16;
    public int NumTilesX = 16;
    public int NumTilesY = 16;
    public int DefaultTileX;
    public int DefaultTileY;
    public bool ShouldDrawRandomTiles;
    public Texture2D Texture;

    private int _gridWidth;
    private int _gridHeight;

    void Update()
    {
        if (ShouldDrawRandomTiles)
        {
            DrawRandomTiles();
        }
    }

    private void DrawRandomTiles()
    {
        var tileColumn = Random.Range(0, NumTilesX);
        var tileRow = Random.Range(0, NumTilesY);

        var x = Random.Range(0, _gridWidth);
        var y = Random.Range(0, _gridHeight);

        UpdateGrid(new Vector2(x, y), new Vector2(tileColumn, tileRow));
    }

    public void UpdateGrid(Vector2 gridIndex, Vector2 tileIndex)
    {
        var mesh = GetComponent<MeshFilter>().mesh;
        var uvs = mesh.uv;

        var tileSizeX = 1.0f / NumTilesX;
        var tileSizeY = 1.0f / NumTilesY;

        mesh.uv = uvs;

        uvs[(int)(_gridWidth * gridIndex.x + gridIndex.y) * 4 + 0] = new Vector2(tileIndex.x * tileSizeX, tileIndex.y * tileSizeY);
        uvs[(int)(_gridWidth * gridIndex.x + gridIndex.y) * 4 + 1] = new Vector2((tileIndex.x + 1) * tileSizeX, tileIndex.y * tileSizeY);
        uvs[(int)(_gridWidth * gridIndex.x + gridIndex.y) * 4 + 2] = new Vector2((tileIndex.x + 1) * tileSizeX, (tileIndex.y + 1) * tileSizeY);
        uvs[(int)(_gridWidth * gridIndex.x + gridIndex.y) * 4 + 3] = new Vector2(tileIndex.x * tileSizeX, (tileIndex.y + 1) * tileSizeY);

        mesh.uv = uvs;
    }

    public void CreatePlane(int gridWidth, int gridHeight)
    {
        _gridWidth = gridWidth;
        _gridHeight = gridHeight;

        var mesh = new Mesh();
        var mf = GetComponent<MeshFilter>();
        mf.renderer.material.SetTexture("_MainTex", Texture);
        mf.mesh = mesh;

        var tileSizeX = 1.0f / NumTilesX;
        var tileSizeY = 1.0f / NumTilesY;

        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var normals = new List<Vector3>();
        var uvs = new List<Vector2>();

        var index = 0;
        for (var x = 0; x < gridWidth; x++) {
            for (var y = 0; y < gridHeight; y++) {
                AddVertices(_gridHeight, _gridWidth, y, x, vertices);
                index = AddTriangles(index, triangles);
                AddNormals(normals);
                AddUvs(DefaultTileX, tileSizeY, tileSizeX, uvs, DefaultTileY);
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
    }

    private static void AddVertices(int tileHeight, int tileWidth, int y, int x, ICollection<Vector3> vertices)
    {
        vertices.Add(new Vector3((x * tileWidth), (y * tileHeight), 0));
        vertices.Add(new Vector3((x * tileWidth) + tileWidth, (y * tileHeight), 0));
        vertices.Add(new Vector3((x * tileWidth) + tileWidth, (y * tileHeight) + tileHeight, 0));
        vertices.Add(new Vector3((x * tileWidth), (y * tileHeight) + tileHeight, 0));
    }

    private static int AddTriangles(int index, ICollection<int> triangles)
    {
        triangles.Add(index + 2);
        triangles.Add(index + 1);
        triangles.Add(index);
        triangles.Add(index);
        triangles.Add(index + 3);
        triangles.Add(index + 2);
        index += 4;
        return index;
    }

    private static void AddNormals(ICollection<Vector3> normals)
    {
        normals.Add(Vector3.forward);
        normals.Add(Vector3.forward);
        normals.Add(Vector3.forward);
        normals.Add(Vector3.forward);
    }

    private static void AddUvs(int tileRow, float tileSizeY, float tileSizeX, ICollection<Vector2> uvs, int tileColumn)
    {
        uvs.Add(new Vector2(tileColumn * tileSizeX, tileRow * tileSizeY));
        uvs.Add(new Vector2((tileColumn + 1) * tileSizeX, tileRow * tileSizeY));
        uvs.Add(new Vector2((tileColumn + 1) * tileSizeX, (tileRow + 1) * tileSizeY));
        uvs.Add(new Vector2(tileColumn * tileSizeX, (tileRow + 1) * tileSizeY));
    }
}