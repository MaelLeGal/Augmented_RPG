using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Aruco;
using Emgu.CV.Util;
public class PoseEstimation
{
    private VectorOfVectorOfPointF markersCorners;
    public VectorOfVectorOfPointF MarkersCorners { set { markersCorners = value; } }

    private float markerSize;
    public float MarkerSize { set { markerSize = value; } }

    private Mat cameraMatrix;
    public Mat CameraMatrix { set { cameraMatrix = value; } }

    private Mat distCoeffs;
    public Mat DistCoeffs { set { distCoeffs = value; } }
    public PoseEstimation()
    {
    }

    public PoseEstimation(VectorOfVectorOfPointF markersCorners_, float markerSize_, Mat cameraMatrix_, Mat distCoeffs_)
    {
        markersCorners = markersCorners_;
        markerSize = markerSize_;
        cameraMatrix = cameraMatrix_;
        distCoeffs = distCoeffs_;
    }

    public (Mat, Mat) Estimate()
    {
        Mat rvecs = new Mat(); // Rotation Vectors for each marker
        Mat tvecs = new Mat(); // Translation Vectors for each marker

        ArucoInvoke.EstimatePoseSingleMarkers(markersCorners, markerSize, cameraMatrix, distCoeffs, rvecs, tvecs);

        return (rvecs, tvecs);
    }
}
