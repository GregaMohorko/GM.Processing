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
Created: 2018-4-9
Author: GregaMohorko
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GM.Processing.Examples.Utility;
using GM.WPF.Windows;

namespace GM.Processing.Examples
{
	public partial class MainWindow : BaseWindow,IDisposable
	{
		// set this to the type of the algorithm to auto-select on startup, or null to disable auto-select
		private static readonly Type autoSelectOnStart = null;

		private Dictionary<Type, UserControl> instantiatedControls;

		public MainWindow()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

			InitializeComponent();

			var vm = new MainWindowViewModel();
			vm.AlgorithmsLoaded += Vm_AlgorithmsLoaded;
			ViewModel = vm;

			instantiatedControls = new Dictionary<Type, UserControl>();

			Task.Run((Action)vm.LoadAlgorithms);
		}

		public void Dispose()
		{
			foreach(UserControl uc in instantiatedControls.Values) {
				if(uc is IDisposable ucDisposable) {
					ucDisposable.Dispose();
				}
			}
		}

		private void Vm_AlgorithmsLoaded(object sender, EventArgs e)
		{
			Dispatcher.Invoke(delegate
			{
				ShowAlgorithms();
			});
		}

		private void ShowAlgorithms()
		{
			var vm = (MainWindowViewModel)ViewModel;

			string baseNamespace = typeof(App).Namespace;

			List<dynamic> alreadyAdded = new List<dynamic>();

			dynamic AddNew(ItemsControl parent, string name)
			{
				var tvi = new TreeViewItem() { Header = name };
				parent.Items.Add(tvi);
				return new { Name = name, TreeViewItem = tvi, Children = new List<dynamic>() };
			}

			TreeViewItem itemToAutoSelect = null;

			foreach(IAlgorithm algorithm in vm.Algorithms) {
				Type algorithmType = algorithm.GetType();
				string algNamespace = algorithmType.Namespace.Substring(baseNamespace.Length+1);
				string[] algNamespaceParts = algNamespace.Split('.');
				dynamic current = alreadyAdded.FirstOrDefault(ad => ad.Name == algNamespaceParts[0]);
				if(current == null) {
					current = AddNew(_TreeView_Algorithms, algNamespaceParts[0]);
					alreadyAdded.Add(current);
				}
				for(int i = 1; i < algNamespaceParts.Length; ++i) {
					string part = algNamespaceParts[i];
					List<dynamic> currentChildren = (List<dynamic>)current.Children;
					dynamic next = currentChildren.FirstOrDefault(ad => ad.Name == part);
					if(next == null) {
						next = AddNew(current.TreeViewItem, part);
						currentChildren.Add(next);
					}
					current = next;
				}
				var currentTreeViewItem = (TreeViewItem)current.TreeViewItem;
				var newTreeViewItem = new TreeViewItem() { Header = algorithm.Name, Tag = algorithm, Cursor = Cursors.Hand };
				currentTreeViewItem.Items.Add(newTreeViewItem);
				if(autoSelectOnStart!=null && itemToAutoSelect==null && autoSelectOnStart == algorithmType) {
					itemToAutoSelect = newTreeViewItem;
				}
			}

			if(itemToAutoSelect != null) {
				itemToAutoSelect.Select();
			}
		}

		private void TreeView_Algorithms_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var selected = (TreeViewItem)e.NewValue;
			if(selected.Tag == null) {
				return;
			}

			var algorithm = (IAlgorithm)selected.Tag;
			Type algorithmType = algorithm.UserControlType;

			if(_ContentControl_Overview.Content!=null && _ContentControl_Overview.Content.GetType() == algorithmType) {
				return;
			}

			var vm = (MainWindowViewModel)ViewModel;
			vm.SelectedAlgorithmName = algorithm.Name;

			if(!instantiatedControls.TryGetValue(algorithmType, out UserControl newContent)) {
				newContent = (UserControl)Activator.CreateInstance(algorithmType);
				instantiatedControls.Add(algorithmType, newContent);
			}

			_ContentControl_Overview.Content = newContent;
		}
	}
}
