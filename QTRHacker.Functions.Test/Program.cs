﻿using Keystone;
using Microsoft.Diagnostics.Runtime;
using QHackLib;
using QHackLib.Assemble;
using QHackLib.FunctionHelper;
using QTRHacker.Functions.GameObjects;
using QTRHacker.Functions.ProjectileImage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QTRHacker.Functions.Test
{
	class Program
	{
		static void Main(string[] args)
		{
			using (GameContext gc = GameContext.OpenGame(Process.GetProcessesByName("Terraria")[0].Id))
			{
				//Console.WriteLine(gc.HContext.MainAddressHelper.GetFunctionAddress("Terraria.Main", "DoUpdate").ToString("X8"));
				//GameContext.NewText(gc, "123", 0);
				//Console.WriteLine(gc.HContext.GetAddressHelper("TRInjections.dll").GetStaticFieldValue<bool>("TRInjections.ScheMaker.ScheMaker","Brushing"));
				Console.Read();
			}
		}
	}
}
