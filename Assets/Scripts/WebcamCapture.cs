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
    public PlaceAsset placement;

    public RawImage webcamScreen;
    public int width, height;

    VideoCapture webcam;
    Mat webcamCapture = new Mat();

    private Mat cameraMatrix;
    private Mat distCoeffs;

    private MarkerDetection markerDetectionInstance = new MarkerDetection();
    private PoseEstimation poseEstimationInstance = new PoseEstimation();
    private CalibrateCamera callibInstance = new CalibrateCamera();

    private int numberOfCalibratingFrames = 0;
    private int currentNumberOfCalibratingFrames = 0;

    // Start is called before the first frame update
    void Start()
    {
        webcam = new VideoCapture(0);
        if (webcam.IsOpened) {

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

    private void HandleWebcamQueryFrame(object sender, System.EventArgs e)
    {
        webcam.Retrieve(webcamCapture);
        if(currentNumberOfCalibratingFrames <= numberOfCalibratingFrames)
        {
            (Mat, Mat) camParams = callibInstance.Calibrate(webcamCapture);
            cameraMatrix = camParams.Item1;
            distCoeffs = camParams.Item2;
            poseEstimationInstance.CameraMatrix = cameraMatrix;
            poseEstimationInstance.DistCoeffs = distCoeffs;
            currentNumberOfCalibratingFrames += 1;
        }
        else
        {
            (VectorOfVectorOfPointF, VectorOfInt) markersInfo = markerDetectionInstance.Detect(webcamCapture);
            poseEstimationInstance.MarkersCorners = markersInfo.Item1;
            poseEstimationInstance.MarkerSize = 0.5f;
            (Mat, Mat) transformationVector = poseEstimationInstance.Estimate();

            for(int i = 0; i < transformationVector.Item1.Size.Height; ++i)
            {
                Mat rvec = transformationVector.Item1.Row(i);
                Mat tvec = transformationVector.Item2.Row(i);
                ArucoInvoke.DrawAxis(webcamCapture, cameraMatrix, distCoeffs, rvec, tvec, 0.5f);
                placement.displayAsset(rvec, tvec, markersInfo.Item2[i]);
                //Debug.Log(markersInfo.Item2[i]);
            }
        }

        System.Threading.Thread.Sleep(100);

        
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
}
