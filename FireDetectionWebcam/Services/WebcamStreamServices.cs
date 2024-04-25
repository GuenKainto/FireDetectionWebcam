using Compunet.YoloV8;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.WpfExtensions;
using System;
using System.IO;
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
        private int _width;
        private int _height;
        private CancellationTokenSource _cancellationTokenSource;
        private YoloV8Predictor predictor;

        public event EventHandler OnYoloDetect;

        public WebcamStreamServices(
            Image imageControl,
            int webcamWidth,
            int webcamHeight,
            int cameraDeviceId)
        {
            _imageControl = imageControl;
            _width = webcamWidth;
            _height = webcamHeight;
            CameraDeviceId = cameraDeviceId;

            var modelPath = Path.Combine(Environment.CurrentDirectory, "Assets\\model\\best.onnx");
            predictor = YoloV8Predictor.Create(modelPath);
        }

        public async Task Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _previewTask = Task.Run(async () =>
            {
                try
                {
                    // Creation and disposal of this object should be done in the same thread 
                    // because if not it throws disconnectedContext exception
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
                                
                                if (OnYoloDetect != null)
                                {
                                    _lastFrame = await PredictServices.PredictAsync(_lastFrame);
                                    //var image = ConvertImageTypeServices.ConvertToImageSharpImage(systemDrawingImage: _lastFrame);
                                    //ImageSelector imageSelector = new(image);
                                    //var result = predictor.Detect(imageSelector);
                                    //var plotted = await result.PlotImageAsync(image);
                                    //_lastFrame = (System.Drawing.Bitmap)ConvertImageTypeServices.ConvertToSystemDrawingImage(imageSharpImage: plotted);
                                }

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
                // To let the exceptions exit
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
                //wait for done previewTask
                await _previewTask;
            }
        }
        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}
