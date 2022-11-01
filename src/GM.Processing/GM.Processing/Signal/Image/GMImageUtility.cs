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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GM.Utility;

namespace GM.Processing.Signal.Image
{
	/// <summary>
	/// Utilities for <see cref="GMImage"/>.
	/// </summary>
	public static class GMImageUtility
	{
		/// <summary>
		/// Applies the plane at the specified index to all other color planes.
		/// </summary>
		/// <param name="image">The image.</param>
		/// <param name="planeIndex">The index of the bit plane to apply to others.</param>
		public static void ApplyOneBitPlaneToAllOtherColorBitPlanes(this GMImage image, int planeIndex)
		{
			if(image == null) {
				throw new ArgumentNullException(nameof(image));
			}
			if(planeIndex >= image.Planes.Count) {
				throw new ArgumentOutOfRangeException(nameof(planeIndex), "The index must be lower than the number of planes of the provided image.");
			}

			GMImagePlane sourcePlane = image[planeIndex];
			int planesCount = Math.Min(image.Planes.Count, 3); // ignore the alpha plane
			for(int i = 1; i < planesCount; ++i) {
				if(i == planeIndex) {
					continue;
				}
				GMImagePlane plane = image[i];
				plane.SetPixels(sourcePlane);
			}
		}

		/// <summary>
		/// Applies the provided segments to this image.
		/// </summary>
		/// <param name="image">The image to segment.</param>
		/// <param name="segmentIDs">An array of the same size as the provided image which holds the IDs of the clusters. These values are used as indexes for segment colors.</param>
		/// <param name="segmentColors">The segment colors (RGB).</param>
		public static void ApplySegments(this GMImage image, int[,] segmentIDs, Color[] segmentColors)
		{
			if(image == null) {
				throw new ArgumentNullException(nameof(image));
			}
			if(segmentIDs == null) {
				throw new ArgumentNullException(nameof(segmentIDs));
			}
			if(segmentColors == null) {
				throw new ArgumentNullException(nameof(segmentColors));
			}
			if(segmentIDs.GetLength(1) != image.Width || segmentIDs.GetLength(0) != image.Height) {
				throw new ArgumentException("The segment IDs must be of the same size as the image.", nameof(segmentIDs));
			}

			// give the pixels of each segment its color value
			for(int y = image.Height - 1; y >= 0; --y) {
				for(int x = image.Width - 1; x >= 0; --x) {
					int segmentID = segmentIDs[y, x];
					if(segmentID == -1) {
						image[x, y] = Color.Black;
						continue;
					}
					Color segmentColor = segmentColors[segmentID];
					image[x, y] = segmentColor;
				}
			}
		}

		/// <summary>
		/// Draws squares at the specified locations with the specified side length and the specified color.
		/// </summary>
		/// <param name="image">The image.</param>
		/// <param name="locations">The locations of the squares.</param>
		/// <param name="sideLength">The length of the squares sides.</param>
		/// <param name="color">The color of the squares.</param>
		public static void DrawSquares(this GMImage image, int[,] locations, int sideLength, Color color)
		{
			if(color == null) {
				throw new ArgumentNullException(nameof(color));
			}

			DrawSquares(image, locations, sideLength, color, color);
		}

		/// <summary>
		/// Draws squares at the specified locations with the specified side length and the specified fill and border colors.
		/// </summary>
		/// <param name="image">The image.</param>
		/// <param name="locations">The locations of the squares.</param>
		/// <param name="sideLength">The length of the squares sides.</param>
		/// <param name="fill">The color of the square.</param>
		/// <param name="border">The color of the square border.</param>
		/// <exception cref="InvalidOperationException">Thrown when the image does not have three image planes.</exception>
		public static void DrawSquares(this GMImage image, int[,] locations, int sideLength, Color fill, Color border)
		{
			if(image == null) {
				throw new ArgumentNullException(nameof(image));
			}
			if(locations == null) {
				throw new ArgumentNullException(nameof(locations));
			}
			if(fill == null) {
				throw new ArgumentNullException(nameof(fill));
			}
			if(border == null) {
				throw new ArgumentNullException(nameof(border));
			}
			if(image.Planes.Count < 3) {
				throw new InvalidOperationException("An image must have at least three color planes in order to draw squares.");
			}

			int sideLengthHalf1 = sideLength / 2;
			int sideLengthHalf2;
			if(sideLength % 2 == 0) {
				sideLengthHalf2 = sideLengthHalf1 - 1;
			} else {
				sideLengthHalf2 = sideLengthHalf1;
			}

			// for each square
			for(int i = locations.GetLength(0) - 1; i >= 0; --i) {
				int squareY = locations[i, 0];
				int squareX = locations[i, 1];

				int yStart = Math.Max(0, Math.Min(image.Height - 1, squareY - sideLengthHalf1));
				int yEnd = Math.Max(0, Math.Min(image.Height - 1, squareY + sideLengthHalf2));
				int xStart = Math.Max(0, Math.Min(image.Width - 1, squareX - sideLengthHalf1));
				int xEnd = Math.Max(0, Math.Min(image.Width - 1, squareX + sideLengthHalf2));
				int yFillStart = yStart + 1;
				int yFillEnd = yEnd - 1;
				int xFillStart = xStart + 1;
				int xFillEnd = xEnd - 1;

				// draw the vertical square borders
				for(int y = yEnd; y >= yStart; --y) {
					image[xStart, y] = border;
					image[xEnd, y] = border;
				}
				// draw the horizontal square borders
				for(int x = xEnd; x >= xStart; --x) {
					image[x, yStart] = border;
					image[x, yEnd] = border;
				}
				// draw the fill
				for(int y = yFillEnd; y >= yFillStart; --y) {
					for(int x = xFillEnd; x >= xFillStart; --x) {
						image[x, y] = fill;
					}
				}
			}
		}

