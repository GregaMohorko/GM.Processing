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
Created: 2018-4-3
Author: GregaMohorko
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GM.Processing.Utility;

namespace GM.Processing.Signal.Image
{
	/// <summary>
	/// Represents an image with image planes that can be easily manipulated with.
	/// </summary>
	public class GMImage
	{
		/// <summary>
		/// The width of the image.
		/// </summary>
		public readonly int Width;
		/// <summary>
		/// The height of the image.
		/// </summary>
		public readonly int Height;

		/// <summary>
		/// Color planes of this image.
		/// <para>You can also use <see cref="this[int]"/> to get a single image plane.</para>
		/// </summary>
		public readonly List<GMImagePlane> Planes;

		/// <summary>
		/// The color palette.
		/// <para>Actually only present for 8-bit images.</para>
		/// </summary>
		public readonly int[] Palette;

		/// <summary>
		/// Creates a new instance of <see cref="GMImage"/> with the provided image planes.
		/// </summary>
		/// <param name="planes">A collection of image color planes.</param>
		/// <param name="palette">The color palette.</param>
		public GMImage(List<GMImagePlane> planes, int[] palette = null)
		{
			if(planes == null) {
				throw new ArgumentNullException(nameof(planes));
			}
			if(!planes.Any()) {
				throw new ArgumentException("At least one plane must be provided.", nameof(planes));
			}
			if(!planes.AllSame(p => p.Width, p => p.Height)) {
				throw new ArgumentException("All planes must be of the same dimensions.", nameof(planes));
			}

			GMImagePlane plane = planes.First();

			Width = plane.Width;
			Height = plane.Height;
			Planes = planes;
			Palette = palette;
		}

		/// <summary>
		/// Creates a new instance of <see cref="GMImage"/> with the color planes copied from the provided image.
		/// </summary>
		/// <param name="image">The image from which to copy the color planes.</param>
		public GMImage(GMImage image)
		{
			Width = image.Width;
			Height = image.Height;
			Planes = image.Planes.Select(p => new GMImagePlane(p)).ToList();
			Palette = image.Palette;
		}

		/// <summary>
		/// Determines whether this image is grayscale (has equal values across all planes).
		/// </summary>
		public bool IsGrayscale
		{
			get
			{
				if(_isGrayscale != null) {
					return _isGrayscale.Value;
				}

				if(Planes.Count < 3) {
					_isGrayscale = true;
					return true;
				}

				for(int y = Height - 1; y >= 0; --y) {
					for(int x = Width - 1; x >= 0; --x) {
						byte value = this[0][x, y];
						for(int i = 1; i < 3; ++i) {
							GMImagePlane plane = this[i];
							byte currentValue = plane[x, y];
							if(currentValue != value) {
								// different value on different bit plane of the same pixel
								_isGrayscale = false;
								return false;
							}
						}
					}
				}

				_isGrayscale = true;
				return true;
			}
		}
		private bool? _isGrayscale;

		/// <summary>
		/// Determines whether there are three bit planes in this image (or four if transparent) and that it is not grayscale.
		/// </summary>
		public bool IsRGB
		{
			get
			{
				if(_isRGB != null) {
					return _isRGB.Value;
				}
				_isRGB = Planes.Count >= 3 && !IsGrayscale;
				return _isRGB.Value;
			}
		}
		private bool? _isRGB;

		/// <summary>
		/// Gets the image plane at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the image plane to get.</param>
		public GMImagePlane this[int index] => Planes[index];

		/// <summary>
		/// Gets or sets the color of the pixel at the specified position.
		/// <para>This can only be called if the image has at least 3 image planes.</para>
		/// </summary>
		/// <param name="x">The X.</param>
		/// <param name="y">The Y.</param>
		public Color this[int x, int y]
		{
			get
			{
				if(!(Planes.Count >= 3)) {
					throw new InvalidOperationException("Image must have at least 3 image planes.");
				}
				byte R = this[0][x, y];
				byte G = this[1][x, y];
				byte B = this[2][x, y];
				return Color.FromArgb(R, G, B);
			}
			set
			{
				if(!(Planes.Count >= 3)) {
					throw new InvalidOperationException("Image must have at least 3 bit planes.");
				}
				this[0][x, y] = value.R;
				this[1][x, y] = value.G;
				this[2][x, y] = value.B;
			}
		}

		/// <summary>
		/// Gets the color of the pixel at the specified position.
		/// <para>This method can only be called if the image has at least 3 image planes.</para>
		/// <para>You can also use <see cref="this[int, int]"/>.</para>
		/// </summary>
		/// <param name="x">The X.</param>
		/// <param name="y">The Y.</param>
		public Color GetPixel(int x, int y)
		{
			return this[x, y];
		}

		/// <summary>
		/// Sets the color of the pixel at the specified position.
		/// <para>This method can only be called if the image has at least 3 image planes.</para>
		/// <para>You can also use <see cref="this[int, int]"/>.</para>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="color"></param>
		public void SetPixel(int x, int y, Color color)
		{
			this[x, y] = color;
		}

