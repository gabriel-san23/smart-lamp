# 💡 Smart Lamp IoT

<p align="center">

![C#](https://img.shields.io/badge/C%23-ASP.NET_MVC-512BD4?style=for-the-badge&logo=csharp&logoColor=white)
![ESP32](https://img.shields.io/badge/ESP32-IoT-E7352C?style=for-the-badge&logo=espressif&logoColor=white)
![MQTT](https://img.shields.io/badge/MQTT-Protocol-660066?style=for-the-badge&logo=mqtt&logoColor=white)
![AWS](https://img.shields.io/badge/AWS-EC2-FF9900?style=for-the-badge&logo=amazonaws&logoColor=white)
![FIWARE](https://img.shields.io/badge/FIWARE-Open_Source_IoT-233559?style=for-the-badge)
![Chart.js](https://img.shields.io/badge/Chart.js-Data_Visualization-FF6384?style=for-the-badge&logo=chartdotjs&logoColor=white)

</p>

<p align="center">
  IoT system for remote control and monitoring of a smart RGB lamp using ESP32, MQTT and FIWARE.
</p>

---

# 📖 About the Project

**Smart Lamp IoT** is an Internet of Things (IoT) project developed to remotely control and monitor an RGB smart lamp through a web interface connected to a physical ESP32-based device.

The project uses the MQTT protocol and the FIWARE ecosystem to enable communication between services, allowing real-time monitoring, remote device control and temporal storage of luminosity measurements.

In addition to controlling the RGB lamp, the system also monitors ambient light using an LDR sensor, displaying the collected data through dynamic charts built with Chart.js.

---

# 🚀 Features

✅ Turn RGB LED on/off  
✅ Change lamp colors  
✅ Brightness control  
✅ Ambient light monitoring  
✅ Historical luminosity records  
✅ Real-time luminosity chart  
✅ Service health check  
✅ Device provisioning and registration  
✅ Real-time MQTT communication  

---

# 🧰 Technologies Used

## 💻 Backend

- C#
- ASP.NET MVC

## 🌐 Frontend

- HTML5
- CSS3
- JavaScript
- Chart.js

## ☁️ Cloud & IoT

- AWS EC2
- Ubuntu Server
- MQTT
- FIWARE
- Orion Context Broker
- STH-Comet

## 🔌 Hardware

- ESP32
- RGB LED
- LDR Sensor
- Breadboard
- Resistors
- Jumper Wires

---

# 🏗️ System Architecture

```text
User
   ↓
Web Interface
(HTML/CSS/JS)
   ↓
ASP.NET MVC Backend
(C#)
   ↓
FIWARE / Orion Context Broker
   ↓
MQTT
   ↓
ESP32
   ↓
RGB LED + LDR Sensor
```

---

# 📂 Project Structure

```bash
SmartLamp/
├── csharp-app/
│   ├── SmartLamp/
│   └── SmartLamp.sln
│
├── esp32/
│   └── smart-lamp-rgb.ino
│
├── docs/
│
├── README.md
├── LICENSE
└── .gitignore
```

---

# 📡 IoT Communication

The ESP32 was integrated with the FIWARE ecosystem using the MQTT protocol, enabling efficient real-time communication between the physical device and the web application.

Through this integration, the system is capable of:

- receiving remote commands;
- updating device states;
- sending sensor data;
- storing temporal measurements;
- performing continuous monitoring.

---

# 📊 Monitoring & Historical Data

The system reads ambient luminosity using an LDR sensor connected to the ESP32.

The collected data is sent to FIWARE and later displayed on the web interface through dynamic charts developed with Chart.js.

## Displayed Information

- Current luminosity level
- Historical luminosity records
- Luminosity × time chart
- Current lamp status

---

# ⚙️ How to Run

## 🔹 ASP.NET MVC Backend

1. Open the solution:

```bash
csharp-app/SmartLamp.sln
```

2. Run the project using Visual Studio.

---

## 🔹 ESP32

1. Open the file:

```bash
esp32/smart-lamp-rgb.ino
```

2. Configure:

- Wi-Fi credentials
- MQTT credentials
- FIWARE parameters

3. Upload the code to the ESP32 using the Arduino IDE.

---

# 📌 Available Operations

| Operation | Description |
|---|---|
| Health Check | Verifies service availability |
| Service Listing | Lists FIWARE services |
| Device Provisioning | Registers device in IoT Agent |
| Device Registration | Registers device in Orion Context Broker |
| On/Off Control | Turns the LED on or off |
| Color Change | Changes RGB LED color |
| Brightness Adjustment | Adjusts lamp brightness |
| Historical Query | Retrieves temporal data |

---

# 🖼️ Demonstration

## 🔧 Physical Prototype

---

## 🌐 Web Interface

---

## 📈 Monitoring Dashboard

---

## 📮 API Tests

---

# 👨‍💻 Team Members

- Daniel Cataneo
- Felipe Nascimento Silva
- Felipe Stefanes Dessico 
- Gabriel Santos Galvão
- Oliver Carraro

---

# 📄 License

This project is licensed under the terms specified in the `LICENSE` file.

---

<p align="center">
  Developed for academic purposes 🚀
</p>
