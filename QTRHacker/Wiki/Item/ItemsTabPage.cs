﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QTRHacker.Controls;
using QTRHacker.Res;
using QTRHacker.Wiki.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QTRHacker.Wiki.Item
{
	public class ItemsTabPage : TabPage
	{
		private const int VALUE_P = 1000000, VALUE_G = 10000, VALUE_S = 100, VALUE_C = 1;
		public readonly static Color ThemeColor = Color.FromArgb(140, 140, 140);
		public readonly static Color GlobalBack = Color.FromArgb(200, 200, 200);

		public readonly MListView ItemListView;
		private readonly MTabControl InfoTabs;
		private readonly ItemInfoSubPage ItemInfoPage;
		private readonly ItemDetailInfoSubPage AccInfoPage;
		private readonly ItemSearcherSubPage SearcherPage;

		private string KeyWord = "";

		public readonly static Dictionary<string, int> ItemIDToI = new();
		public readonly static Dictionary<int, string> ItemIDToS = new();

		public readonly static List<ItemData> ItemDatum = new();
		public readonly static List<RecipeData> RecipeDatum = new();

		private static void Init()
		{
			using var s = Assembly.GetExecutingAssembly().GetManifestResourceStream("QTRHacker.Res.Game.WikiRes.zip");
			using ZipArchive z = new(s);
			using (var u = new StreamReader(z.GetEntry("ID/ItemID.json").Open()))
			{
				ItemIDToI.Clear();
				ItemIDToS.Clear();
				JsonConvert.DeserializeObject<Dictionary<string, int>>(u.ReadToEnd()).ToList().ForEach(t =>
				{
					ItemIDToI[t.Key] = t.Value;
					ItemIDToS[t.Value] = t.Key;
				});
			}
			ItemDatum.Clear();
			RecipeDatum.Clear();
			using (var u = new StreamReader(z.GetEntry("ItemInfo.json").Open()))
				ItemDatum.AddRange(JsonConvert.DeserializeObject<List<ItemData>>(u.ReadToEnd()));
			using (var u = new StreamReader(z.GetEntry("RecipeInfo.json").Open()))
				RecipeDatum.AddRange(JsonConvert.DeserializeObject<List<RecipeData>>(u.ReadToEnd()));
		}

		public ItemsTabPage()
		{
			if (!ItemDatum.Any())
				Init();
			GC.Collect();
			BackColor = Color.LightGray;
			BorderStyle = BorderStyle.None;

			ItemListView = new MListView();
			ItemListView.BackColor = Color.FromArgb(100, 100, 100);
			ItemListView.ColumnBackColor = Color.FromArgb(100, 100, 100);
			ItemListView.Bounds = new Rectangle(5, 5, 450, 440);
			ItemListView.FullRowSelect = true;
			ItemListView.MultiSelect = false;
			ItemListView.HideSelection = false;
			ItemListView.View = View.Details;
			ItemListView.Columns.Add(HackContext.CurrentLanguage["Index"], 50);
			ItemListView.Columns.Add(HackContext.CurrentLanguage["Rare"], 50);
			ItemListView.Columns.Add(HackContext.CurrentLanguage["EnglishName"], 125);
			ItemListView.Columns.Add(HackContext.CurrentLanguage["ChineseName"], 125);
			ItemListView.Columns.Add(HackContext.CurrentLanguage["Type"], 100);

			ItemListView.Layout += (s, e) =>
			{
				if (ItemListView.Width - ItemListView.ClientSize.Width > 10)
					ItemListView.Columns[4].Width = 80;
				else
					ItemListView.Columns[4].Width = 100;
			};

			ItemListView.MouseDoubleClick += (s, e) =>
			{
				int id = Convert.ToInt32(ItemListView.SelectedItems[0].Text.ToString());
				var pos = HackContext.GameContext.MyPlayer.Position;
				int num = Functions.GameObjects.Terraria.Item.NewItem(HackContext.GameContext, (int)pos.X, (int)pos.Y, 0, 0, id, ItemDatum[id].MaxStack, false, 0, true);
				Functions.GameObjects.Terraria.NetMessage.SendData(HackContext.GameContext, 21, -1, -1, 0, num, 0, 0, 0, 0, 0, 0);

			};
			ContextMenuStrip strip = ItemListView.ContextMenuStrip = new ContextMenuStrip();
			strip.Items.Add(HackContext.CurrentLanguage["AddToInvMax"]).Click += (s, e) =>
			{
				int id = Convert.ToInt32(ItemListView.SelectedItems[0].Text.ToString());
				var pos = HackContext.GameContext.MyPlayer.Position;
				int num = Functions.GameObjects.Terraria.Item.NewItem(HackContext.GameContext, (int)pos.X, (int)pos.Y, 0, 0, id, ItemDatum[id].MaxStack, false, 0, true);
				Functions.GameObjects.Terraria.NetMessage.SendData(HackContext.GameContext, 21, -1, -1, 0, num, 0, 0, 0, 0, 0, 0);
			};
			strip.Items.Add(HackContext.CurrentLanguage["AddToInvOne"]).Click += (s, e) =>
			{
				int id = Convert.ToInt32(ItemListView.SelectedItems[0].Text.ToString());
				var pos = HackContext.GameContext.MyPlayer.Position;
				int num = Functions.GameObjects.Terraria.Item.NewItem(HackContext.GameContext, (int)pos.X, (int)pos.Y, 0, 0, id, 1, false, 0, true);
				Functions.GameObjects.Terraria.NetMessage.SendData(HackContext.GameContext, 21, -1, -1, 0, num, 0, 0, 0, 0, 0, 0);
			};
			strip.Items.Add(HackContext.CurrentLanguage["ShowRecipeTree"]).Click += (s, e) =>
			{
				int id = Convert.ToInt32(ItemListView.SelectedItems[0].Text.ToString());
				RecipeTreeForm.ShowTree(id);
			};
			ItemListView.SelectedIndexChanged += ItemListView_SelectedIndexChanged;

			ItemInfoPage = new ItemInfoSubPage();
			ItemInfoPage.OnRequireItemDoubleClick += RequireItems_MouseDoubleClick;
			ItemInfoPage.OnRecipeToItemDoubleClick += RecipeToItems_MouseDoubleClick;

			AccInfoPage = new ItemDetailInfoSubPage();


			SearcherPage = new ItemSearcherSubPage();
			SearcherPage.BlockCheckBox.Click += Filter_CheckedChanged;
			SearcherPage.WallCheckBox.Click += Filter_CheckedChanged;
			SearcherPage.QuestItemCheckBox.Click += Filter_CheckedChanged;
			SearcherPage.HeadCheckBox.Click += Filter_CheckedChanged;
			SearcherPage.BodyCheckBox.Click += Filter_CheckedChanged;
			SearcherPage.LegCheckBox.Click += Filter_CheckedChanged;
			SearcherPage.AccessoryCheckBox.Click += Filter_CheckedChanged;
			SearcherPage.MeleeCheckBox.Click += Filter_CheckedChanged;
			SearcherPage.RangedCheckBox.Click += Filter_CheckedChanged;
			SearcherPage.MagicCheckBox.Click += Filter_CheckedChanged;
			SearcherPage.SummonCheckBox.Click += Filter_CheckedChanged;
			SearcherPage.BuffCheckBox.Click += Filter_CheckedChanged;
			SearcherPage.ConsumableCheckBox.Click += Filter_CheckedChanged;
			SearcherPage.OthersCheckBox.Click += Filter_CheckedChanged;

			SearcherPage.KeyWordTextBox.KeyDown += (s, e) =>
			{
				if (e.KeyCode == Keys.Enter)
				{
					e.Handled = true;
					KeyWord = SearcherPage.KeyWordTextBox.Text;
					RefreshItems();
				}
			};

			SearcherPage.ReverseButton.Click += (s, e) =>
			{
				ReverseCheck();
				RefreshItems();
			};

			SearcherPage.SearchButton.Click += (s, e) =>
			{
				KeyWord = SearcherPage.KeyWordTextBox.Text;
				RefreshItems();
			};

			SearcherPage.ResetButton.Click += (s, e) =>
			{
				KeyWord = "";
				SearcherPage.KeyWordTextBox.Text = "";
				RefreshItems();
			};

			InfoTabs = new MTabControl();
			InfoTabs.HeaderBackColor = Color.FromArgb(100, 100, 100);
			InfoTabs.HeaderSelectedBackColor = ThemeColor;
			InfoTabs.Bounds = new Rectangle(460, 5, 270, 440);
			InfoTabs.Controls.Add(ItemInfoPage);
			InfoTabs.Controls.Add(AccInfoPage);
			InfoTabs.Controls.Add(SearcherPage);

			Controls.Add(ItemListView);
			Controls.Add(InfoTabs);
		}

		private void Filter_CheckedChanged(object sender, EventArgs e)
		{
			RefreshItems();
		}

		private void RecipeToItems_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			var a = (sender as ListBox);
			if (a.SelectedItem != null)
			{
				var v = (a.SelectedItem.ToString());
				var c = v.IndexOf("[");
				var d = v.IndexOf("]");
				var b = ItemListView.Items[v.Substring(c + 1, d - c - 1)];
				if (b != null)
				{
					ItemListView.EnsureVisible(b.Index);
					b.Selected = true;
				}
			}
		}

		private void RequireItems_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			var a = (sender as ListBox);
			if (a.SelectedItem != null)
			{
				var v = (a.SelectedItem.ToString());
				var c = v.IndexOf("[");
				var d = v.IndexOf("]");
				var b = ItemListView.Items[v.Substring(c + 1, d - c - 1)];
				if (b != null)
				{
					ItemListView.EnsureVisible(b.Index);
					b.Selected = true;
				}
			}
		}

		public static string GetValueString(int value)
		{
			int p = value / VALUE_P;
			int a = value % VALUE_P;
			int g = a / VALUE_G;
			a %= VALUE_G;
			int s = a / VALUE_S;
			a %= VALUE_S;
			int c = a / VALUE_C;
			return p + HackContext.CurrentLanguage["Platinum"] + " " + g +
				HackContext.CurrentLanguage["Gold"] + " " + s +
				HackContext.CurrentLanguage["Silver"] + " " + c +
				HackContext.CurrentLanguage["Copper"] + "";
		}

		private void ItemListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (ItemListView.SelectedIndices.Count > 0)
			{
				int i = Convert.ToInt32(ItemListView.SelectedItems[0].Text.ToString());

				ItemInfoPage.SetData(i);
				AccInfoPage.SetData(i);
			}
			else
			{
				ItemInfoPage.ResetData();
				AccInfoPage.ResetData();
			}
			ItemListView.Focus();
		}

		private bool Filter(ItemData j)
		{
			List<bool> b = new List<bool>
			{
				j.CreateTile != -1,
				j.CreateWall != -1,
				j.HeadSlot != -1,
				j.BodySlot != -1,
				j.LegSlot != -1,
				j.Accessory,
				j.Melee,
				j.Ranged,
				j.Magic,
				j.Summon || j.Sentry,
				j.BuffType != 0,
				j.Consumable,
				j.QuestItem
			};
			bool r = false;
			r |= (SearcherPage.BlockCheckBox.Checked && b[0]);
			r |= (SearcherPage.WallCheckBox.Checked && b[1]);
			r |= (SearcherPage.HeadCheckBox.Checked && b[2]);
			r |= (SearcherPage.BodyCheckBox.Checked && b[3]);
			r |= (SearcherPage.LegCheckBox.Checked && b[4]);
			r |= (SearcherPage.AccessoryCheckBox.Checked && b[5]);
			r |= (SearcherPage.MeleeCheckBox.Checked && b[6]);
			r |= (SearcherPage.RangedCheckBox.Checked && b[7]);
			r |= (SearcherPage.MagicCheckBox.Checked && b[8]);
			r |= (SearcherPage.SummonCheckBox.Checked && b[9]);
			r |= (SearcherPage.BuffCheckBox.Checked && b[10]);
			r |= (SearcherPage.ConsumableCheckBox.Checked && b[11]);
			r |= (SearcherPage.QuestItemCheckBox.Checked && b[12]);
			if (b.TrueForAll(t => !t) && SearcherPage.OthersCheckBox.Checked)
				return true;
			return r;
		}

		private static string GetItemType(ItemData j)
		{
			if (j.CreateTile != -1) return HackContext.CurrentLanguage["Blocks"];
			if (j.CreateWall != -1) return HackContext.CurrentLanguage["Walls"];
			if (j.QuestItem) return HackContext.CurrentLanguage["Quest"];
			if (j.HeadSlot != -1) return HackContext.CurrentLanguage["Head"];
			if (j.BodySlot != -1) return HackContext.CurrentLanguage["Body"];
			if (j.LegSlot != -1) return HackContext.CurrentLanguage["Leg"];
			if (j.Accessory) return HackContext.CurrentLanguage["Accessory"];
			if (j.Melee) return HackContext.CurrentLanguage["Melee"];
			if (j.Ranged) return HackContext.CurrentLanguage["Ranged"];
			if (j.Magic) return HackContext.CurrentLanguage["Magic"];
			if ((j.Summon || j.Sentry)) return HackContext.CurrentLanguage["Summon"];
			if (j.BuffType != 0) return HackContext.CurrentLanguage["Buff"];
			if (j.Consumable) return HackContext.CurrentLanguage["Consumable"];
			return HackContext.CurrentLanguage["None"];
		}

		public void ReverseCheck()
		{
			SearcherPage.BlockCheckBox.Checked = !SearcherPage.BlockCheckBox.Checked;
			SearcherPage.WallCheckBox.Checked = !SearcherPage.WallCheckBox.Checked;
			SearcherPage.QuestItemCheckBox.Checked = !SearcherPage.QuestItemCheckBox.Checked;
			SearcherPage.HeadCheckBox.Checked = !SearcherPage.HeadCheckBox.Checked;
			SearcherPage.BodyCheckBox.Checked = !SearcherPage.BodyCheckBox.Checked;
			SearcherPage.LegCheckBox.Checked = !SearcherPage.LegCheckBox.Checked;
			SearcherPage.AccessoryCheckBox.Checked = !SearcherPage.AccessoryCheckBox.Checked;
			SearcherPage.MeleeCheckBox.Checked = !SearcherPage.MeleeCheckBox.Checked;
			SearcherPage.RangedCheckBox.Checked = !SearcherPage.RangedCheckBox.Checked;
			SearcherPage.MagicCheckBox.Checked = !SearcherPage.MagicCheckBox.Checked;
			SearcherPage.SummonCheckBox.Checked = !SearcherPage.SummonCheckBox.Checked;
			SearcherPage.BuffCheckBox.Checked = !SearcherPage.BuffCheckBox.Checked;
			SearcherPage.ConsumableCheckBox.Checked = !SearcherPage.ConsumableCheckBox.Checked;
			SearcherPage.OthersCheckBox.Checked = !SearcherPage.OthersCheckBox.Checked;
		}

		public void RefreshItems()
		{
			ItemListView.BeginUpdate();
			ItemListView.Items.Clear();
			for (int i = 0; i < ItemDatum.Count; i++)
			{
				var itm = ItemDatum[i];
				if (itm.Type == 0 || !Filter(itm)) continue;
				string name_en = HackContext.GameLocLoader_en.GetItemName(ItemIDToS[i]);
				string name_cn = HackContext.GameLocLoader_cn.GetItemName(ItemIDToS[i]);

				bool flag = false;
				flag |= i.ToString().Contains(KeyWord, StringComparison.OrdinalIgnoreCase);
				flag |= name_en.Contains(KeyWord, StringComparison.OrdinalIgnoreCase);
				flag |= name_cn.Contains(KeyWord, StringComparison.OrdinalIgnoreCase);
				flag |= itm.Shoot.ToString().Contains(KeyWord, StringComparison.OrdinalIgnoreCase);
				flag |= itm.CreateTile.ToString().Contains(KeyWord, StringComparison.OrdinalIgnoreCase);
				flag |= itm.CreateWall.ToString().Contains(KeyWord, StringComparison.OrdinalIgnoreCase);
				if (flag)
				{
					ListViewItem lvi = new(i.ToString());
					lvi.Name = i.ToString();
					lvi.SubItems.Add(itm.Rare.ToString());
					lvi.SubItems.Add(name_en);
					lvi.SubItems.Add(name_cn);
					lvi.SubItems.Add(GetItemType(itm));
					ItemListView.Items.Add(lvi);
				}
			}
			ItemListView.EndUpdate();
			ItemListView.PerformLayout();
		}
	}
}
