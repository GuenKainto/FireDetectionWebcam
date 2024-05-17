using FireDetectionWebcam.Models;
using FireDetectionWebcam.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace FireDetectionWebcam.ViewModels
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    internal class WebcamViewModel : BindableBase
    {
        #region properties
        private WebcamStreamServices _webcamStreamServices;
        public ObservableCollection<CameraDevice> ListCameras { get; set; }

        private CameraDevice _cameraSelected;
        public CameraDevice CameraSelected
        {
            get => _cameraSelected;
            set
            {
                if (_cameraSelected != value)
                {
                    _cameraSelected = value;
                    OnPropertyChanged(nameof(CameraSelected));
                }
            }
        }

        private string _cameraIp;
        public string CameraIp
        {
            get => _cameraIp;
            set
            {
                if(_cameraIp != value)
                {
                    _cameraIp = value;
                    OnPropertyChanged(nameof(CameraIp));
                }
            }
        }

        private bool _isUseWebcamWifiChecked;
        public bool IsUseWebcamWifiChecked
        {
            get => _isUseWebcamWifiChecked;
            set
            {
                if (_isUseWebcamWifiChecked != value)
                {
                    _isUseWebcamWifiChecked = value;
                    OnPropertyChanged(nameof(IsUseWebcamWifiChecked));
                }
            }
        }

        private bool _isListCamerasEnabled;
        public bool IsListCamerasEnabled
        {
            get => _isListCamerasEnabled;
            set
            {
                if (_isListCamerasEnabled != value)
                {
                    _isListCamerasEnabled = value;
                    OnPropertyChanged(nameof(IsListCamerasEnabled));
                }
            }
        }

        private bool _isCameraIpEnabled;
        public bool IsCameraIpEnabled
        {
            get => _isCameraIpEnabled;
            set
            {
                if (_isCameraIpEnabled != value)
                {
                    _isCameraIpEnabled = value;
                    OnPropertyChanged(nameof(IsCameraIpEnabled));
                }
            }
        }

        private bool _isUseWebcamWifiEnabled;
        public bool IsUseWebcamWifiEnabled
        {
            get => _isUseWebcamWifiEnabled;
            set
            {
                if (_isUseWebcamWifiEnabled != value)
                {
                    _isUseWebcamWifiEnabled = value;
                    OnPropertyChanged(nameof(IsUseWebcamWifiEnabled));
                }
            }
        }

        private bool _isYoloChecked;
        public bool IsYoloChecked
        {
            get => _isYoloChecked;
            set
            {
                if (_isYoloChecked != value)
                {
                    _isYoloChecked = value;
                    OnPropertyChanged(nameof(IsYoloChecked));
                    if (IsYoloChecked) YoloDetectCB_Checked();
                    else YoloDetectCB_UnChecked();
                }
            }
        }

        private bool _isYoloEnabled;
        public bool IsYoloEnabled
        {
            get => _isYoloEnabled;
            set
            {
                if (_isYoloEnabled != value)
                {
                    _isYoloEnabled = value;
                    OnPropertyChanged(nameof(IsYoloEnabled));
                }
            }
        }

        private bool _isStartEnabled;
        public bool IsStartEnabled
        {
            get => _isStartEnabled;
            set
            {
                if (_isStartEnabled != value)
                {
                    _isStartEnabled = value;
                    OnPropertyChanged(nameof(IsStartEnabled));
                }
            }
        }

        private bool _isStopEnabled;
        public bool IsStopEnabled
        {
            get => _isStopEnabled;
            set
            {
                if (_isStopEnabled != value)
                {
                    _isStopEnabled = value;
                    OnPropertyChanged(nameof(IsStopEnabled));
                }
            }
        }

        private bool _isReloadCameraDevicesEnabled;
        public bool IsReloadCameraDevicesEnabled
        {
            get => _isReloadCameraDevicesEnabled;
            set 
            {
                if (_isReloadCameraDevicesEnabled != value)
                {
                    _isReloadCameraDevicesEnabled = value;
                    OnPropertyChanged(nameof(IsReloadCameraDevicesEnabled));
                }
            }
        }
        #endregion

        #region Command
        public MyCommand LoadedCommand { get; set; }
        private void OnLoaded(object obj)
        {
            if (obj is Views.WebcamView)
            {
                LoadCameraDevicesCmB();
                IsYoloEnabled = false;
                IsStartEnabled = true;
                IsStopEnabled = false;
                IsUseWebcamWifiEnabled = true;
                IsListCamerasEnabled = true;
                IsCameraIpEnabled = true;
                IsReloadCameraDevicesEnabled = true;
            }
        }
        public MyCommand StartCommand { get; set; }
        private async void OnStart(object obj)
        {
            if (obj is Views.WebcamView wd)
            {
                IsReloadCameraDevicesEnabled = false;
                IsStopEnabled = true;
                IsStartEnabled = false;
                var previousStateIsYoloChecked = IsYoloChecked;
                //if (_webcamStreamServices == null || _webcamStreamServices.cameraDeviceId != CameraSelected.OpenCvId || CameraIp.Length != 0)
                if (CameraSelected != null || (CameraIp != null && CameraIp.Length != 0))
                {
                    IsUseWebcamWifiEnabled = false;
                    IsListCamerasEnabled = false;
                    IsCameraIpEnabled = false;
                    IsYoloEnabled = false;
                    IsYoloChecked = false;
                    if (_webcamStreamServices != null)
                    {
                        _webcamStreamServices?.Dispose();
                    }

                    string cameraIp = "";
                    int openCVId = -1;
                    if (IsUseWebcamWifiChecked == true)
                    {
                        if (CameraIp != null && CameraIp.Length != 0)
                        {
                            cameraIp = CameraIp;
                            _webcamStreamServices = new WebcamStreamServices(
                               imageControl: wd.webcamPreview,
                               cameraDeviceId: openCVId,
                               cameraIp: cameraIp,
                               iou: 0.45f,
                               confidence: 0.329f,
                               isUseWebcamWifi: true
                           );
                        }
                        else
                        {
                            MessageBox.Show("Please enter the ip of camera");
                            ReloadEnable();
                        }
                    }
                    else
                    {
                        if (CameraSelected != null)
                        {
                            openCVId = CameraSelected.OpenCvId;
                            _webcamStreamServices = new WebcamStreamServices(
                               imageControl: wd.webcamPreview,
                               cameraDeviceId: openCVId,
                               cameraIp: cameraIp,
                               iou: 0.45f,
                               confidence: 0.329f,
                               isUseWebcamWifi: false
                           );
                        }
                        else
                        {
                            MessageBox.Show("Please the camera");
                        }
                    }
                    try
                    {
                        await _webcamStreamServices.Start();
                        IsYoloEnabled = true;
                        if (previousStateIsYoloChecked) IsYoloChecked = true;
                    }
                    catch (ApplicationException)
                    {
                        MessageBox.Show("Error: Can't connect to webcam, please check again !", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        ReloadEnable();
                    }


                }
                else
                {
                    MessageBox.Show("Please chose webcam or enter the ip of webcam!");
                    ReloadEnable();
                }
            }
        }
        public MyCommand StopCommand { get; set; }
        private async void OnStop(object obj)
        {
            if (obj is Views.WebcamView)
            {
                await _webcamStreamServices.Stop();
                ReloadEnable();
            }
        }

        public MyCommand ReloadCameraDevicesCommand { get; set; }
        private void OnReloadCameraDevices(object obj)
        {
            if (obj is Views.WebcamView)
            {
                if (_webcamStreamServices == null) 
                {
                    LoadCameraDevicesCmB();
                    CameraSelected = null;
                }
            }
        }
        #endregion
    
        public WebcamViewModel()
        {
            Init_Command();
            Init_Model();
        }

        private void Init_Model()
        {
            ListCameras = [];
        }

        private void Init_Command()
        {
            LoadedCommand = new MyCommand(OnLoaded, () => true);
            StartCommand = new MyCommand(OnStart, () => true);
            StopCommand = new MyCommand(OnStop, () => true);
        }

        private void LoadCameraDevicesCmB()
        {
            var listCamera = CameraDevicesServices.GetAllCameraConnected();
            foreach (var camera in listCamera)
            {
                ListCameras.Add(camera);
            }
        }

        private void YoloDetectCB_Checked()
        {
            if (_webcamStreamServices != null)
            {
                _webcamStreamServices.onYoloDetect = true;
            }
        }

        private void YoloDetectCB_UnChecked()
        {
            if (_webcamStreamServices != null)
            {
                _webcamStreamServices.onYoloDetect = false;
            }
        }

        private void ReloadEnable()
        {
            IsUseWebcamWifiEnabled = true;
            IsListCamerasEnabled = true;
            IsCameraIpEnabled = true;
            IsYoloEnabled = false;
            IsYoloChecked = false;
            IsStartEnabled = true;
            IsStopEnabled = false;
            IsReloadCameraDevicesEnabled = true;
        }
    }
}
