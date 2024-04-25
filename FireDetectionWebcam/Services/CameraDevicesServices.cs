using DirectShowLib;
using FireDetectionWebcam.Models;
using System.Collections.Generic;

namespace FireDetectionWebcam.Services
{
    internal static class CameraDevicesServices
    {
        internal static List<CameraDevice> GetAllCameraConnected()
        {
            var list = new List<CameraDevice>();
            var listCameraDevicesConnected = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            
            int openCvId = 0;
            foreach (var device in listCameraDevicesConnected)
            {
                CameraDevice cameraDevice = new CameraDevice(openCvId, device.Name, device.DevicePath);
                list.Add(cameraDevice);
                openCvId++;
            }
            return list;

        }
    }
}
