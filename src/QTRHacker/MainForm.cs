﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Newtonsoft.Json;
using QTRHacker.Configs;
using QTRHacker.Controls;
using QTRHacker.PagePanels;
using QTRHacker.Res;
using QTRHacker.Wiki;
using QTRHacker.XNAControls;

namespace QTRHacker
{
	public partial class MainForm : MForm
	{
		private readonly Panel ButtonsPanel, ContentPanel;
		public readonly PagePanel MainPagePanel, BasicPagePanel, PlayerPagePanel, ProjectilePagePanel, ScriptsPagePanel, SchesPagePanel,
			MiscPagePanel, ChatSenderPanel, AimBotPagePanel, AboutPagePanel;
		public static MainForm MainFormInstance { get; private set; }
		public readonly PageGroup Group1, Group2;
		public PageGroup ExpandedGroup;
		public readonly static int ButtonsPanelWidth = 132;
		private int GroupsIndex = 0;
		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);

			if (System.Diagnostics.Process.GetProcessesByName("QTRHacker").Length > 1)
			{
				MessageBox.Show("You have already started a hack.\nPlease close the current one before trying to start again.");
				Environment.Exit(0);
			}
		}
		public MainForm()
		{
			HackContext.Initialize();//before everything

			CFG_QTRHacker cfg = HackContext.Configs["CFG_QTRHacker"] as CFG_QTRHacker;
			if (cfg.FirstRunning)
			{
				cfg.FirstRunning = false;
				if (System.Threading.Thread.CurrentThread.CurrentCulture.Name == "zh-CN")
				{
					if (MessageBox.Show("检测到当前环境为中文\n是否选择语言为中文？\n你可以稍后在配置文件./Content/Config/CFG_QTRHacker.json中更改", "第一次运行", MessageBoxButtons.YesNo) == DialogResult.Yes)
						cfg.IsCN = true;
					else
						cfg.IsCN = false;
				}
				HackContext.SaveConfigs();
				HackContext.Initialize();//again
			}
			MainPanel.BackColor = Color.FromArgb(30, 30, 30);
			CheckForIllegalCrossThreadCalls = false;
			MainFormInstance = this;
			ClientSize = new Size(300 + ButtonsPanelWidth, 400);
			Text = $"QTRHacker-V{Assembly.GetExecutingAssembly().GetName().Version}";
			FormBorderStyle = FormBorderStyle.None;

			ButtonsPanel = new Panel();
			ButtonsPanel.Bounds = new Rectangle(0, 0, ButtonsPanelWidth, MainPanel.Height);
			ButtonsPanel.BackColor = Color.FromArgb(90, 90, 90);
			MainPanel.Controls.Add(ButtonsPanel);



			Image img_MainPage = null, img_Basic = null, img_Player = null,
				img_Projectile = null, img_Misc = null, img_Scripts = null,
				img_Sche = null, img_ChatSender = null, img_AimBot = null,
				img_About = null;
			using (Stream st = new MemoryStream(GameResLoader.ItemImageData["Item_171"]))
				img_MainPage = Image.FromStream(st);
			using (Stream st = new MemoryStream(GameResLoader.ItemImageData["Item_990"]))
				img_Basic = Image.FromStream(st);
			using (Stream st = Assembly.GetExecutingAssembly().GetManifestResourceStream("QTRHacker.Res.Image.player.png"))
				img_Player = Image.FromStream(st);
			using (Stream st = new MemoryStream(GameResLoader.ItemImageData["Item_42"]))
				img_Projectile = Image.FromStream(st);
			using (Stream st = new MemoryStream(GameResLoader.ItemImageData["Item_518"]))
				img_Scripts = Image.FromStream(st);
			using (Stream st = new MemoryStream(GameResLoader.ItemImageData["Item_3124"]))
				img_Misc = Image.FromStream(st);
			using (Stream st = new MemoryStream(GameResLoader.ItemImageData["Item_109"]))
				img_About = Image.FromStream(st);
			using (Stream st = new MemoryStream(GameResLoader.ItemImageData["Item_531"]))
				img_ChatSender = Image.FromStream(st);


			using (Stream st = new MemoryStream(GameResLoader.ItemImageData["Item_3"]))
				img_Sche = Image.FromStream(st);
			using (Stream st = new MemoryStream(GameResLoader.ItemImageData["Item_164"]))
				img_AimBot = Image.FromStream(st);

			ContentPanel = new Panel();
			ContentPanel.Bounds = new Rectangle(ButtonsPanel.Width, 0, MainPanel.Width - ButtonsPanel.Width, MainPanel.Height);
			MainPanel.Controls.Add(ContentPanel);

			int pageWidth = ContentPanel.Width;

			MainPagePanel = new PagePanel_MainPage(pageWidth, MainPanel.Height);
			AboutPagePanel = new PagePanel_About(pageWidth, MainPanel.Height);
			BasicPagePanel = new PagePanel_Basic(pageWidth, MainPanel.Height);
			PlayerPagePanel = new PagePanel_Player(pageWidth, MainPanel.Height);
			ProjectilePagePanel = new PagePanel_Projectile(pageWidth, MainPanel.Height);
			ScriptsPagePanel = new PagePanel_Scripts(pageWidth, MainPanel.Height);
			MiscPagePanel = new PagePanel_Misc(pageWidth, MainPanel.Height);
			ChatSenderPanel = new PagePanel_ChatSender(pageWidth, MainPanel.Height);


			SchesPagePanel = new PagePanel_Sches(pageWidth, MainPanel.Height);
			//AimBotPagePanel = new PagePanel_AimBot(pageWidth, MainPanel.Height);


			ExpandedGroup = Group1 = AddGroup();
			Group2 = AddGroup();


			AddButton(Group1, HackContext.CurrentLanguage["Basic"], img_Basic, BasicPagePanel).Enabled = false;
			AddButton(Group1, HackContext.CurrentLanguage["Players"], img_Player, PlayerPagePanel).Enabled = false;
			/*AddButton(Group1, HackContext.CurrentLanguage["Projectiles"], img_Projectile, ProjectilePagePanel).Enabled = false;*/
			AddButton(Group1, HackContext.CurrentLanguage["Scripts"], img_Scripts, ScriptsPagePanel).Enabled = false;
			//AddButton(Group1, HackContext.CurrentLanguage["ChatSender"], img_ChatSender, ChatSenderPanel).Enabled = false;
			AddButton(Group1, HackContext.CurrentLanguage["Miscs"], img_Misc, MiscPagePanel).Enabled = false;

			if (HackContext.CurrentLanguage.Name == "zh-CN")
				AddButton(Group1, HackContext.CurrentLanguage["About"], img_About, AboutPagePanel).Enabled = true;
			AddButton(Group1, HackContext.CurrentLanguage["MainPage"], img_MainPage, MainPagePanel).Selected = true;

			AddButton(Group2, HackContext.CurrentLanguage["Sches"], img_Sche, SchesPagePanel).Enabled = false;
			//AddButton(Group2, CurrentLanguage["AimBot"], img_AimBot, AimBotPagePanel).Enabled = false;

			Icon = ConvertToIcon(img_Basic);
		}
		public void OnInitialized()
		{
			foreach (var c in ButtonsPanel.Controls)
			{
				foreach (var cc in ((Control)c).Controls)
				{
					(cc as Control).Enabled = true;
				}
			}
			(Group1.Controls[0] as ImageButtonS).Selected = true;
		}

		/// <summary>
		/// from: http://www.cnblogs.com/ahdung/p/ConvertToIcon.html
		/// cridit to: AhDung
		/// </summary>
		/// <param name="image"></param>
		/// <param name="nullTonull"></param>
		/// <returns></returns>
		public static Icon ConvertToIcon(Image image)
		{
			if (image == null)
				return null;
			using MemoryStream msImg = new(), msIco = new();
			image.Save(msImg, ImageFormat.Png);
			using var bin = new BinaryWriter(msIco);
			bin.Write((short)0);
			bin.Write((short)1);
			bin.Write((short)1);

			bin.Write((byte)image.Width);
			bin.Write((byte)image.Height);
			bin.Write((byte)0);
			bin.Write((byte)0);
			bin.Write((short)0);
			bin.Write((short)32);
			bin.Write((int)msImg.Length);
			bin.Write(22);

			bin.Write(msImg.ToArray());
			bin.Flush();
			bin.Seek(0, SeekOrigin.Begin);
			return new Icon(msIco);
		}

		private PageGroup AddGroup()
		{
			PageGroup group = new PageGroup();
			group.Location = new Point(ButtonsPanel.Width - 100 - 32 * GroupsIndex, 0);
			group.Height = ButtonsPanel.Height;
			ButtonsPanel.Controls.Add(group);
			GroupsIndex++;
			return group;
		}

		private ImageButtonS AddButton(PageGroup group, string Text, Image Icon, Control content)
		{
			var button = group.AddButton(Text, Icon, (s, e) =>
			  {
				  ImageButtonS bs = (s as ImageButtonS);
				  if (!bs.Selected)
					  return;
				  PageGroup previousGroup = ExpandedGroup;
				  PageGroup targetGroup = bs.Parent as PageGroup;
				  targetGroup.BringToFront();

				  foreach (var i in ExpandedGroup.Controls)
					  if (i is ImageButtonS ii && i != bs)
						  ii.Selected = false;

				  Point tmpP = previousGroup.Location;
				  previousGroup.Location = targetGroup.Location;
				  targetGroup.Location = tmpP;

				  Size tmpS = previousGroup.Size;
				  previousGroup.Size = targetGroup.Size;
				  targetGroup.Size = tmpS;

				  ExpandedGroup = targetGroup;

				  previousGroup.Invalidate();
				  targetGroup.Invalidate();

				  if (content == null)
					  return;
				  ContentPanel.Controls.Clear();
				  ContentPanel.Controls.Add(content);
			  });
			button.Paint += (s, e) =>
			{
				var control = s as Control;
				using Pen pen1 = new Pen(Color.FromArgb(100, Color.White));
				using Pen pen2 = new Pen(Color.FromArgb(10, Color.White));
				Point tl = new Point(0, 0);
				Point tr = new Point(control.Width - 1, 0);
				Point bl = new Point(0, control.Height - 1);
				Point br = new Point(control.Width - 1, control.Height - 1);
				e.Graphics.DrawLine(pen1, tl, bl);
				e.Graphics.DrawLine(pen2, tl, tr);
				e.Graphics.DrawLine(pen2, bl, br);
			};
			return button;
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			Environment.Exit(0);
		}
	}
}
