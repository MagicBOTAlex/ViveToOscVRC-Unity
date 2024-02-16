using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UnityEngine;
using Rug.Osc;
using Valve.VR;
using UnityEngine.Rendering.VirtualTexturing;

public class OSC_Service : MonoBehaviour
{
    [SerializeField] private string Ip_address = "192.168.0.37";
    [SerializeField] private int port = 9000;

    [SerializeField] private float TickDelay = 0.1f;
    float lowestPoint = 0; // Used for floor-offset (Maybe auto calibrated ¯\_(ツ)_/¯)

    private OscSender sender;

    private void Start()
    {
        IPAddress address = IPAddress.Parse(Ip_address);

        sender = new OscSender(address, port);
        sender.Connect();

        StartCoroutine(GetTrackerDataLoop());
    }

    IEnumerator GetTrackerDataLoop()
    {
        while (true)
        {
            // If tracker flies away, then reset floor offset
            if (Mathf.Abs(lowestPoint) > 3)
            {
                lowestPoint = 0;
            }

            for (uint i = 0; i < OpenVR.k_unMaxTrackedDeviceCount; i++)
            {
                var deviceClass = OpenVR_Service.vrSystem.GetTrackedDeviceClass(i);
                if (deviceClass != ETrackedDeviceClass.GenericTracker) continue;


                TrackerInfo? trackerInfo = OpenVR_Service.GetTrackerPose(i);
                if (trackerInfo == null) continue;

                Vector3 pos = trackerInfo.Value.Position;
                Vector3 rot = trackerInfo.Value.EulerRotation;
                //rot *= 180;

                //rot = new Vector3(rot.X, rot.Y, rot.Z);

                if (lowestPoint > pos.y)
                {
                    lowestPoint = pos.y;
                }
                else
                {
                    pos.y -= lowestPoint;
                }


                sender.Send(new OscMessage($"/tracking/trackers/{i}/position", pos.x, pos.y, pos.z));
                sender.Send(new OscMessage($"/tracking/trackers/{i}/rotation", rot.x, rot.y, rot.z));
            }

            yield return new WaitForSecondsRealtime(TickDelay);
        }
    }
}
