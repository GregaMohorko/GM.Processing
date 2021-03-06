﻿/*
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

namespace GM.Processing.Signal.Image
{
	/// <summary>
	/// Utilities for <see cref="GMImagePlane"/>.
	/// </summary>
	public static class GMImagePlaneUtility
	{
		/// <summary>
		/// Converts this image plane to a two-dimensional array of doubles.
		/// </summary>
		/// <param name="plane">The image plane to convert.</param>
		public static double[,] ToDoubles(this GMImagePlane plane)
		{
			if(plane == null) {
				throw new ArgumentNullException(nameof(plane));
			}

			double[,] doubles = new double[plane.Height, plane.Width];
			for(int y = plane.Height - 1; y >= 0; --y) {
				for(int x = plane.Width - 1; x >= 0; --x) {
					byte value = plane[x, y];
					double V = value / 255d;
					doubles[y, x] = V;
				}
			}
			return doubles;
		}
	}
}
