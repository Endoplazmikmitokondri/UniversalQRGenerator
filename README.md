# Universal QR Code Generator 🔳

![.NET 10](https://img.shields.io/badge/.NET-10.0-blue.svg)
![Platform](https://img.shields.io/badge/Platform-Windows-lightgrey.svg)
![License](https://img.shields.io/badge/License-MIT-green.svg)

**Universal QR Code Generator** is an easy-to-use, open-source QR code generation tool developed for Windows. In addition to standard text, it supports special data types such as Wi-Fi, VCard, and WhatsApp.

## ✨ Features

* **Wide Data Support:**

  * Plain Text / URL
  * Wi-Fi Networks (Automatic connection without manually entering the password)
  * VCard (Automatically add contacts to the address book)
  * WhatsApp Messages (Start a chat directly)

* **Customizable Design:**

  * Change QR code and background colors as you wish.
  * Smart contrast control: The system automatically warns you if the selected colors are too similar to be readable.

* **Logo Integration:**

  * Add your own custom logo (PNG/JPG) to the center of the QR code.
  * Adjust the percentage of the QR area covered by the logo (Displays a warning for sizes >20% to preserve readability).

* **Fast Export:** Save generated QR codes in high-resolution PNG format.

## 🚀 Installation & Usage

### 1. Installation via Setup File (Recommended)

To install the application directly on your computer, download the latest `UniversalQRGenerator_Setup.exe` file from the **Releases** section and run it. (.NET installation is not required; the application runs as self-contained.)

### 2. For Developers (Build from Source)

If you want to build or modify the project on your own machine:

1. Clone the repository:

```bash
git clone https://github.com/Endoplazmikmitokondri/UniversalQRGenerator.git
```
