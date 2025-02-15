﻿using QTRHacker.Core.GameObjects.Terraria;
using QTRHacker.ViewModels.PlayerEditor;
using QTRHacker.Views.PlayerEditor;
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

namespace QTRHacker.Views.PlayerEditor
{
	/// <summary>
	/// ItemSlotsEditor.xaml 的交互逻辑
	/// </summary>
	public partial class ItemSlotsEditor : UserControl
	{
		public ItemSlotsEditorViewModel ViewModel => DataContext as ItemSlotsEditorViewModel;
		public ItemSlotsEditor()
		{
			InitializeComponent();
		}
	}
}
