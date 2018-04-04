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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GM.Processing.Signal.Image
{
	/// <summary>
	/// Represents a single color plane of an image.
	/// </summary>
	public class GMImagePlane
	{
		/// <summary>
		/// The width of this plane.
		/// </summary>
		public readonly int Width;
		/// <summary>
		/// The height of this plane.
		/// </summary>
		public readonly int Height;

		/// <summary>
		/// The pixels of this image plane.
		/// <para>Index representations: [y, x].</para>
		/// <para>You can also use <see cref="this[int, int]"/>.</para>
		/// </summary>
		public readonly byte[,] Pixels;

		/// <summary>
		/// Creates a new instance of <see cref="GMImagePlane"/> with the specified width and height.
		/// </summary>
		/// <param name="width">The width of the plane.</param>
		/// <param name="height">The height of the plane.</param>
		public GMImagePlane(int width, int height) : this(new byte[height, width]) { }

		/// <summary>
		/// Creates a new instance of <see cref="GMImagePlane"/> with the pixels copied from the provided byte array.
		/// </summary>
		/// <param name="pixels">The pixels of the plane.</param>
		public GMImagePlane(byte[,] pixels)
		{
			Width = pixels.GetLength(1);
			Height = pixels.GetLength(0);
			Pixels = new byte[Height, Width];
			Array.Copy(pixels, Pixels, pixels.Length);
		}

		/// <summary>
		/// Creates a new instance of <see cref="GMImagePlane"/> with the pixels copied from the provided image plane.
		/// </summary>
		/// <param name="imagePlane">The image plane to copy from.</param>
		public GMImagePlane(GMImagePlane imagePlane)
		{
			Width = imagePlane.Width;
			Height = imagePlane.Height;
			Pixels = new byte[Height, Width];
			SetPixels(imagePlane);
		}

		/// <summary>
		/// Gets or sets the value of the pixel at the specified location.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public byte this[int x, int y]
		{
			get => Pixels[y, x];
			set => Pixels[y, x] = value;
		}

		/// <summary>
		/// Gets the value of the pixel at the specified location.
		/// <para>You can use <see cref="this[int, int]"/>.</para>
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public byte GetPixel(int x, int y)
		{
			return this[x, y];
		}

		/// <summary>
		/// Gets the value of the pixel at the specified position. If the specified position is outside of the boundaries (for a maximum of the images size), a mirrored value will be returned.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public byte GetPixelMirrored(int x, int y)
		{
			if(x < 0) {
				x = -1 - x;
			} else if(x >= Width) {
				x = 2 * Width - x - 1;
			}
			if(y < 0) {
				y = -1 - y;
			} else if(y >= Height) {
				y = 2 * Height - y - 1;
			}

			return this[x, y];
		}

		/// <summary>
		/// Sets the value of the pixel at the specified location.
		/// <para>You can use <see cref="this[int, int]"/>.</para>
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="value">The value to set.</param>
		public void SetPixel(int x, int y, byte value)
		{
			this[x, y] = value;
		}

		/// <summary>
		/// Sets the pixels of this image plane to the same values as the provided plane.
		/// </summary>
		/// <param name="sourcePlane">The image plane from which to copy pixels.</param>
		public void SetPixels(GMImagePlane sourcePlane)
		{
			if(sourcePlane.Width != Width || sourcePlane.Height != Height) {
				throw new ArgumentException("The source image plane must be of same size as this plane.", nameof(sourcePlane));
			}

			Array.Copy(sourcePlane.Pixels, Pixels, Pixels.Length);
		}

		/// <summary>
		/// Creates a new instance of <see cref="GMImagePlane"/> with the specified width and height and the specified data.
		/// </summary>
		/// <param name="width">The width of the plane.</param>
		/// <param name="height">The height of the plane.</param>
		/// <param name="data">One-dimensional array of bytes that should contained stacked rows of pixel values.</param>
		/// <param name="offset">The offset at which to start copying from the data.</param>
		/// <param name="step">The step with which to walk through the data.</param>
		public static GMImagePlane Create(int width, int height, byte[] data, int offset = 0, int step = 1)
		{
			var plane = new GMImagePlane(width, height);
			int pos2 = offset;
			for(int y = 0; y < height; y++) {
				for(int x = 0; x < width; x++) {
					plane[x, y] = data[pos2];
					pos2 += step;
				}
			}
			return plane;
		}

		/// <summary>
		/// Creates a new instance of <see cref="GMImagePlane"/> from the provided double values. The double values should be in the range of [0,1], where 0.0 represents 0 and 1.0 represents 255.
		/// </summary>
		/// <param name="doubles">Array of doubles.</param>
		public static GMImagePlane FromDoubles(double[,] doubles)
		{
			var plane = new GMImagePlane(doubles.GetLength(1), doubles.GetLength(0));
			for(int y = plane.Height - 1; y >= 0; --y) {
				for(int x = plane.Width - 1; x >= 0; --x) {
					double V = doubles[y, x];
					byte value = (byte)(V * 255);
					plane[x, y] = value;
				}
			}
			return plane;
		}
	}
}
