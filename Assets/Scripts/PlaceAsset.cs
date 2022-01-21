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
        Debug.Log(transvectors.GetData().GetValue(0,0,0));
        Debug.Log(transvectors.GetData().GetValue(0,0,1));
        Debug.Log(transvectors.GetData().GetValue(0,0,2));
    }
}
