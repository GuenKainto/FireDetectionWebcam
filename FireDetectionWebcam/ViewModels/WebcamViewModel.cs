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

        private string _gpuSupport;
        public string GPUSupport
        {
            get => _gpuSupport;
            set
            {
                if(_gpuSupport != value)
                {
                    _gpuSupport = value;
                    OnPropertyChanged(nameof(GPUSupport));
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

        private bool _isUseGPUChecked;
        public bool IsUseGPUChecked
        {
            get => _isUseGPUChecked;
            set
            {
                if (_isUseGPUChecked != value)
                {
                    _isUseGPUChecked = value;
                    OnPropertyChanged(nameof(_isUseGPUChecked));
                    if (IsUseGPUChecked) IsUseGPUCB_Checked();
                    else IsUseGPUCB_UnChecked();
                }
            }
        }

        private bool _isUseGPUEnabled;
        public bool IsUseGPUEnabled
        {
            get => _isUseGPUEnabled;
            set
            {
                if (_isUseGPUEnabled != value)
                {
                    _isUseGPUEnabled = value;
                    OnPropertyChanged(nameof(IsUseGPUEnabled));
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
                IsUseGPUEnabled = false;
                IsStartEnabled = true;
                IsStopEnabled = false;
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
                var previousStateIsUseGPUChecked = IsUseGPUChecked;
                if (_webcamStreamServices == null || _webcamStreamServices.CameraDeviceId != CameraSelected.OpenCvId)
                {
                    IsYoloEnabled = false;
                    IsYoloChecked = false;
                    IsUseGPUEnabled = false;
                    IsUseGPUChecked = false;
                    _webcamStreamServices?.Dispose();
                    _webcamStreamServices = new WebcamStreamServices(
                        imageControl: wd.webcamPreview,
                        cameraDeviceId: CameraSelected.OpenCvId
                    );
                }

                try
                {
                    await _webcamStreamServices.Start();
                    IsYoloEnabled = true;
                    if (previousStateIsYoloChecked) IsYoloChecked = true;

                    if (GPUInfoServices.haveGPUNvidia())
                    {
                        GPUSupport = "Your device have GPU Nvidia\nGPU is supported ";
                        IsUseGPUEnabled = true;
                        if (previousStateIsUseGPUChecked) IsUseGPUChecked = true;
                    }
                    else 
                    {
                        GPUSupport = "Your device don't have GPU Nvidia\nNot support with GPU ";
                        IsUseGPUEnabled = false;
                        IsUseGPUChecked = false;
                    } 
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public MyCommand StopCommand { get; set; }
        private async void OnStop(object obj)
        {
            if (obj is Views.WebcamView)
            {
                await _webcamStreamServices.Stop();

                IsYoloEnabled = false;
                IsYoloChecked = false;
                IsUseGPUEnabled = false;
                IsUseGPUChecked = false;
                IsStartEnabled = true;
                IsStopEnabled = false;
                IsReloadCameraDevicesEnabled = true;
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
                _webcamStreamServices.OnYoloDetect = true;
            }
        }

        private void YoloDetectCB_UnChecked()
        {
            if (_webcamStreamServices != null)
            {
                _webcamStreamServices.OnYoloDetect = false;
            }
        }

        private void IsUseGPUCB_Checked()
        {
            if(_webcamStreamServices != null)
            {
                _webcamStreamServices.OnUseGPU = true;
            }
        }

        private void IsUseGPUCB_UnChecked()
        {
            if (_webcamStreamServices != null)
            {
                _webcamStreamServices.OnUseGPU = false;
            }
        }
    }
}
