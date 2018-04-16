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
Created: 2018-4-8
Author: GregaMohorko
*/

using System;
using System.Linq;
using System.Threading;
using GM.Utility;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearRegression;

namespace GM.Processing.Signal.Image.Bracketing
{
	/// <summary>
	/// Implementation of Exposure Bracketing, a postprocessing technique to create a high dynamic range image that exposes different portions of the image by different amounts.
	/// <para>https://en.wikipedia.org/wiki/Bracketing#Exposure_bracketing</para>
	/// <para>Exposure bracketing usually deals with high-contrast subjects.</para>
	/// <para>The photographer chooses to take one picture at a given exposure, one or more brighter, and one or more darker, in order to select the most satisfactory image.</para>
	/// </summary>
	public static class ExposureBracketing
	{
		/// <summary>
		/// Reconstructs the High Dynamic Range radiance image from the specified images.
		/// </summary>
		/// <param name="images">A number of digitized photographs taken from the same vantage point with different known exposure durations.</param>
		/// <param name="N">Number of random pixel locations to sample from. Computational complexity considerations make it impractical to use every pixel location in this algorithm.</param>
		/// <param name="smoothness">Determines the amount of smoothness. It weights the smoothness term relative to the data fitting term, and should be chosen appropriately for the amount of noise expected in the measurements.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the process.</param>
		/// <param name="progress">An action to report the progress [0.0-1.0].</param>
		/// <param name="rand">The <see cref="Random"/> object to use for generating random pixel locations.</param>
		public static GMImage ReconstructHDR(GMImage[] images, int N = 256, int smoothness=10, CancellationToken? cancellationToken=null, Action<double> progress=null, Random rand=null)
		{
			if(images == null) {
				throw new ArgumentNullException(nameof(images));
			}
			if(images.Length < 2) {
				throw new ArgumentOutOfRangeException(nameof(images), "At least 2 images should be provided.");
			}
			if(!images.AllSame(im => im.Height, im => im.Width)) {
				throw new ArgumentException("All images must be of same size.",nameof(images));
			}
			if(!images.All(im => im.Planes.Count >= 3)) {
				throw new ArgumentException("Currently, only RGB images are supported.", nameof(images));
			}

			int i,y,x,j,p;

			int imageHeight = images[0].Height;
			int imageWidth = images[0].Width;

			CancellationToken cancelToken = cancellationToken ?? CancellationToken.None;

			double[] logExposureTimes;
			#region READ EXPOSURE DURATIONS
			logExposureTimes = new double[images.Length];
			// read exposure durations from the images metadata
			for(i = 0; i < images.Length; ++i) {
				GMImage image = images[i];
				if(!image.Metadata.TryGetDouble(ImageMetadataTags.ExifExposureTime, out double exposureTime)) {
					throw new ArgumentException($"The image at index '{i}' has no metadata about {nameof(ImageMetadataTags.ExifExposureTime)}.", nameof(images));
				}
				double logExposureTime = Math.Log(exposureTime);
				logExposureTimes[i] = logExposureTime;
			}
			#endregion READ EXPOSURE DURATIONS

			// We will assume that the scene is static and that this process is completed quickly enough that lighting changes can be safely ignored.

			// It can then be assumed that the film irradiance values Ei for each pixel i are constant.

			// Zmin & Zmax ... least and greatest pixel values (integers)
			// w(z) ... weight at pixel value z
			byte Zmin;
			byte Zmax;
			int[] w;
			byte Zmid;
			int Zmax1;
			#region FIND MIN AND MAX VALUES
			// assumes Zmin = 0 & Zmax = 255, therefore this is hardcoded
			// if this is not desired, this should be removed and code below uncommented
			Zmin = 0;
			Zmax = 255;
			Zmid = 127;
			Zmax1 = 256;
			// weights can also be hardcoded because of the Zmin and Zmax assumption
			w = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 126, 125, 124, 123, 122, 121, 120, 119, 118, 117, 116, 115, 114, 113, 112, 111, 110, 109, 108, 107, 106, 105, 104, 103, 102, 101, 100, 99, 98, 97, 96, 95, 94, 93, 92, 91, 90, 89, 88, 87, 86, 85, 84, 83, 82, 81, 80, 79, 78, 77, 76, 75, 74, 73, 72, 71, 70, 69, 68, 67, 66, 65, 64, 63, 62, 61, 60, 59, 58, 57, 56, 55, 54, 53, 52, 51, 50, 49, 48, 47, 46, 45, 44, 43, 42, 41, 40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };

