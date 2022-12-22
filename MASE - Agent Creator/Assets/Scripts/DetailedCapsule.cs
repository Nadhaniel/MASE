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
    private GameObject boneTool;
    [SerializeField] int Segments = 24;
    [SerializeField] float Radius = 0.5f;
    [SerializeField] int Rings = 14;
    [SerializeField] float Lenght = 28;
    [SerializeField] Vector2 MinMaxBones = new Vector2(2, 10);

    private List<Bone> bones = new List<Bone>();
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

        //AddBone(0);
        //Add method and Addtoback method missing

        CapsuleMeshGeneration();
    }
    #region MeshSetup
    private void CapsuleMeshGeneration()
    {
        //Mesh Generation
        mesh.Clear();

        //Vertices & BoneWeights
        List<Vector3> vertices = new List<Vector3>();
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
        for (int ringIndex = 0; ringIndex < Rings; ringIndex++)
        {
            //float boneIndexFloat = (float)ringIndex / Rings;
            //int boneIndex = Mathf.FloorToInt(boneIndexFloat);
            //float bonePercent = boneIndexFloat - boneIndex;

            //int boneIndex0 = (boneIndex > 0) ? boneIndex - 1 : 0;
            //int boneIndex2 = (boneIndex < bones.Count - 1) ? boneIndex + 1 : bones.Count - 1;
            //int boneIndex1 = boneIndex;

            //float weight0 = (boneIndex > 0) ? (1f - bonePercent) * 0.5f : 0f;
            //float weight2 = (boneIndex < bones.Count - 1) ? bonePercent * 0.5f : 0f;
            //float weight1 = 1f - (weight0 + weight2);
            for (int i = 0; i < Segments; i++)
            {
                float angle = i * 360f / Segments;
                float x = Radius * Mathf.Cos(angle * Mathf.Deg2Rad);
                float y = Radius * Mathf.Sin(angle * Mathf.Deg2Rad);
                float z = ringIndex * Lenght / Rings;

                vertices.Add(new Vector3(x, y, Radius + z));
                //boneWeights.Add(new BoneWeight() {boneIndex0 = boneIndex0, boneIndex1 = boneIndex1, boneIndex2 = boneIndex2, weight0 = weight0, weight1 = weight1, weight2 = weight2 });
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
                //boneWeights.Add(new BoneWeight() { boneIndex0 = bones.Count - 1, weight0 = 1 });
            }
        }
        vertices.Add(new Vector3(0, 0, 2f * Radius + Lenght));
        //boneWeights.Add(new BoneWeight() { boneIndex0 = bones.Count - 1, weight0 = 1 });

        mesh.vertices = vertices.ToArray();
        //mesh.boneWeights = boneWeights.ToArray();
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

        //    if (boneIndex > 0) {
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
        //    boneTransforms[boneIndex].localPosition = bones[boneIndex].position;
        //    boneTransforms[boneIndex].localRotation = bones[boneIndex].rotation;
        //    skinnedMeshRenderer.SetBlendShapeWeight(boneIndex, bones[boneIndex].scale.magnitude); //Possible bug here with using scale.magnitute here
        //}
        //Mesh skinnedMesh = new Mesh();
        //skinnedMeshRenderer.BakeMesh(skinnedMesh);
        //meshCollider.sharedMesh = skinnedMesh;
        #endregion
    }

    #endregion


    public void AddBone(int index, Vector3 position, Quaternion rotation, float size)
    {
        if ((bones.Count + 1) <= MinMaxBones.y) 
        {
            GameObject boneobj = Instantiate(boneTool, root, false);
            boneobj.name = "Bone." + (root.childCount - 1);
            boneobj.layer = LayerMask.NameToLayer("tools");

            if (bones.Count == 0)
            {
                DestroyImmediate(boneobj.GetComponent<HingeJoint>());
            }

            CapsuleMeshGeneration();
        }
    }
    public void AddToFront() 
    {

    }
    private void UpdateBoneConfig()
    {
        
    }
}
