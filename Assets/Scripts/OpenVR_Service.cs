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
            position = new Vector3(-position.x, position.y, position.z);

            // Rotation
            Quaternion quaternion = mat34.ExtractRotation();

            quaternion = Quaternion.Euler(0, 180, 0) * quaternion; // Reverse yaw

            Vector3 rotation = quaternion.eulerAngles;

            // Again, i don't know why
            //rotation = new Vector3(-rotation.x, rotation.y, rotation.z);

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
