using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Aruco;
using Emgu.CV.Util;

public class PlaceAsset : MonoBehaviour
{
    public GameObject orc;
    public List<GameObject> listObject;
    public Material terrainMaterial;

    Dictionary dictMarkers = new Dictionary(Dictionary.PredefinedDictionaryName.Dict6X6_250);

    public void displayAsset(VectorOfPointF pointMarqueur, int marqueurID)
    {
       
    }

    public void createTerrain(Mat rotvectors, Mat transvectors, int markerID)
    {
        (Vector3, Quaternion) transformAsset = computeTransform(rotvectors, transvectors);

        Vector3 worldPos = transformAsset.Item1;
        Quaternion worldRot = transformAsset.Item2;

        GameObject TerrainObj;
        if (GameObject.Find("TerrainObj"))
        {
            TerrainObj = GameObject.Find("TerrainObj");
        }
        else
        {
            TerrainObj = new GameObject("TerrainObj");
        }        

        TerrainData _TerrainData = new TerrainData();

        _TerrainData.size = new Vector3(10, 600, 10);
        _TerrainData.heightmapResolution = 512;
        _TerrainData.baseMapResolution = 1024;
        _TerrainData.SetDetailResolution(1024, 16);

        int _heightmapWidth = _TerrainData.heightmapResolution;
        int _heightmapHeight = _TerrainData.heightmapResolution;

        TerrainCollider _TerrainCollider = TerrainObj.AddComponent<TerrainCollider>();
        Terrain _Terrain2 = TerrainObj.AddComponent<Terrain>();

        _TerrainCollider.terrainData = _TerrainData;
        _Terrain2.terrainData = _TerrainData;
        _Terrain2.materialTemplate = terrainMaterial;

        TerrainObj.transform.position = worldPos;
        TerrainObj.transform.rotation = worldRot;

        //GameObject _Terrain = Terrain.CreateTerrainGameObject(_TerrainData);

    }
    public void displayAsset(Mat rotvectors, Mat transvectors, int marqueurID)
    {
        (Vector3, Quaternion) transformAsset = computeTransform(rotvectors, transvectors);

        Vector3 worldPos = transformAsset.Item1;
        Quaternion worldRot = transformAsset.Item2;

        GameObject Capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        Capsule.transform.position = worldPos;
        Capsule.transform.rotation = worldRot;
        Capsule.GetComponent<Renderer>().material.color = Color.red;
    }

    public (Vector3, Quaternion) computeTransform(Mat rotvectors, Mat transvectors)
    {
        Vector3 localPos;
        localPos.x = float.Parse(transvectors.GetData().GetValue(0, 0, 0).ToString());
        localPos.y = -float.Parse(transvectors.GetData().GetValue(0, 0, 1).ToString());
        localPos.z = float.Parse(transvectors.GetData().GetValue(0, 0, 2).ToString());

        Vector3 worldPos = transform.TransformPoint(localPos);

        double flip = (double)rotvectors.GetData().GetValue(0, 0, 0);
        flip = -flip;
        rotvectors.GetData().SetValue(flip, 0, 0, 0);

        Mat rotMatrix = new Mat();
        CvInvoke.Rodrigues(rotvectors, rotMatrix);

        Vector3 forward;
        forward.x = float.Parse(rotMatrix.GetData().GetValue(2, 0).ToString());
        forward.y = float.Parse(rotMatrix.GetData().GetValue(2, 1).ToString());
        forward.z = float.Parse(rotMatrix.GetData().GetValue(2, 2).ToString());

        Vector3 up;
        up.x = float.Parse(rotMatrix.GetData().GetValue(1, 0).ToString());
        up.y = float.Parse(rotMatrix.GetData().GetValue(1, 1).ToString());
        up.z = float.Parse(rotMatrix.GetData().GetValue(1, 2).ToString());

        forward.Normalize();
        up.Normalize();

        Quaternion rot = Quaternion.LookRotation(forward, up);

        rot *= Quaternion.Euler(0, 0, 180);

        Quaternion worldRot = transform.rotation * rot;

        return (worldPos, worldRot);
    }
}
