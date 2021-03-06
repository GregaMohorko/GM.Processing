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
Created: 2018-4-9
Author: GregaMohorko
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GM.WPF.MVVM;

namespace GM.Processing.Examples
{
	class MainWindowViewModel:ViewModel
	{
		public event EventHandler AlgorithmsLoaded;

		public string Message_TreeView_Algorithms { get; private set; }
		public string SelectedAlgorithmName { get; set; }
		public List<IAlgorithm> Algorithms { get; private set; }

		public MainWindowViewModel()
		{
			if(IsInDesignMode) {
				Message_TreeView_Algorithms = "";
				SelectedAlgorithmName = "Selected algorithm name";
				return;
			}
		}

		public void LoadAlgorithms()
		{
			Message_TreeView_Algorithms = "Loading ...";

			Type iAlgorithmType = typeof(IAlgorithm);
			Assembly assembly = (typeof(MainWindowViewModel)).Assembly;
			Type[] types=assembly.GetTypes();

			var algorithmTypes=types
				.Where(t => t.IsClass && !t.IsAbstract && iAlgorithmType.IsAssignableFrom(t))
				.OrderBy(t => t.FullName);

			var algorithms = new List<IAlgorithm>();
			foreach(Type algorithmType in algorithmTypes) {
				IAlgorithm algorithm = (IAlgorithm)Activator.CreateInstance(algorithmType);
				algorithms.Add(algorithm);
			}

			Message_TreeView_Algorithms = null;

			Algorithms = algorithms;
			AlgorithmsLoaded?.Invoke(this, EventArgs.Empty);
		}
	}
}
