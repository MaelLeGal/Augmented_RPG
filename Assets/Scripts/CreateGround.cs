using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Aruco;
using Emgu.CV.Util;

public class CreateGround : MonoBehaviour
{

    public Material mat;
    public PlaceAsset placementAsset;
    public GameObject webcamObj;
    private WebcamCapture capture;
    private void Start()
	{
        capture = webcamObj.GetComponent<WebcamCapture>(); 

        //création du mesh du terrain
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        List<Vector3> vertices = new List<Vector3>();
        for (int i = 0; i < 17; ++i)
        {
            vertices.Add(new Vector3(0,0,0));
        }

        List<int> triangles = new List<int>();

        triangles.Add(0);
        triangles.Add(3);
        triangles.Add(2);

        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(3);

        triangles.Add(1);
        triangles.Add(4);
        triangles.Add(3);

        triangles.Add(2);
        triangles.Add(3);
        triangles.Add(5);

        triangles.Add(3);
        triangles.Add(6);
        triangles.Add(5);

        triangles.Add(3);
        triangles.Add(4);
        triangles.Add(6);

        triangles.Add(2);
        triangles.Add(5);
        triangles.Add(7);

        triangles.Add(4);
        triangles.Add(9);
        triangles.Add(6);

        triangles.Add(5);
        triangles.Add(8);
        triangles.Add(7);

        triangles.Add(5);
        triangles.Add(6);
        triangles.Add(8);

        triangles.Add(6);
        triangles.Add(9);
        triangles.Add(8);

        triangles.Add(7);
        triangles.Add(8);
        triangles.Add(10);

        triangles.Add(8);
        triangles.Add(11);
        triangles.Add(10);

        triangles.Add(8);
        triangles.Add(9);
        triangles.Add(11);

        triangles.Add(7);
        triangles.Add(10);
        triangles.Add(12);

        triangles.Add(9);
        triangles.Add(14);
        triangles.Add(11);

        triangles.Add(10);
        triangles.Add(13);
        triangles.Add(12);

        triangles.Add(10);
        triangles.Add(11);
        triangles.Add(13);

        triangles.Add(11);
        triangles.Add(14);
        triangles.Add(13);

        triangles.Add(12);
        triangles.Add(13);
        triangles.Add(15);

        triangles.Add(13);
        triangles.Add(16);
        triangles.Add(15);

        triangles.Add(13);
        triangles.Add(14);
        triangles.Add(16);


        Mesh msh = new Mesh();                          // Création et remplissage du Mesh

        msh.vertices = vertices.ToArray();
        msh.triangles = triangles.ToArray();
        msh.RecalculateNormals();

        gameObject.GetComponent<MeshFilter>().mesh = msh;           // Remplissage du Mesh et ajout du matériel
        gameObject.GetComponent<MeshRenderer>().material = mat;


    }
    
    public void modifyGroundPoint(Mat rotvectors, Mat transvectors, int markersInfo)
    {
        //(Vector3, Quaternion) transformAsset = GetComponent<PlaceAsset>().computeTransform(rotvectors, transvectors);

        Vector3 localPos;
        localPos.x = float.Parse(transvectors.GetData().GetValue(0, 0, 0).ToString());
        localPos.y = -float.Parse(transvectors.GetData().GetValue(0, 0, 1).ToString());
        localPos.z = float.Parse(transvectors.GetData().GetValue(0, 0, 2).ToString());

        Vector3 worldPos = Camera.main.transform.TransformPoint(localPos);
        
        Mesh msh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = msh.vertices;
        vertices[markersInfo] = worldPos;
        msh.vertices = vertices;
        msh.RecalculateNormals();
        gameObject.GetComponent<MeshFilter>().mesh = msh;
    }

    public void recenter(Vector3 center)
    {
        Mesh msh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = msh.vertices;
		for (int i = 0; i < vertices.Length; i++)
		{
            vertices[i] -= center;
		}
        msh.vertices = vertices;
        msh.RecalculateNormals();
        gameObject.GetComponent<MeshFilter>().mesh = msh;
    }

}
