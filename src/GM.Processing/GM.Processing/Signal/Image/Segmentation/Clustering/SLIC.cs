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
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using GM.Utility;

namespace GM.Processing.Signal.Image.Segmentation.Clustering
{
	/// <summary>
	/// Implementation of Simple Linear Iterative Clustering (SLIC) algorithm for superpixel segmentation. The algorithm adapts a k-means clustering approach to efficiently generate superpixels.
	/// <para>http://scikit-image.org/docs/dev/auto_examples/segmentation/plot_segmentations.html#slic-k-means-based-image-segmentation</para>
	/// <para>Superpixel algorithms group pixels into perceptually meaningful atomic regions, which can be used to replace the rigid structure of the pixel grid. They capture image redundancy, provide a convenient primitive from which to compute image features, and greatly reduce the complexity of subsequent image processing tasks. They have become key building blocks of many computer vision algorithms, such as depth estimation, segmentation, body model estimation and object localization.</para>
	/// </summary>
	public static class SLIC
	{
		/// <summary>
		/// Segments the provided image using Simple Linear Iterative Clustering (SLIC) algorithm.
		/// </summary>
		/// <param name="image">The image to segment.</param>
		/// <param name="k">The desired number of approximately equally-sized superpixels.</param>
		/// <param name="m">Controlls the compactness of the superpixels. It allows us to weigh the relative importance between color similarity and spatial proximity. When this value is large, spatial proximity is more important and the resulting superpixels are more compact (i.e. they have a lower area to perimeter ratio). When this value is small, the resulting superpixels adhere more tightly to image boundaries, but have less regular size and shape. It should be in the range [1, 40].</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the process.</param>
		public static void Segmentize(GMImage image, int k,double m, CancellationToken? cancellationToken=null)
		{
			if(image == null) {
				throw new ArgumentNullException(nameof(image));
			}

			Segmentize(image, k, m, out int[,] clusterIDs, out Color[] clusterColors, cancellationToken);
			image.ApplySegments(clusterIDs, clusterColors);
		}

		/// <summary>
		/// Segments the provided image using Simple Linear Iterative Clustering (SLIC) algorithm.
		/// </summary>
		/// <param name="image">The image to segment.</param>
		/// <param name="k">The desired number of approximately equally-sized superpixels.</param>
		/// <param name="m">Controlls the compactness of the superpixels. It allows us to weigh the relative importance between color similarity and spatial proximity. When this value is large, spatial proximity is more important and the resulting superpixels are more compact (i.e. they have a lower area to perimeter ratio). When this value is small, the resulting superpixels adhere more tightly to image boundaries, but have less regular size and shape. It should be in the range [1, 40].</param>
		/// <param name="clusterIDs">An array of the same size as the provided image which holds the IDs of the clusters. Use these values as indexes for clusterColors.</param>
		/// <param name="clusterColors">The cluster colors (RGB).</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the process.</param>
		public static void Segmentize(GMImage image,int k,double m,out int[,] clusterIDs,out Color[] clusterColors, CancellationToken? cancellationToken = null)
		{
			Segmentize(image, k, m, out clusterIDs, out clusterColors, out int[,] clusterCenters, cancellationToken);
		}

		/// <summary>
		/// Segments the provided image using Simple Linear Iterative Clustering (SLIC) algorithm.
		/// </summary>
		/// <param name="image">The image to segment.</param>
		/// <param name="k">The desired number of approximately equally-sized superpixels.</param>
		/// <param name="m">Controlls the compactness of the superpixels. It allows us to weigh the relative importance between color similarity and spatial proximity. When this value is large, spatial proximity is more important and the resulting superpixels are more compact (i.e. they have a lower area to perimeter ratio). When this value is small, the resulting superpixels adhere more tightly to image boundaries, but have less regular size and shape. It should be in the range [1, 40].</param>
		/// <param name="clusterIDs">An array of the same size as the provided image which holds the IDs of the clusters. Use these values as indexes for clusterColors.</param>
		/// <param name="clusterColors">The cluster colors (RGB).</param>
		/// <param name="clusterCenters">The center positions of the clusters.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the process.</param>
		public static void Segmentize(GMImage image,int k,double m,out int[,] clusterIDs,out Color[] clusterColors,out int[,] clusterCenters, CancellationToken? cancellationToken = null)
		{
			SLICImpl(image, k, m, out clusterIDs, out clusterColors, out clusterCenters,cancellationToken??CancellationToken.None);
		}

