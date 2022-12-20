using System.Collections;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Net.NetworkInformation;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DetailedCapsule : MonoBehaviour
{
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private MeshCollider meshCollider;
    private Transform root;
    private Mesh mesh;
    public int Segments = 24;
    public float Radius = 0.5f;
    public int Rings = 14;
    public float Lenght = 28;


#if UNITY_EDITOR
    public void OnValidate()
    {
        Initialize();
    }
#endif

    private void Initialize() {
        if (!mesh) {
            GameObject model = new GameObject("Model");
            model.transform.SetParent(transform);
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;

            root = new GameObject("Root").transform;
            root.SetParent(transform);
            root.localPosition = Vector3.zero;
            root.localRotation = Quaternion.identity;

            skinnedMeshRenderer = model.AddComponent<SkinnedMeshRenderer>();
            skinnedMeshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
            skinnedMeshRenderer.sharedMesh = mesh = model.AddComponent<MeshFilter>().sharedMesh = new Mesh();
            skinnedMeshRenderer.rootBone = root.transform;
            meshCollider = model.AddComponent<MeshCollider>();
            //model.AddComponent<Test>();

            mesh.name = "Body";
        }

        //Add method and Addtoback method missing

        Setup();
    }

    private void Setup()
    {
        //Mesh Generation
        mesh.Clear();

        //Vertices
        List<Vector3> vertices = new List<Vector3>();
        //Top hemisphere
        vertices.Add(new Vector3(0, 0, 0));
        for (int ringIndex = 1; ringIndex < Segments / 2; ringIndex++)
        {
            float percent = (float)ringIndex / (Segments / 2);
            float ringRadius = Radius * Mathf.Sin(90f * percent * Mathf.Deg2Rad);
            float ringDistance = Radius * (-Mathf.Cos(90f * percent * Mathf.Deg2Rad) + 1f);

            for (int i = 0; i < Segments; i++)
            {
                float angle = i * 360f / Segments;
                float x = ringRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
                float y = ringRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
                float z = ringDistance;

                vertices.Add(new Vector3(x, y, z));
                //boneweights
            }
        }

        //Middle Cyclinder
        for (int ringIndex = 0; ringIndex < Rings; ringIndex++)
        {
            for (int i = 0; i < Segments; i++)
            {
                float angle = i * 360f / Segments;
                float x = Radius * Mathf.Cos(angle * Mathf.Deg2Rad);
                float y = Radius * Mathf.Sin(angle * Mathf.Deg2Rad);
                float z = ringIndex * Lenght / Rings;

                vertices.Add(new Vector3(x, y, Radius + z));
            }
        }

        //Bottom Hemisphere

        for (int ringIndex = 0; ringIndex < Segments / 2; ringIndex++)
        {
            float percent = (float)ringIndex / (Segments / 2);
            float ringRadius = Radius * Mathf.Cos(90f * percent * Mathf.Deg2Rad);
            float ringDistance = Radius * Mathf.Sin(90f * percent * Mathf.Deg2Rad);

            for (int i = 0; i < Segments; i++)
            {
                float angle = i * 360f / Segments;
                float x = ringRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
                float y = ringRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
                float z = ringDistance;

                vertices.Add(new Vector3(x, y, Radius + Lenght + z));
                //boneweights
            }
        }
        vertices.Add(new Vector3(0, 0, 2f * Radius + Lenght));

        mesh.vertices = vertices.ToArray();

        //Triangles
        List<int> triangles = new List<int>();

        for (int i = 0; i < Segments; i++)
        {
            int seamOffset = i != Segments - 1 ? 0 : Segments; //? = sh0rthand for an if statement

            triangles.Add(0);
            triangles.Add(i + 2 - seamOffset);
            triangles.Add(i + 1);
        }

        int rings = Rings + (2 * (Segments / 2 - 1));
        for (int ringIndex = 0; ringIndex < rings; ringIndex++)
        {
            int ringOffset = 1 + ringIndex * Segments;

            for (int i = 0; i < Segments; i++)
            {
                int seamOffset = i != Segments - 1 ? 0 : Segments;

                triangles.Add(ringOffset + i);
                triangles.Add(ringOffset + i + 1 - seamOffset);
                triangles.Add(ringOffset + i + 1 - seamOffset + Segments);

                triangles.Add(ringOffset + i + 1 - seamOffset + Segments);
                triangles.Add(ringOffset + i + Segments);
                triangles.Add(ringOffset + i);
            }
        }

        int topIndex = 1 + (rings + 1) * Segments;
        for (int i = 0; i < Segments; i++)
        {
            int seamOffset = i != Segments - 1 ? 0 : Segments;

            triangles.Add(topIndex);
            triangles.Add(topIndex - i - 2 + seamOffset);
            triangles.Add(topIndex - i - 1);
        }
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();
        mesh.Optimize();
    }


}
