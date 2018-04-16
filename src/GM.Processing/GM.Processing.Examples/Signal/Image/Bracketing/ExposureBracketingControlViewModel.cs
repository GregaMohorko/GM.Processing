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

Project: GM.Processing.Examples
Created: 2018-4-10
Author: GregaMohorko
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GalaSoft.MvvmLight.CommandWpf;
using GM.Processing.Examples.Common;
using GM.Processing.Signal.Image;
using GM.Processing.Signal.Image.Bracketing;

namespace GM.Processing.Examples.Signal.Image.Bracketing
{
	class ExposureBracketingControlViewModel:LoggableViewModel,IDisposable
	{
		// set this to null to disable auto-select
		private const string AUTO_SELECT_DIR = null;

		public RelayCommand Command_SelectDirectory { get; private set; }
		public RelayCommand Command_Run { get; private set; }
		public RelayCommand Command_Cancel { get; private set; }

		public string Directory { get; private set; }
		public string ElapsedTime { get; private set; }
		public double Progress { get; private set; }
		public int Smoothness { get; set; }
		public int SampleCount { get; set; }

		public bool CanEditSettings { get; private set; }

		private List<string> imageFiles;
		private CancellationTokenSource cancellationTokenSource;
		private Stopwatch stopwatch;

		public ExposureBracketingControlViewModel()
		{
			CanEditSettings = true;
			Smoothness = 10;
			SampleCount = 256;

			if(IsInDesignMode) {
				return;
			}

			Command_SelectDirectory = new RelayCommand(SelectDirectory, () => CanEditSettings);
			Command_Run = new RelayCommand(Run, () => Directory != null && CanEditSettings);
			Command_Cancel = new RelayCommand(Cancel, () => Directory != null && !CanEditSettings);

			if(AUTO_SELECT_DIR != null) {
				Directory = AUTO_SELECT_DIR;
				Run();
			}
		}

		public void Dispose()
		{
			cancellationTokenSource?.Cancel();
			cancellationTokenSource?.Dispose();
			cancellationTokenSource = null;
		}

		private void SelectDirectory()
		{
			using(var dialog = new FolderBrowserDialog()) {
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
					return !fileName.Contains(" - EXPOSURE BRACKETING");
				}).ToList();

			if(imageFiles.Count == 0) {
				LogLine("No images in selected directory.");
				cancellationTokenSource.Dispose();
				cancellationTokenSource = null;
				CanEditSettings = true;
				return;
			}

			StartCountingTime();
			await Task.Run((Action)DoWork,cancellationTokenSource.Token);

			LogLine(cancellationTokenSource.IsCancellationRequested ? "Cancelled." : "Finished.");
			cancellationTokenSource?.Dispose();
			cancellationTokenSource = null;
			CanEditSettings = true;
			Progress = 0;
			Command_Run.RaiseCanExecuteChanged();
		}

		private void StartCountingTime()
		{
			stopwatch = Stopwatch.StartNew();
			Task.Run(async delegate
			{
				while(cancellationTokenSource!=null && !cancellationTokenSource.IsCancellationRequested) {
					ElapsedTime = stopwatch.Elapsed.ToString("hh':'mm':'ss");
					await Task.Delay(1000);
				}
				stopwatch.Stop();
				stopwatch = null;
			});
		}

		private void DoWork()
		{
			LogLine("Opening images ...");
			Progress = 0;

			List<GMImage> hdrImages = new List<GMImage>();
			for(int i = 0; i < imageFiles.Count; ++i) {
				string imagePath = imageFiles[i];
				GMImage image;
				try {
					image = GMImage.Open(imagePath);
				} catch {
					LogLine($"Could not open file: '{Path.GetFileName(imagePath)}'.");
					return;
				}
				if(cancellationTokenSource.IsCancellationRequested) {
					return;
				}
				hdrImages.Add(image);
				Progress = (i + 1) / (double)imageFiles.Count;
			}

			string reconstructedFileName = Path.GetFileName(Directory) + $" - EXPOSURE BRACKETING N={SampleCount}, sampleCount={SampleCount}.png";
			string reconstructedFilePath = Path.Combine(Directory, reconstructedFileName);

			LogLine("Processing ...");
			Progress = 0;

			GMImage reconstructedImage = ExposureBracketing.ReconstructHDR(hdrImages.ToArray(), SampleCount, Smoothness, cancellationTokenSource.Token, (prog) => { Progress = prog; });
			if(cancellationTokenSource==null || cancellationTokenSource.IsCancellationRequested) {
				return;
			}

			reconstructedImage.Save(reconstructedFilePath);
			LogLine($"Saved image as '{reconstructedFileName}'.");
		}
	}
}
