using System.Management;

namespace FireDetectionWebcam.Services
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    internal class GPUInfoServices
    {
        public static bool haveGPUNvidia()
        {
            using var searcher = new ManagementObjectSearcher("select * from Win32_VideoController");
            foreach (ManagementObject obj in searcher.Get())
            {
                string gpuName = "" + obj["Name"];
                if (gpuName.Contains("Nvidia")) return true;
            }
            return false;
        }
    }
}
