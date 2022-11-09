using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFC : MonoBehaviour
{
    Blocks ListofBlocks = new Blocks();
    public int size = 100;
    Grid test = new Grid();
    int[,] grid;

    private void Start()
    {
        grid = new int[size, size];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                grid[x, y] = 0;
            }
        }
    }

    void DrawTerrainmesh(GameObject[,] grid) {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                GameObject block = grid[x, y];
                Vector3 a = new Vector3(x - .5f, 0, y + .5f);
                Vector3 b = new Vector3(x + .5f, 0, y + .5f);
                Vector3 c = new Vector3(x - .5f, 0, y - .5f);
                Vector3 d = new Vector3(x + .5f, 0, y - .5f);
                Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                for (int k = 0; k < 6; k++)
                {
                    vertices.Add(v[k]);
                    triangles.Add(triangles.Count);
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRend = gameObject.AddComponent<MeshRenderer>();
    }
}
