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

    Dictionary dictMarkers = new Dictionary(Dictionary.PredefinedDictionaryName.Dict6X6_250);

    public void displayAsset(VectorOfPointF pointMarqueur, int marqueurID)
    {
       
    }
    public void displayAsset(Mat rotvectors, Mat transvectors, int marqueurID)
    {
        
        Debug.Log(float.Parse(transvectors.GetData().GetValue(0, 0, 0).ToString()));

        Vector3 localPos;
        localPos.x = float.Parse(transvectors.GetData().GetValue(0, 0, 0).ToString());
        localPos.y = -float.Parse(transvectors.GetData().GetValue(0, 0, 1).ToString());
        localPos.z = float.Parse(transvectors.GetData().GetValue(0, 0, 2).ToString());

        Vector3 worldPos = transform.TransformPoint(localPos);

        //double[] flip = (double[])rotvectors.GetData().GetValue(0, 0);
        //flip[1] = -flip[1];
        //rotvectors.GetData().SetValue(flip, 0,0);

        Mat rotMatrix = new Mat();
        CvInvoke.Rodrigues(rotvectors, rotMatrix);

        Vector3 forward;
        Debug.Log(rotMatrix.GetData().GetValue(0, 0, 2).ToString());
        forward.x = float.Parse(rotMatrix.GetData().GetValue(2, 0, 0).ToString());
        forward.y = float.Parse(rotMatrix.GetData().GetValue(2, 1, 0).ToString());
        forward.z = float.Parse(rotMatrix.GetData().GetValue(2, 2, 0).ToString());

        Vector3 up;
        up.x = float.Parse(rotMatrix.GetData().GetValue(1, 0, 0).ToString());
        up.y = float.Parse(rotMatrix.GetData().GetValue(1, 1, 0).ToString());
        up.z = float.Parse(rotMatrix.GetData().GetValue(1, 2, 0).ToString());

        Quaternion rot = Quaternion.LookRotation(forward, up);

        rot *= Quaternion.Euler(0, 0, 180);

        Quaternion worldrot = transform.rotation * rot;

        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        sphere.transform.position = worldPos;
        sphere.transform.rotation = worldrot;

    }
}
