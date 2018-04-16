/*
MIT License

Copyright (c) 2018 Grega Mohorko

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

Project: GM.Processing
Created: 2018-4-10
Author: GregaMohorko
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GM.Processing.Signal.Image
{
#pragma warning disable CS1570 // XML comment has badly formed XML
	/// <summary>
	/// Contains Windows GDI+ image property tags as constants.
	/// <para>https://msdn.microsoft.com/en-us/library/ms534416.aspx?f=255&MSPPError=-2147217396</para>
	/// </summary>
	public static class ImageMetadataTags
#pragma warning restore CS1570 // XML comment has badly formed XML
	{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable IDE1006 // Naming Styles
		public const int GpsVer = 0x0000;
		public const int GpsLatitudeRef = 0x0001;
		public const int GpsLatitude = 0x0002;
		public const int GpsLongitudeRef = 0x0003;
		public const int GpsLongitude = 0x0004;
		public const int GpsAltitudeRef = 0x0005;
		public const int GpsAltitude = 0x0006;
		public const int GpsGpsTime = 0x0007;
		public const int GpsGpsSatellites = 0x0008;
		public const int GpsGpsStatus = 0x0009;
		public const int GpsGpsMeasureMode = 0x000A;
		public const int GpsGpsDop = 0x000B;
		public const int GpsSpeedRef = 0x000C;
		public const int GpsSpeed = 0x000D;
		public const int GpsTrackRef = 0x000E;
		public const int GpsTrack = 0x000F;
		public const int GpsImgDirRef = 0x0010;
		public const int GpsImgDir = 0x0011;
		public const int GpsMapDatum = 0x0012;
		public const int GpsDestLatRef = 0x0013;
		public const int GpsDestLat = 0x0014;
		public const int GpsDestLongRef = 0x0015;
		public const int GpsDestLong = 0x0016;
		public const int GpsDestBearRef = 0x0017;
		public const int GpsDestBear = 0x0018;
		public const int GpsDestDistRef = 0x0019;
		public const int GpsDestDist = 0x001A;
		public const int NewSubfileType = 0x00FE;
		public const int SubfileType = 0x00FF;
		public const int ImageWidth = 0x0100;
		public const int ImageHeight = 0x0101;
		public const int BitsPerSample = 0x0102;
		public const int Compression = 0x0103;
		public const int PhotometricInterp = 0x0106;
		public const int ThreshHolding = 0x0107;
		public const int CellWidth = 0x0108;
		public const int CellHeight = 0x0109;
		public const int FillOrder = 0x010A;
		public const int DocumentName = 0x010D;
		public const int ImageDescription = 0x010E;
		public const int EquipMake = 0x010F;
		public const int EquipModel = 0x0110;
		public const int StripOffsets = 0x0111;
		public const int Orientation = 0x0112;
		public const int SamplesPerPixel = 0x0115;
		public const int RowsPerStrip = 0x0116;
		public const int StripBytesCount = 0x0117;
		public const int MinSampleValue = 0x0118;
		public const int MaxSampleValue = 0x0119;
		public const int XResolution = 0x011A;
		public const int YResolution = 0x011B;
		public const int PlanarConfig = 0x011C;
		public const int PageName = 0x011D;
		public const int XPosition = 0x011E;
		public const int YPosition = 0x011F;
		public const int FreeOffset = 0x0120;
		public const int FreeByteCounts = 0x0121;
		public const int GrayResponseUnit = 0x0122;
		public const int GrayResponseCurve = 0x0123;
		public const int T4Option = 0x0124;
		public const int T6Option = 0x0125;
		public const int ResolutionUnit = 0x0128;
		public const int PageNumber = 0x0129;
		public const int TransferFunction = 0x012D;
		public const int SoftwareUsed = 0x0131;
		public const int DateTime = 0x0132;
		public const int Artist = 0x013B;
		public const int HostComputer = 0x013C;
		public const int Predictor = 0x013D;
		public const int WhitePoint = 0x013E;
		public const int PrimaryChromaticities = 0x013F;
		public const int ColorMap = 0x0140;
		public const int HalftoneHints = 0x0141;
		public const int TileWidth = 0x0142;
		public const int TileLength = 0x0143;
		public const int TileOffset = 0x0144;
		public const int TileByteCounts = 0x0145;
		public const int InkSet = 0x014C;
		public const int InkNames = 0x014D;
		public const int NumberOfInks = 0x014E;
		public const int DotRange = 0x0150;
		public const int TargetPrinter = 0x0151;
		public const int ExtraSamples = 0x0152;
		public const int SampleFormat = 0x0153;
		public const int SMinSampleValue = 0x0154;
		public const int SMaxSampleValue = 0x0155;
		public const int TransferRange = 0x0156;
		public const int JPEGProc = 0x0200;
		public const int JPEGInterFormat = 0x0201;
		public const int JPEGInterLength = 0x0202;
		public const int JPEGRestartInterval = 0x0203;
		public const int JPEGLosslessPredictors = 0x0205;
		public const int JPEGPointTransforms = 0x0206;
		public const int JPEGQTables = 0x0207;
		public const int JPEGDCTables = 0x0208;
		public const int JPEGACTables = 0x0209;
		public const int YCbCrCoefficients = 0x0211;
		public const int YCbCrSubsampling = 0x0212;
		public const int YCbCrPositioning = 0x0213;
		public const int REFBlackWhite = 0x0214;
		public const int Gamma = 0x0301;
		public const int ICCProfileDescriptor = 0x0302;
		public const int SRGBRenderingIntent = 0x0303;
		public const int ImageTitle = 0x0320;
		public const int ResolutionXUnit = 0x5001;
		public const int ResolutionYUnit = 0x5002;
		public const int ResolutionXLengthUnit = 0x5003;
		public const int ResolutionYLengthUnit = 0x5004;
		public const int PrintFlags = 0x5005;
		public const int PrintFlagsVersion = 0x5006;
		public const int PrintFlagsCrop = 0x5007;
		public const int PrintFlagsBleedWidth = 0x5008;
		public const int PrintFlagsBleedWidthScale = 0x5009;
		public const int HalftoneLPI = 0x500A;
		public const int HalftoneLPIUnit = 0x500B;
		public const int HalftoneDegree = 0x500C;
		public const int HalftoneShape = 0x500D;
		public const int HalftoneMisc = 0x500E;
		public const int HalftoneScreen = 0x500F;
		public const int JPEGQuality = 0x5010;
		public const int GridSize = 0x5011;
		public const int ThumbnailFormat = 0x5012;
		public const int ThumbnailWidth = 0x5013;
		public const int ThumbnailHeight = 0x5014;
		public const int ThumbnailColorDepth = 0x5015;
		public const int ThumbnailPlanes = 0x5016;
		public const int ThumbnailRawBytes = 0x5017;
		public const int ThumbnailSize = 0x5018;
		public const int ThumbnailCompressedSize = 0x5019;
		public const int ColorTransferFunction = 0x501A;
		public const int ThumbnailData = 0x501B;
		public const int ThumbnailImageWidth = 0x5020;
		public const int ThumbnailImageHeight = 0x5021;
		public const int ThumbnailBitsPerSample = 0x5022;
		public const int ThumbnailCompression = 0x5023;
		public const int ThumbnailPhotometricInterp = 0x5024;
		public const int ThumbnailImageDescription = 0x5025;
		public const int ThumbnailEquipMake = 0x5026;
		public const int ThumbnailEquipModel = 0x5027;
		public const int ThumbnailStripOffsets = 0x5028;
		public const int ThumbnailOrientation = 0x5029;
		public const int ThumbnailSamplesPerPixel = 0x502A;
		public const int ThumbnailRowsPerStrip = 0x502B;
		public const int ThumbnailStripBytesCount = 0x502C;
		public const int ThumbnailResolutionX = 0x502D;
		public const int ThumbnailResolutionY = 0x502E;
		public const int ThumbnailPlanarConfig = 0x502F;
		public const int ThumbnailResolutionUnit = 0x5030;
		public const int ThumbnailTransferFunction = 0x5031;
		public const int ThumbnailSoftwareUsed = 0x5032;
		public const int ThumbnailDateTime = 0x5033;
		public const int ThumbnailArtist = 0x5034;
		public const int ThumbnailWhitePoint = 0x5035;
		public const int ThumbnailPrimaryChromaticities = 0x5036;
		public const int ThumbnailYCbCrCoefficients = 0x5037;
		public const int ThumbnailYCbCrSubsampling = 0x5038;
		public const int ThumbnailYCbCrPositioning = 0x5039;
		public const int ThumbnailRefBlackWhite = 0x503A;
		public const int ThumbnailCopyRight = 0x503B;
		public const int LuminanceTable = 0x5090;
		public const int ChrominanceTable = 0x5091;
		public const int FrameDelay = 0x5100;
		public const int LoopCount = 0x5101;
		public const int GlobalPalette = 0x5102;
		public const int IndexBackground = 0x5103;
		public const int IndexTransparent = 0x5104;
		public const int PixelUnit = 0x5110;
		public const int PixelPerUnitX = 0x5111;
		public const int PixelPerUnitY = 0x5112;
		public const int PaletteHistogram = 0x5113;
		public const int Copyright = 0x8298;
		/// <summary>
		/// Exposure time, measured in seconds.
		/// </summary>
		public const int ExifExposureTime = 0x829A;
		public const int ExifFNumber = 0x829D;
		public const int ExifIFD = 0x8769;
		public const int ICCProfile = 0x8773;
		public const int ExifExposureProg = 0x8822;
		public const int ExifSpectralSense = 0x8824;
		public const int GpsIFD = 0x8825;
		public const int ExifISOSpeed = 0x8827;
		public const int ExifOECF = 0x8828;
		public const int ExifVer = 0x9000;
		public const int ExifDTOrig = 0x9003;
		public const int ExifDTDigitized = 0x9004;
		public const int ExifCompConfig = 0x9101;
		public const int ExifCompBPP = 0x9102;
		public const int ExifShutterSpeed = 0x9201;
		public const int ExifAperture = 0x9202;
		public const int ExifBrightness = 0x9203;
		public const int ExifExposureBias = 0x9204;
		public const int ExifMaxAperture = 0x9205;
		public const int ExifSubjectDist = 0x9206;
		public const int ExifMeteringMode = 0x9207;
		public const int ExifLightSource = 0x9208;
		public const int ExifFlash = 0x9209;
		public const int ExifFocalLength = 0x920A;
		public const int ExifMakerNote = 0x927C;
		public const int ExifUserComment = 0x9286;
		public const int ExifDTSubsec = 0x9290;
		public const int ExifDTOrigSS = 0x9291;
		public const int ExifDTDigSS = 0x9292;
		public const int ExifFPXVer = 0xA000;
		public const int ExifColorSpace = 0xA001;
		public const int ExifPixXDim = 0xA002;
		public const int ExifPixYDim = 0xA003;
		public const int ExifRelatedWav = 0xA004;
		public const int ExifInterop = 0xA005;
		public const int ExifFlashEnergy = 0xA20B;
		public const int ExifSpatialFR = 0xA20C;
		public const int ExifFocalXRes = 0xA20E;
		public const int ExifFocalYRes = 0xA20F;
		public const int ExifFocalResUnit = 0xA210;
		public const int ExifSubjectLoc = 0xA214;
		/// <summary>
		/// Exposure index selected on the camera or input device at the time the image was captured.
		/// </summary>
		public const int ExifExposureIndex = 0xA215;
		public const int ExifSensingMethod = 0xA217;
		public const int ExifFileSource = 0xA300;
		public const int ExifSceneType = 0xA301;
		public const int ExifCfaPattern = 0xA302;
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
	}
}
