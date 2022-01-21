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

public class MarkerDetection
{
    public MarkerDetection()
    {
    }

    public (VectorOfVectorOfPointF, VectorOfInt) Detect(Mat image) {
        // Markers ID
        VectorOfInt markersID = new VectorOfInt();
        // marker corners & rejected candidates
        VectorOfVectorOfPointF markersCorner = new VectorOfVectorOfPointF();
        VectorOfVectorOfPointF rejectedCandidates = new VectorOfVectorOfPointF();
        // Detector parameters for tuning the algorithm
        DetectorParameters parameters = new DetectorParameters();
        parameters = DetectorParameters.GetDefault();
        // dictionary of aruco's markers
        Dictionary dictMarkers = new Dictionary(Dictionary.PredefinedDictionaryName.Dict6X6_250);

        // convert image
        Mat grayFrame = new Mat(image.Width, image.Height, DepthType.Cv8U, 1);
        CvInvoke.CvtColor(image, grayFrame, ColorConversion.Bgr2Gray);
        // detect markers
        ArucoInvoke.DetectMarkers(grayFrame, dictMarkers, markersCorner, markersID, parameters, rejectedCandidates);

        if (markersID.Size > 0)
        {
            Debug.Log("Markers found");
        }

        return (markersCorner, markersID);
    }
}
