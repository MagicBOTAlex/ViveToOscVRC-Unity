using Rug.Osc;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using Valve.VR;

public class OpenVR_Service : MonoBehaviour
{
    public static CVRSystem vrSystem = default!;
    private GameObject DummyObject; // Used for getting the correct rotation of the tracker

    void Start()
    {
        InitializeOpenVR();
    }

    public static TrackerInfo? GetTrackerPose(uint trackerIndex)
    {
        var poses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
        vrSystem.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, 0, poses);

        var pose = poses[trackerIndex];
        if (pose.bPoseIsValid)
        {
            HmdMatrix34_t mat34 = pose.mDeviceToAbsoluteTracking;

            // Position
            Vector3 position = new Vector3(mat34.m3,
                                       mat34.m7,
                                       mat34.m11);

            // Don't know why but this has to be done apparently
            position = new Vector3(position.x, position.y, -position.z);

            // Rotation
            Vector3 rotation = mat34.ExtractRotation().eulerAngles;

            //rotation = rotation.ToDegrees();

            //rotation = new Vector3(rotation.z, rotation.x, rotation.y); // rotation not correct
                                                                        //Vector3 rotation = new Vector3(mat34.m2, mat34.m6, mat34.m10);
                                                                        //rotation *= 360;

            TrackerInfo trackerInfo = new TrackerInfo();
            trackerInfo.Position = position;
            trackerInfo.EulerRotation = rotation;

            return trackerInfo;
        }

        return null;
    }

    public static void InitializeOpenVR()
    {
        var error = EVRInitError.None;
        vrSystem = OpenVR.Init(ref error, EVRApplicationType.VRApplication_Background);

        if (error != EVRInitError.None)
        {
            throw new Exception("Unable to initialize OpenVR: " + error.ToString());
        }
    }
}
