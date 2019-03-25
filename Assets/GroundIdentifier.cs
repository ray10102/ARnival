using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class GroundIdentifier : MonoBehaviour
{
    public static float Height = float.MinValue;
    public bool GroundHeightUpdated;
    
    public Transform BBoxTransform;
    public Vector3   BBoxExtents;
    
    [BitMask(typeof(MLWorldPlanesQueryFlags))]
    public MLWorldPlanesQueryFlags QueryFlags;

    private MLWorldPlanesQueryParams _queryParams = new MLWorldPlanesQueryParams();
    private List<MLWorldPlane> _planeCache = new List<MLWorldPlane>();

    // Use this for initialization
    void Start ()
    {
        MLWorldPlanes.Start();
        RequestPlanes();
    }

    private void OnDestroy()
    {
        MLWorldPlanes.Stop();
    }

    void RequestPlanes()
    {
        _queryParams.Flags = QueryFlags;
        _queryParams.MaxResults = 100;
        _queryParams.BoundsCenter = BBoxTransform.position;
        _queryParams.BoundsRotation = BBoxTransform.rotation;
        _queryParams.BoundsExtents = BBoxExtents;

        MLWorldPlanes.GetPlanes(_queryParams, HandleOnReceivedPlanes);
    }

    private void HandleOnReceivedPlanes(MLResult result, MLWorldPlane[] planes, MLWorldPlaneBoundaries[] boundaries)
    {
        _planeCache.Clear();

        float ySum = 0;
        for (int i = 0; i < planes.Length; ++i)
        {
            ySum += planes[i].Center.y;
            _planeCache.Add(planes[i]);
        }

        Height = ySum / planes.Length;
        GroundHeightUpdated = true;
        transform.position = new Vector3(0, Height, 0);
        transform.rotation = Quaternion.identity;
        GamePosition[] games = FindObjectsOfType<GamePosition>();
        foreach (GamePosition game in games)
        {
            game.Reset();
        }
    }
}