# Fire Detection Webcam

This project be created a software for fire detection with webcam in real time with YOLOv8 model

## Getting Started

- C# (.NET 8.0)
  + OOP
  + Task
  + Event
- WPF
- MVVM
- YOLOv8 (convert to ONNX)

### Prerequisites

- Visual studio 2022 Community (professional or enterprise)
- Windows 10, 11


### Installation

- git clone .
- Install .NET 8.0 (if not already installed)
- Open .sln file by Visual studio 2022 
- Install package (VS 2022 will auto install package)
- Build

## Features
- Real-time fire detection: This application uses a webcam to detect fires in real-time.
- Supports USB Webcam and Wifi Webcam: You can use both a USB Webcam and a Wifi Webcam (using RTSP url) with this application.

### How to use

- B1. Chose Webcam in combo Box if you want use Webcam USB, or enter the rtsp url of Webcam Wifi (if you use this, check Webcam Wifi box)
- B2: Click "Start"
- B3: Check use detect to run detect webcam (result with show up in view)

#### Note

- Software not support GPU yet
- It may be delayed whene use Webcam Wifi, need strong wifi
- When running the webcam along with fire detection, the FPS will be reduced if your CPU is not strong enough
- you can set iou and confidence in FireDetectionViewModel 

## Contributing

- GuenKainto (Nguyễn Khánh Thọ)
