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
using System.Threading;

namespace GM.Processing.Signal.Image.ContrastEnhancement
{
	/// <summary>
	/// Implementation of (Contrast Limited) Histogram equalization (HE (or CLHE if limited)) algorithm that improves image contrast using the image's histogram.
	/// <para>This method is useful in images with backgrounds and foregrounds that are both bright or both dark, where the distribution of pixel values is similar throughout the image.</para>
	/// <para>https://en.wikipedia.org/wiki/Histogram_equalization</para>
	/// </summary>
	public static class HistogramEqualization
	{
		/// <summary>
		/// Adjusts the contrast of the provided image using the Histogram equalization (HE) method.
		/// <para>If the image is not grayscale, the image is first converted to HSV color space and the algorithm is applied to the Value channel.</para>
		/// </summary>
		/// <param name="image">The image to adjust contrast to.</param>
		/// <param name="clipLimit">A value that limits the amplification. Common values are between 3 and 4.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the process.</param>
		public static void AdjustContrast(GMImage image, double clipLimit = 0, CancellationToken? cancellationToken=null)
		{
			if(image == null) {
				throw new ArgumentNullException(nameof(image));
			}

			if(image.IsGrayscale) {
				AdjustContrastOfGrayscale(image, clipLimit, cancellationToken);
			} else {
				AdjustContrastOfRGB(image, clipLimit, cancellationToken);
			}
		}

		/// <summary>
		/// Adjusts the contrast of the provided image using the Histogram equalization (HE) method. This method assumes that the image has three bit planes (R, G and B) and will first convert it to HSV color space and apply the algorithm there.
		/// </summary>
		/// <param name="image">The image to adjust contrast to.</param>
		/// <param name="clipLimit">A value that limits the amplification. Common values are between 3 and 4.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the process.</param>
		public static void AdjustContrastOfRGB(GMImage image, double clipLimit = 0, CancellationToken? cancellationToken = null)
		{
			if(image == null) {
				throw new ArgumentNullException(nameof(image));
			}
			if(!image.IsRGB) {
				throw new ArgumentException("The image must be RGB.", nameof(image));
			}

			// convert to HSV
			image.ToHSV(out double[,] hues, out double[,] saturations, out double[,] values);
			// transform the values from double to byte
			var valuePlane = GMImagePlane.FromDoubles(values);
			// apply the algorithm on the value plane
			HistogramEqualizationImpl(valuePlane, clipLimit, cancellationToken ?? CancellationToken.None);
			// transform the values back to double
			values = valuePlane.ToDoubles();
			// apply colors
			image.SetPixelsFromHSV(hues, saturations, values);
		}

		/// <summary>
		/// Adjusts the contrast of the provided image using the Histogram equalization (HE) method. This method assumes that the image is grayscale and will run the algorithm on one bit plane and then apply it to others.
		/// </summary>
		/// <param name="image">The image to adjust contrast to.</param>
		/// <param name="clipLimit">A value that limits the amplification. Common values are between 3 and 4.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the process.</param>
		public static void AdjustContrastOfGrayscale(GMImage image, double clipLimit = 0, CancellationToken? cancellationToken = null)
		{
			if(image == null) {
				throw new ArgumentNullException(nameof(image));
			}

			// apply the algorithm on the first plane
			GMImagePlane firstPlane = image[0];
			HistogramEqualizationImpl(firstPlane, clipLimit, cancellationToken??CancellationToken.None);

			// apply it to all other bit planes
			image.ApplyOneBitPlaneToAllOtherColorBitPlanes(0);
		}

		private static void HistogramEqualizationImpl(GMImagePlane plane, double clipLimit, CancellationToken cancellationToken)
		{
			var histogram = ImageHistogram.Get(plane);

			int MxN = plane.Width * plane.Height;

			ClipHistogram(histogram, clipLimit, MxN);

			double MxN1 = MxN - 1;

			// cumulative distribution function
			int cdfMin = -1;
			int cdf = -1;

			var equalizations = new byte[256];

			if(cancellationToken.IsCancellationRequested) {
				return;
			}

			int occurenceCount;
			for(int value = 0; value < 256; ++value) {
				occurenceCount = histogram[value];
				if(occurenceCount == 0) {
					continue;
				}
				if(cdfMin == -1) {
					cdfMin = occurenceCount;
					cdf = cdfMin;
					// the first value will always be zero
					equalizations[value] = 0;
					continue;
				}
				// add the count of occurences
				cdf += occurenceCount;
				// calculate the equalization
				equalizations[value] = (byte)Math.Round(((cdf - cdfMin) / MxN1) * 255);
			}

			if(cancellationToken.IsCancellationRequested) {
				return;
			}

			// apply new values to the image plane
			byte currentValue;
			byte newValue;
			int x;
			for(int y = plane.Height - 1; y >= 0; --y) {
				if(cancellationToken.IsCancellationRequested) {
					return;
				}
				for(x = plane.Width - 1; x >= 0; --x) {
					currentValue = plane[x, y];
					newValue = equalizations[currentValue];
					plane[x, y] = newValue;
				}
			}
		}

		/// <summary>
		/// Clips the histogram to the specified limit.
		/// </summary>
		/// <param name="histogram">The histogram to clip.</param>
		/// <param name="clipLimit">The limit to clip to. Common values are between 3 and 4. If this value is below or equal to zero, clipping is not done.</param>
		/// <param name="pixelCount">Number of pixels in the picture. This value is important for the calculation of the actual clip level.</param>
		public static void ClipHistogram(int[] histogram, double clipLimit, int pixelCount)
		{
			if(histogram == null) {
				throw new ArgumentNullException(nameof(histogram));
			}

			if(clipLimit <= 0) {
				return;
			}

			// calculate the clip level
			int clipLevel = (int)Math.Round((clipLimit * pixelCount) / 256d);

			int occurenceCount;
			int excessCount = 0;
			int value;
			for(value = 255; value >= 0; --value) {
				occurenceCount = histogram[value];
				if(occurenceCount > clipLevel) {
					// clip it
					histogram[value] = clipLevel;
					// add it to excess count
					excessCount += occurenceCount - clipLevel;
				}
			}

			// It is advantageous not to discard the part of the histogram that exceeds the clip limit but to redistribute it equally among all histogram bins.
			int addPerBin = (int)Math.Round(excessCount / 256d);
			for(value = 255; value >= 0; --value) {
				histogram[value] += addPerBin;
			}

			// The redistribution will push some bins over the clip limit again, resulting in an effective clip limit that is larger than the prescribed limit and the exact value of which depends on the image. If this is undesirable, the redistribution procedure can be repeated recursively until the excess is negligible.
		}
	}
}
