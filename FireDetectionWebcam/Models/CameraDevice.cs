namespace FireDetectionWebcam.Models
{
    internal class CameraDevice
    {
        public int OpenCvId { get; set; }
        public string Name { get; set; }
        public string DeviceId { get; set; }

        public CameraDevice(int openCvId, string name, string deviceId)
        {
            OpenCvId = openCvId;
            Name = name;
            DeviceId = deviceId;
        }
    }
}
