using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;

    public void Drawtexture(Texture2D texture) {
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(MeshData meshData) {
        meshFilter.sharedMesh = meshData.CreateMesh();

        if (FindObjectOfType<MapGenerator>() == null)
        {
            meshFilter.transform.localScale = Vector3.one * FindObjectOfType<MapGen_TerrainScene>().Terraindata.uniformScale;
        }
        else
        {
            meshFilter.transform.localScale = Vector3.one * FindObjectOfType<MapGenerator>().Terraindata.uniformScale;
        }
        meshCollider.sharedMesh = meshFilter.sharedMesh;
    }
}
