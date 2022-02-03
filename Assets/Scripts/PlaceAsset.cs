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
    public List<GameObject> listObject;
    public int scaleProps = 1;
    private Dictionary<int, GameObject> alreadyInPlace = new Dictionary<int, GameObject>();

    private GameObject TerrainObj;
    void Start()
    {
        TerrainObj = gameObject;
    }


    /* Old placement of the ground
    public Material terrainMaterial;
    public void createTerrain(Mat rotvectors, Mat transvectors, int markerID)
    {
        (Vector3, Quaternion) transformAsset = computeTransform(rotvectors, transvectors);

        Vector3 worldPos = transformAsset.Item1;
        Quaternion worldRot = transformAsset.Item2;

        if (GameObject.Find("TerrainObj"))
        {
            TerrainObj = GameObject.Find("TerrainObj");
        }
        else
        {
            TerrainObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
        }

        worldPos.y = 1f;

        TerrainObj.transform.position = worldPos;
        TerrainObj.transform.localScale = Vector3.one * scaleProps;
        TerrainObj.name = "TerrainObj";

        /*
        TerrainData _TerrainData = new TerrainData();

        Debug.Log(_TerrainData);

        _TerrainData.size = new Vector3(10, 600, 10);
        _TerrainData.heightmapResolution = 512;
        _TerrainData.baseMapResolution = 1024;
        _TerrainData.SetDetailResolution(1024, 16);

        int _heightmapWidth = _TerrainData.heightmapResolution;
        int _heightmapHeight = _TerrainData.heightmapResolution;

        TerrainCollider _TerrainCollider = TerrainObj.AddComponent<TerrainCollider>();
        Terrain _Terrain2 = TerrainObj.AddComponent<Terrain>();

        _TerrainCollider = TerrainObj.GetComponent<TerrainCollider>();
        _Terrain2 = TerrainObj.GetComponent<Terrain>();

        Debug.Log(_TerrainCollider);
        Debug.Log(_Terrain2);

        //_TerrainCollider.terrainData = _TerrainData;
        _Terrain2.terrainData = _TerrainData;
        _Terrain2.materialTemplate = terrainMaterial;

        worldPos.y = 0.1f;

        TerrainObj.transform.position = worldPos;
        TerrainObj.transform.rotation = worldRot;

        //GameObject _Terrain = Terrain.CreateTerrainGameObject(_TerrainData);

    }
    */



    public void displayAssetInWorldAR(Mat rotvectors, Mat transvectors, int marqueurID)
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

        Vector3 worldPos = Camera.main.transform.TransformPoint(localPos);

        
        
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

        Debug.Log(forward);

        Quaternion rot = Quaternion.LookRotation(up, forward);

        //rot *= Quaternion.Euler(0, 0, 180);
        //Quaternion worldRot = transform.rotation * rot;

        return (worldPos, rot);
    }

    public void displayAsset(Mat rotvectors, Mat transvectors, Vector3 posboard, int marqueurID)
    {
        (Vector3, Quaternion) transformAsset = GetComponent<PlaceAsset>().computeTransform(rotvectors, transvectors);
        Vector3 worldPos = transformAsset.Item1 - posboard;
        Quaternion rot = transformAsset.Item2;

        if (alreadyInPlace.ContainsKey(marqueurID))
        {
            //Debug.Log("meh");
            //Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            /*alreadyInPlace[marqueurID].transform.position = new Vector3(worldPos.x,300, worldPos.z);
            alreadyInPlace[marqueurID].transform.rotation = Quaternion.Euler(0, worldrot.y, 0);
            alreadyInPlace[marqueurID].transform.localScale = new Vector3(scale, scale, scale);*/

            alreadyInPlace[marqueurID].transform.position = worldPos;

            //rot = new Quaternion(rot.x, rot.y, 0, 0);
            //alreadyInPlace[marqueurID].transform.rotation = rot;

            //alreadyInPlace[marqueurID].transform.localScale = new Vector3(scale, scale, scale);

        }
        else
        {
            GameObject obj = listObject[marqueurID];
            //Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            /*obj.transform.position = new Vector3(worldPos.x, 1, worldPos.z);
            obj.transform.rotation = Quaternion.Euler(0,worldrot.y,0);
            alreadyInPlace.Add(marqueurID, Instantiate(obj));*/

            obj.transform.position = worldPos;
            obj.transform.rotation = Quaternion.identity;
            obj.transform.Rotate(Vector3.up, 90);
            obj.transform.Rotate(Vector3.forward, (float)0);
            obj.transform.Rotate(Vector3.right, (float)0);
            //obj.transform.rotation = rot;
            obj.transform.localScale = new Vector3(scaleProps, scaleProps, scaleProps);

            alreadyInPlace.Add(marqueurID, Instantiate(obj));
        }
    }
}
