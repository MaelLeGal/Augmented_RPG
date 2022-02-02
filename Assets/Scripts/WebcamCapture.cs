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

public class WebcamCapture : MonoBehaviour
{
    public GameObject ground;
    private PlaceAsset placement;
    public int scale = 10;
    private Vector3 meanPosBoard = Vector3.zero;

    public RawImage webcamScreen;
    public int width, height;

    VideoCapture webcam;
    Mat webcamCapture = new Mat();

    private Mat cameraMatrix;
    private Mat distCoeffs;

    [HideInInspector]
    public MarkerDetection markerDetectionInstance = new MarkerDetection();
    [HideInInspector]
    public PoseEstimation poseEstimationInstance = new PoseEstimation();
    [HideInInspector]
    public CalibrateCamera callibInstance = new CalibrateCamera();


    // Start is called before the first frame update
    void Start()
    {
        placement = ground.GetComponent<PlaceAsset>();
        webcam = new VideoCapture(0);
        if (webcam.IsOpened)
        {
            webcam.ImageGrabbed += HandleWebcamQueryFrame;
            webcam.Start();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (webcam.IsOpened)
        {
            webcam.Grab();
            DisplayFrameOnPlane();
        }

    }

    private void OnDestroy()
    {
        if (webcam != null)
        {
            webcam.Stop();
            webcam.Dispose();
        }
    }
    

    private void DisplayFrameOnPlane()
    {
        CvInvoke.Resize(webcamCapture, webcamCapture, new System.Drawing.Size(width, height));
        CvInvoke.CvtColor(webcamCapture, webcamCapture, ColorConversion.Bgr2Rgba);
        CvInvoke.Flip(webcamCapture, webcamCapture, FlipType.Vertical);


        Texture2D tex = webcamCapture.ToTexture2D();

        tex.LoadRawTextureData(webcamCapture.ToImage<Rgba, byte>().Bytes);
        tex.Apply();

        webcamScreen.texture = tex;

    }

    private ((Mat, Mat), VectorOfInt) estimatepos()
    {
        webcam.Retrieve(webcamCapture);

        //plateaux
        (Mat, Mat) camParams = callibInstance.Calibrate(webcamCapture);
        cameraMatrix = camParams.Item1;
        distCoeffs = camParams.Item2;
        poseEstimationInstance.CameraMatrix = cameraMatrix;
        poseEstimationInstance.DistCoeffs = distCoeffs;

        (VectorOfVectorOfPointF, VectorOfInt) markersInfo = markerDetectionInstance.Detect(webcamCapture);
        poseEstimationInstance.MarkersCorners = markersInfo.Item1;
        poseEstimationInstance.MarkerSize = 0.5f;
        return (poseEstimationInstance.Estimate(scale), markersInfo.Item2);
    }

    private void HandleWebcamQueryFrame(object sender, System.EventArgs e)
    {

        ((Mat, Mat), VectorOfInt) temp = estimatepos();
        (Mat, Mat) transformationVector = temp.Item1;
        VectorOfInt markersInfo = temp.Item2;

        //marqueurs
        for (int i = 0; i < transformationVector.Item1.Size.Height; ++i)
        {
            Mat rvec2 = transformationVector.Item1.Row(i);
            Mat tvec2 = transformationVector.Item2.Row(i);
            ArucoInvoke.DrawAxis(webcamCapture, cameraMatrix, distCoeffs, rvec2, tvec2, 0.5f);
        
            placement.displayAsset(rvec2, tvec2, meanPosBoard, markersInfo[i]);
            
        }
        
    }

    public void createGround()
    {
        ((Mat, Mat), VectorOfInt) temp = estimatepos();
        (Mat, Mat) transformationVector = temp.Item1;
        VectorOfInt markersInfo = temp.Item2;

        //calibrate ground during 50 frames
        int numberOfCalibratingFrames = 50;
        int currentNumberOfCalibratingFrames = 0;
        CreateGround creationPt = ground.GetComponent<CreateGround>();
        while (currentNumberOfCalibratingFrames <= numberOfCalibratingFrames)
        {
            temp = estimatepos();
            transformationVector = temp.Item1;
            markersInfo = temp.Item2;

            for (int i = 0; i < transformationVector.Item1.Size.Height; ++i)
            {
                creationPt.modifyGroundPoint(transformationVector.Item1.Row(i), transformationVector.Item2.Row(i), markersInfo[i]);
            }
            currentNumberOfCalibratingFrames++;
        }

        //position of unity camera = mean of all of the point
        
        for (int i = 0; i < transformationVector.Item2.Size.Height; ++i)
        {
            Vector3 localPos;
            localPos.x = float.Parse(transformationVector.Item2.Row(i).GetData().GetValue(0, 0, 0).ToString());
            localPos.y = -float.Parse(transformationVector.Item2.Row(i).GetData().GetValue(0, 0, 1).ToString());
            localPos.z = float.Parse(transformationVector.Item2.Row(i).GetData().GetValue(0, 0, 2).ToString());
            meanPosBoard += Camera.main.transform.TransformPoint(localPos);
            //rotBoard = transformationVector.Item1.Row(i);
        }
        if (transformationVector.Item1.Size.Height > 0)
        {
            meanPosBoard /= transformationVector.Item1.Size.Height;
        }

        //Debug.Log(meanPosBoard);
        creationPt.recenter(meanPosBoard);
        //Camera.main.transform.position = meanPosBoard + new Vector3(0, 100, 0);
        Camera.main.transform.LookAt(ground.transform.position);
    }
}
