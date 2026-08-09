# ApneScan

ApneScan is a document scanning application with a focus on simplicity and ease of use. Scan your documents from WIA, TWAIN, SANE, and ESCL scanners, organize the pages as you like, and save them as PDF, TIFF, JPEG, or PNG. Optical character recognition (OCR) is available using [Tesseract](https://github.com/tesseract-ocr/tesseract).

System requirements:
- Windows 7+ (x64, x86)
- macOS 10.15+ (x64, arm64)
- Linux (x64, arm64) (GTK 3.20+, glibc 2.27+, libsane)

## ApneScan.Sdk (for developers)

[ApneScan.Sdk](https://github.com/Skaler2015/APNESCAN3/tree/main/ApneScan.Sdk) is a fully-featured scanning library, supporting WIA, TWAIN, SANE, and ESCL scanners on Windows, Mac, and Linux.

## Build Instructions

ApneScan is built with .NET. To build the full solution:

```
dotnet build ApneScan.sln
```

Individual projects (for example the console app) can be run with:

```
dotnet run --project ApneScan.App.Console
```

## Credits

ApneScan is based on the open-source [NAPS2](https://github.com/cyanfish/naps2) project by Ben Olden-Cooligan and contributors. Many thanks to the original authors and the NAPS2 community.

## License

ApneScan is licensed under the GNU GPL 2.0 (or later). Some projects have additional license options:
- ApneScan.Escl.* - GNU LGPL 2.1 (or later)
- ApneScan.Images.* - GNU LGPL 2.1 (or later)
- ApneScan.Internals - GNU LGPL 2.1 (or later)
- ApneScan.Sdk - GNU LGPL 2.1 (or later)
- ApneScan.Sdk.Samples - MIT