		/// <summary>
		/// Sets the pixel values to the provided HSV values.
		/// </summary>
		/// <param name="image">The image.</param>
		/// <param name="hues">An array of HUEs.</param>
		/// <param name="saturations">An array of saturations.</param>
		/// <param name="values">An array of values.</param>
		/// <exception cref="InvalidOperationException">Thrown when the image does not have three image planes.</exception>
		public static void SetPixelsFromHSV(this GMImage image, double[,] hues, double[,] saturations, double[,] values)
		{
			if(image == null) {
				throw new ArgumentNullException(nameof(image));
			}
			if(hues == null) {
				throw new ArgumentNullException(nameof(hues));
			}
			if(saturations == null) {
				throw new ArgumentNullException(nameof(saturations));
			}
			if(values == null) {
				throw new ArgumentNullException(nameof(values));
			}
			if(image.Planes.Count < 3) {
				throw new InvalidOperationException("An image must have at least three color planes in order to set pixels from HSV.");
			}
			if(hues.GetLength(1) != image.Width || hues.GetLength(0) != image.Height) {
				throw new ArgumentException("The array of HUEs must be of the same size as the image.", nameof(hues));
			}
			if(saturations.GetLength(1) != image.Width || saturations.GetLength(0) != image.Height) {
				throw new ArgumentException("The array of saturations must be of the same size as the image.", nameof(saturations));
			}
			if(values.GetLength(1) != image.Width || values.GetLength(0) != image.Height) {
				throw new ArgumentException("The array of values must be of the same size as the image.", nameof(values));
			}

			for(int y = image.Height - 1; y >= 0; --y) {
				for(int x = image.Width - 1; x >= 0; --x) {
					double H = hues[y, x];
					double S = saturations[y, x];
					double V = values[y, x];
					Color color = ColorUtility.HSVToRGB(H, S, V);
					image[x, y] = color;
				}
			}
		}

		/// <summary>
		/// Converts this image to an array of arrays where the indexes of the first outer array are Y and X coordinates. The index of the inner array represents each image plane.
		/// </summary>
		/// <param name="image">The image.</param>
		public static double [,][] ToDoubles(this GMImage image)
		{
			if(image == null) {
				throw new ArgumentNullException(nameof(image));
			}
			var doubles = new double[image.Height, image.Width][];
			for(int y = image.Height - 1; y >= 0; --y) {
				for(int x = image.Width - 1; x >= 0; --x) {
					var planes = new double[image.Planes.Count];
					for(int i = image.Planes.Count - 1; i >= 0; --i) {
						byte value = image[i][x, y];
						planes[i] = value / 255d;
					}
					doubles[y, x] = planes;
				}
			}
			return doubles;
		}

		/// <summary>
		/// Converts this image to CIE-L*ab color space.
		/// <para>Returns an array of arrays where the indexes of the first outer array are Y and X coordinates. The index of the inner array represents components of the CIE-L*ab color space (L, a and b).</para>
		/// </summary>
		/// <param name="image">The image.</param>
		/// <exception cref="InvalidOperationException">Thrown when the image does not have three image planes.</exception>
		public static double[,][] ToCIELAB(this GMImage image)
		{
			if(image == null) {
				throw new ArgumentNullException(nameof(image));
			}
			if(image.Planes.Count < 3) {
				throw new InvalidOperationException("An image must have three image planes in order to convert it to CIE-L*ab color space.");
			}

			var cielab = new double[image.Height, image.Width][];
			for(int y = image.Height - 1; y >= 0; --y) {
				for(int x = image.Width - 1; x >= 0; --x) {
					byte R = image[0][x, y];
					byte G = image[1][x, y];
					byte B = image[2][x, y];
					ColorUtility.RGBToCIELAB(R, G, B, out double L, out double a, out double b);
					cielab[y, x] = new double[3] { L, a, b };
				}
			}
			return cielab;
		}

		/// <summary>
		/// Converts this image to HSV color space.
		/// </summary>
		/// <param name="image">The image.</param>
		/// <param name="hues">The HUEs.</param>
		/// <param name="saturations">The saturations.</param>
		/// <param name="values">The values.</param>
		public static void ToHSV(this GMImage image, out double[,] hues, out double[,] saturations, out double[,] values)
		{
			if(image == null) {
				throw new ArgumentNullException(nameof(image));
			}
			if(image.Planes.Count < 3) {
				throw new InvalidOperationException("An image must have three image planes in order to convert it to CIE-L*ab color space.");
			}

			hues = new double[image.Height, image.Width];
			saturations = new double[image.Height, image.Width];
			values = new double[image.Height, image.Width];
			for(int y = image.Height - 1; y >= 0; --y) {
				for(int x = image.Width - 1; x >= 0; --x) {
					Color color = image[x, y];
					ColorUtility.RGBToHSV(color, out double H, out double S, out double V);
					hues[y, x] = H;
					saturations[y, x] = S;
					values[y, x] = V;
				}
			}
		}
	}
}
