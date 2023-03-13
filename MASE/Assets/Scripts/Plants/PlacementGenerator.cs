using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlacementGenerator : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] GameObject mesh;

    [Header("Raycast Settings")]
    [SerializeField] int density;

    [Space]

    [SerializeField] float minHeight;
    [SerializeField] float maxHeight;
    [SerializeField] Vector2 xRange;
    [SerializeField] Vector2 zRange;

    [Header("Prefab Variation Settings")]
    [SerializeField, Range(0, 1)] float rotateTowardsNormal;
    [SerializeField] Vector2 rotationRange;
    [SerializeField] Vector3 minScale;
    [SerializeField] Vector3 maxScale;

    public void Generate()
    {
        Clear();
        for (int i = 0; i < density; i++)
        {
            float sampleX = Random.Range(xRange.x, xRange.y);
            float sampleY = Random.Range(zRange.x, zRange.y);
            Vector3 rayStart = new Vector3(sampleX, maxHeight, sampleY);

            if (!Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, Mathf.Infinity))
            {
                continue;
            }

            if (hit.point.y < minHeight)
            {
                continue;
            }

            GameObject instatiatedPrefab = Instantiate(this.prefab, mesh.transform);
            instatiatedPrefab.transform.position = hit.point;
            instatiatedPrefab.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);
            instatiatedPrefab.transform.rotation = Quaternion.Lerp(mesh.transform.rotation, mesh.transform.rotation * Quaternion.FromToRotation(instatiatedPrefab.transform.up, hit.normal), rotateTowardsNormal);
            instatiatedPrefab.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }
    }

    public void Clear()
    {
        while (mesh.transform.childCount != 0)
        {
            DestroyImmediate(mesh.transform.GetChild(0).gameObject);
        }
    }
}
