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

using System.Drawing;
using GM.Processing.Signal.Image;

namespace GM.Processing.ComputerScience.ComputerGraphics.ContourLines
{
	/// <summary>
	/// The most simple implementation of drawing contour lines. It simply compares each pixel to the 8 surrounding values (that are not yet determined to be part of a contour line) and draws a contour if at least 2 of them differs from the current pixel.
	/// <para>This method is actually only usable when the image is segmented.</para>
	/// </summary>
	public static class SegmentContourLines
	{
		private static readonly int[] surroundPixelsX = { -1, -1, 0, 1, 1, 1, 0, -1 };
		private static readonly int[] surroundPixelsY = { 0, -1, -1, -1, 0, 1, 1, 1 };

		/// <summary>
		/// Displays a single pixel wide contour around the segments.
		/// </summary>
		/// <param name="image">The image to display segment contours.</param>
		/// <param name="segmentIDs">An array of the same size as the provided image which holds the IDs of the clusters.</param>
		/// <param name="contourColor">The color of the contour.</param>
		public static void DrawContours(this GMImage image, int[,] segmentIDs, Color contourColor)
		{
			// determines whether a pixel at a specific position is already a contour
			bool[,] isContour = new bool[image.Height, image.Width];

			for(int y = image.Height - 1; y >= 0; --y) {
				for(int x = image.Width - 1; x >= 0; --x) {
					int segmentBorderCount = 0;

					// compare the pixel to its 8 neighbours
					for(int i = 7; i >= 0; --i) {
						int xx = x + surroundPixelsX[i];
						int yy = y + surroundPixelsY[i];
						if(xx < 0 || xx >= image.Width || yy < 0 || yy >= image.Height) {
							continue;
						}
						if(!isContour[yy, xx] && segmentIDs[y, x] != segmentIDs[yy, xx]) {
							++segmentBorderCount;
						}
					}

					if(segmentBorderCount >= 2) {
						image[x, y] = contourColor;
						isContour[y, x] = true;
					}
				}
			}
		}
	}
}
