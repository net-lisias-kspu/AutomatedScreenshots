/*
	This file is part of Automated Screenshots /L Unleashed
		© 2018-2021 Lisias T : http://lisias.net <support@lisias.net>
		© 2015-2018 LinuxGuruJamer

	Automated Screenshots /L Unleashed is licensed as follows:
		* GPL 3.0 : https://www.gnu.org/licenses/gpl-3.0.txt

	Automated Screenshots /L Unleashed is distributed in the hope that
	it will be useful, but WITHOUT ANY WARRANTY; without even the implied
	warranty of	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the GNU General Public License 3.0
	along with Automated Screenshots /L Unleashed.
	If not, see <https://www.gnu.org/licenses/>.

*/
using System;
using System.IO;

namespace AutomatedScreenshots
{
	public class Configuration
	{
		private static readonly Configuration instance = new Configuration ();

		public  ushort MAX_SUPERSIZE = 4;

//		public bool screenshotAtIntervals { get; set; }
		public float screenshotInterval { get; set; }
		public bool convertToJPG { get; set; }
		public bool keepOrginalPNG { get; set; }

		private string _screenshotPath = KSPe.IO.Hierarchy.SCREENSHOT.Solve();
		public string screenshotPath {
			get => _screenshotPath;
			set
			{
				//check if directory doesn't exist
				if (!System.IO.Directory.Exists(value))
				{
					Log.trace("Directory does not exist");
					//if it doesn't, try to create it
					try
					{
						Log.trace("Trying to create directory");
						System.IO.Directory.CreateDirectory(value);
					}
					catch (Exception e)
					{
						Log.trace("Exception trying to create directory: {0}", e.Message);
						return;
					}
					Log.trace("Directory created");
				}
				this._screenshotPath = value;
			}
		}
		public string filename { get; set; }
		public bool asynchronous { get; set; }
		public ushort JPGQuality { get; set; }
		public bool screenshotOnSceneChange { get; set; }
		public bool onSpecialEvent { get; set; }
		public bool noGUIOnScreenshot { get; set; }
		public bool guiOnScreenshot { get; set; }
		public string keycode { get; set; }

		public bool precrashSnapshots { get; set; }
		public ushort secondsUntilImpact {get; set; }
		public ushort hsAltitudeLimit { get; set; }
		public ushort hsMinVerticalSpeed { get; set; }
		public float hsScreenshotInterval { get; set; }

		public ushort supersize { get; set; }

		internal Boolean BlizzyToolbarIsAvailable = false;

		//
		// Automated saves info
		//
		public bool autoSave;
		public ushort minBetweenSaves;
		public string savePrefix;
		public ushort numToRotate;
		public bool autoSaveOnGameStart;
	//	public string toggleAutoSave;

		public Configuration ()
		{
//			screenshotAtIntervals = false;
			screenshotInterval = 5.0F;
			convertToJPG = true;
			keepOrginalPNG = false;
			screenshotPath = FileOperations.ROOT_PATH + "Screenshots/";
			noGUIOnScreenshot = false;
			guiOnScreenshot = true;
			filename = "AS-[cnt]";
			asynchronous = false;
			JPGQuality = 75;
			screenshotOnSceneChange = false;
			onSpecialEvent = false;
			keycode = "F6";

			precrashSnapshots = false;
			secondsUntilImpact = 5;
			hsAltitudeLimit = 25;
			hsMinVerticalSpeed = 3;
			hsScreenshotInterval = 0.2F;

			supersize = 0;

			autoSave = false;
			minBetweenSaves = 5;
			savePrefix = "rotate-[cnt]";
			numToRotate = 15;
			autoSaveOnGameStart = false;
			//toggleAutoSave = "Ctrl-F6";
		}

		public static Configuration Instance {
			get {
				return instance;
			}
		}

		public void Save ()
		{
			Log.trace("Configuration.Save");
			FileOperations.SaveConfiguration (this, FileOperations.AS_CFG_FILE);
			AS.changeCallbacks = true;
		}

		public void Load ()
		{
			Log.trace("Configuration.Load");
			FileOperations.LoadConfiguration (this, FileOperations.AS_CFG_FILE);
		}

	}
}
