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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GM.Processing.Signal.Image.ContrastEnhancement
{
	/// <summary>
	/// Implementation of Adaptive histogram equalization (AHE) algorithm that improves image contrast using the image's histogram and also adapts to local changes in contrast.
	/// <para>This method is useful in images that contain regions that are significantly lighter or darker than most of the image. It is suitable for improving the local contrast and enhancing the definitions of edges in each region of an image.</para>
	/// <para>https://en.wikipedia.org/wiki/Adaptive_histogram_equalization</para>
	/// </summary>
	public static class AdaptiveHistogramEqualization
	{
		/// <summary>
		/// Adjusts the contrast of the provided image using the Adaptive histogram equalization (AHE) technique.
		/// <para>If the image is not grayscale, the image is first converted to HSV color space and the algorithm is applied to the Value channel.</para>
		/// </summary>
		/// <param name="image">The image to adjust contrast adaptively to.</param>
		/// <param name="tileSize">The size of the neighbourhood region. It constitutes a characteristic length scale: contrast at smaller scales is enhanced, while contrast at larger scales is reduced.</param>
		/// <param name="clipLimit">A value that limits the amplification. Common values are between 3 and 4.</param>
		public static void AdjustContrast(GMImage image, int tileSize = 128, double clipLimit = 0)
		{
			if(image.IsGrayscale) {
				AdjustContrastOfGrayscale(image, tileSize, clipLimit);
			} else {
				AdjustContrastOfRGB(image, tileSize, clipLimit);
			}
		}

		/// <summary>
		/// Adjusts the contrast of the provided image using the Adaptive histogram equalization (AHE) technique. This method assumes that the image has three bit planes (R, G and B) and will first convert it to HSV color space and apply the algorithm there.
		/// <param name="image">The image to adjust contrast adaptively to.</param>
		/// <param name="tileSize">The size of the neighbourhood region. It constitutes a characteristic length scale: contrast at smaller scales is enhanced, while contrast at larger scales is reduced.</param>
		/// <param name="clipLimit">A value that limits the amplification. Common values are between 3 and 4.</param>
		/// </summary>
		public static void AdjustContrastOfRGB(GMImage image, int tileSize = 128, double clipLimit = 0)
		{
			if(image.Planes.Count != 3) {
				throw new ArgumentException("The image must have at least three planes.", nameof(image));
			}

			// convert to HSV
			image.ToHSV(out double[,] hues, out double[,] saturations, out double[,] values);
			// transform the values from double to byte
			var valuePlane = GMImagePlane.FromDoubles(values);
			// apply the algorithm on the value plane
			AdaptiveHistogramEqualizationImpl(valuePlane, tileSize, clipLimit);
			// transform the values back to double
			values = valuePlane.ToDoubles();
			// apply colors
			image.SetPixelsFromHSV(hues, saturations, values);
		}

		/// <summary>
		/// Adjusts the contrast of the provided image using the Adaptive histogram equalization (AHE) technique. This method assumes that the image is grayscale and will run the algorithm on one bit plane and then apply it to others.
		/// </summary>
		/// <param name="image">The image to adjust contrast adaptively to.</param>
		/// <param name="tileSize">The size of the neighbourhood region. It constitutes a characteristic length scale: contrast at smaller scales is enhanced, while contrast at larger scales is reduced.</param>
		/// <param name="clipLimit">A value that limits the amplification. Common values are between 3 and 4.</param>
		public static void AdjustContrastOfGrayscale(GMImage image, int tileSize = 128, double clipLimit = 0)
		{
			// apply the algorithm on the first plane
			GMImagePlane firstPlane = image[0];
			AdaptiveHistogramEqualizationImpl(firstPlane, tileSize, clipLimit);

			// apply it to all other bit planes
			image.ApplyOneBitPlaneToAllOtherColorBitPlanes(0);
		}

		private static void AdaptiveHistogramEqualizationImpl(GMImagePlane plane, int tileSize, double clipLimit)
		{
			var newPlane = new GMImagePlane(plane);

			int MxN = tileSize * tileSize;
			double MxN1 = MxN - 1;

			int xBorder = plane.Width - 1;

			int y = plane.Height - 1;
			int x = xBorder;

			// get the histogram of the first neighbourhood region
			var window = ImageHistogram.Get(plane, x, y, tileSize, tileSize);
			int[] histogramBuffer = new int[256];
			int[] histogram;

			byte currentValue;
			byte oldValue;
			byte newValue;
			int cdfMin;
			int cdf;
			int occurenceCount;
			int xOldColumn;
			int xNewColumn;
			int value;
			int xx;
			int yy;
			int yOldTopRow;

			// it starts on the top-right side
			bool isGoingRight = false;
			// vertical movement
			while(true) {
				// horizontal movement
				while(true) {
					// transform the pixel
					if(clipLimit <= 0) {
						histogram = window;
					} else {
						Array.Copy(window, histogramBuffer, window.Length);
						HistogramEqualization.ClipHistogram(histogramBuffer, clipLimit, MxN);
						histogram = histogramBuffer;
					}
					currentValue = plane[x, y];
					cdfMin = -1;
					cdf = 0;
					for(value = 0; value < 256; ++value) {
						occurenceCount = histogram[value];
						if(occurenceCount == 0) {
							continue;
						}
						if(cdfMin == -1) {
							cdfMin = occurenceCount;
						}
						// add the count of occurences
						cdf += occurenceCount;
						if(value == currentValue) {
							// calculate the equalization
							newValue = (byte)Math.Round(((cdf - cdfMin) / MxN1) * 255);
							newPlane[x, y] = newValue;
							break;
						}
					}

					// move next
					if(isGoingRight) {
						if(x == xBorder) {
							break;
						}
						// subtract the left column
						xOldColumn = x;
						++x;
						// add the new right column
						xNewColumn = x + tileSize - 1;
					} else {
						if(x == 0) {
							break;
						}
						// subtract the right column
						xOldColumn = x + tileSize - 1;
						// add the new left column
						--x;
						xNewColumn = x;
					}

					// move the window
					for(yy = y + tileSize - 1; yy >= y; --yy) {
						oldValue = plane.GetPixelMirrored(xOldColumn, yy);
						newValue = plane.GetPixelMirrored(xNewColumn, yy);
						if(oldValue == newValue) {
							continue;
						}
						--window[oldValue];
						++window[newValue];
					}
				}

				if(y == 0) {
					break;
				}

				// the window moves down and updates
				{
					// subtract the top row
					yOldTopRow = y + tileSize - 1;
					// add the new bottom row
					--y;
					for(xx = x + tileSize - 1; xx >= x; --xx) {
						oldValue = plane.GetPixelMirrored(xx, yOldTopRow);
						newValue = plane.GetPixelMirrored(xx, y);
						if(oldValue == newValue) {
							continue;
						}
						--window[oldValue];
						++window[newValue];
					}
				}

				// change the direction
				isGoingRight = !isGoingRight;
				x = isGoingRight ? 0 : xBorder;
			}
			
			plane.SetPixels(newPlane);
		}
	}
}