		private static void SLICImpl(GMImage image, int k, double m, out int[,] clusterIDs, out Color[] clusterColors, out int[,] clusterCenters, CancellationToken cancellationToken)
		{
			if(image == null) {
				throw new ArgumentNullException(nameof(image));
			}

			// convert from RGB to CIELAB color space
			double[,][] cielab = image.ToCIELAB();

			// [L a b]
			var centerColors = new double[k, 3];
			// [y x]
			clusterCenters = new int[k, 2];
			int S; // grid interval
			#region CLUSTERS INITIALIZATION
			// initialize cluster centers Ck by sampling pixels at regular grid steps S
			{
				// The clustering procedure begins with an initialization step where k initial cluster centers Ci = [li ai bi xi yi] are sampled on a regular grid spaced S pixels apart.

				// To produce roughly equally sized superpixels, the grid interval is S = sqrt(N / k).
				int N = image.Height * image.Width;
				S = (int)Math.Round(Math.Sqrt(N / (double)k));

				// set vertical and horizontal margins to make sure that there will actually be k centers
				// because sometimes it can happen that without doing this, the center count would be less
				int verticalMargin = S / 2;
				int horizontalMargin = S / 2;
				if(((image.Height / S) * (image.Width / S)) < k) {
					int clusterCountAfterHorizontalAlignment = (image.Height / S) * ((image.Width / S) + 1);
					int clusterCountAfterVerticalAlignment = ((image.Height / S) + 1) * (image.Width / S);
					int clusterCountAfterBothAlignment = (((image.Height / S) + 1) * ((image.Width / S) + 1));
					int newHorizontalMargin = (image.Width % S) / 2;
					int newVerticalMargin = (image.Height % S) / 2;

					if(clusterCountAfterHorizontalAlignment >= k && (clusterCountAfterVerticalAlignment < k || newHorizontalMargin > newVerticalMargin)) {
						// decrease only horizontal margin
						horizontalMargin = newHorizontalMargin;
					} else if(clusterCountAfterVerticalAlignment >= k) {
						// decrease only vertical margin
						verticalMargin = newVerticalMargin;
					} else if(clusterCountAfterBothAlignment >= k) {
						// decrease both margins
						horizontalMargin = newHorizontalMargin;
						verticalMargin = newVerticalMargin;
					} else {
						throw new ArgumentOutOfRangeException(nameof(k), "The provided desired number of superpixels is invalid. Usually this happens when the image is too small or the provided number of superpixels is too big.");
					}
				}

				int i = 0;
				int xEnd = image.Width - Math.Max(horizontalMargin, 1);
				int xStart = horizontalMargin == 0 ? -1 : horizontalMargin;
				int yStart = verticalMargin == 0 ? -1 : verticalMargin;
				int heightSub1 = image.Height - 1;
				int widthSub1 = image.Width - 1;
				for(int y = image.Height - Math.Max(verticalMargin, 1); y >= yStart; y -= S) {
					int ySafe = Math.Max(y, 0);
					for(int x = xEnd; x >= xStart; x -= S) {
						if(cancellationToken.IsCancellationRequested) {
							clusterIDs = null;
							clusterColors = null;
							clusterCenters = null;
							return;
						}

						if(i == k) {
							y = -1;
							break;
						}

						int xSafe = Math.Max(x, 0);

						int yy, xx;
						#region FINDING LOCAL MINIMUM
						// find the local minimum (gradient-wise) in a 3x3 neighbourhood
						{
							// The centers are moved to seed locations corresponding to the lowest gradient position in a 3 × 3 neighborhood. This is done to avoid centering a superpixel on an edge, and to reduce the chance of seeding a superpixel with a noisy pixel.
							double minGradient = double.MaxValue;
							yy = -1;
							xx = -1;
							// 3x3 neighbourhood
							for(int ny = ySafe + 1; ny >= ySafe - 1; --ny) {
								for(int nx = xSafe + 1; nx >= xSafe - 1; --nx) {
									int nySafe = Math.Max(0, Math.Min(ny, heightSub1));
									int nxSafe = Math.Max(0, Math.Min(nx, widthSub1));
									double[] cAbove = cielab[Math.Min(ny + 1, heightSub1), nxSafe];
									double[] cRight = cielab[nySafe, Math.Min(nx + 1, widthSub1)];
									double[] c = cielab[nySafe, nxSafe];
									double iAbove = cAbove[0] * 0.11 + cAbove[1] * 0.59 + cAbove[2] * 0.3;
									double iRight = cRight[0] * 0.11 + cRight[1] * 0.59 + cRight[2] * 0.3;
									double ii = c[0] * 0.11 + c[1] * 0.59 + c[2] * 0.3;
									// compute horizontal and vertical gradients
									double currentGradient = Math.Abs(iAbove - ii) + Math.Abs(iRight - ii);
									if(currentGradient < minGradient) {
										minGradient = currentGradient;
										yy = nySafe;
										xx = nxSafe;
									}
								}
							}
						}
						#endregion FINDING LOCAL MINIMUM

						double[] color = cielab[yy, xx];
						double L = color[0];
						double a = color[1];
						double b = color[2];

						centerColors[i, 0] = L;
						centerColors[i, 1] = a;
						centerColors[i, 2] = b;
						clusterCenters[i, 0] = yy;
						clusterCenters[i, 1] = xx;
						++i;
					}
				}

				// make sure that all centers were initialized
				Debug.Assert(i == k);
			}
			#endregion CLUSTERS INITIALIZATION

			clusterIDs = new int[image.Height, image.Width];
			double[,] distances = new double[image.Height, image.Width];

			var centerCounts = new int[k];

			// The assignment and update steps can be repeated iteratively until the error converges, but we have found that 10 iterations suffices for most images.
			for(int iterations = 10; iterations > 0; --iterations) {
				#region CLUSTER ASSIGNMENT
				// Next, in the assignment step, each pixel i is associated with the nearest cluster center whose search region overlaps its location.
				// This is only possible through the introduction of a distance measure D, which determines the nearest cluster center for each pixel.

				// reset labels and distances
				for(int y = image.Height - 1; y >= 0; --y) {
					for(int x = image.Width - 1; x >= 0; --x) {
						clusterIDs[y, x] = -1;
						distances[y, x] = double.MaxValue;
					}
				}

				if(cancellationToken.IsCancellationRequested) {
					clusterIDs = null;
					clusterColors = null;
					clusterCenters = null;
					return;
				}

				// for each cluster center
				for(int j = k - 1; j >= 0; --j) {
					// Since the expected spatial extent of a superpixel is a region of approximate size S × S, the search for similar pixels is done in a region 2S × 2S around the superpixel center.
					int clusterCenterY = clusterCenters[j, 0];
					int clusterCenterX = clusterCenters[j, 1];
					int regionTop = clusterCenterY + S - 1;
					int regionBottom = clusterCenterY - S;
					int regionRight = clusterCenterX + S - 1;
					int regionLeft = clusterCenterX - S;
					// for each pixel in a 2S x 2S region around cluster center
					for(int y = regionTop; y >= regionBottom; --y) {
						if(y < 0 || y >= image.Height) {
							continue;
						}
						for(int x = regionRight; x >= regionLeft; --x) {
							if(x < 0 || x >= image.Width) {
								continue;
							}
							double[] i = cielab[y, x];
							double lj = centerColors[j, 0];
							double aj = centerColors[j, 1];
							double bj = centerColors[j, 2];
							double li = i[0];
							double ai = i[1];
							double bi = i[2];
							// compute the distance between the pixel and cluster center
							double dc = Math.Sqrt(Math.Pow(lj - li, 2) + Math.Pow(aj - ai, 2) + Math.Pow(bj - bi, 2));
							double ds = Math.Sqrt(Math.Pow(clusterCenterX - x, 2) + Math.Pow(clusterCenterY - y, 2));
							double D = Math.Sqrt(Math.Pow(dc, 2) + Math.Pow(ds / S, 2) * Math.Pow(m, 2));
							if(D < distances[y, x]) {
								distances[y, x] = D;
								clusterIDs[y, x] = j;
							}
						}
					}
				}
				#endregion CLUSTER ASSIGNMENT

				if(cancellationToken.IsCancellationRequested) {
					clusterIDs = null;
					clusterColors = null;
					clusterCenters = null;
					return;
				}

				#region UPDATE
				// Once each pixel has been associated to the nearest cluster center, an update step adjusts the cluster centers to be the mean [l a b x y] vector of all the pixels belonging to the cluster.

				// clear the centers
				for(int i = k - 1; i >= 0; --i) {
					centerColors[i, 0] = centerColors[i, 1] = centerColors[i, 2] = 0;
					clusterCenters[i, 0] = clusterCenters[i, 1] = 0;
					centerCounts[i] = 0;
				}

				// Compute new cluster centers.

				// First, sum up values from all the pixels belonging to a certain cluster
				for(int y = image.Height - 1; y >= 0; --y) {
					for(int x = image.Width - 1; x >= 0; --x) {
						int currentCluster = clusterIDs[y, x];
						if(currentCluster == -1) {
							continue;
						}
						double[] color = cielab[y, x];
						centerColors[currentCluster, 0] += color[0];
						centerColors[currentCluster, 1] += color[1];
						centerColors[currentCluster, 2] += color[2];
						clusterCenters[currentCluster, 0] += y;
						clusterCenters[currentCluster, 1] += x;
						++centerCounts[currentCluster];
					}
				}

				if(cancellationToken.IsCancellationRequested) {
					clusterIDs = null;
					clusterColors = null;
					clusterCenters = null;
					return;
				}

				// Then, normalize the clusters to get the mean values
				for(int i = k - 1; i >= 0; --i) {
					if(centerCounts[i] == 0) {
						continue;
					}
					centerColors[i, 0] /= centerCounts[i];
					centerColors[i, 1] /= centerCounts[i];
					centerColors[i, 2] /= centerCounts[i];
					clusterCenters[i, 0] /= centerCounts[i];
					clusterCenters[i, 1] /= centerCounts[i];
				}
				#endregion UPDATE
			}

			// convert from CIELAB back to RGB
			clusterColors = new Color[k];
			for(int i = k - 1; i >= 0; --i) {
				double L = centerColors[i, 0];
				double a = centerColors[i, 1];
				double b = centerColors[i, 2];
				clusterColors[i] = ColorUtility.CIELABToRGB(L, a, b);
			}
		}
	}
}
