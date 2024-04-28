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
        public int CameraDeviceId ;
        private Image _imageControl;
        private CancellationTokenSource _cancellationTokenSource;

        //private int _currentFrameCount = 0;
        //private const int _detectEveryNFrame = 2;

        public bool OnUseGPU;
        public bool OnYoloDetect;

        public WebcamStreamServices(
            Image imageControl,
            int cameraDeviceId)
        {
            _imageControl = imageControl;
            CameraDeviceId = cameraDeviceId;
            OnUseGPU = false;
        }

        public async Task Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _previewTask = Task.Run(async () =>
            {
                try
                {
                    var videoCapture = new VideoCapture();

                    if (!videoCapture.Open(CameraDeviceId))
                    {
                        throw new ApplicationException("Cannot connect to camera");
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
                                    if (OnYoloDetect)
                                    {
                                        if (OnUseGPU)
                                        {
                                            _lastFrame = await PredictServices.PredictAsyncWithGPU(_lastFrame);
                                        }
                                        else
                                        {
                                            _lastFrame = await PredictServices.PredictAsyncWithCPU(_lastFrame);
                                        }
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
                }catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
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
