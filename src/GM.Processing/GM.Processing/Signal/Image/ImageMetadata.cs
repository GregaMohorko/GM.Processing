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
Created: 2018-4-10
Author: GregaMohorko
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GM.Utility;

namespace GM.Processing.Signal.Image
{
	/// <summary>
	/// 
	/// </summary>
	public class ImageMetadata
	{
		/// <summary>
		/// Property items (pieces of metadata) stored in the image associated with this metadata object.
		/// </summary>
		public readonly List<PropertyItem> PropertyItems;

		/// <summary>
		/// Creates a new empty instance of <see cref="ImageMetadata"/>.
		/// </summary>
		public ImageMetadata()
		{
			PropertyItems = new List<PropertyItem>();
		}

		/// <summary>
		/// Creates a new instance of <see cref="ImageMetadata"/> with the provided property items.
		/// </summary>
		/// <param name="propertyItems">The property items (pieces of metadata).</param>
		public ImageMetadata(PropertyItem[] propertyItems)
		{
			if(propertyItems == null) {
				throw new ArgumentNullException(nameof(propertyItems));
			}

			PropertyItems = propertyItems.ToList();
		}

		/// <summary>
		/// Creates a new instance of <see cref="ImageMetadata"/> with all property items copied from the provided image metadata.
		/// </summary>
		/// <param name="imageMetadata">The image metadata to copy from</param>
		public ImageMetadata(ImageMetadata imageMetadata)
		{
			if(imageMetadata == null) {
				throw new ArgumentNullException(nameof(imageMetadata));
			}

			PropertyItems = imageMetadata.PropertyItems.Select(pi => (PropertyItem)pi.DeepCopy()).ToList();
		}

		/// <summary>
		/// Determines whether this image metadata contains a property with the specified tag.
		/// </summary>
		/// <param name="metadataTag">The metadata tag. Use constants in <see cref="ImageMetadataTags"/> to easily get the tag that you want.</param>
		public bool Contains(int metadataTag)
		{
			return PropertyItems.Any(pi => pi.Id == metadataTag);
		}

		/// <summary>
		/// Gets or sets the <see cref="PropertyItem"/> for the specified metadata tag.
		/// </summary>
		/// <param name="metadataTag">The metadata tag. Use constants in <see cref="ImageMetadataTags"/> to easily get the tag that you want.</param>
		public PropertyItem this[int metadataTag]
		{
			get => PropertyItems.FirstOrDefault(pi => pi.Id == metadataTag);
			set
			{
				if(metadataTag != value.Id) {
					throw new ArgumentException("The metadataTag must match the Id of the new PropertyItem.", nameof(metadataTag));
				}
				PropertyItem current = this[metadataTag];
				if(current != null) {
					PropertyItems.Remove(current);
				}
				PropertyItems.Add(value);
			}
		}

		/// <summary>
		/// Returns the <see cref="PropertyItem"/> of the specified metadata tag, or null if no property with this tag is present in this metadata object.
		/// <para>You can also use <see cref="this[int]"/>.</para>
		/// </summary>
		/// <param name="metadataTag">The metadata tag. Use constants in <see cref="ImageMetadataTags"/> to easily get the tag that you want.</param>
		public PropertyItem Get(int metadataTag)
		{
			return this[metadataTag];
		}

		/// <summary>
		/// Returns the value of the specified metadata tag as double.
		/// </summary>
		/// <param name="metadataTag">The metadata tag. Use constants in <see cref="ImageMetadataTags"/> to easily get the tag that you want.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the specified metadata tag is not present in this metadata object.</exception>
		/// <exception cref="InvalidOperationException">Thrown when the specified metadata tag does not represent a property of type double.</exception>
		public double GetDouble(int metadataTag)
		{
			if(!TryGetDouble(metadataTag,out double value)) {
				throw new ArgumentOutOfRangeException(nameof(metadataTag));
			}
			return value;
		}

		/// <summary>
		/// Gets the value of the specified metadata tag as double. The return value indicates whether the metadata tag is even present in this metadata object.
		/// </summary>
		/// <param name="metadataTag">The metadata tag. Use constants in <see cref="ImageMetadataTags"/> to easily get the tag that you want.</param>
		/// <param name="value">The value of the specified metadata property.</param>
		/// <exception cref="InvalidOperationException">Thrown when the specified metadata tag does not represent a property of type double.</exception>
		public bool TryGetDouble(int metadataTag, out double value)
		{
			PropertyItem property = this[metadataTag];
			if(property == null) {
				value = default(double);
				return false;
			}

			// https://docs.microsoft.com/en-us/dotnet/api/system.drawing.imaging.propertyitem.type
			if(property.Type != 5) {
				throw new InvalidOperationException("The specified metadata tag does not represent a property of type double.");
			}

			Debug.Assert(property.Len == 8);
			Debug.Assert(property.Value.Length == 8);

			uint numerator = BitConverter.ToUInt32(property.Value, 0);
			uint denominator = BitConverter.ToUInt32(property.Value, 4);
			
			value = numerator / (double)denominator;
			return true;
		}
	}
}
