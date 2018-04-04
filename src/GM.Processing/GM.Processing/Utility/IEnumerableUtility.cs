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

namespace GM.Processing.Utility
{
	// FIXME GM.Utility

	/// <summary>
	/// 
	/// </summary>
	public static class IEnumerableUtility
	{
		/// <summary>
		/// Determines whether all elements in this collection produce the same value with the provided value selector. Compares using the <see cref="object.Equals(object)"/> method.
		/// </summary>
		/// <typeparam name="T">The type of the elements.</typeparam>
		/// <typeparam name="TValue">The type of the values to compare.</typeparam>
		/// <param name="enumerable">The collection.</param>
		/// <param name="valueSelector">A transform function to apply to each element to select the value on which to compare elements.</param>
		public static bool AllSame<T, TValue>(this IEnumerable<T> enumerable, Func<T, TValue> valueSelector)
		{
			if(enumerable == null) {
				throw new ArgumentNullException(nameof(enumerable));
			}
			if(valueSelector == null) {
				throw new ArgumentNullException(nameof(valueSelector));
			}
			if(!enumerable.Any()) {
				return true;
			}
			TValue sample = valueSelector(enumerable.First());
			return enumerable.All(e => sample.Equals(valueSelector(e)));
		}

		/// <summary>
		/// Determines whether all elements in this collection produce the same values with the provided value selectors. Compares using the <see cref="object.Equals(object)"/> method.
		/// </summary>
		/// <typeparam name="T">The type of the elements.</typeparam>
		/// <typeparam name="TValue1">The type of the first values to compare.</typeparam>
		/// <typeparam name="TValue2">The type of the second values to compare.</typeparam>
		/// <param name="enumerable">The collection.</param>
		/// <param name="valueSelector1">A transform function to apply to each element to select the first value on which to compare elements.</param>
		/// <param name="valueSelector2">A transform function to apply to each element to select the second value on which to compare elements.</param>
		public static bool AllSame<T, TValue1, TValue2>(this IEnumerable<T> enumerable, Func<T, TValue1> valueSelector1, Func<T, TValue2> valueSelector2)
		{
			if(enumerable == null) {
				throw new ArgumentNullException(nameof(enumerable));
			}
			if(valueSelector1 == null) {
				throw new ArgumentNullException(nameof(valueSelector1));
			}
			if(valueSelector2 == null) {
				throw new ArgumentNullException(nameof(valueSelector2));
			}
			if(!enumerable.Any()) {
				return true;
			}
			T first = enumerable.First();
			TValue1 sample1 = valueSelector1(first);
			TValue2 sample2 = valueSelector2(first);
			return enumerable.All(e => sample1.Equals(valueSelector1(e)) && sample2.Equals(valueSelector2(e)));
		}
	}
}
