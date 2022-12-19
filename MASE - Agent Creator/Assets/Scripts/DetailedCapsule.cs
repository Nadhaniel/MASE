using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DetailedCapsule : MonoBehaviour
{
#if UNITY_EDITOR
    public void OnValidate()
    {
        GenerateCapsuleMesh();
    }
#endif

    public enum UvProfile : int {Fixed = 0, Aspect = 1, Uniform = 2 }
    UvProfile profile = UvProfile.Aspect;

    private int longitudes = 32;
    public int latitudes = 16;
    public int rings = 0;
    public float depth = 1.0f;
    public float radius = 0.5f;
    public float height = 2f;
    public int segments = 24;

    int points;
    Vector3[] vertices;
    Mesh CapsuleMesh;

    //static GameObject InstantMesh(string name, Mesh mesh, float capsuleDepth = 1.0f, float capsuleRadius = 0.5f) { //instatiates basic capsule parameters and creates capsule object
    //    GameObject capsule = new GameObject(name);
    //    MeshFilter mf = capsule.AddComponent<MeshFilter>();
    //    MeshRenderer mr = capsule.AddComponent<MeshRenderer>();
    //    CapsuleCollider cc = capsule.AddComponent<CapsuleCollider>();

    //    mf.sharedMesh = mesh;
    //    mr.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");
    //    cc.height = capsuleDepth + capsuleRadius * 2.0f;
    //    cc.radius = capsuleRadius;
    //    Selection.activeObject = capsule;

    //    return capsule;
    //}

    

    void GenerateCapsuleMesh() {
        if (segments % 2 != 0)
        { 
            segments++;
        }

        points = segments + 1;

        float[] pX = new float[points];
        float[] pZ = new float[points];
        float[] pY = new float[points];
        float[] pR = new float[points];

        float calcH = 0f;
        float calcV = 0f;

        for (int i = 0; i < points; i++)
        {
            pX[i] = Mathf.Sin(calcH * Mathf.Deg2Rad);
            pZ[i] = Mathf.Cos(calcH * Mathf.Deg2Rad);
            pY[i] = Mathf.Cos(calcH * Mathf.Deg2Rad);
            pR[i] = Mathf.Cos(calcH * Mathf.Deg2Rad);

            calcH += 360f / (float)segments;
            calcV += 180f / (float)segments;
        }

        // -Vertices and UVs

        vertices = new Vector3[points * (points + 1)];
        Vector2[] uvs = new Vector2[vertices.Length];

        //Y 0ffset is half the heigh minus the diameter
        float yOff = (height - (radius * 2f)) * 0.5f;

        if (yOff < 0) {
            yOff = 0;
        }

        //uv calculations
        float stepX = 1f / ((float)(points - 1));
        float uvX, uvY;

        //Top Hemisphere
        int top = Mathf.CeilToInt((float)points * 0.5f);

        int ind = 0;
        for (int y = 0; y < top; y++)
        {
            for (int x = 0; x < top; x++)
            {
                vertices[ind] = new Vector3(pX[x] * pR[y], pY[y], pZ[x] * pR[y]) * radius;
                vertices[ind].y = yOff + vertices[ind].y;

                uvX = 1f - (stepX * (float)x);
                uvY = (vertices[ind].y + (height * 0.5f)) / height;
                uvs[ind] = new Vector2(uvX, uvY);

                ind++;
            }
        }

        //Bottom Hemisphere
        int btm = Mathf.FloorToInt((float)points * 0.5f);

        for (int y = 0; y < points; y++)
        {
            for (int x = 0; x < points; x++)
            {
                vertices[ind] = new Vector3(pX[x] * pR[y], pY[y], pZ[x] * pR[y]) * radius;
                vertices[ind].y = -yOff + vertices[ind].y;

                uvX = 1f - (stepX * (float)x);
                uvY = (vertices[ind].y + (height * 0.5f)) / height;
                uvs[ind] = new Vector2(uvX, uvY);

                ind++;
            }
        }

        // Triangles

        int[] Triangles = new int[(segments * (segments + 1) * 2 * 3)];

        for (int y = 0, t = 0; y < segments + 1; y++)
        {
            for (int x = 0; x < segments; x++, t += 6)
            {
                Triangles[t + 0] = ((y + 0) * (segments + 1)) + x + 0;
                Triangles[t + 1] = ((y + 1) * (segments + 1)) + x + 0;
                Triangles[t + 2] = ((y + 1) * (segments + 1)) + x + 1;

                Triangles[t + 3] = ((y + 0) * (segments + 1)) + x + 1;
                Triangles[t + 4] = ((y + 0) * (segments + 1)) + x + 0;
                Triangles[t + 5] = ((y + 1) * (segments + 1)) + x + 1;
            }
        }

        //Assign Mesh
        MeshFilter mf = gameObject.GetComponent<MeshFilter>();
        Mesh mesh = mf.sharedMesh;

        if (!mesh) {
            mesh = new Mesh();
            mf.sharedMesh = mesh;
        }
        mesh.Clear();

        mesh.name = "Capsule";
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = Triangles;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }


    Mesh CapsuleData()
    {
        bool calcMiddle = rings > 0;
        int halfLats = latitudes / 2;
        int halfLatsn1 = halfLats - 1;
        int halfLatsn2 = halfLats - 2;
        int ringsp1 = rings + 1;
        int lonsp1 = longitudes + 1;
        float halfDepth = depth * 0.5f;
        float summit = halfDepth + radius;

        //Vertex index offsets
        int vertOffsetNorthHemi = longitudes;
        int vertOffsetNorthEquator = vertOffsetNorthHemi + lonsp1 * halfLatsn1;
        int vertOffsetCyclinder = vertOffsetNorthEquator + lonsp1;
        int vertOffsetSouthEquator = calcMiddle ? vertOffsetCyclinder + lonsp1 * rings : vertOffsetCyclinder;
        int vertOffsetSouthHemi = vertOffsetSouthEquator + lonsp1;
        int vertOffsetSouthPolar = vertOffsetSouthHemi + lonsp1 * halfLatsn2;
        int vertOffsetSouthCap = vertOffsetSouthPolar + lonsp1;

        int vertLen = vertOffsetSouthCap + longitudes;
        Vector3[] vs = new Vector3[vertLen];
        Vector2[] vts = new Vector2[vertLen];
        Vector3[] vns = new Vector3[vertLen];

        float toTheta = 2.0f * Mathf.PI / longitudes;
        float toPhi = Mathf.PI / latitudes;
        float toTexHorizontal = 1.0f / longitudes;
        float toTexVertical = 1.0f / halfLats;

        return null;
    }

    private void Start()
    {
        
    }
    private void Update()
    {
       
    }

}
