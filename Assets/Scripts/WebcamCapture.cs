using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

public class WebcamCapture : MonoBehaviour
{
    public RawImage webcamScreen;
    public int width, height;

    VideoCapture webcam;
    Mat webcamCapture = new Mat();

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