			//Zmin = byte.MaxValue;
			//Zmax = byte.MinValue;
			//for(i = 0; i < images.Length; ++i) {
			//	GMImage image = images[i];
			//	for(int j = 2; j >= 0; --j) {
			//		GMImagePlane plane = image[j];
			//		for(int y = imageHeight - 1; y >= 0; --y) {
			//			for(int x = imageWidth - 1; x >= 0; --x) {
			//				byte current = plane[x, y];
			//				if(current > Zmax) {
			//					Zmax = current;
			//				}
			//				if(current < Zmin) {
			//					Zmin = current;
			//				}
			//			}
			//		}
			//	}
			//}
			//Zmid = (byte)((Zmin + Zmax) / 2);
			//Zmax1 = Zmax + 1;

			//#region WEIGHTS
			//w = new int[Zmax + 1];
			//for(i = Zmax; i >= Zmin; --i) {
			//	w[i] = Weight(Zmin, Zmax, Zmid, i);
			//}
			//#endregion WEIGHTS

			#endregion FIND MIN AND MAX VALUES

			// lE(i,p) .. log film irradiance at pixel location i and image plane p
			// lE is commented out because it is not used later on, but is left just for reference
			//double[,] lE;
			// g(z,p) ... log exposure corresponding to pixel value z and image plane p
			double[,] g;
			#region RECOVERING THE FILM RESPONSE FUNCTION
			// We wish to recover the function g and the irradiances Ei that best satisfy the set of equations arising from Equation 2 (g(Zij) = ln Ei + ln tj) in a least-squared error sense.
			// We note that recovering g only requires recovering the finite number of values that g(z) can take since the domain of Z, pixel brightness values, is finite.
			{
				// Zij ... pixel value (i=index over pixels, j=index over exposure times)
				// N ... number of pixel locations
				// P ... number of photographs

				int P = images.Length;

				int rows = N * P + Zmax1 + 1;
				int columns = N + Zmax1;
				var A = new DenseMatrix[3];
				var B = new DenseVector[3];
				// for each image plane
				for(p = 2; p >= 0; --p) {
					A[p] = new DenseMatrix(rows, columns);
					B[p] = new DenseVector(rows);
				}

				// Clearly, the pixel locations should be chosen so that they have a reasonably even distribution of pixel values from Zmin to Zmax, and so that they are spatially well distributed in the image.
				// Furthermore, the pixels are best sampled from regions of the image with low intensity variance so that radiance can be assumed to be constant across the area of the pixel, and the effect of optical blur of the imaging system is minimized.
				// So far we have performed this task by hand, though it could easily be automated.
				if(rand == null) {
					rand = new Random();
				}
				int[] randY = rand.RandomInts(0, imageHeight, N);
				int[] randX = rand.RandomInts(0, imageHeight, N);

				// Include the data-fitting equations
				int k = 0;
				int zmax1i;
				// for each sample point
				for(i = 0; i < N; ++i) {
					y = randY[i];
					x = randX[i];
					zmax1i = Zmax1 + i;
					// for each image
					for(j = 0; j < P; ++j) {
						// for each image plane
						for(p = 2; p >= 0; --p) {
							byte z = images[j][p][x, y];
							int wij = w[z];
							A[p][k, z] = wij;
							A[p][k, zmax1i] = -wij;
							B[p][k] = wij * logExposureTimes[j];
						}
						++k;
					}
				}

				// The solution for the g(z) and Ei values can only be up to a single scale factor.
				// To establish a scale factor, we introduce the additional constraint g(Zmid) = 0, where Zmid = 0.5*(Zmin + Zmax), simply by adding this as an equation in the linear system.
				// The meaning of this constraint is that a pixel with value midway between Zmin and Zmax will be assumed to have unit exposure.
				// fix the curve by setting its middle value to 0
				// for each image plane
				for(p = 2; p >= 0; --p) {
					A[p][k, Zmid] = 1;
				}
				++k;

				// Include the smoothness equations
				i = Zmin;
				// for each possible z value
				for(int i1=i+1,i2=i+2; i < Zmax; ++i,++i1,++i2) {
					int wi1 = w[i1];
					int smoothnesswi1 = smoothness * wi1;
					int m2smowi1 = -2 * smoothnesswi1;
					// for each image plane
					for(p = 2; p >= 0; --p) {
						A[p][k, i] = smoothnesswi1;
						A[p][k, i1] = m2smowi1;
						A[p][k, i2] = smoothnesswi1;
					}
					++k;
				}

				if(cancelToken.IsCancellationRequested) {
					return null;
				} else {
					progress?.Invoke(0.1);
				}

				// Solve the system using SVD
				double[][] X = new double[3][];
				{ // multi thread
					var threads = new Thread[3];
					// for each image plane
					for(p = 2; p >= 0; --p) {
						threads[p] = new Thread((param)=>
						{
							int plane = (int)param;
							X[plane] = MultipleRegression.Svd(A[plane], B[plane]).AsArray();
						});
					}
					for(p = 2; p >= 0; --p) {
						threads[p].Start(p);
					}
					for(p = 2; p >= 0; --p) {
						threads[p].Join();
					}
				}

				// copy values from X to g and lE
				g = new double[Zmax1, 3];
				// for each z value
				for(i = Zmax; i >= 0; --i) {
					// for each image plane
					for(p = 2; p >= 0; --p) {
						g[i, p] = X[p][i];
					}
				}
				//lE = new double[N, 3];
				//// for each sample location
				//for(i = N - 1; i > Zmax; --i) {
				//	// for each image plane
				//	for(p = 2; p >= 0; --p) {
				//		lE[i, p] = X[p][i];
				//	}
				//}
			}
			#endregion RECOVERING THE FILM RESPONSE FUNCTION
			
