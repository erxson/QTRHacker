﻿using QTRHacker.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QTRHacker.Controls
{
	public class PageGroup : UserControl
	{
		private bool _expanded;
		private int ButtonsNumber = 0;
		public int ExpandedWidth = 100;
		public int NonExpandedWidth = 30;

		public bool Expanded
		{
			get => _expanded;
			set
			{
				_expanded = value;
				Width = _expanded ? ExpandedWidth : NonExpandedWidth;
			}
		}
		public PageGroup()
		{
			BackColor = Color.FromArgb(255, 74, 74, 74);
		}

		public ImageButtonS AddButton(string Text, Image icon, Action<object, EventArgs> OnSelected)
		{
			ImageButtonS b = new ImageButtonS(icon);
			b.Location = new Point(0, 30 * (ButtonsNumber++));
			b.Text = Text;
			Controls.Add(b);
			b.OnSelected += OnSelected;
			return b;
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
		}
	}
}
