using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DetailedCapsule : MonoBehaviour
{
    public enum UvProfile : int {Fixed = 0, Aspect = 1, Uniform = 2 }
    UvProfile profile = UvProfile.Aspect;

    private int longitudes = 32;
    public int latitudes = 16;
    public int rings = 0;
    public float depth = 1.0f;
    public float radius = 0.5f;
    Mesh CapsuleMesh;

    static GameObject InstantMesh(string name, Mesh mesh, float capsuleDepth = 1.0f, float capsuleRadius = 0.5f) { //instatiates basic capsule parameters and creates capsule object
        GameObject capsule = new GameObject(name);
        MeshFilter mf = capsule.AddComponent<MeshFilter>();
        MeshRenderer mr = capsule.AddComponent<MeshRenderer>();
        CapsuleCollider cc = capsule.AddComponent<CapsuleCollider>();

        mf.sharedMesh = mesh;
        mr.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");
        cc.height = capsuleDepth + capsuleRadius * 2.0f;
        cc.radius = capsuleRadius;
        Selection.activeObject = capsule;

        return capsule;
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
        latitudes = latitudes % 2 != 0 ? latitudes + 1 : latitudes;
        CapsuleMesh = CapsuleData();
    }

}
