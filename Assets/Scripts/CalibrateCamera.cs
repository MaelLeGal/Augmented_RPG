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
    public CalibrateCamera()
    {

    }

    public (Mat, Mat) Calibrate(string pathToCameraConfigFile)
    {
        FileStorage fs = new FileStorage(pathToCameraConfigFile, FileStorage.Mode.Read);

        Mat cameraMatrix = new Mat(new System.Drawing.Size(3, 3), DepthType.Cv32F, 1);
        Mat distCoeffs = new Mat(1, 8, DepthType.Cv32F, 1);
        fs["cameraMatrix"].ReadMat(cameraMatrix);
        fs["dist_coeffs"].ReadMat(distCoeffs);

        return (cameraMatrix, distCoeffs);
    }
}
