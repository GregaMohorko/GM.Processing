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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GalaSoft.MvvmLight.CommandWpf;
using GM.Processing.Examples.Common;
using GM.Processing.Signal.Image;
using GM.Processing.Signal.Image.ContrastEnhancement;

namespace GM.Processing.Examples.Signal.Image.ContrastEnhancement
{
	class HistogramEqualizationControlViewModel:LoggableViewModel,IDisposable
	{
		private static readonly double[] clipLimits = { 1, 2, 3, 3.5, 4, 8 };

		public RelayCommand Command_SelectDirectory { get; private set; }
		public RelayCommand Command_Run { get; private set; }
		public RelayCommand Command_Cancel { get; private set; }

		public string Directory { get; private set; }

		public bool CanEditSettings { get; private set; }
		public List<int> ThreadCountItems { get; private set; }
		public int ThreadCount { get; set; }
		public bool Overwrite { get; set; }

		private List<string> imageFiles;
		private volatile object imageFiles_lock = new object();
		private CancellationTokenSource cancellationTokenSource;

		public HistogramEqualizationControlViewModel()
		{
			ThreadCountItems = Enumerable.Range(1, Environment.ProcessorCount).ToList();
			ThreadCount = Math.Max(1, Environment.ProcessorCount / 2);

			CanEditSettings = true;

			if(IsInDesignMode) {
				return;
			}
			
			Command_SelectDirectory = new RelayCommand(SelectDirectory, () => CanEditSettings);
			Command_Run = new RelayCommand(Run, () => Directory!=null && CanEditSettings);
			Command_Cancel = new RelayCommand(Cancel, () => Directory != null && !CanEditSettings);
		}

		public void Dispose()
		{
			cancellationTokenSource?.Cancel();
			cancellationTokenSource?.Dispose();
			cancellationTokenSource = null;
		}

		private void SelectDirectory()
		{
			using(var dialog=new FolderBrowserDialog()) {
				dialog.ShowNewFolderButton = false;
				if(dialog.ShowDialog() != DialogResult.OK) {
					return;
				}
				Directory = dialog.SelectedPath;
			}
		}

		private void Cancel()
		{
			cancellationTokenSource?.Cancel();
		}

		private async void Run()
		{
			CanEditSettings = false;
			cancellationTokenSource = new CancellationTokenSource();

			LogLine("Starting.");

			// read all files from the Images directory
			imageFiles = System.IO.Directory.EnumerateFiles(Directory)
				.Where(f =>
				{
					string fileName = Path.GetFileNameWithoutExtension(f);
					return !fileName.Contains(" - CLHE cl=") && !fileName.Contains(" - HE");
				}).ToList();

			if(imageFiles.Count == 0) {
				LogLine("No images in selected directory.");
				cancellationTokenSource.Dispose();
				cancellationTokenSource = null;
				CanEditSettings = true;
				return;
			}

			var tasks = new Task[ThreadCount];
			for(int i = 0; i < tasks.Length; ++i) {
				tasks[i]=Task.Run((Action)ThreadWork,cancellationTokenSource.Token);
			}
			await Task.WhenAll(tasks);

			LogLine(cancellationTokenSource.IsCancellationRequested ? "Cancelled." : "Finished.");
			cancellationTokenSource?.Dispose();
			cancellationTokenSource = null;
			CanEditSettings = true;
			Command_Run.RaiseCanExecuteChanged();
		}

		private void ThreadWork()
		{
			while(true) {
				if(cancellationTokenSource.IsCancellationRequested) {
					break;
				}

				string imageFile;
				lock(imageFiles_lock) {
					if(imageFiles.Count == 0) {
						break;
					}
					imageFile = imageFiles[0];
					imageFiles.RemoveAt(0);
					LogLine($"({imageFiles.Count} left) Processing {Path.GetFileName(imageFile)} ...");
				}

				GMImage image;
				try {
					image = GMImage.Open(imageFile);
				} catch {
					LogLine($"Could not open file {Path.GetFileName(imageFile)}.");
					continue;
				}

				string baseImageFile = Path.Combine(Directory, Path.GetFileNameWithoutExtension(imageFile));

				// HE
				string HEfilePath = baseImageFile + " - HE.png";
				if(Overwrite || !File.Exists(HEfilePath)) {
					string HEfileName = Path.GetFileName(HEfilePath);
					var HE = new GMImage(image);
					LogLine($"Processing {HEfileName} ...");
					Processing.Signal.Image.ContrastEnhancement.HistogramEqualization.AdjustContrast(HE,0,cancellationTokenSource.Token);
					HE.Save(HEfilePath);
					LogLine($"Finished {HEfileName}.");
				}

				// CLHE
				for(int i = 0; i < clipLimits.Length; ++i) {
					if(cancellationTokenSource.IsCancellationRequested) {
						break;
					}
					double clipLimit = clipLimits[i];
					string CLHEfilePath = baseImageFile + $" - CLHE cl={clipLimit.ToString("N2")}.png";
					if(!Overwrite && File.Exists(CLHEfilePath)) {
						continue;
					}
					string CLHEfileName = Path.GetFileName(CLHEfilePath);
					var CLHE = new GMImage(image);
					LogLine($"Processing {CLHEfileName} ...");
					HistogramEqualization.AdjustContrast(CLHE,clipLimit, cancellationTokenSource.Token);
					if(cancellationTokenSource.IsCancellationRequested) {
						break;
					}
					CLHE.Save(CLHEfilePath);
					LogLine($"Finished {CLHEfileName}.");
				}
			}
		}
	}
}
