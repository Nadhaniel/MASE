using System.Collections;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Net.NetworkInformation;
using UnityEngine.UIElements;
using UnityEngine.XR;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DetailedCapsule : MonoBehaviour
{
    #region Fields
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private MeshCollider meshCollider;
    private Transform root;
    private Mesh mesh;
    [Header("Tools")]
    [SerializeField] private GameObject boneTool;
   
    [Space]
    
    [Header("Capsule Properties")]

    [SerializeField] [Range(4,100)] int Segments = 24;
    [SerializeField] float Radius = 0.5f;
    [SerializeField] [Range(2,100)] int Rings = 14;
    [SerializeField] float Lenght = 28;
    [SerializeField] Vector2 MinMaxBones = new Vector2(2, 10);

    private List<Bone> bones = new List<Bone>();
    private List<Vector3> vertices = new List<Vector3>();
    #endregion


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

            mesh.name = "Body";
        }

        CapsuleMeshGeneration(vertices);
    }
    #region MeshSetup
    private void CapsuleMeshGeneration(List<Vector3> vertices)
    {
        //Mesh Generation
        mesh.Clear();

        //Vertices & BoneWeights
        vertices = new List<Vector3>();
        List<BoneWeight> boneWeights = new List<BoneWeight>();

        
        #region Top HemiSphere
        vertices.Add(new Vector3(0, 0, 0));
        //boneWeights.Add(new BoneWeight() {boneIndex0 = 0, weight0 = 1 });
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
                //boneWeights.Add(new BoneWeight() { boneIndex0 = 0, weight0 = 1f });
            }
        }
        #endregion
        
        #region Cylinder
        for (int ringIndex = 0; ringIndex < Rings ; ringIndex++)
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
        #endregion
        
        #region Bottom HemiSphere

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
            }
        }
        vertices.Add(new Vector3(0, 0, 2f * Radius + Lenght));

        mesh.vertices = vertices.ToArray();
        #endregion
        
        #region Triangles
        List<int> triangles = new List<int>();

        for (int i = 0; i < Segments; i++)
        {
            int seamOffset = i != Segments - 1 ? 0 : Segments;

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
        #endregion

        #region UVS
        #endregion

        Bounds bounds = new Bounds(new Vector3(0, 0, (Lenght + 1) / 2), new Vector3(1, 1, (Lenght + 1)));
        skinnedMeshRenderer.localBounds = bounds;

        #region Bones
        //Transform[] boneTransforms = new Transform[bones.Count];
        //Matrix4x4[] bindPoses = new Matrix4x4[bones.Count];
        //Vector3[] deltaZeroArr = new Vector3[vertices.Count];
        //for (int vertIndex = 0; vertIndex < vertices.Count; vertIndex++)
        //{
        //    deltaZeroArr[vertIndex] = Vector3.zero;
        //}
        //for (int boneIndex = 0; boneIndex < bones.Count; boneIndex++)
        //{
        //    boneTransforms[boneIndex] = root.GetChild(boneIndex);
        //    boneTransforms[boneIndex].localPosition = Vector3.forward * (Radius + Lenght * (0.5f + boneIndex));
        //    boneTransforms[boneIndex].localRotation = Quaternion.identity;
        //    bindPoses[boneIndex] = boneTransforms[boneIndex].worldToLocalMatrix * transform.localToWorldMatrix;

        //    if (boneIndex > 0)
        //    {
        //        HingeJoint hingeJoint = boneTransforms[boneIndex].GetComponent<HingeJoint>();
        //        hingeJoint.anchor = new Vector3(0, 0, -Lenght / 2f);
        //        hingeJoint.connectedBody = boneTransforms[boneIndex - 1].GetComponent<Rigidbody>();
        //    }

        //    Vector3[] deltaVertices = new Vector3[vertices.Count];
        //    for (int vertIndex = 0; vertIndex < vertices.Count; vertIndex++)
        //    {
        //        float distToBone = Mathf.Clamp(Vector3.Distance(vertices[vertIndex], boneTransforms[boneIndex].localPosition), 0, 2f * Lenght);
        //        Vector3 dirToBone = (vertices[vertIndex] - boneTransforms[boneIndex].localPosition).normalized;
        //        deltaVertices[vertIndex] = dirToBone * (2f * Lenght - distToBone);
        //    }

        //    mesh.AddBlendShapeFrame("Bone." + boneIndex, 0, deltaZeroArr, deltaZeroArr, deltaZeroArr);
        //    mesh.AddBlendShapeFrame("Bone." + boneIndex, 100, deltaVertices, deltaZeroArr, deltaZeroArr);
        //}

        //mesh.bindposes = bindPoses;
        //skinnedMeshRenderer.bones = boneTransforms;

        mesh.RecalculateNormals();
        mesh.Optimize();


        #endregion

        #region Mesh Molding
        //for (int boneIndex = 0; boneIndex < bones.Count; boneIndex++)
        //{
        //    boneTransforms[boneIndex].position = bones[boneIndex].Position;
        //    boneTransforms[boneIndex].rotation = bones[boneIndex].Rotation;
        //    skinnedMeshRenderer.SetBlendShapeWeight(boneIndex, bones[boneIndex].Size);
        //}
        //Mesh skinnedMesh = new Mesh();
        //skinnedMeshRenderer.BakeMesh(skinnedMesh);
        //meshCollider.sharedMesh = skinnedMesh;
        #endregion
    }

    #endregion

    public void AddToFront() 
    {
        Lenght += 1;
        Rings += 4;
        CapsuleMeshGeneration(vertices);
    }
    public void RemoveFromFront()
    {
        Lenght -= 1;
        Rings -= 4;
        CapsuleMeshGeneration(vertices);
    }
}
