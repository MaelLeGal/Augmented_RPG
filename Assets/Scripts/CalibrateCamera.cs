using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Aruco;
using Emgu.CV.Util;
public class CalibrateCamera
{

    private VectorOfVectorOfPointF allCharucoCorners = new VectorOfVectorOfPointF();
    private VectorOfInt allCharucoIds = new VectorOfInt();

    private int numberOfCalibratingFrames = 250;
    private int currentNumberOfCalibratingFrames = 0;
    public CalibrateCamera()
    {

    }

    public (Mat, Mat) Calibrate(Mat image)
    {
        /*System.Drawing.Size imgSize = new System.Drawing.Size(500, 500);

        CharucoBoard board = new CharucoBoard(7, 5, 0.2f, 0.03f, new Dictionary(Dictionary.PredefinedDictionaryName.Dict6X6_250));

        VectorOfVectorOfPointF charucoCorners = new VectorOfVectorOfPointF();
        VectorOfInt charucoIds = new VectorOfInt();
        VectorOfVectorOfPointF rejectedCandidates = new VectorOfVectorOfPointF();


        DetectorParameters parameters = new DetectorParameters();
        parameters = DetectorParameters.GetDefault();
        // dictionary of aruco's markers
        Dictionary dictMarkers = new Dictionary(Dictionary.PredefinedDictionaryName.Dict6X6_250);

        // convert image
        Mat grayFrame = new Mat(image.Width, image.Height, DepthType.Cv8U, 1);
        CvInvoke.CvtColor(image, grayFrame, ColorConversion.Bgr2Gray);
        // detect markers
        ArucoInvoke.DetectMarkers(grayFrame, dictMarkers, charucoCorners, charucoIds, parameters, rejectedCandidates);

        //Debug.Log("Corners : " + charucoCorners.Size);
        //Debug.Log("Ids : " + charucoIds.Size);

        allCharucoCorners.Push(charucoCorners);
        allCharucoIds.Push(charucoIds);

        Mat cameraMatrix = new Mat();
        Mat distCoeffs = new Mat();

        Debug.Log("Corner size : " + allCharucoCorners.Size);
        Debug.Log("Ids size : " + allCharucoIds.Size);
        Debug.Log("Frame number : " + currentNumberOfCalibratingFrames);
        if (currentNumberOfCalibratingFrames == numberOfCalibratingFrames)
        {
            VectorOfMat rvecs = new VectorOfMat();
            VectorOfMat tvecs = new VectorOfMat();
            //Mat rvecs = new Mat();
            //Mat tvecs = new Mat();
            CalibType calibrationFlags = CalibType.Default;
            MCvTermCriteria criteria = new MCvTermCriteria(30,0.01);

            Debug.Log(allCharucoCorners.Size);
            Debug.Log(allCharucoIds.Size);
            Debug.Log(calibrationFlags);
            Debug.Log(criteria.MaxIter);
            Debug.Log(criteria.Epsilon);
            Debug.Log(criteria.Type);

            ArucoInvoke.CalibrateCameraCharuco(allCharucoCorners, allCharucoIds, board, imgSize, cameraMatrix, distCoeffs, rvecs, tvecs, calibrationFlags, criteria);
            Debug.Log(cameraMatrix.Size);
            Debug.Log(distCoeffs.Size);
        }

        currentNumberOfCalibratingFrames+=1;

        return (cameraMatrix, distCoeffs);*/

        FileStorage fs = new FileStorage("Assets/Resources/cameraParameters.xml", FileStorage.Mode.Read);

        Mat cameraMatrix = new Mat(new System.Drawing.Size(3, 3), DepthType.Cv32F, 1);
        Mat distCoeffs = new Mat(1, 8, DepthType.Cv32F, 1);
        fs["cameraMatrix"].ReadMat(cameraMatrix);
        fs["dist_coeffs"].ReadMat(distCoeffs);

        return (cameraMatrix, distCoeffs);
    }
}
