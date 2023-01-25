using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TerrainData : UpdateableData
{
    public float uniformScale = 2.5f;

    public bool useFalloff;

    public float meshHeightMultplier;
    public AnimationCurve meshHeightCurve;

    public float minHeight
    {
        get { return uniformScale * meshHeightMultplier * meshHeightCurve.Evaluate(0); }
    }

    public float maxHeight
    {
        get { return uniformScale * meshHeightMultplier * meshHeightCurve.Evaluate(1); }
    }
}