			if(cancelToken.IsCancellationRequested) {
				return null;
			} else {
				progress?.Invoke(0.8);
			}

			GMImage result;
			#region RECONSTRUCTING THE HDR RADIANCE IMAGE
			// For robustness, and to recover high dynamic range radiance values, we should use all the available exposures for a particular pixel to compute its radiance.
			// For this, we reuse the weighting function to give higher weight to exposures in which the pixel’s value is closer to the middle of the response function.
			// Combining the multiple exposures has the effect of reducing noise in the recovered radiance values.
			// It also reduces the effects of imaging artifacts such as film grain.
			// Since the weighting function ignores saturated pixel values, “blooming” artifacts have little impact on the reconstructed radiance values.
			{
				double[,,] radianceMap = new double[imageHeight, imageWidth, 3];
				double current;
				double min = double.MaxValue;
				double max = double.MinValue;
				#region RADIANCE MAPS
				{
					int[] weightSums = new int[3];
					int weightlessIndex = images.Length / 2;
					for(y = 0; y < imageHeight; ++y) {
						for(x = 0; x < imageWidth; ++x) {
							weightSums.Reset();
							// for all images
							for(j = 0; j < images.Length; ++j) {
								GMImage imagej = images[j];
								// for all three image planes
								for(p = 2; p >= 0; --p) {
									byte z = imagej[p][x, y];
									weightSums[p] += w[z];
								}
							}

							// for all three image planes
							for(p = 2; p >= 0; --p) {
								current = 0;
								if(weightSums[p] > 0) {
									// for all images
									for(j = 0; j < images.Length; ++j) {
										byte z = images[j][p][x, y];
										current += (w[z] * (g[z, p] - logExposureTimes[j])) / weightSums[p];
									}
								} else {
									byte z = images[weightlessIndex][p][x, y];
									current += g[z, p] - logExposureTimes[weightlessIndex];
								}

								radianceMap[y, x, p] = current;

								if(current < min) {
									min = current;
								}
								if(current > max) {
									max = current;
								}
							}
						}
					}
				}
				#endregion RADIANCE MAPS

				// normalize
				double normalizedRadianceMax = double.MinValue;
				double maxmin = max - min;
				for(y = 0; y < imageHeight; ++y) {
					for(x = 0; x < imageWidth; ++x) {
						// for each image plane
						for(p = 2; p >= 0; --p) {
							current = (radianceMap[y, x, p]-min)/maxmin;
							radianceMap[y, x, p] = current;
							if(current > normalizedRadianceMax) {
								normalizedRadianceMax = current;
							}
						}
					}
				}

				// create the image
				result = new GMImage(imageWidth, imageHeight, 3);
				for(y = 0; y < imageHeight; ++y) {
					for(x = 0; x < imageWidth; ++x) {
						// for each image plane
						for(p = 2; p >= 0; --p) {
							current = radianceMap[y, x, p];
							result[p][x, y] = (byte)((current / normalizedRadianceMax) * byte.MaxValue);
						}
					}
				}
			}
			#endregion RECONSTRUCTING THE HDR RADIANCE IMAGE

			return result;
		}

		/// <summary>
		/// Weighting function.
		/// <para>
		/// The solution can be made to have a much better fit by anticipating the basic shape of the response function. Since g(z) will typically have a steep slope near Zmin and Zmax, we should expect that g(z) will be less smooth and will fit the data more poorly near these extremes. To recognize this, we can introduce a weighting function w(z) to emphasize the smoothness and fitting terms toward the middle of the curve. A sensible choice of w is a simple hat function.
		/// </para>
		/// </summary>
		/// <param name="Zmin">Min value.</param>
		/// <param name="Zmax">Max value.</param>
		/// <param name="Zmid">Mid value.</param>
		/// <param name="value">Pixel value.</param>
		private static int Weight(byte Zmin, byte Zmax, byte Zmid, int value)
		{
			if(value <= Zmid) {
				return value - Zmin;
			}
			return Zmax - value;
		}
	}
}
