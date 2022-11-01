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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GM.WPF.MVVM;

namespace GM.Processing.Examples.Common
{
	class LoggableViewModel:ViewModel,IDisposable
	{
		public string Log { get; private set; }
		private volatile object log_lock = new object();

		protected CancellationTokenSource cancellationTokenSource;

		public string ElapsedTime { get; private set; }
		private Stopwatch stopwatch;

		public LoggableViewModel()
		{
			Log = "";
		}
		
		/// <summary>
		/// Disposes the cancellation token source.
		/// </summary>
		public void Dispose()
		{
			cancellationTokenSource?.Cancel();
			cancellationTokenSource?.Dispose();
			cancellationTokenSource = null;
		}

		/// <summary>
		/// Is thread safe.
		/// </summary>
		protected void LogLine(string message)
		{
			lock(log_lock) {
				Log = $"[{DateTime.Now.ToString()}] {message}" + Environment.NewLine + Log;
			}
		}

		protected void StartCountingTime()
		{
			stopwatch = Stopwatch.StartNew();
			Task.Run(async delegate
			{
				while(cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested) {
					ElapsedTime = stopwatch.Elapsed.ToString("hh':'mm':'ss");
					await Task.Delay(1000);
				}
				stopwatch.Stop();
				stopwatch = null;
			});
		}
	}
}
