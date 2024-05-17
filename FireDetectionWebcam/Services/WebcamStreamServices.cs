using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.WpfExtensions;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FireDetectionWebcam.Services
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    internal class WebcamStreamServices : IDisposable
    {
        private System.Drawing.Bitmap _lastFrame;
        private Task _previewTask;
        private Image _imageControl;
        private CancellationTokenSource _cancellationTokenSource;
        
        public int cameraDeviceId ;
        public string cameraIp;
        public bool onYoloDetect ;
        public bool isUseWebcamWifi;
        public float iou;
        public float confidence;

        //private int _currentFrameCount = 0;
        //private const int _detectEveryNFrame = 2;

        /// <summary>
        /// set cameraDeviceId = -1 and cameraIp not null to use Webcam Wifi
        /// set cameraDeviceID != -1 and cameraIp="" to use Webcam USB
        /// </summary>
        /// <param name="imageControl"></param>
        /// <param name="cameraDeviceId"></param>
        /// <param name="cameraIp"></param>
        public WebcamStreamServices(
            Image imageControl,
            int cameraDeviceId,
            string cameraIp,
            float iou,
            float confidence,
            bool isUseWebcamWifi)
        {
            _imageControl = imageControl;
            this.cameraDeviceId = cameraDeviceId;
            this.cameraIp = cameraIp;
            this.iou = iou;
            this.confidence = confidence;
            this.isUseWebcamWifi = isUseWebcamWifi;
        }
        /// <summary>
        /// start
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _previewTask = Task.Run(async () =>
            {
                try
                {
                    var videoCapture = new VideoCapture();
                    if (cameraIp.Length != 0 && isUseWebcamWifi == true)
                    {
                       // MessageBox.Show("Webcam ip :" + cameraIp);
                        videoCapture = new VideoCapture(cameraIp);
                        if (!videoCapture.Open(cameraIp))
                        {
                            throw new ApplicationException("Cannot connect to camera");
                        }
                    }
                    else if (cameraDeviceId != -1 && isUseWebcamWifi == false)
                    {
                        //MessageBox.Show("Webcam USB");
                        if (!videoCapture.Open(cameraDeviceId))
                        {
                            throw new ApplicationException("Cannot connect to camera");
                        }
                    }
                    using (var frame = new Mat())
                    {
                        while (!_cancellationTokenSource.IsCancellationRequested)
                        {
                            videoCapture.Read(frame);

                            if (!frame.Empty())
                            {
                                _lastFrame = BitmapConverter.ToBitmap(frame);
                                //if (_currentFrameCount % _detectEveryNFrame == 0) 
                                //{
                                    if (onYoloDetect)
                                    {
                                        _lastFrame = await PredictServices.PredictAsyncWithCPU(_lastFrame,iou,confidence);
                                    }
                                //}
                                
                                var lastFrameBitmapImage = _lastFrame.ToBitmapSource();
                                lastFrameBitmapImage.Freeze();
                                _imageControl.Dispatcher.Invoke(() => 
                                    _imageControl.Source = lastFrameBitmapImage
                                );
                            }
                            // 30 FPS
                            await Task.Delay(33);
                        }
                    }

                    videoCapture?.Dispose();
                }catch (ApplicationException)
                {
                    MessageBox.Show("Error: Can't connect to webcam, please check again !", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }, _cancellationTokenSource.Token);

            if (_previewTask.IsFaulted)
            {
                await _previewTask;
            }
        }
        public async Task Stop()
        {
            // If "Dispose" before stop
            if (_cancellationTokenSource.IsCancellationRequested)
                return;

            if (!_previewTask.IsCompleted)
            {
                _cancellationTokenSource.Cancel();
                await _previewTask;
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}
