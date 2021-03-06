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

Project: GM.Processing.Examples
Created: 2018-4-10
Author: GregaMohorko
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GM.Processing.Examples.Utility
{
	// FIXME GM.WPF

	/// <summary>
	/// Utilities for <see cref="TreeView"/>.
	/// </summary>
	public static class TreeViewUtility
	{
		/// <summary>
		/// Selects this <see cref="TreeViewItem"/>. It also expands the <see cref="TreeView"/> items to this item..
		/// </summary>
		/// <param name="treeViewItem">The item to select and expand to.</param>
		public static void Select(this TreeViewItem treeViewItem)
		{
			// expand all parents
			TreeViewItem parent = treeViewItem.Parent as TreeViewItem;
			while(parent != null) {
				parent.IsExpanded = true;
				parent = parent.Parent as TreeViewItem;
			}
			// select the item
			treeViewItem.IsSelected = true;
		}
	}
}
