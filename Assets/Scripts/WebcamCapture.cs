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
    public RawImage webcamScreen;
    public int width, height;

    VideoCapture webcam;
    Mat webcamCapture = new Mat();

    private Mat cameraMatrix;
    private Mat distCoeffs;

    private MarkerDetection markerDetectionInstance = new MarkerDetection();
    private PoseEstimation poseEstimationInstance = new PoseEstimation();

    // Start is called before the first frame update
    void Start()
    {
        webcam = new VideoCapture(0);
        if (webcam.IsOpened) {

            webcam.ImageGrabbed += HandleWebcamQueryFrame;
            webcam.Start();

            CalibrateCamera callibInstance = new CalibrateCamera();
            (Mat, Mat) camParams = callibInstance.Calibrate("/Assets/Resources/cameraParameters.xml");
            cameraMatrix = camParams.Item1;
            distCoeffs = camParams.Item2;
            poseEstimationInstance.CameraMatrix = cameraMatrix;
            poseEstimationInstance.DistCoeffs = distCoeffs;
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
        (VectorOfVectorOfPointF, VectorOfInt) markersInfo = markerDetectionInstance.Detect(webcamCapture);
        poseEstimationInstance.MarkersCorners = markersInfo.Item1;
        poseEstimationInstance.MarkerSize = 0.5f;
        //poseEstimationInstance.Estimate();
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
