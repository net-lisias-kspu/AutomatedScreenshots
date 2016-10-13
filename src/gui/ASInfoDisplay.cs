﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace AutomatedScreenshots
{
		
	class ASInfoDisplay
	{

		//Singleton

		private static ASInfoDisplay instance = null;

		public static ASInfoDisplay Instance {
			get {
				if (instance == null)
					instance = new ASInfoDisplay ();
				return instance;
			}
		}

		//Properties

		public const float WINDOW_WIDTH_MINIMIZED = 60;
		public const float WINDOW_WIDTH_DEFAULT = 250;
		public const float WINDOW_WIDTH_BIG = 320;
		public const float WINDOW_HEIGHT = 360;
		public const float WINDOW_HEIGHT_BIG = 480;
		public const float WINDOW_HEIGHT_MINIMIZED = 64;

		public static bool infoDisplayActive = false;
		public static bool infoDisplayMinimized = false;
		public static bool infoDisplayDetailed = false;
		public static bool infoDisplayOptions = false;
		public static Rect infoWindowPos = new Rect (20, Screen.height / 2 - WINDOW_HEIGHT / 2, WINDOW_WIDTH_DEFAULT, WINDOW_HEIGHT);
		public static Vector2 infoScrollPos = Vector2.zero;

	}
}
