using System.Management;
using System.Windows;

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
                MessageBox.Show("Name:" + gpuName);
                if (gpuName.Contains("NVIDIA")) return true;
            }
            return false;
        }
    }
}
