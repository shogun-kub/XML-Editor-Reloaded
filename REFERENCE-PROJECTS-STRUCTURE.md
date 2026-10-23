# Reference Projects — Structure

This document is a catalogue of the projects under `References/`. They are **read-only** reference material: they are used for algorithms, UX decisions, DTO naming, and test data, and must never be modified during work sessions. `References/` is excluded from git via the root `.gitignore`.

The trees in sections 1–3 are complete. For the multi-gigabyte references (FreeImage, the engine source, and the two game installs) the trees are structural: directories, file counts, and editor-relevant folders are expanded, while bulk assets and bundled third-party code are summarized. A literal listing of 123,000+ files would be noise. Version-control, IDE, and build-output folders (`.git/`, `.vs/`, `bin/`, `obj/`) are omitted.

| Project | Kind | Technology | Size | Role |
|---|---|---|---|---|
| [`JA 2 1.13 Image viewer`](#1-ja-2-113-image-viewer) | utility | C# / WinForms | 22 MB | STI decoding reference |
| [`xml-editor`](#2-xml-editor) | legacy editor | VB.NET / WinForms | 3.3 MB | UX and feature reference |
| [`JA_2_1.13_TableData_Editor`](#3-ja_2_113_tabledata_editor) | prototype | C# / WinForms | 12 MB | metadata schema and DTOs |
| [`FreeImage`](#4-freeimage) | native library | C/C++ 3.18.0 | 42 MB | PNG and other raster formats |
| [`Ja 2 1.13 source`](#5-ja-2-113-source) | engine source | C++ / CMake | 5.4 GB | last-resort field semantics (ask first) |
| [`JA 2 1.13 v4`](#6-ja-2-113-v4) | game data | JA2 1.13 install | 3.7 GB | primary data set |
| [`JA 2 1.13 v5`](#7-ja-2-113-v5) | game data | JA2 1.13 install | 3.6 GB | second data set |

---

## 1. JA 2 1.13 Image viewer

**Path:** `References/JA 2 1.13 Image viewer/`

A small WinForms utility that opens `.sti` and `.png` files, displays STI sub-images, and lets the user cycle through frames with the keyboard (`a`/`4` — previous, `d`/`6` — next). It is the primary reference for STI decoding.

### 1.1 Summary

- **Type:** WinForms executable, C#.
- **Project:** `JA 2 1.13 Image viewer.csproj`, `net10.0-windows7.0`, `WinExe`, root namespace `JA_2_1._13_Image_viewer`.
- **Entry point:** `Program.cs` — `Main(string[] args)` opens `ImageViewer` with the optional file path argument.
- **Own source:** 3 C# files, 285 lines total.
- **Dependencies:**
  - `StiLib.dll` — legacy .NET Framework assembly (32 KB, x86, 2008). Public types: `StciData`, `IndexedConverter`, `RGBConverter`, `ExtendedBitmap`, `ETRLEData`, `ETRLEObject`, `Indexed`, `RGB`, `BMP`, `TIFF`, `GIF`, `LocalPaletteException`.
  - `FreeImageNET` — .NET wrapper built from the vendored `FreeImage/Wrapper/FreeImage.NET` sources.
  - `FreeImage.dll` — native FreeImage 3.x build (x86-64), copied to the output directory.
- **Vendored third-party code:** the `FreeImage/` folder (482 files including build outputs) is the upstream FreeImage distribution: native DLLs and headers for x86/x64 (`Dist/`), C/C++ examples, and wrappers for Delphi, FreeImage.NET, FreeImagePlus, and VB6. It supports common formats (PNG, JPEG, TIFF, GIF, …), **not STI**.

### 1.2 What to reuse

- The STI workflow in `ImageViewer.cs`:
  1. `new StciData(path, 0)` reads the STI header and frame directory.
  2. If `_Indexed` is present — `IndexedConverter.LoadIndexedImageData(...)` then `ConvertEtrleDataToBitmaps(..., 0)[index]`.
  3. If `_RGB` is present — `RGBConverter.GetBitmap(...)`.
  4. Palette index 0 (blue in the test code) is made transparent with `Bitmap.MakeTransparent`.
- The item image layout and transparency heuristics from the VB editor (`ItemImageManager.vb`) complement this project: palette index 0 is the transparent color, and item sprites additionally use bright green for outlines.
- The standalone STI decoder in `References/xml-editor/BackEnd/Images/STI.vb` is a dependency-free implementation of the same format and is easier to port to .NET 10 than the binary `StiLib.dll`.

### 1.3 Caveats

- `StiLib.dll` targets .NET Framework and is x86; it cannot be referenced from a .NET 10 x64 application. Reimplement the algorithms in `XmlEditorReloaded.Imaging`.
- `FreeImageNET.dll` is also an old .NET Framework build. FreeImage is not required for STI and should not become a dependency of the new editor unless additional image formats are needed.
- The viewer loads one file at a time, uses `Bitmap`/GDI+, and has no caching — fine for a utility, not a model for the editor's image layer.
- The namespace `JA_2_1_13._13_Image_viewer` is malformed (duplicated `_13`) and must not be copied.

### 1.4 Complete tree

```
├── FreeImage/
│   ├── Dist/
│   │   ├── x32/
│   │   │   ├── FreeImage.dll
│   │   │   ├── FreeImage.h
│   │   │   └── FreeImage.lib
│   │   ├── x64/
│   │   │   ├── FreeImage.dll
│   │   │   ├── FreeImage.h
│   │   │   └── FreeImage.lib
│   │   └── delete.me
│   ├── Examples/
│   │   ├── Generic/
│   │   │   ├── BatchLoad.cpp
│   │   │   ├── CloneMultiPage.cpp
│   │   │   ├── CreateAlpha.cpp
│   │   │   ├── FIFImportExport.cpp
│   │   │   ├── FIIO_Mem.cpp
│   │   │   ├── FIIO_Mem.h
│   │   │   ├── LoadFromHandle.cpp
│   │   │   ├── LoadFromMemory-classified.cpp
│   │   │   ├── LoadFromMemory.cpp
│   │   │   └── ShowMetadata.cpp
│   │   ├── Linux/
│   │   │   ├── linux-gtk.c
│   │   │   ├── linux-svgalib.c
│   │   │   └── Makefile
│   │   ├── OpenGL/
│   │   │   └── TextureManager/
│   │   │       ├── readme.txt
│   │   │       ├── TextureManager.cpp
│   │   │       └── TextureManager.h
│   │   └── Plugin/
│   │       ├── PluginCradle.cpp
│   │       └── PluginCradle.h
│   ├── Wrapper/
│   │   ├── Delphi/
│   │   │   ├── demo/
│   │   │   │   ├── ImagePreview/
│   │   │   │   │   ├── ImagePreview.dpr
│   │   │   │   │   ├── ImagePreview.res
│   │   │   │   │   ├── MainFrm.dfm
│   │   │   │   │   ├── MainFrm.pas
│   │   │   │   │   ├── Readme.txt
│   │   │   │   │   └── Tiger.jpg
│   │   │   │   ├── MultiBitmap/
│   │   │   │   │   ├── mbMainForm.dfm
│   │   │   │   │   ├── mbMainForm.pas
│   │   │   │   │   ├── MultiBitmap.dpr
│   │   │   │   │   ├── MultiBitmap.res
│   │   │   │   │   └── MultiBitmap.stat
│   │   │   │   ├── TargaUnit/
│   │   │   │   │   ├── Readme.txt
│   │   │   │   │   └── TargaImage.pas
│   │   │   │   └── WinBitmap/
│   │   │   │       ├── MainDemo.dpr
│   │   │   │       ├── MainDemo.res
│   │   │   │       ├── MainForm.dfm
│   │   │   │       └── MainForm.pas
│   │   │   ├── src/
│   │   │   │   ├── FreeBitmap.pas
│   │   │   │   ├── FreeImage.pas
│   │   │   │   ├── FreeUtils.pas
│   │   │   │   └── Version.inc
│   │   │   ├── _clean.bat
│   │   │   ├── license.txt
│   │   │   └── WhatsNew_Delphi.txt
│   │   ├── FreeImage.NET/
│   │   │   ├── cpp/
│   │   │   │   └── FreeImageIO/
│   │   │   │       ├── app.ico
│   │   │   │       ├── app.rc
│   │   │   │       ├── AssemblyInfo.cpp
│   │   │   │       ├── FreeImageIO.Net.cpp
│   │   │   │       ├── FreeImageIO.Net.h
│   │   │   │       ├── FreeImageIO.Net.vcproj
│   │   │   │       ├── FreeImageIO.sln
│   │   │   │       ├── ReadMe.txt
│   │   │   │       ├── resource.h
│   │   │   │       ├── Stdafx.cpp
│   │   │   │       └── Stdafx.h
│   │   │   └── cs/
│   │   │       ├── Backup/
│   │   │       │   ├── Library/
│   │   │       │   │   ├── Classes/
│   │   │       │   │   │   ├── FreeImageBitmap.cs
│   │   │       │   │   │   ├── FreeImageEngine.cs
│   │   │       │   │   │   ├── FreeImagePlugin.cs
│   │   │       │   │   │   ├── FreeImageStreamIO.cs
│   │   │       │   │   │   ├── GifInformation.cs
│   │   │       │   │   │   ├── ImageMetadata.cs
│   │   │       │   │   │   ├── LocalPlugin.cs
│   │   │       │   │   │   ├── MemoryArray.cs
│   │   │       │   │   │   ├── MetadataModel.cs
│   │   │       │   │   │   ├── MetadataModels.cs
│   │   │       │   │   │   ├── MetadataTag.cs
│   │   │       │   │   │   ├── Palette.cs
│   │   │       │   │   │   ├── PluginRepository.cs
│   │   │       │   │   │   ├── Scanline.cs
│   │   │       │   │   │   └── StreamWrapper.cs
│   │   │       │   │   ├── Enumerations/
│   │   │       │   │   │   ├── DisposalMethodType.cs
│   │   │       │   │   │   ├── FREE_IMAGE_COLOR_CHANNEL.cs
│   │   │       │   │   │   ├── FREE_IMAGE_COLOR_DEPTH.cs
│   │   │       │   │   │   ├── FREE_IMAGE_COLOR_OPTIONS.cs
│   │   │       │   │   │   ├── FREE_IMAGE_COLOR_TYPE.cs
│   │   │       │   │   │   ├── FREE_IMAGE_COMPARE_FLAGS.cs
│   │   │       │   │   │   ├── FREE_IMAGE_DITHER.cs
│   │   │       │   │   │   ├── FREE_IMAGE_FILTER.cs
│   │   │       │   │   │   ├── FREE_IMAGE_FORMAT.cs
│   │   │       │   │   │   ├── FREE_IMAGE_JPEG_OPERATION.cs
│   │   │       │   │   │   ├── FREE_IMAGE_LOAD_FLAGS.cs
│   │   │       │   │   │   ├── FREE_IMAGE_MDMODEL.cs
│   │   │       │   │   │   ├── FREE_IMAGE_MDTYPE.cs
│   │   │       │   │   │   ├── FREE_IMAGE_METADATA_COPY.cs
│   │   │       │   │   │   ├── FREE_IMAGE_QUANTIZE.cs
│   │   │       │   │   │   ├── FREE_IMAGE_SAVE_FLAGS.cs
│   │   │       │   │   │   ├── FREE_IMAGE_TMO.cs
│   │   │       │   │   │   ├── FREE_IMAGE_TYPE.cs
│   │   │       │   │   │   ├── ICC_FLAGS.cs
│   │   │       │   │   │   └── MD_SEARCH_FLAGS.cs
│   │   │       │   │   ├── Properties/
│   │   │       │   │   │   └── AssemblyInfo.cs
│   │   │       │   │   ├── Structs/
│   │   │       │   │   │   ├── BITMAP.cs
│   │   │       │   │   │   ├── BITMAPINFO.cs
│   │   │       │   │   │   ├── BITMAPINFOHEADER.cs
│   │   │       │   │   │   ├── FI16RGB555.cs
│   │   │       │   │   │   ├── FI16RGB565.cs
│   │   │       │   │   │   ├── FI1BIT.cs
│   │   │       │   │   │   ├── FI4BIT.cs
│   │   │       │   │   │   ├── fi_handle.cs
│   │   │       │   │   │   ├── FIBITMAP.cs
│   │   │       │   │   │   ├── FICOMPLEX.cs
│   │   │       │   │   │   ├── FIICCPROFILE.cs
│   │   │       │   │   │   ├── FIMEMORY.cs
│   │   │       │   │   │   ├── FIMETADATA.cs
│   │   │       │   │   │   ├── FIMULTIBITMAP.cs
│   │   │       │   │   │   ├── FIRational.cs
│   │   │       │   │   │   ├── FIRGB16.cs
│   │   │       │   │   │   ├── FIRGBA16.cs
│   │   │       │   │   │   ├── FIRGBAF.cs
│   │   │       │   │   │   ├── FIRGBF.cs
│   │   │       │   │   │   ├── FITAG.cs
│   │   │       │   │   │   ├── FIURational.cs
│   │   │       │   │   │   ├── FreeImageIO.cs
│   │   │       │   │   │   ├── Plugin.cs
│   │   │       │   │   │   ├── RGBQUAD.cs
│   │   │       │   │   │   └── RGBTRIPLE.cs
│   │   │       │   │   ├── Delegates.cs
│   │   │       │   │   ├── FreeImageStaticImports.cs
│   │   │       │   │   ├── FreeImageWrapper.cs
│   │   │       │   │   └── Library.csproj
│   │   │       │   ├── Samples/
│   │   │       │   │   ├── Sample 01 - Loading and saving/
│   │   │       │   │   │   ├── Properties/
│   │   │       │   │   │   │   └── AssemblyInfo.cs
│   │   │       │   │   │   ├── Program.cs
│   │   │       │   │   │   ├── Sample 01 - Loading and saving.csproj
│   │   │       │   │   │   └── Sample.jpg
│   │   │       │   │   ├── Sample 02 - Multipaged bitmaps/
│   │   │       │   │   │   ├── Properties/
│   │   │       │   │   │   │   └── AssemblyInfo.cs
│   │   │       │   │   │   ├── multipaged.tif
│   │   │       │   │   │   ├── Program.cs
│   │   │       │   │   │   └── Sample 02 - Multipaged bitmaps.csproj
│   │   │       │   │   ├── Sample 03 - Allocating/
│   │   │       │   │   │   ├── Properties/
│   │   │       │   │   │   │   └── AssemblyInfo.cs
│   │   │       │   │   │   ├── Program.cs
│   │   │       │   │   │   └── Sample 03 - Allocating.csproj
│   │   │       │   │   ├── Sample 04 - Getting bitmap informations/
│   │   │       │   │   │   ├── Properties/
│   │   │       │   │   │   │   └── AssemblyInfo.cs
│   │   │       │   │   │   ├── MainForm.cs
│   │   │       │   │   │   ├── MainForm.Designer.cs
│   │   │       │   │   │   ├── MainForm.resx
│   │   │       │   │   │   └── Sample 04 - Getting bitmap informations.csproj
│   │   │       │   │   ├── Sample 05 - Working with pixels/
│   │   │       │   │   │   ├── Properties/
│   │   │       │   │   │   │   └── AssemblyInfo.cs
│   │   │       │   │   │   ├── Program.cs
│   │   │       │   │   │   ├── Sample 05 - Working with pixels.csproj
│   │   │       │   │   │   ├── Sample.jpg
│   │   │       │   │   │   └── Sample.tif
│   │   │       │   │   ├── Sample 06 - Converting/
│   │   │       │   │   │   ├── Properties/
│   │   │       │   │   │   │   └── AssemblyInfo.cs
│   │   │       │   │   │   ├── MainForm.cs
│   │   │       │   │   │   ├── MainForm.Designer.cs
│   │   │       │   │   │   ├── MainForm.resx
│   │   │       │   │   │   ├── Sample 06 - Converting.csproj
│   │   │       │   │   │   └── Sample.jpg
│   │   │       │   │   ├── Sample 07 - ICC Profiles/
│   │   │       │   │   │   ├── Properties/
│   │   │       │   │   │   │   └── AssemblyInfo.cs
│   │   │       │   │   │   ├── Program.cs
│   │   │       │   │   │   ├── Sample 07 - ICC Profiles.csproj
│   │   │       │   │   │   └── Sample.jpg
│   │   │       │   │   ├── Sample 08 - Creating a plugin/
│   │   │       │   │   │   ├── Properties/
│   │   │       │   │   │   │   └── AssemblyInfo.cs
│   │   │       │   │   │   ├── Sample 08 - Creating a plugin.csproj
│   │   │       │   │   │   ├── Sample.jpg
│   │   │       │   │   │   ├── SampleForm.cs
│   │   │       │   │   │   ├── SampleForm.Designer.cs
│   │   │       │   │   │   ├── SampleForm.resx
│   │   │       │   │   │   └── SerializationPlugin.cs
│   │   │       │   │   ├── Sample 09 - Working with streams/
│   │   │       │   │   │   ├── Properties/
│   │   │       │   │   │   │   └── AssemblyInfo.cs
│   │   │       │   │   │   ├── Sample 09 - Working with streams.csproj
│   │   │       │   │   │   ├── SampleForm.cs
│   │   │       │   │   │   ├── SampleForm.Designer.cs
│   │   │       │   │   │   └── SampleForm.resx
│   │   │       │   │   ├── Sample 10 - Metadata/
│   │   │       │   │   │   ├── Properties/
│   │   │       │   │   │   │   └── AssemblyInfo.cs
│   │   │       │   │   │   ├── MainForm.cs
│   │   │       │   │   │   ├── MainForm.Designer.cs
│   │   │       │   │   │   ├── MainForm.resx
│   │   │       │   │   │   └── Sample 10 - Metadata.csproj
│   │   │       │   │   └── Sample 11 - Using the FreeImageBitmap class/
│   │   │       │   │       ├── Properties/
│   │   │       │   │       │   └── AssemblyInfo.cs
│   │   │       │   │       ├── MainForm.cs
│   │   │       │   │       ├── MainForm.Designer.cs
│   │   │       │   │       ├── MainForm.resx
│   │   │       │   │       ├── MetaDataFrame.cs
│   │   │       │   │       ├── MetaDataFrame.Designer.cs
│   │   │       │   │       ├── MetaDataFrame.resx
│   │   │       │   │       └── Sample 11 - Using the FreeImageBitmap class.csproj
│   │   │       │   ├── SourceFileMerger/
│   │   │       │   │   ├── Properties/
│   │   │       │   │   │   └── AssemblyInfo.cs
│   │   │       │   │   ├── FreeImage.cs.template
│   │   │       │   │   ├── Program.cs
│   │   │       │   │   └── SourceFileMerger.csproj
│   │   │       │   ├── UnitTest/
│   │   │       │   │   ├── Properties/
│   │   │       │   │   │   └── AssemblyInfo.cs
│   │   │       │   │   ├── FreeImage.cs
│   │   │       │   │   ├── ImageManager.cs
│   │   │       │   │   ├── UnitTest.cs
│   │   │       │   │   └── UnitTest.csproj
│   │   │       │   └── FreeImage.NET.sln
│   │   │       ├── Bin/
│   │   │       │   └── delete.me
│   │   │       ├── Doc/
│   │   │       │   ├── 3.11.0/
│   │   │       │   │   └── FreeImage.NET.shfbproj
│   │   │       │   ├── 3.12.0/
│   │   │       │   │   └── FreeImage.NET.shfbproj
│   │   │       │   ├── 3.13.0/
│   │   │       │   │   └── FreeImage.NET.shfbproj
│   │   │       │   ├── FreeImage.NET.shfbproj
│   │   │       │   └── SHFB.txt
│   │   │       ├── Library/
│   │   │       │   ├── Classes/
│   │   │       │   │   ├── FreeImageBitmap.cs
│   │   │       │   │   ├── FreeImageEngine.cs
│   │   │       │   │   ├── FreeImagePlugin.cs
│   │   │       │   │   ├── FreeImageStreamIO.cs
│   │   │       │   │   ├── GifInformation.cs
│   │   │       │   │   ├── ImageMetadata.cs
│   │   │       │   │   ├── LocalPlugin.cs
│   │   │       │   │   ├── MemoryArray.cs
│   │   │       │   │   ├── MetadataModel.cs
│   │   │       │   │   ├── MetadataModels.cs
│   │   │       │   │   ├── MetadataTag.cs
│   │   │       │   │   ├── Palette.cs
│   │   │       │   │   ├── PluginRepository.cs
│   │   │       │   │   ├── Scanline.cs
│   │   │       │   │   └── StreamWrapper.cs
│   │   │       │   ├── Enumerations/
│   │   │       │   │   ├── DisposalMethodType.cs
│   │   │       │   │   ├── FREE_IMAGE_COLOR_CHANNEL.cs
│   │   │       │   │   ├── FREE_IMAGE_COLOR_DEPTH.cs
│   │   │       │   │   ├── FREE_IMAGE_COLOR_OPTIONS.cs
│   │   │       │   │   ├── FREE_IMAGE_COLOR_TYPE.cs
│   │   │       │   │   ├── FREE_IMAGE_COMPARE_FLAGS.cs
│   │   │       │   │   ├── FREE_IMAGE_DITHER.cs
│   │   │       │   │   ├── FREE_IMAGE_FILTER.cs
│   │   │       │   │   ├── FREE_IMAGE_FORMAT.cs
│   │   │       │   │   ├── FREE_IMAGE_JPEG_OPERATION.cs
│   │   │       │   │   ├── FREE_IMAGE_LOAD_FLAGS.cs
│   │   │       │   │   ├── FREE_IMAGE_MDMODEL.cs
│   │   │       │   │   ├── FREE_IMAGE_MDTYPE.cs
│   │   │       │   │   ├── FREE_IMAGE_METADATA_COPY.cs
│   │   │       │   │   ├── FREE_IMAGE_QUANTIZE.cs
│   │   │       │   │   ├── FREE_IMAGE_SAVE_FLAGS.cs
│   │   │       │   │   ├── FREE_IMAGE_TMO.cs
│   │   │       │   │   ├── FREE_IMAGE_TYPE.cs
│   │   │       │   │   ├── ICC_FLAGS.cs
│   │   │       │   │   └── MD_SEARCH_FLAGS.cs
│   │   │       │   ├── Properties/
│   │   │       │   │   └── AssemblyInfo.cs
│   │   │       │   ├── Structs/
│   │   │       │   │   ├── BITMAP.cs
│   │   │       │   │   ├── BITMAPINFO.cs
│   │   │       │   │   ├── BITMAPINFOHEADER.cs
│   │   │       │   │   ├── FI16RGB555.cs
│   │   │       │   │   ├── FI16RGB565.cs
│   │   │       │   │   ├── FI1BIT.cs
│   │   │       │   │   ├── FI4BIT.cs
│   │   │       │   │   ├── fi_handle.cs
│   │   │       │   │   ├── FIBITMAP.cs
│   │   │       │   │   ├── FICOMPLEX.cs
│   │   │       │   │   ├── FIICCPROFILE.cs
│   │   │       │   │   ├── FIMEMORY.cs
│   │   │       │   │   ├── FIMETADATA.cs
│   │   │       │   │   ├── FIMULTIBITMAP.cs
│   │   │       │   │   ├── FIRational.cs
│   │   │       │   │   ├── FIRGB16.cs
│   │   │       │   │   ├── FIRGBA16.cs
│   │   │       │   │   ├── FIRGBAF.cs
│   │   │       │   │   ├── FIRGBF.cs
│   │   │       │   │   ├── FITAG.cs
│   │   │       │   │   ├── FIURational.cs
│   │   │       │   │   ├── FreeImageIO.cs
│   │   │       │   │   ├── Plugin.cs
│   │   │       │   │   ├── RGBQUAD.cs
│   │   │       │   │   └── RGBTRIPLE.cs
│   │   │       │   ├── build.bat
│   │   │       │   ├── Delegates.cs
│   │   │       │   ├── FreeImageStaticImports.cs
│   │   │       │   ├── FreeImageWrapper.cs
│   │   │       │   ├── Library.2005.csproj
│   │   │       │   └── Library.csproj
│   │   │       ├── Samples/
│   │   │       │   ├── Sample 01 - Loading and saving/
│   │   │       │   │   ├── Properties/
│   │   │       │   │   │   └── AssemblyInfo.cs
│   │   │       │   │   ├── Program.cs
│   │   │       │   │   ├── Sample 01 - Loading and saving.2005.csproj
│   │   │       │   │   ├── Sample 01 - Loading and saving.csproj
│   │   │       │   │   └── Sample.jpg
│   │   │       │   ├── Sample 02 - Multipaged bitmaps/
│   │   │       │   │   ├── Properties/
│   │   │       │   │   │   └── AssemblyInfo.cs
│   │   │       │   │   ├── multipaged.tif
│   │   │       │   │   ├── Program.cs
│   │   │       │   │   ├── Sample 02 - Multipaged bitmaps.2005.csproj
│   │   │       │   │   └── Sample 02 - Multipaged bitmaps.csproj
│   │   │       │   ├── Sample 03 - Allocating/
│   │   │       │   │   ├── Properties/
│   │   │       │   │   │   └── AssemblyInfo.cs
│   │   │       │   │   ├── Program.cs
│   │   │       │   │   ├── Sample 03 - Allocating.2005.csproj
│   │   │       │   │   └── Sample 03 - Allocating.csproj
│   │   │       │   ├── Sample 04 - Getting bitmap informations/
│   │   │       │   │   ├── Properties/
│   │   │       │   │   │   └── AssemblyInfo.cs
│   │   │       │   │   ├── MainForm.cs
│   │   │       │   │   ├── MainForm.Designer.cs
│   │   │       │   │   ├── MainForm.resx
│   │   │       │   │   ├── Sample 04 - Getting bitmap informations.2005.csproj
│   │   │       │   │   └── Sample 04 - Getting bitmap informations.csproj
│   │   │       │   ├── Sample 05 - Working with pixels/
│   │   │       │   │   ├── Properties/
│   │   │       │   │   │   └── AssemblyInfo.cs
│   │   │       │   │   ├── Program.cs
│   │   │       │   │   ├── Sample 05 - Working with pixels.2005.csproj
│   │   │       │   │   ├── Sample 05 - Working with pixels.csproj
│   │   │       │   │   ├── Sample.jpg
│   │   │       │   │   └── Sample.tif
│   │   │       │   ├── Sample 06 - Converting/
│   │   │       │   │   ├── Properties/
│   │   │       │   │   │   └── AssemblyInfo.cs
│   │   │       │   │   ├── MainForm.cs
│   │   │       │   │   ├── MainForm.Designer.cs
│   │   │       │   │   ├── MainForm.resx
│   │   │       │   │   ├── Sample 06 - Converting.2005.csproj
│   │   │       │   │   ├── Sample 06 - Converting.csproj
│   │   │       │   │   └── Sample.jpg
│   │   │       │   ├── Sample 07 - ICC Profiles/
│   │   │       │   │   ├── Properties/
│   │   │       │   │   │   └── AssemblyInfo.cs
│   │   │       │   │   ├── Program.cs
│   │   │       │   │   ├── Sample 07 - ICC Profiles.2005.csproj
│   │   │       │   │   ├── Sample 07 - ICC Profiles.csproj
│   │   │       │   │   └── Sample.jpg
│   │   │       │   ├── Sample 08 - Creating a plugin/
│   │   │       │   │   ├── Properties/
│   │   │       │   │   │   └── AssemblyInfo.cs
│   │   │       │   │   ├── Sample 08 - Creating a plugin.2005.csproj
│   │   │       │   │   ├── Sample 08 - Creating a plugin.csproj
│   │   │       │   │   ├── Sample.jpg
│   │   │       │   │   ├── SampleForm.cs
│   │   │       │   │   ├── SampleForm.Designer.cs
│   │   │       │   │   ├── SampleForm.resx
│   │   │       │   │   └── SerializationPlugin.cs
│   │   │       │   ├── Sample 09 - Working with streams/
│   │   │       │   │   ├── Properties/
│   │   │       │   │   │   └── AssemblyInfo.cs
│   │   │       │   │   ├── Sample 09 - Working with streams.2005.csproj
│   │   │       │   │   ├── Sample 09 - Working with streams.csproj
│   │   │       │   │   ├── SampleForm.cs
│   │   │       │   │   ├── SampleForm.Designer.cs
│   │   │       │   │   └── SampleForm.resx
│   │   │       │   ├── Sample 10 - Metadata/
│   │   │       │   │   ├── Properties/
│   │   │       │   │   │   └── AssemblyInfo.cs
│   │   │       │   │   ├── MainForm.cs
│   │   │       │   │   ├── MainForm.Designer.cs
│   │   │       │   │   ├── MainForm.resx
│   │   │       │   │   ├── Sample 10 - Metadata.2005.csproj
│   │   │       │   │   └── Sample 10 - Metadata.csproj
│   │   │       │   └── Sample 11 - Using the FreeImageBitmap class/
│   │   │       │       ├── Properties/
│   │   │       │       │   └── AssemblyInfo.cs
│   │   │       │       ├── MainForm.cs
│   │   │       │       ├── MainForm.Designer.cs
│   │   │       │       ├── MainForm.resx
│   │   │       │       ├── MetaDataFrame.cs
│   │   │       │       ├── MetaDataFrame.Designer.cs
│   │   │       │       ├── MetaDataFrame.resx
│   │   │       │       ├── Sample 11 - Using the FreeImageBitmap class.2005.csproj
│   │   │       │       └── Sample 11 - Using the FreeImageBitmap class.csproj
│   │   │       ├── SourceFileMerger/
│   │   │       │   ├── Properties/
│   │   │       │   │   └── AssemblyInfo.cs
│   │   │       │   ├── FreeImage.cs.template
│   │   │       │   ├── Program.cs
│   │   │       │   ├── SourceFileMerger.2005.csproj
│   │   │       │   └── SourceFileMerger.csproj
│   │   │       ├── UnitTest/
│   │   │       │   ├── Properties/
│   │   │       │   │   └── AssemblyInfo.cs
│   │   │       │   ├── FreeImage.cs
│   │   │       │   ├── ImageManager.cs
│   │   │       │   ├── NUnit.txt
│   │   │       │   ├── UnitTest.2005.csproj
│   │   │       │   ├── UnitTest.cs
│   │   │       │   └── UnitTest.csproj
│   │   │       ├── .editorconfig
│   │   │       ├── clean.bat
│   │   │       ├── Content.txt
│   │   │       ├── FreeImage.NET.nunit
│   │   │       ├── FreeImage.NET.sln
│   │   │       ├── UpgradeLog.htm
│   │   │       └── Whats_New.NET.txt
│   │   ├── FreeImagePlus/
│   │   │   ├── dist/
│   │   │   │   ├── x32/
│   │   │   │   │   ├── FreeImagePlus.dll
│   │   │   │   │   ├── FreeImagePlus.h
│   │   │   │   │   └── FreeImagePlus.lib
│   │   │   │   ├── x64/
│   │   │   │   │   ├── FreeImagePlus.dll
│   │   │   │   │   ├── FreeImagePlus.h
│   │   │   │   │   └── FreeImagePlus.lib
│   │   │   │   └── delete.me
│   │   │   ├── doc/
│   │   │   │   ├── footer.html
│   │   │   │   ├── freeimage.png
│   │   │   │   ├── freeimagedoc.jpg
│   │   │   │   ├── FreeImagePlus.dox
│   │   │   │   └── index.html
│   │   │   ├── src/
│   │   │   │   ├── fipImage.cpp
│   │   │   │   ├── fipMemoryIO.cpp
│   │   │   │   ├── fipMetadataFind.cpp
│   │   │   │   ├── fipMultiPage.cpp
│   │   │   │   ├── fipTag.cpp
│   │   │   │   ├── fipWinImage.cpp
│   │   │   │   └── FreeImagePlus.cpp
│   │   │   ├── test/
│   │   │   │   ├── fipTest.2013.sln
│   │   │   │   ├── fipTest.2013.vcxproj
│   │   │   │   ├── fipTest.2017.sln
│   │   │   │   ├── fipTest.2017.vcxproj
│   │   │   │   ├── fipTest.cpp
│   │   │   │   ├── fipTest.h
│   │   │   │   ├── fipTestMemIO.cpp
│   │   │   │   ├── fipTestMPage.cpp
│   │   │   │   ├── fipTestMPageMemory.cpp
│   │   │   │   ├── fipTestMPageStream.cpp
│   │   │   │   ├── test.jpg
│   │   │   │   └── test.tif
│   │   │   ├── clean.bat
│   │   │   ├── FreeImagePlus.2013.sln
│   │   │   ├── FreeImagePlus.2013.vcxproj
│   │   │   ├── FreeImagePlus.2013.vcxproj.filters
│   │   │   ├── FreeImagePlus.2017.sln
│   │   │   ├── FreeImagePlus.2017.vcxproj
│   │   │   ├── FreeImagePlus.2017.vcxproj.filters
│   │   │   ├── FreeImagePlus.h
│   │   │   ├── FreeImagePlus.rc
│   │   │   └── WhatsNew_FIP.txt
│   │   └── VB6/
│   │       └── src/
│   │           ├── MFreeImage.bas
│   │           └── WhatsNew_VB.txt
│   ├── license-fi.txt
│   ├── license-gplv2.txt
│   ├── license-gplv3.txt
│   ├── README.minGW
│   └── Whatsnew.txt
├── Properties/
│   ├── launchSettings.json
│   ├── Resources.Designer.cs
│   └── Resources.resx
├── FreeImage.dll
├── FreeImage.h
├── FreeImage.lib
├── ImageViewer.cs
├── ImageViewer.Designer.cs
├── ImageViewer.resx
├── JA 2 1.13 Image viewer.csproj
├── JA 2 1.13 Image viewer.csproj.user
├── JA 2 1.13 Image viewer.sln
├── Program.cs
└── StiLib.dll
```

---

## 2. xml-editor

**Path:** `References/xml-editor/`

The complete legacy XML Editor for JA2 1.13, written in VB.NET with WinForms. It is the main source of functional and UX requirements for XML Editor Reloaded. The README states it is based on revision 168 of the SVN repository `ja2_v1.13_xml_editor`; the local git history contains later fixes (latest: `Fix crash when clicking bottom row when adding new entry (#38)`, 2026-01-24).

### 2.1 Summary

- **Type:** VB.NET solution with two projects:
  - `BackEnd/BackEnd.vbproj` — class library (`net9.0-windows7.0`).
  - `GUI/GUI.vbproj` — WinForms executable named `XML Editor` (`net9.0-windows7.0`, startup object `GUI.Main`).
- **Own source:** 122 files, ~39,000 lines including designer files; backend ~6,500 lines, GUI ~32,700 lines.
- **Architecture:** a `DataSet`-based in-memory database. The schema of roughly 70 tables is built programmatically in `BackEnd/DatabaseCode.vb`; `XmlDB` loads/saves `DataTable`s through per-table handler classes derived from `DefaultTable`; table and column metadata is stored in `DataTable.ExtendedProperties`.
- **Dependencies:** `Microsoft.Windows.Compatibility` 9.0.0, `System.Configuration.ConfigurationManager` 9.0.0; the backend also has an unnecessary `Microsoft.AspNetCore.App` framework reference.
- **Runtime configuration:** `GUI/XMLEditorInit.xml` (data directories, language directories, AP maximum, "hide nothing items"), plus `XMLEditorSettings.xml` for user settings written by `SettingsUtility.vb`.

### 2.2 Backend structure

| File | Lines | Purpose |
|---|---:|---|
| `DataManager.vb` | 224 | Top-level facade: data directory, language-specific directories, working-data cache, item images, load/save events. |
| `DatabaseCode.vb` | 2,504 | Builds the complete `DataSet` schema: items, weapons, merges, magazines, launchables, ammo types/strings, attachments, armour, explosives, localized items/ammo, sounds, IMP/enemy/militia equipment, shopkeeper inventories and control tables, LBE, pockets, silhouettes, merc starting gear, attachment slots/points, item-to-explosive, transformations, disease, food, clothes, random items; also defines relations and lookup tables. |
| `Data Access/XmlDB.vb` | 346 | `DataSet` wrapper: load/save all tables or one table, working-data load/save, events, handler initialization, new/duplicate/delete/copy rows, next primary key. |
| `Data Access/Utility.vb` | 202 | Extension methods and constants: `TableProperty`, `ColumnProperty` (lookup, width, hidden, sub-table, tooltip), shopkeeper names, `ItemClass` flags. |
| `Data Access/Tables.vb` | 241 | Constants for table names and important field names. |
| `Data Access/TableComments.vb` | 94 | Long XML documentation strings shown for selected tables (clothes, ammo types). |
| `Data Access/Tables/DefaultTable.vb` | 580 | Base table handler: read/write XML, row copy/overwrite, PK handling, error display. |
| `Data Access/Tables/ItemTable.vb` | 646 | Item-specific handler: extends the item table with image and lookup columns, merges item sub-tables on save, handles "nothing" items. |
| `Data Access/Tables/ControlTable.vb` | 232 | Shopkeeper control tables: reads dealer inventory/control data from a shared XML structure. |
| `Data Access/Tables/ItemTransformationTable.vb` | 117 | Item transformation records. |
| `Data Access/Tables/MercStartingGearTable.vb` | 194 | Merc starting gear records. |
| `Data Access/Tables/EnemyItemTable.vb` | 74 | Enemy/militia item choice tables. |
| `Data Access/Tables/InventoryTable.vb` | 75 | Inventory-related tables. |
| `Data Access/Tables/WeaponTable.vb` | 69 | Weapon table specifics. |
| `Data Access/Tables/SoundTable.vb` | 46 | Sound tables. |
| `Data Access/Tables/IteTable.vb` | 46 | Items-to-explosives table. |
| `Data Access/Tables/DuplicateEntryTable.vb` | 35 | Tables where duplicate entries are allowed/handled specially. |
| `Data Access/Tables/AutoIncrementTable.vb` | 25 | Automatic primary-key allocation. |
| `Images/STI.vb` | 225 | Dependency-free STI parser and ETRLE decoder to `Bitmap` (header, palette, sub-images). |
| `Images/ItemImageManager.vb` | 175 | Loads big/medium/small item images (`BigItems`, `Interface`, `tilesets\0`), determines transparency, exposes image arrays. |
| `ErrorHandler.vb` | 231 | Error logging (text file, event log), message dialogs, fatal error event. |
| `LookupFile.vb` | 46 | Parses lookup XML files into `(id, name)` rows. |
| `Localization.vb` | 40 | Detects language-specific file names (German, Russian, Polish, Italian, French, Chinese, Taiwanese, Dutch). |

### 2.3 GUI structure

| File | Lines | Purpose |
|---|---:|---|
| `Main.vb` | 118 | Entry point: reads `XMLEditorInit.xml`, creates one `DataManager` per configured data directory, shows splash, creates database, loads images, starts `MainForm`. |
| `App Forms/MainForm.vb` (+ designer) | 771 (+1,847) | MDI parent window: menus for every table, data-set switching, view options, save, status bar. |
| `App Forms/SplashForm.vb` | 44 (+119) | Splash screen with loading progress. |
| `App Forms/AboutForm.vb` | 26 (+179) | About dialog. |
| `Grid Forms/GridForm.vb` (+ designer) | 351 (+149) | Base grid form: binds a `DataView`, drag-and-drop copy between data sets, context menu, filter/sort, bulk value operations, column widths, status bar. |
| `Grid Forms/DataGridForm.vb` | 29 (+37) | Editable grid for ordinary tables; allocates PK on new rows. |
| `Grid Forms/ItemGridForm.vb` | 85 (+36) | Item list with image column; view details, new, duplicate, delete. |
| `Grid Forms/LookupGridForm.vb` | 21 (+37) | Editable lookup table grid. |
| `Grid Forms/ChildDataGridForm.vb` | 21 (+30) | Editable child/sub-table grid. |
| `Grid Forms/MercGearGridForm.vb` | 35 (+30) | Read-only merc starting-gear grid with "view details". |
| `Grid Forms/GridUtility.vb` | 196 | Builds grid columns from metadata: check box, image, lookup combo box, text; widths, tooltips, blank lookup rows. |
| `Data Forms/BaseDataForm.vb` (+ designer) | 176 (+98) | Data-binding engine for detail forms: a control's `Tag` names the column; supports text, check box, numeric, picture, and lookup combo bindings. |
| `Data Forms/ItemDataForm.vb` (+ designer) | 1,708 (+13,996) | Full item editor: general fields, weapon/armour/drug/food sections, flags, attachments, image preview, class changes. |
| `Data Forms/MercGearDataForm.vb` (+ designer) | 82 (+9,814) | Merc starting gear editor with several gear kits. |
| `Forms/BitCodeMakerForm.vb` | 95 (+204) | Checkbox editor for bit-flag values. |
| `Forms/ChangeClassForm.vb` | 51 (+93) | Converts an item to another item class. |
| `Forms/ColumnSelectForm.vb` | 63 (+81) | Show/hide grid columns. |
| `Forms/CustomFilterForm.vb` | 32 (+80) | Custom row filter expression. |
| `Forms/EnterItemIDForm.vb` | 38 (+79) | Prompts for an item ID. |
| `Forms/NewItemForm.vb` | 47 (+122) | Creates a new item with a chosen class. |
| `Forms/LoadingForm.vb` | 22 (+58) | Loading overlay. |
| `Forms/SimpleFormBase.vb` | 30 (+72) | Base dialog with OK/Cancel and an overridable validation action. |
| `IniFile.vb` | 135 | Reads `XMLEditorInit.xml`. |
| `SettingsUtility.vb` | 90 | Key/value settings persisted to `XMLEditorSettings.xml`. |
| `Resources/DisplayText.*` | 1,094 | Localized UI strings (designer-generated resource class). |
| `Resources/OtherText.*` | 148 | Additional localized strings and resource images. |

### 2.4 UX features to port

- Multiple data directories, up to ten, with separate language-specific game directories.
- MDI-style workspace with a grid per table and detail forms for complex records.
- Grid sorting, filtering by selected value / excluding value / custom expression, column selection, bulk add/multiply/set operations.
- Row duplication and copying between data sets with overwrite control.
- Item grid with images and a large item detail editor.
- Merc starting gear editor with multiple kits.
- Bit-flag editor and lookup combo boxes driven by metadata.
- Working-data cache (`XmlWorkingData/EditorData.xml`) invalidated on version change.
- Configurable AP maximum used when displaying action-point values.

### 2.5 Caveats

- The schema is hardcoded in `DatabaseCode.vb` (2,504 lines); there is no external metadata database. XML Editor Reloaded should move this into SQLite metadata.
- `Application.DoEvents()` is used throughout loading and saving; this must not be reproduced in WPF.
- Designer files are enormous (`ItemDataForm.Designer.vb` is ~14,000 lines), which makes the forms hard to maintain. The new UI should be composed from smaller views.
- VB.NET-specific patterns (`My.Settings`, `ProjectData`, late binding) do not translate directly to C#.
- The GUI project writes its output into a game folder path hardcoded in the `.vbproj`; the new solution must not depend on a local game installation to build.
- No automated tests.

### 2.6 Complete tree

```
├── BackEnd/
│   ├── Data Access/
│   │   ├── Tables/
│   │   │   ├── AutoIncrementTable.vb
│   │   │   ├── ControlTable.vb
│   │   │   ├── DefaultTable.vb
│   │   │   ├── DuplicateEntryTable.vb
│   │   │   ├── EnemyItemTable.vb
│   │   │   ├── InventoryTable.vb
│   │   │   ├── ItemTable.vb
│   │   │   ├── ItemTransformationTable.vb
│   │   │   ├── IteTable.vb
│   │   │   ├── MercStartingGearTable.vb
│   │   │   ├── SoundTable.vb
│   │   │   └── WeaponTable.vb
│   │   ├── TableComments.vb
│   │   ├── Tables.vb
│   │   ├── Utility.vb
│   │   └── XmlDB.vb
│   ├── Images/
│   │   ├── ItemImageManager.vb
│   │   └── STI.vb
│   ├── My Project/
│   │   ├── Application.Designer.vb
│   │   ├── Application.myapp
│   │   ├── AssemblyInfo.vb
│   │   ├── launchSettings.json
│   │   ├── Resources.Designer.vb
│   │   ├── Resources.resx
│   │   ├── Settings.Designer.vb
│   │   └── Settings.settings
│   ├── BackEnd.vbproj
│   ├── DatabaseCode.vb
│   ├── DataManager.vb
│   ├── ErrorHandler.vb
│   ├── Localization.vb
│   └── LookupFile.vb
├── GUI/
│   ├── App Forms/
│   │   ├── AboutForm.Designer.vb
│   │   ├── AboutForm.resx
│   │   ├── AboutForm.vb
│   │   ├── MainForm.Designer.vb
│   │   ├── MainForm.resx
│   │   ├── MainForm.vb
│   │   ├── SplashForm.Designer.vb
│   │   ├── SplashForm.resx
│   │   └── SplashForm.vb
│   ├── Data Forms/
│   │   ├── BaseDataForm.Designer.vb
│   │   ├── BaseDataForm.resx
│   │   ├── BaseDataForm.vb
│   │   ├── ItemDataForm.Designer.vb
│   │   ├── ItemDataForm.resx
│   │   ├── ItemDataForm.vb
│   │   ├── MercGearDataForm.Designer.vb
│   │   ├── MercGearDataForm.resx
│   │   └── MercGearDataForm.vb
│   ├── Forms/
│   │   ├── BitCodeMakerForm.Designer.vb
│   │   ├── BitCodeMakerForm.resx
│   │   ├── BitCodeMakerForm.vb
│   │   ├── ChangeClassForm.Designer.vb
│   │   ├── ChangeClassForm.resx
│   │   ├── ChangeClassForm.vb
│   │   ├── ColumnSelectForm.Designer.vb
│   │   ├── ColumnSelectForm.resx
│   │   ├── ColumnSelectForm.vb
│   │   ├── CustomFilterForm.Designer.vb
│   │   ├── CustomFilterForm.resx
│   │   ├── CustomFilterForm.vb
│   │   ├── EnterItemIDForm.Designer.vb
│   │   ├── EnterItemIDForm.resx
│   │   ├── EnterItemIDForm.vb
│   │   ├── LoadingForm.Designer.vb
│   │   ├── LoadingForm.resx
│   │   ├── LoadingForm.vb
│   │   ├── NewItemForm.Designer.vb
│   │   ├── NewItemForm.resx
│   │   ├── NewItemForm.vb
│   │   ├── SimpleFormBase.Designer.vb
│   │   ├── SimpleFormBase.resx
│   │   └── SimpleFormBase.vb
│   ├── Grid Forms/
│   │   ├── ChildDataGridForm.Designer.vb
│   │   ├── ChildDataGridForm.vb
│   │   ├── DataGridForm.Designer.vb
│   │   ├── DataGridForm.resx
│   │   ├── DataGridForm.vb
│   │   ├── GridForm.Designer.vb
│   │   ├── GridForm.resx
│   │   ├── GridForm.vb
│   │   ├── GridUtility.vb
│   │   ├── ItemGridForm.Designer.vb
│   │   ├── ItemGridForm.resx
│   │   ├── ItemGridForm.vb
│   │   ├── LookupGridForm.Designer.vb
│   │   ├── LookupGridForm.resx
│   │   ├── LookupGridForm.vb
│   │   ├── MercGearGridForm.Designer.vb
│   │   └── MercGearGridForm.vb
│   ├── My Project/
│   │   ├── app.manifest
│   │   ├── Application.Designer.vb
│   │   ├── Application.myapp
│   │   ├── AssemblyInfo.vb
│   │   ├── launchSettings.json
│   │   ├── Resources.Designer.vb
│   │   ├── Resources.resx
│   │   ├── Settings.Designer.vb
│   │   └── Settings.settings
│   ├── Resources/
│   │   ├── About.jpg
│   │   ├── bigitem slot.JPG
│   │   ├── camo02.jpg
│   │   ├── DESKTOP1.JPG
│   │   ├── DisplayText.Designer.vb
│   │   ├── DisplayText.resx
│   │   ├── ja2-aim.jpg
│   │   ├── med item slot.JPG
│   │   ├── OtherText.Designer.vb
│   │   └── OtherText.resx
│   ├── app.config
│   ├── ApplicationEvents.vb
│   ├── editor.ico
│   ├── GUI.vbproj
│   ├── IniFile.vb
│   ├── Main.vb
│   ├── SettingsUtility.vb
│   └── XMLEditorInit.xml
├── .editorconfig
├── .gitignore
├── README.md
└── XMLEditor_VS2010.sln
```

---

## 3. JA_2_1.13_TableData_Editor

**Path:** `References/JA_2_1.13_TableData_Editor/`

An early C# prototype of the new editor. It is mostly a collection of DTO classes plus a metadata-driven XML parser sketch; the UI is a stub. It is valuable for the metadata SQLite schema and for the naming of DTO fields, not as a working application.

### 3.1 Summary

- **Type:** WinForms executable, C#.
- **Project:** `JA_2_1_13.TableData_Editor.csproj`, `net5.0-windows`, `WinExe`, root namespace `JA_2_1_13.TableData_Editor`.
- **Dependencies:** `Dapper` 2.0.90, `Microsoft.Data.Sqlite` 5.0.5, `StiLib` (project-relative reference to the image viewer's DLL; currently unused in code).
- **Own source:** 50 files, ~1,800 lines.
- **Git history:** latest commit `Class hierarchy changed.` (2022-03-31).
- **Entry point:** `Program.cs` → `MainForm`.
- **Metadata:** `Metadata/table_metadata.sqlite`, copied to the output directory.

### 3.2 What is implemented

- `TableDataParser.GetTableMetadata()` — reads `TableMappings`, `ListFields`, and `DictFields` from SQLite via Dapper and builds `TableMetadata` objects.
- `TableDataParser.ParseSimpleTable<T>()` — prototype XML-to-DTO parser driven by metadata. It handles primitive properties, enums, nested XML via `XmlSerializer`, and stores unknown elements in `DTO_Base.additionalProperties`. The method is incomplete: it never adds the constructed object to the result list, the list/dictionary branches are empty, and the unknown-table branch is empty.
- DTO model:
  - `DTO_Base` with `additionalProperties`.
  - 30 item-related DTOs (`DTO_Item`, `DTO_Weapon`, `DTO_Armour`, `DTO_Drug`, `DTO_Food`, `DTO_Magazine`, `DTO_Attachment`, `DTO_Pocket`, etc.).
  - `DTO_Lookup`.
  - `DTO_Item.DTO_PositionModifiers` nested class (stand/crouch/prone modifiers) and the `SpreadPatterns` enum.
- `EditorSettings` — static wrapper over `Properties.Settings` for `GamePath` and `TableDataPath`.
- `SettingsForm` — game path browser (only `GamePath` is wired into the UI).
- `MainForm` — a single test button that loads metadata and shows a message box.
- Empty placeholder folders: `Models/FullModels`, `Models/TableModels`.

### 3.3 Metadata database

`Metadata/table_metadata.sqlite` contains:

| Table | Rows | Description |
|---|---:|---|
| `TableMappings` | 53 | 30 data tables (`Items.*`) and 23 lookup tables (`Lookup.*`). Columns: `Id`, `xml_table`, `dto_name`, `is_lookup`, `xml_root`, `list_item_name`. |
| `ListFields` | 12 | Repeated child elements that map to collection properties. Examples: `DrugEffect → DrugEffects`, `AvailableAttachmentPoint → AvailableAttachmentPoints`, `northfacingtile → northfacingtiles`. |
| `DictFields` | 3 | Indexed element groups. Examples: `ItemCapacityPerSize%Index% → ItemCapacityPerSize`, `randomitem%Index% → randomitem`, `item%Index% → item`. |

`TableMappings` examples:

```
Items.Items      → DTO_Item      / ITEMLIST        / ITEM
Items.Weapons    → DTO_Weapon    / WEAPONLIST      / WEAPON
Items.Pockets    → DTO_Pocket    / POCKETLIST      / POCKET
Lookup.ItemClass → (null)        / JA2Data         / ItemClass
```

The `xml_table` value maps to a file path by replacing dots with backslashes and appending `.xml` (`Items.Items` → `Items\Items.xml`). Lookup rows are marked with `is_lookup = 1`.

### 3.4 Caveats

- Targets `net5.0-windows` and packages from 2021; the code may not compile as-is against the current SDK without retargeting.
- `ParseSimpleTable<T>` is unfinished and has a bug (no `result.Add`). Do not copy it verbatim.
- DTO properties mirror XML element names, including inconsistent casing (`uiIndex`, `szItemName`, `STAND_MODIFIERS`); the new model should define a clear naming policy and map XML names explicitly.
- The StiLib reference is unused and points into another reference project's folder — a fragile relative path. The new solution must not reference anything under `References/`.
- No tests, no validation, no save path.

### 3.5 Complete tree

```
├── Forms/
│   ├── MainForm.cs
│   ├── MainForm.Designer.cs
│   ├── MainForm.resx
│   ├── SettingsForm.cs
│   ├── SettingsForm.Designer.cs
│   └── SettingsForm.resx
├── JA_2_1.13_TableData_Editor/
├── Metadata/
│   └── table_metadata.sqlite
├── Models/
│   ├── DTO/
│   │   ├── Items/
│   │   │   ├── DTO_AmmoString.cs
│   │   │   ├── DTO_AmmoType.cs
│   │   │   ├── DTO_Armour.cs
│   │   │   ├── DTO_Attachment.cs
│   │   │   ├── DTO_AttachmentComboMerge.cs
│   │   │   ├── DTO_AttachmentInfo.cs
│   │   │   ├── DTO_AttachmentSlots.cs
│   │   │   ├── DTO_Clothes.cs
│   │   │   ├── DTO_CompatibleFaceItem.cs
│   │   │   ├── DTO_Drug.cs
│   │   │   ├── DTO_ExplosionData.cs
│   │   │   ├── DTO_Explosive.cs
│   │   │   ├── DTO_Food.cs
│   │   │   ├── DTO_FoodOpinion.cs
│   │   │   ├── DTO_IncompatibleAttachments.cs
│   │   │   ├── DTO_Item.cs
│   │   │   ├── DTO_ItemTransformation.cs
│   │   │   ├── DTO_Key.cs
│   │   │   ├── DTO_Launchable.cs
│   │   │   ├── DTO_LoadBearingEquipment.cs
│   │   │   ├── DTO_Lock.cs
│   │   │   ├── DTO_Magazine.cs
│   │   │   ├── DTO_Merge.cs
│   │   │   ├── DTO_Pocket.cs
│   │   │   ├── DTO_PocketPopups.cs
│   │   │   ├── DTO_RandomItem.cs
│   │   │   ├── DTO_StructureConstruct.cs
│   │   │   ├── DTO_StructureDeconstruct.cs
│   │   │   ├── DTO_StructureMove.cs
│   │   │   └── DTO_Weapon.cs
│   │   ├── Lookup/
│   │   │   └── DTO_Lookup.cs
│   │   └── DTO_Base.cs
│   ├── FullModels/
│   ├── TableModels/
│   ├── EditorSettings.cs
│   └── Settings.cs
├── Properties/
│   ├── Settings.Designer.cs
│   └── Settings.settings
├── .gitignore
├── app.config
├── JA_2_1_13.TableData_Editor.csproj
├── JA_2_1_13.TableData_Editor.csproj.user
├── JA_2_1_13.TableData_Editor.sln
├── Program.cs
└── TableDataParser.cs
```

---

## 4. FreeImage

**Path:** `References/FreeImage/`

A fork of The FreeImage Project, version **3.18.0**. It is a native image-decoding library used as a reference for reading PNG (the format used by JA2 1.13 alongside STI) and other raster formats. It does **not** support STI.

### 4.1 Summary

- **Type:** C/C++ native library with MSVC 2013/2017 solutions, CMake, and platform makefiles.
- **Origin:** `https://github.com/jpvanoosten/FreeImage` — the README calls it "yet another fork"; 11 commits, latest 2022-06-13 (CMake support).
- **Size:** 42 MB, 1,310 files; 30 MB of that is `Source/`.
- **License:** FreeImage Public License / GPL; see `LICENSE`, `license-fi.txt`, `license-gplv2.txt`, `license-gplv3.txt`.
- **Format plugins (32):** BMP, CUT, DDS, EXR, G3, GIF, HDR, ICO, IFF, J2K, JNG, JP2, JPEG, JXR, KOALA, MNG, PCD, PCX, PFM, PICT, PNG, PNM, PSD, RAS, RAW, SGI, TARGA, TIFF, WBMP, WebP, XBM, XPM.
- **Bundled codecs:** LibPNG, ZLib, LibJPEG, LibTIFF4, LibWebP, OpenEXR, LibJXR, LibOpenJPEG, LibRawLite.
- **Not related to the older `FreeImage` folder inside `JA 2 1.13 Image viewer`** — that one is a vendored binary distribution; this is a source fork.

### 4.2 What to reuse

- Native decoding for PNG, TIFF, JPEG, and other formats if the editor must read files beyond STI.
- `Wrapper/FreeImage.NET/cs/Library` — C# P/Invoke wrapper (67 files) with `FreeImageBitmap`, `FreeImageEngine`, plugin management, and metadata classes.
- `TestAPI` — format tests and sample images (`sample.png`, `exif.jpg`, `exif.jxr`).
- **Caveat:** the .NET wrapper targets the old .NET Framework. For .NET 10, retarget it or write a minimal P/Invoke layer around the native DLL. WPF already decodes PNG through `BitmapDecoder`, so FreeImage is optional and should not be added to the backend without a concrete need.

### 4.3 Structure

```
FreeImage/
├── Source/                              (1,006 files, 30 MB)
│   ├── FreeImage/                       (73: core API, plugin registry, format plugins)
│   ├── FreeImageToolkit/                (14: bitmap tools, quantization, tone mapping)
│   ├── FreeImageLib/                    (4: static library project)
│   ├── Metadata/                        (9: EXIF/IPTC/XMP models)
│   ├── LibPNG/                          (41)
│   ├── ZLib/                            (40)
│   ├── LibJPEG/                         (98)
│   ├── LibTIFF4/                        (76)
│   ├── LibWebP/                         (169)
│   ├── OpenEXR/                         (318)
│   ├── LibJXR/                          (49)
│   ├── LibOpenJPEG/                     (66)
│   ├── LibRawLite/                      (40)
│   └── FreeImage.h, FreeImageIO.h, Plugin.h, CMakeLists.txt
├── Wrapper/                             (228)
│   ├── FreeImage.NET/                   (167: C# wrapper, C++/CLI IO plugin, samples)
│   ├── FreeImagePlus/                   (35: C++ wrapper)
│   ├── Delphi/                          (24)
│   └── VB6/                             (2)
├── TestAPI/                             (22: tests, sample.png, exif images)
├── Examples/                            (20)
├── FreeImage.2013.sln / .vcxproj
├── FreeImage.2017.sln / .vcxproj
├── CMakeLists.txt, Makefile*, clean scripts
└── README.md, LICENSE, license-*.txt, Whatsnew.txt
```

---

## 5. Ja 2 1.13 source

**Path:** `References/Ja 2 1.13 source/`

The C++ source of the JA2 1.13 engine, together with the current game data shipped in the same repository. It is the ultimate authority on how every table field is consumed, but it is huge and expensive to search.

> **Exploration policy:** always ask the user for permission before searching, reading, or analyzing this project. Permission is granted per task and does not carry over to later tasks. The description below was prepared with a one-time permission and does not imply ongoing access.

### 5.1 Summary

- **Type:** C++ engine, CMake build (`CMakeLists.txt`, `CMakePresets.json`), MSVC-oriented. XML is parsed with Expat.
- **Size:** 5.4 GB, 123,377 files; the vast majority is bundled game data, not source code.
- **Code volume:** roughly 560 `.cpp`/`.c` files and 620 headers across the engine modules.
- **Git:** latest commit 2026-10-18 (`Cache lua scripts' evaluation`).
- **XML loading:** 100 `XML_*` files. `Tactical/XML.h` defines `TABLEDATA_DIRECTORY "TableData\"` (line 58) and the element taxonomy; loaders live in `Tactical/XML_*.cpp`, `Strategic/XML_*.cpp`, `Laptop/XML_*.cpp`, `Ja2/XML_*.cpp`, and `TileEngine/XML_*.cpp`.
- **Core data code:** `Tactical/Items.cpp` (16,215 lines), `Tactical/Weapons.cpp` (12,221), `Tactical/EnemyItemDrops.cpp`, `Laptop/BaseTable.cpp` (948).
- **Bundled data:** `gamedir/Base/TableData` (277 XML files) is the data bundled with the engine source; `gamedir-languages` contains 622 localized XML files for seven languages. The repository also bundles `XML Editor.exe` and `XMLEditorInit.xml`, the legacy editor this project replaces.

### 5.2 What to consult

- Exact meaning, range, and default behavior of a field that is not documented anywhere else.
- How the engine treats a missing element versus an element with a default value.
- Which fields are read by which system (items, weapons, strategic map, laptop screens, etc.).
- How the newest schema differs from v4/v5 when implementing version-independent handling.

Do not use this project as a source of DTOs or UI ideas; it is a last resort for semantics only.

### 5.3 Structure

```
Ja 2 1.13 source/
├── Tactical/              (261 files; items, weapons, soldiers, XML_* table loaders)
├── Laptop/                (190; laptop UI, BaseTable, XML_* loaders)
├── Strategic/             (119; campaign, XML_* loaders)
├── Multiplayer/           (138)
├── Ja2/                   (77; init, screens, XML_* loaders)
├── Utils/                 (76; XMLAccess, XMLProperties, XMLWriter, XML_Parser.h)
├── TileEngine/            (79; tilesets, explosion data, XML_* loaders)
├── sgp/                   (88; engine core, FileMan, compression)
├── Editor/                (53; map editor, XML_ActionItems.cpp)
├── i18n/                  (37)
├── ModularizedTacticalAI/ (27)
├── TacticalAI/            (21)
├── lua/                   (16)
├── export/                (29; standalone XML writer)
├── ext/                   (336; bundled Expat, Lua 5.1.5, other third-party code)
├── gamedir/               (92,913; full game directory; Base/TableData has 277 XML)
├── gamedir-languages/     (28,884; 622 localized XML across seven languages)
├── cmake/ (6), crash-telemetry/ (7), tools/ (2), wine/ (3)
└── CMakeLists.txt, CMakePresets.json, README.md, .editorconfig
```

---

## 6. JA 2 1.13 v4

**Path:** `References/JA 2 1.13 v4/`

A complete GOG-based Jagged Alliance 2 installation with the 1.13 mod, Russian-localized. This is the primary test data set for the editor.

### 6.1 Summary

- **Size:** 3.7 GB. The game root contains `ja2.exe`, `INI Editor.exe`, `BackEnd.dll`, `Data`, `Data-1.13`, `Data-UB`, `Docs`, `Profiles`, graphics wrappers, and configuration files.
- **Editor target:** `Data-1.13/TableData` — 264 files (263 XML + one readme), 12 MB.
- **Lookup tables:** 23, the same set as v5.
- **Localization:** Russian build — 25 `Russian.*` overlay files. v5 is the English base build with no overlays; this is intentional.
- **Images:** `Data-1.13/BigItems` (1,568 STI), `Data-1.13/Interface` (25 files), `Data-1.13/tilesets/0` (4 STI).
- **XML encoding:** 162 of 263 files carry a UTF-8 BOM; 91 files use CRLF and 172 use LF-only line endings.

### 6.2 TableData layout

| Location | Files | Contents |
|---|---:|---|
| `TableData/` (root) | 37 XML | availability, backgrounds, difficulty, disease, names, ranks, profiles, vehicles, spread patterns, etc. |
| `Army/` | 4 | army composition, garrison groups, patrol groups, uniform colors |
| `BriefingRoom/` | 1 | briefing room data |
| `Email/` | 4 | 2 base + 2 Russian email tables |
| `Inventory/` | 28 | 21 active tables, 6 backups under `Backup original Gun Choices/`, 1 Russian |
| `Items/` | 33 | 28 base + 5 Russian |
| `Layout/` | 1 | main menu layout |
| `Lookup/` | 23 | ID → name lookup tables |
| `Map/` | 42 | 36 XML (16 root + 20 `ExtraItems/`) + 5 Russian + 1 readme |
| `MapAction/` | 1 | map action definitions |
| `Multiplayer/` | 1 | multiplayer table |
| `NPCInventory/` | 81 | 60 `AdditionalDealer_*`, 20 named dealer inventories, `Merchants.xml` |
| `Profiles/` | 6 | merc profile tables |
| `Sounds/` | 2 | `Sounds.xml`, `BurstSounds.xml` |
| **Total** | **264** | 263 XML + 1 readme |

Root XML files (v4 also has a `Russian.*` counterpart for 12 of them):

```
AIMAvailability.xml, Backgrounds.xml, CampaignStatsEvents.xml, CivGroupNames.xml,
DifficultySettings.xml, Disease.xml, EnemyNames.xml, EnemyRank.xml, FaceGear.xml,
HiddenNames.xml, History.xml, IMPPortraits.xml, IMPVoices.xml, LoadScreenHints.xml,
MercAvailability.xml, MercOpinions.xml, MercProfiles.xml, MercQuote.xml,
MilitiaIndividual.xml, OldAIMArchive.xml, RPCFacesSmall.xml, RandomStats.xml,
SpreadPatterns.xml, SquadNames.xml, Vehicles.xml
```

`Items/` (28 base files; v4 adds `Russian.AmmoStrings.xml`, `Russian.AttachmentSlots.xml`, `Russian.Items.xml`, `Russian.PocketPopups.xml`, `Russian.Pockets.xml`):

```
AmmoStrings.xml, AmmoTypes.xml, Armours.xml, AttachmentComboMerges.xml,
AttachmentInfo.xml, AttachmentSlots.xml, Attachments.xml, Clothes.xml,
CompatibleFaceItems.xml, Drugs.xml, ExplosionData.xml, Explosives.xml, Food.xml,
FoodOpinion.xml, IncompatibleAttachments.xml, Item_Transformations.xml, Items.xml,
Launchables.xml, LoadBearingEquipment.xml, Magazines.xml, Merges.xml,
PocketPopups.xml, Pockets.xml, RandomItem.xml, StructureConstruct.xml,
StructureDeconstruct.xml, StructureMove.xml, Weapons.xml
```

`Lookup/` (23 files, identical set in v5):

```
AmmoChoices.xml, AmmoFlag.xml, ArmourClass.xml, AttachmentClass.xml,
AttachmentPoint.xml, AttachmentSystem.xml, Cursor.xml, DrugType.xml,
ExplosionSize.xml, ExplosionType.xml, ItemClass.xml, ItemFlag.xml, LBEClass.xml,
MagazineType.xml, MergeType.xml, NasAttachmentClass.xml, NasLayoutClass.xml,
PocketSize.xml, Separability.xml, Silhouette.xml, SkillCheckType.xml,
WeaponClass.xml, WeaponType.xml
```

### 6.3 Images

| Folder | Files | Notes |
|---|---:|---|
| `BigItems/` | 1,568 STI | `GUN###.sti` and `p#item###.sti` sprites. Case and index width are inconsistent (`GUN00.STI`, `Gun375.sti`, `P1ITEM100.STI`), so lookup must be case-insensitive and support 1–3 digit indices. |
| `Interface/` | 25 | 4 uppercase STI, 8 lowercase STI, 13 PNG. Includes `MDGUNS.sti` and `MDP1ITEMS`–`MDP3ITEMS`; the PNGs are UI backgrounds and posters. |
| `tilesets/0/` | 4 STI | `smguns.sti`, `SMP1ITEMS.STI`–`SMP3ITEMS.STI` — small item images. |
| `tilesets/` (all) | 1,204 | Map tilesets; only `tilesets/0` is used for item images. |

### 6.4 XML shape

- `Items/Items.xml`: 1,776 records, 233 child elements in the first record, 224 distinct tag names, and 640 distinct field sets across records. **Records are sparse** — most rows contain only a small subset of the union of fields.
- `Items/Weapons.xml`: 389 records, 48 distinct tags in the union, 70 distinct field sets.
- `Items/Attachments.xml`: 6,079 records; 6,077 have 3 fields and 2 have 4 fields.
- Floating-point values are stored as text (`1.3`, `17.5`, `27.4`, `-0.2`, `16.56`), and some leaf elements are empty. Both forms must round-trip unchanged.

### 6.5 Localization files

v4 is the **Russian** build and v5 is the **English base** build. The absence of `Russian.*` files in v5 is intentional, not missing data.

Localized data is stored as overlay files named `<Language>.<BaseFile>.xml` next to the base file. The legacy editor detects them by the language prefix in `BackEnd/Localization.vb` (German, Russian, Polish, Italian, French, Chinese, Taiwanese, Dutch); the reference data uses only `Russian.`.

The 25 overlays in v4:

```
Root:       Russian.Backgrounds.xml, Russian.CampaignStatsEvents.xml,
            Russian.CivGroupNames.xml, Russian.DifficultySettings.xml,
            Russian.Disease.xml, Russian.EnemyNames.xml, Russian.EnemyRank.xml,
            Russian.History.xml, Russian.LoadScreenHints.xml,
            Russian.MercProfiles.xml, Russian.OldAIMArchive.xml, Russian.Vehicles.xml
Email/:     Russian.EmailMercAvailable.xml, Russian.EmailMercLevelUp.xml
Inventory/: Russian.MercStartingGear.xml
Items/:     Russian.AmmoStrings.xml, Russian.AttachmentSlots.xml,
            Russian.Items.xml, Russian.PocketPopups.xml, Russian.Pockets.xml
Map/:       Russian.Cities.xml, Russian.FacilityTypes.xml, Russian.Minerals.xml,
            Russian.SectorNames.xml, Russian.ShippingDestinations.xml
```

An overlay is not a full table: it contains the record key and the fields it overrides.

| Overlay | Key | Fields in the overlay | Coverage vs base |
|---|---|---|---|
| `Items/Russian.Items.xml` | `uiIndex` | `szItemName`, `szLongItemName`, `szItemDesc`, `szBRName`, `szBRDesc` | 1,742 / 1,776 (IDs 0–1741) |
| `Items/Russian.AmmoStrings.xml` | `uiIndex` | `AmmoCaliber`, `BRCaliber` | 50 / 51 |
| `Items/Russian.AttachmentSlots.xml` | `uiSlotIndex` | `szSlotName` | 62 / 63 |
| `Items/Russian.Pockets.xml` | `pIndex` | `pName` | 61 / 62 |
| `Items/Russian.PocketPopups.xml` | `pIndex` | `pName`; also `popup`/`generator` structural nodes | 20 / 20 |
| `Inventory/Russian.MercStartingGear.xml` | `mIndex` | `mName`, `GEARKIT/mGearKitName` | 255 / 255 |
| `Russian.Backgrounds.xml` | `uiIndex` | `szName`, `szShortName`, `szDescription` | 316 / 357 (IDs 0–315) |
| `Russian.Disease.xml` | `uiIndex` | `szName`, `szFatName`, `szDescription` | 18 / 20 |
| `Russian.EnemyRank.xml` | `uiIndex` | `szRank` | 9 / 10 |
| `Russian.History.xml` | `uiIndex` | `sHistory` | 80 / 81 |
| `Russian.LoadScreenHints.xml` | `uiIndex` | `szName`, `lore` | 133 / 159 |
| `Russian.OldAIMArchive.xml` | `uiIndex` | `NickName`, `Name`, `Bio` | 25 / 28 |
| `Map/Russian.FacilityTypes.xml` | `ubIndex` | `szFacilityName`, `szFacilityShortName`, `ASSIGNMENT` | 22 / 25 |
| `Map/Russian.Cities.xml` | `COUNTRY`; `CITYLIST/CITY` by `uiIndex` | `countryName`, `countryNoun`, `townName` | nested |
| `Map/Russian.SectorNames.xml` | `SectorGrid` | `szUnexploredName`, `szDetailedUnexploredName`, `szExploredName`, `szDetailedExploredName` | 256 / 256 |
| `Map/Russian.Minerals.xml` | `uiIndex` | `sNameType`, `sMineral` | 2 / 2 |
| `Map/Russian.ShippingDestinations.xml` | `uiIndex` | `name` | 17 / 17 |
| `Email/Russian.EmailMercAvailable.xml` | `uiIndex` | `Subject`, `Message` | 255 / 255 |
| `Email/Russian.EmailMercLevelUp.xml` | `uiIndex` | `Subject`, `Message` | 255 / 255 |
| `Russian.CampaignStatsEvents.xml` | `uiIndex` | `szText0`–`szText9` | 16 / 16 |
| `Russian.CivGroupNames.xml` | `uiIndex` | `szGroup` | 255 / 255 |
| `Russian.DifficultySettings.xml` | `uiIndex` | `Name`, `ConfirmText` | 5 / 5 |
| `Russian.EnemyNames.xml` | `uiIndex` | `szGroup` (one record also carries `Enabled` and `Sector`) | 256 / 256 |
| `Russian.MercProfiles.xml` | `uiIndex` | `zName`, `zNickname` | 255 / 255 |
| `Russian.Vehicles.xml` | `uiIndex` | `Name`, `LongName`, `ShortName` | 255 / 255 |

Nested structures need their own merge keys:

- `CITY_INFO/COUNTRY` is a singleton; `CITY_INFO/CITYLIST/CITY` is keyed by `uiIndex`, translated field `townName`.
- `FACILITYTYPES/FACILITYTYPE` is keyed by `ubIndex`; its `ASSIGNMENT` children are keyed by `ubAssignmentType`, translated field `szTooltipText`.
- `MERCGEARLIST/MERCGEAR` is keyed by `mIndex`; `GEARKIT` entries are aligned by position (usually five kits, but some overlays contain four); only `mGearKitName` is in the overlay.

Notes:

- Overlays may contain non-text structural fields (`Enabled`, `Sector`, `popup`, `generator`); they must be preserved.
- Coverage is incomplete: 34 items, 41 backgrounds, 26 load-screen hints, and several other records have no translation. The editor must fall back to the base value and mark them as untranslated.
- Overlay encoding is mixed: 4 of 25 files have a BOM, 16 use CRLF and 9 use LF.
- v5 has no language overlays. `Email/EmailSenderNameList.xml` is a regular base table, not a localization file.

---

## 7. JA 2 1.13 v5

**Path:** `References/JA 2 1.13 v5/`

Another complete Jagged Alliance 2 1.13 installation with a different data set. The file layout matches v4; the data and formatting do not.

### 7.1 Summary

- **Size:** 3.6 GB, same top-level game layout as v4.
- **Editor target:** `Data-1.13/TableData` — 240 files (239 XML + one readme), 10 MB.
- **Lookup tables:** 23, identical names to v4.
- **Localization:** English base build — no language overlays. This is intentional; `Email/EmailSenderNameList.xml` is a normal base table, not a localization file.
- **Images:** `BigItems` 1,567 STI (v4 additionally has `GUN375.sti`), `Interface` 25 identical files, `tilesets/0` 4 identical STI.
- **XML encoding:** 159 of 239 files carry a UTF-8 BOM; all 239 files use CRLF.

### 7.2 TableData layout

| Location | Files | Difference from v4 |
|---|---:|---|
| `TableData/` (root) | 25 XML | v4 has 12 additional `Russian.*` files |
| `Army/` | 4 | identical set |
| `BriefingRoom/` | 1 | identical |
| `Email/` | 3 | v4 has 4 (2 base + 2 Russian); v5 adds `EmailSenderNameList.xml` |
| `Inventory/` | 27 | v4 adds `Russian.MercStartingGear.xml` |
| `Items/` | 28 | v4 adds 5 `Russian.*` files |
| `Layout/` | 1 | identical |
| `Lookup/` | 23 | identical names |
| `Map/` | 37 | v4 adds 5 `Russian.*` files |
| `MapAction/` | 1 | identical |
| `Multiplayer/` | 1 | identical |
| `NPCInventory/` | 81 | identical names |
| `Profiles/` | 6 | identical |
| `Sounds/` | 2 | identical |
| **Total** | **240** | 239 XML + 1 readme |

### 7.3 Comparison with v4

Of the 239 XML files present in both versions:

- **71 are byte-identical, 168 differ.**
- **All 239 have identical tag sets** — the column schema is the same.
- Record-count differences are limited to:
  - `Items/Items.xml`: 1,776 → 1,775
  - `Items/Weapons.xml`: 389 → 388
  - `Items/Attachments.xml`: 6,079 → 6,023
- Formatting differs: v5 normalizes all files to CRLF; v4 mixes CRLF and LF.
- **v5 `Lookup/AttachmentPoint.xml` is malformed**: it contains an extra `</AttachmentPoint>` at line 31. A strict parser rejects the file; v4's copy is valid. The editor must report this without silently rewriting the file.

### 7.4 Schema differences in the engine source data

The data bundled with `References/Ja 2 1.13 source` is a third data set: 277 XML files in `gamedir/Base/TableData`, 213 shared with v5. Of those shared files, **17 have different tag sets** (compared with v5), for example:

- `Items/Weapons.xml`: v5 has `szNWSSSound`, source does not.
- `Items/LoadBearingEquipment.xml`: v5 has `lbeCombo`, `lbeAvailableVolume`, and related fields.
- `Items/Pockets.xml`: v5 has numbered `ItemCapacityPerSize*` fields.
- `Items/Items.xml`: source has `Detonator` and `RemoteDetonator`, v5 does not.
- `MercProfiles.xml`: source has `bEvolution`; v5 has `GrowthModifier*` fields instead.

The source `Items.xml` has only 347 records (base data), while the modded v4/v5 have about 1,775 — the game installs are heavily modded, and the three data sets disagree on field sets. Field semantics and default values must come from the engine, not from record counts or from any single data set.

---

## 8. Cross-project comparison

### 8.1 Editor implementations

| Concern | `xml-editor` | `JA 2 1.13 Image viewer` | `JA_2_1.13_TableData_Editor` | Target (XML Editor Reloaded) |
|---|---|---|---|---|
| UI technology | WinForms, MDI | WinForms, single window | WinForms, stub | WPF, MVVM |
| Target framework | `net9.0-windows7.0` | `net10.0-windows7.0` | `net5.0-windows` | `net10.0` backend / `net10.0-windows` app |
| Table metadata | Hardcoded `DataSet` schema | — | SQLite metadata DB | SQLite metadata DB |
| XML access | `DataSet.ReadXml` / typed handlers | — | `XmlDocument` + reflection | Metadata-driven reader/writer with round-trip preservation |
| STI decoding | Own `STI.vb` + `ItemImageManager` | `StiLib.dll` (legacy) | `StiLib` reference (unused) | New `XmlEditorReloaded.Imaging` implementation |
| Item images | Big/medium/small catalogs | Single-file viewer | — | Same catalogs, cached, async |
| Maturity | Complete legacy editor | Working utility | Early prototype | Planned |

### 8.2 Data sets

| Property | v4 | v5 | Engine source `gamedir/Base/TableData` |
|---|---|---|---|
| TableData files | 264 (263 XML) | 240 (239 XML) | 277 XML |
| Items records | 1,776 | 1,775 | 347 |
| Lookup tables | 23 | 23 | 23 |
| Localized XML | 25 Russian overlay files | none (English base) | 622 files across 7 language dirs |
| BOM / line endings | 162 BOM; 91 CRLF, 172 LF | 159 BOM; all CRLF | not measured |
| Schema vs v5 | identical tag sets | — | 17 shared files differ |
| Notable issue | — | malformed `Lookup/AttachmentPoint.xml` | newest fields; base item set |

The key conclusion for the editor: **v4 and v5 differ in data and formatting, not in column schema**, but the engine source data uses a different field set. The editor must preserve whatever fields a file contains and must never assume a fixed column list. Localization is a per-file overlay, not a separate data set: v4 is the English base plus 25 Russian overlays, v5 is the English base alone. The editor must merge overlays by key and write translations back to the overlay only.

---

## 9. Notes for future reference projects

When adding a new reference project:

1. Place it under `References/<Project Name>/` without modifying its contents.
2. Add a section with: purpose, type/framework, dependencies, what to reuse, caveats, and a tree. For multi-gigabyte references, summarize bulk assets and expand only the editor-relevant folders.
3. Add the project to the summary table and to the comparison tables if it covers a new concern.
4. If it contains a huge or resource-heavy source tree, state the permission rule explicitly (as done for `Ja 2 1.13 source`).
5. If it contains game data, record the version, file counts, record counts, XML tag-set differences, BOM/line-ending characteristics, and any malformed files.
6. Never add references to `References/` from projects under `xml-editor-reloaded/`; copy the needed algorithms, metadata, or sample data instead.