		/// <summary>
		/// Saves this image to the specified file.
		/// </summary>
		/// <param name="fileName">A string that contains the name of the file to which to save this image.</param>
		public void Save(string fileName)
		{
			Bitmap bmp = ToBitmap();
			
			ImageFormat imageFormat;
			{
				string extensionUpper = Path.GetExtension(fileName).ToUpper();
				switch(extensionUpper) {
					case ".GIF":
						imageFormat = ImageFormat.Gif;
						break;
					case ".ICO":
						imageFormat = ImageFormat.Icon;
						break;
					case ".JPG":
					case ".JPEG":
						imageFormat = ImageFormat.Jpeg;
						break;
					case ".PNG":
						imageFormat = ImageFormat.Png;
						break;
					case ".TIFF":
						imageFormat = ImageFormat.Tiff;
						break;
					case ".BMP":
					default:
						imageFormat = ImageFormat.Bmp;
						break;
				}
			}

			bmp.Save(fileName, imageFormat);
		}

		/// <summary>
		/// Creates and returns a <see cref="Bitmap"/> from this image.
		/// </summary>
		public Bitmap ToBitmap()
		{
			PixelFormat pixelFormat;
			switch(Planes.Count) {
				case 1:
					pixelFormat = PixelFormat.Format8bppIndexed;
					break;
				case 2:
					// throws a weird exception ...
					//pixelFormat = PixelFormat.Format16bppGrayScale;
					pixelFormat = PixelFormat.Format32bppArgb;
					break;
				case 3:
					pixelFormat = PixelFormat.Format24bppRgb;
					break;
				case 4:
					pixelFormat = PixelFormat.Format32bppArgb;
					break;
				default:
					throw new NotImplementedException($"Unsupported number of planes: {Planes.Count}.");
			}

			var bmp = new Bitmap(Width, Height, pixelFormat);
			if(Palette != null && bmp.Palette.Entries.Length == Palette.Length) {
				ColorPalette palette = bmp.Palette;
				for(int i = 0; i < palette.Entries.Length; i++) {
					palette.Entries[i] = Color.FromArgb(Palette[i]);
				}
				bmp.Palette = palette;
			}

			var data = new byte[Width * Height * Planes.Count];
			int pos = 0;
			int pos2 = 0;
			for(int y = 0; y < Height; ++y) {
				for(int x = 0; x < Width; ++x) {
					for(int bp = 0; bp < Planes.Count; ++bp) {
						data[pos++] = this[bp][x, y];
					}
					++pos2;
				}
			}

			BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, pixelFormat);
			Marshal.Copy(data, 0, bmpData.Scan0, data.Length);
			bmp.UnlockBits(bmpData);

			return bmp;
		}

		/// <summary>
		/// Creates a <see cref="GMImage"/> from the specified file.
		/// </summary>
		/// <param name="fileName">The bitmap file name and path.</param>
		public static GMImage Open(string fileName)
		{
			using(var bmp=new Bitmap(fileName)) {
				// locks the image into system memory
				BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);

				int bytesPerPixel;
				// file explorer -> right click on the image file -> Properties -> Details -> Bit depth
				switch(bmp.PixelFormat) {
					case PixelFormat.Format8bppIndexed:
						bytesPerPixel = 1;
						break;
					case PixelFormat.Format16bppGrayScale:
						bytesPerPixel = 2;
						break;
					case PixelFormat.Format24bppRgb:
						bytesPerPixel = 3;
						break;
					case PixelFormat.Format32bppArgb:
						bytesPerPixel = 4;
						break;
					default:
						throw new NotImplementedException("Unsupported pixel format: " + bmp.PixelFormat);
				}

				var data = new byte[bmp.Width * bmp.Height * bytesPerPixel];

				Marshal.Copy(bmpData.Scan0, data, 0, data.Length);

				// unlock the image from system memory
				bmp.UnlockBits(bmpData);

				var planes = new List<GMImagePlane>(bytesPerPixel);
				for(int i = 0; i < bytesPerPixel; i++) {
					GMImagePlane newPlane = GMImagePlane.Create(bmp.Width, bmp.Height, data, i, bytesPerPixel);
					planes.Add(newPlane);
				}

				int[] palette = null;
				if(bytesPerPixel == 1) {
					palette = new int[256];
					for(int i = bmp.Palette.Entries.Length - 1; i >= 0; i--) {
						palette[i] = bmp.Palette.Entries[i].ToArgb();
					}
				}

				return new GMImage(planes, palette);
			}
		}

		/// <summary>
		/// Creates a new instance of <see cref="GMImage"/> with randomly set plane values.
		/// </summary>
		/// <param name="width">The width of the image.</param>
		/// <param name="height">The height of the image.</param>
		/// <param name="planeCount">The number of planes.</param>
		public static GMImage CreateRandom(int width, int height, int planeCount)
		{
			var r = new Random();
			var planes = new List<GMImagePlane>(planeCount);
			for(int i = planeCount; i > 0; i--) {
				var randomBytes = new byte[width * height];
				r.NextBytes(randomBytes);
				var newPlane = GMImagePlane.Create(width, height, randomBytes);
				planes.Add(newPlane);
			}

			return new GMImage(planes);
		}
	}
}
