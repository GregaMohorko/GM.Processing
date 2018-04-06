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
Created: 2018-4-4
Author: GregaMohorko
*/

namespace GM.Processing.Signal.Image
{
	/// <summary>
	/// Utility class for getting the image histogram from images.
	/// <para>https://en.wikipedia.org/wiki/Image_histogram</para>
	/// </summary>
	public static class ImageHistogram
	{
		/// <summary>
		/// Gets the histogram of the provided image plane.
		/// </summary>
		/// <param name="plane">The image plane.</param>
		public static int[] Get(GMImagePlane plane)
		{
			return Get(plane, 0, 0, plane.Width, plane.Height);
		}

		/// <summary>
		/// Gets the histogram for just the specified region of the image plane.
		/// <para>If part of the region is outside of image boundaries, mirrored values are used.</para>
		/// </summary>
		/// <param name="plane">The image plane.</param>
		/// <param name="centerX">The x coordinate of the center position of the region.</param>
		/// <param name="centerY">The y coordinate of the center position of the region.</param>
		/// <param name="width">The width of the region.</param>
		/// <param name="height">The height of the region.</param>
		public static int[] GetFromCenter(GMImagePlane plane, int centerX, int centerY, int width,int height)
		{
			return Get(plane, centerX - width / 2, centerY - height / 2, width, height);
		}

		/// <summary>
		/// Gets the histogram for just the specified region of the image plane.
		/// <para>If part of the region is outside of image boundaries, mirrored values are used.</para>
		/// </summary>
		/// <param name="plane">The image plane.</param>
		/// <param name="x">The x position (left side) of the region.</param>
		/// <param name="y">The y position (bottom) of the region.</param>
		/// <param name="width">The width of the region.</param>
		/// <param name="height">The height of the region.</param>
		public static int[] Get(GMImagePlane plane, int x, int y, int width, int height)
		{
			var hist = new int[256];
			byte value;
			int yy, xx;
			for(yy = y + height - 1; yy >= y; --yy) {
				for(xx = x + width - 1; xx >= x; --xx) {
					value = plane.GetPixelMirrored(xx, yy);
					++hist[value];
				}
			}
			return hist;
		}
	}
}
