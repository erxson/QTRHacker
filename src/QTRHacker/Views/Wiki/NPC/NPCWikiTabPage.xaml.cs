﻿using QTRHacker.ViewModels.Wiki.NPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace QTRHacker.Views.Wiki.NPC
{
	/// <summary>
	/// NPCWikiTabPage.xaml 的交互逻辑
	/// </summary>
	public partial class NPCWikiTabPage : UserControl
	{
		public NPCPageViewModel ViewModel => DataContext as NPCPageViewModel;
		public NPCWikiTabPage()
		{
			InitializeComponent();
		}

		private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			ViewModel.AddSelectedNPCToGame();
		}
	}
}
