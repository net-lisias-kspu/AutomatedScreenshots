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
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AutomatedScreenshots
{
	
	public enum UIOnScreenshot : ushort
	{
		show = 0,
		hide,
		both
    };

	[KSPAddon (KSPAddon.Startup.MainMenu, true)]
	public partial class AS : MonoBehaviour
	{

		public const String TITLE = "Automated Screenshots and Saves";

		private float lastUpdate = 0.0f;
		private float lastPrecrashUpdate = 0.0f;
		private int cnt = 0;
		//		private float snapshotInterval = 5.0f;
		public static bool doSnapshots = false;
		private static bool doSave = false;

		private bool specialScene = false;
		private bool precrash = false;
		private bool newScene = false;
		private bool sceneReady = false;

		public bool isSceneReady ()
		{
			return sceneReady;
		}

		public bool isSpecialScene ()
		{
			return specialScene;
		}

		public bool isPreCrash ()
		{
			return precrash;
		}

		public bool isNewScene ()
		{
			return newScene;
		}

		private float lastSceneUpdate = 0.0f;
		private float sceneReadyAt = 0.0f;
		private string jpgName = null;
		private bool screenshotTaken = false;
		private string lastScreenshot = null;
		public static bool changeCallbacks;
		public static Configuration configuration = new Configuration ();
		public static KeyCode activeKeycode;
		private bool wasUIVisible = true;
		private bool dualScreenshots = false;
		public MainMenuGui gui = null;
		private Service.ImageConverter imageConverter = null;
		private float lastBackup = 0.0f;
		public Thread backupThread = null;

		//int saveFileCnt = 0;

		public void DoSave ()
		{
			doSave = true;
		}

		public void DoSnapshot ()
		{
			doSnapshots = true;
			this.gui.updateButtonStatus();
		}


		static AS ()
		{
		}

		public AS ()
		{
			Log.trace("New instance of Automated Screenshots: AS constructor");
		}

		public void Awake()
		{
			Log.trace("Awake");
			GameEvents.onGUIApplicationLauncherUnreadifying.Add(hideNow);
		}

		public void hideNow(GameScenes scene)
		{
			if (null != this.gui)
				this.gui.GUIToggleFalse();
		}

		public void Start ()
		{
			Log.trace("Start");
			DontDestroyOnLoad (this);
			this.imageConverter = Service.ImageConverter.Create(this);
			FileOperations.MoveCfgToDataDir();

			configuration.Load ();
		}

		public void Update ()
		{
			if (this.gui == null) {
				Log.dbg("this.gui == null");
				this.gui = MainMenuGui.Create(this);
				this.gui.SetVisible (false);
				RegisterEvents ();
			}

			if (changeCallbacks) {
				Log.dbg("Update - changeCallbacks: {0}", changeCallbacks);
				RegisterEvents ();
			}

			if (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl))
			{
				if(Input.GetKeyDown(KeyCode.F6))
				{
					Log.dbg("KeyCode: CTRL-F6 pressed");
					AS.configuration.autoSave = !AS.configuration.autoSave;
					Log.trace("AutoSave: {0}", AS.configuration.autoSave);
					this.gui.updateButtonStatus();
				}
			}
			else if(Input.GetKeyDown(activeKeycode))
			{
				Log.dbg("KeyCode: {0} pressed at GameScene: {1}", activeKeycode, HighLogic.LoadedScene);
				if (HighLogic.LoadedScene != GameScenes.MAINMENU) {
					doSnapshots = !doSnapshots;
					if (!doSnapshots && screenshotTaken && configuration.noGUIOnScreenshot && wasUIVisible)
						GameEvents.onShowUI.Fire();
				}
				else
					doSnapshots = false;

				Log.dbg("LoadedScene = {0}  doSnapshots: {1}", HighLogic.LoadedScene, doSnapshots);
				this.gui.updateButtonStatus();
			}
		}

		public void LateUpdate ()
		{
			string pngName;

			if (doSave || (AS.configuration.autoSave && ((Time.realtimeSinceStartup - lastBackup) > AS.configuration.minBetweenSaves * 60))) {
				lastBackup = Time.realtimeSinceStartup;
				SaveFilesHandlers sfh = new SaveFilesHandlers ();
				sfh.startBackup (this);
				doSave = false;
			}

			if (doSnapshots)
			{
				Log.dbg("In LateUpdate, doSnapshots");

				if (screenshotTaken && configuration.noGUIOnScreenshot && wasUIVisible && (null != this.lastScreenshot && System.IO.File.Exists(this.lastScreenshot))) // FIXME hitting the filesyste on Updates are bad. Try to work around this.
					GameEvents.onShowUI.Fire ();

				if (AS.configuration.precrashSnapshots) {
					if (FlightGlobals.ActiveVessel != null) {
						Vessel vessel = FlightGlobals.ActiveVessel;

						if ((-vessel.verticalSpeed > AS.configuration.hsMinVerticalSpeed) &&
							((FlightGlobals.ship_altitude / -vessel.verticalSpeed < AS.configuration.secondsUntilImpact) ||
							(FlightGlobals.ship_altitude < AS.configuration.hsAltitudeLimit)
							)) {

							if (Time.realtimeSinceStartup - lastPrecrashUpdate > configuration.hsScreenshotInterval) {
								this.precrash = true;
								lastPrecrashUpdate = Time.realtimeSinceStartup;

								Log.dbg("vessel.verticalSpeed: {0}", vessel.verticalSpeed);
								Log.dbg("FlightGlobals.ship_altitude: {0}", FlightGlobals.ship_altitude);
								Log.dbg("FlightGlobals.ship_altitude  / -vessel.verticalSpeed: {0}", (FlightGlobals.ship_altitude / -vessel.verticalSpeed));
							}
						}
					}
				}

				if ((this.specialScene && !this.newScene) || this.precrash || dualScreenshots ||
					( /*AS.configuration.screenshotAtIntervals && */
						((this.newScene && (this.sceneReady && Time.realtimeSinceStartup - sceneReadyAt > 0.1)  && Time.realtimeSinceStartup - lastSceneUpdate > 1) ||
							((Time.realtimeSinceStartup - lastUpdate) > configuration.screenshotInterval && !this.newScene)
						)
					))
				{
					Log.dbg("this.specialScene: {0}", this.specialScene);
					Log.dbg("this.precrash: {0}", this.precrash);
					Log.dbg("dualScreenshots: {0}", this.dualScreenshots);
					Log.dbg("this.newScene: {0}", this.newScene);
					Log.dbg("this.sceneReady: {0}", this.sceneReady);
					Log.dbg("Time.realtimeSinceStartup - sceneReadyAt: {0}", (Time.realtimeSinceStartup - sceneReadyAt));
					Log.dbg("Time.realtimeSinceStartup - lastSceneUpdate: {0}", (Time.realtimeSinceStartup - lastSceneUpdate));
					Log.dbg("Time.realtimeSinceStartup - lastUpdate: {0}", (Time.realtimeSinceStartup - lastUpdate));

					Log.dbg("Taking screenshot");
					Log.dbg("CurrentDirectory: {0}", System.IO.Directory.GetCurrentDirectory ());
					Log.dbg("FileOperations.ScreenshotFolder: {0}", Configuration.Instance.screenshotPath);
					newScene = false;
					this.specialScene = false;

					do {
						string s = AddInfo (configuration.filename, cnt++, sceneReady, specialScene, precrash);

						pngName = System.IO.Path.GetFullPath(Configuration.Instance.screenshotPath) + s + ".png";
						jpgName = System.IO.Path.GetFullPath(Configuration.Instance.screenshotPath) + s + ".jpg";
					} while (System.IO.File.Exists (pngName) || System.IO.File.Exists (jpgName));

					this.precrash = false;

					if (configuration.noGUIOnScreenshot)
						GameEvents.onHideUI.Fire ();
					if (configuration.noGUIOnScreenshot && configuration.guiOnScreenshot) {
						if (!dualScreenshots)
							dualScreenshots = true;
						else if (dualScreenshots) {
							dualScreenshots = false;
							GameEvents.onShowUI.Fire ();
						}
					}

					if (!dualScreenshots)
					{
						lastUpdate = Time.realtimeSinceStartup;
						screenshotTaken = true;
					}

					// If Historian is available, then tell it to activate
					HistorianSupport.Instance.set_m_Active();

					// Change second number for supersize.  If non-zero,
					// then multiplies the resolution by that number
					// Must be an integer
					KSPe.Util.Image.Screenshot.Capture(pngName, configuration.supersize);
					this.lastScreenshot = pngName;

					if (configuration.convertToJPG)
						this.imageConverter.AddJob(pngName, configuration.keepOrginalPNG, jpgName, configuration.JPGQuality);
				}
			}
		}

		public void RegisterEvents ()
		{
			Log.trace("RegisterEvents");

			RegisterSceneChanges (false);
			RegisterSpecialEvents (false);
			RegisterSceneChanges (AS.configuration.screenshotOnSceneChange);
			RegisterSpecialEvents (AS.configuration.onSpecialEvent);
			changeCallbacks = false;
			GameEvents.onGameStateLoad.Add (setAutosave);
		}

		void setAutosave(ConfigNode evt)
		{
			Log.trace("setAutosave");
			AS.configuration.autoSave = AS.configuration.autoSaveOnGameStart;
			gui.updateButtonStatus ();
		}

		private void RegisterSceneChanges (bool  enable)
		{
			Log.trace("RegisterSceneChanges: {0}", enable);
			if (enable) {
				GameEvents.onGameSceneLoadRequested.Add (this.CallbackGameSceneLoadRequested);
				//GameEvents.onLevelWasLoaded.Add (this.CallbackLevelWasLoaded);
			} else {
				GameEvents.onGameSceneLoadRequested.Remove (this.CallbackGameSceneLoadRequested);
				//GameEvents.onLevelWasLoadedGUIReady.Remove (this.CallbackLevelWasLoaded);
			}
		}

        void OnEnable()
        {
            //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
            SceneManager.sceneLoaded += CallbackLevelWasLoaded;
        }

        void OnDisable()
        {
            //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
            SceneManager.sceneLoaded -= CallbackLevelWasLoaded;
        }

        //
        // Register and unregister all the special events here
        //
        private void RegisterSpecialEvents (bool enable)
		{
			Log.trace("RegisterSpecialEvents: {0}", enable);
			if (enable) {
				GameEvents.onActiveJointNeedUpdate.Add (this.CallbackVesselEventHappened);
				GameEvents.onCollision.Add (this.CallbackEventReportHappened);

				GameEvents.onCrash.Add (this.CallbackEventReportHappened);
				GameEvents.onCrashSplashdown.Add (this.CallbackEventReportHappened);
				GameEvents.onCrewKilled.Add (this.CallbackEventReportHappened);
				GameEvents.onCrewOnEva.Add (this.CallbackCrewEvaLoaded);
				GameEvents.onFlagPlant.Add (this.CallbackVesselEventHappened);
				GameEvents.onKrakensbaneDisengage.Add (this.CallbackVector3dWasLoaded);
				GameEvents.onKrakensbaneEngage.Add (this.CallbackVector3dWasLoaded);
				GameEvents.onLaunch.Add (this.CallbackEventReportHappened);
				GameEvents.onPartCouple.Add (this.CallbackPartCouple);
				GameEvents.onPartDie.Add (this.CallbackPartDieWasLoaded);
				GameEvents.onPartExplode.Add (this.CallbackPartExplode);
				GameEvents.onPartJointBreak.Add (this.CallbackPartJointWasLoaded);

				GameEvents.onPartUndock.Add (this.CallbackPartDieWasLoaded);
				GameEvents.onStageActivate.Add (this.CallbackStageActivateWasLoaded);
				GameEvents.onStageSeparation.Add (this.CallbackEventReportHappened);

				GameEvents.onVesselChange.Add (this.CallbackOnVesselChange);
				GameEvents.onVesselOrbitClosed.Add (this.CallbackVesselEventHappened);
				GameEvents.onVesselOrbitEscaped.Add (this.CallbackVesselEventHappened);
				GameEvents.onVesselSOIChanged.Add (this.CallbackSOIChanged);
			} else {
				GameEvents.onActiveJointNeedUpdate.Remove (this.CallbackVesselEventHappened);
				GameEvents.onCollision.Remove (this.CallbackEventReportHappened);

				GameEvents.onCrash.Remove (this.CallbackEventReportHappened);
				GameEvents.onCrashSplashdown.Remove (this.CallbackEventReportHappened);
				GameEvents.onCrewKilled.Remove (this.CallbackEventReportHappened);
				GameEvents.onCrewOnEva.Remove (this.CallbackCrewEvaLoaded);
				GameEvents.onFlagPlant.Remove (this.CallbackVesselEventHappened);
				GameEvents.onKrakensbaneDisengage.Remove (this.CallbackVector3dWasLoaded);
				GameEvents.onKrakensbaneEngage.Remove (this.CallbackVector3dWasLoaded);
				GameEvents.onLaunch.Remove (this.CallbackEventReportHappened);
				GameEvents.onPartCouple.Remove (this.CallbackPartCouple);
				GameEvents.onPartDie.Remove (this.CallbackPartDieWasLoaded);
				GameEvents.onPartExplode.Remove (this.CallbackPartExplode);
				GameEvents.onPartJointBreak.Remove (this.CallbackPartJointWasLoaded);

				GameEvents.onPartUndock.Remove (this.CallbackPartDieWasLoaded);
				GameEvents.onStageActivate.Remove (this.CallbackStageActivateWasLoaded);
				GameEvents.onStageSeparation.Remove (this.CallbackEventReportHappened);

				GameEvents.onVesselChange.Remove (this.CallbackOnVesselChange);
				GameEvents.onVesselOrbitClosed.Remove (this.CallbackVesselEventHappened);
				GameEvents.onVesselOrbitEscaped.Remove (this.CallbackVesselEventHappened);
				GameEvents.onVesselSOIChanged.Remove (this.CallbackSOIChanged);
			}
		}

		private void CallbackGameSceneLoadRequested (GameScenes scene)
		{
			Log.trace("CallbackGameSceneLoadRequested");
			if (AS.configuration.screenshotOnSceneChange) {
				this.newScene = true;
				this.sceneReady = false;

			}
		}

		private void CallbackLevelWasLoaded(Scene scene, LoadSceneMode mode)
        {
			Log.trace("CallbackLevelWasLoaded");
			this.sceneReady = true;
			lastSceneUpdate = Time.realtimeSinceStartup;
			sceneReadyAt = Time.realtimeSinceStartup;
		}

		private void CallbackOnVesselChange (Vessel evt)
		{
			Log.trace("CallbackOnVesselChange");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			lastSceneUpdate = Time.realtimeSinceStartup;
		}

		private void CallbackVesselEventHappened (Vessel evt)
		{
			Log.trace("CallbackVesselEventHappened");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			lastSceneUpdate = Time.realtimeSinceStartup;
		}

		private void CallbackEventReportHappened (EventReport evt)
		{
			Log.trace("CallbackEventReportHappened");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			lastSceneUpdate = Time.realtimeSinceStartup;
		}

		private void CallbackVector3dWasLoaded (Vector3d vector)
		{
			Log.trace("CallbackVector3dWasLoaded");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			//lastSceneUpdate = Time.realtimeSinceStartup;
		}

		private void CallbackPartDieWasLoaded (Part part)
		{
			Log.trace("CallbackPartDieWasLoaded");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			//lastSceneUpdate = Time.realtimeSinceStartup;
		}

		private void CallbackPartJointWasLoaded (PartJoint partjoint, float f)
		{
			Log.trace("CallbackPartJointWasLoaded");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			//lastSceneUpdate = Time.realtimeSinceStartup;
		}

		private void CallbackStageActivateWasLoaded (int i)
		{
			Log.trace("CallbackStageActivateWasLoaded");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			//lastSceneUpdate = Time.realtimeSinceStartup;
		}

		private void CallbackCrewEvaLoaded (GameEvents.FromToAction<Part, Part> action)
		{
			Log.trace("CallbackCrewEvaLoaded");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			//lastSceneUpdate = Time.realtimeSinceStartup;
		}

		private void CallbackPartCouple (GameEvents.FromToAction<Part, Part> action)
		{
			Log.trace("CallbackPartCouple");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			//lastSceneUpdate = Time.realtimeSinceStartup;
		}

		private void CallbackPartExplode (GameEvents.ExplosionReaction action)
		{
			Log.trace("CallbackPartExplode");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			//lastSceneUpdate = Time.realtimeSinceStartup;
		}

		private void CallbackSOIChanged (GameEvents.HostedFromToAction<Vessel, CelestialBody >  action)
		{
			Log.trace("CallbackSOIChanged");
			//this.newScene = true;
			//this.sceneReady = true;
			this.specialScene = true;
			//lastSceneUpdate = Time.realtimeSinceStartup;
		}

		internal void OnDestroy ()
		{
			Log.trace("destroying Automated Screenshots");
			ToolbarController.Instance.Destroy();
			configuration.Save ();
		}

		public static KeyCode setActiveKeycode (string keycode)
		{
			AS.activeKeycode = (KeyCode)Enum.Parse (typeof(KeyCode), keycode);
			if (AS.activeKeycode == KeyCode.None) {
				Log.warn("Make sure to use the list of keys to set the key! Reverting to F6");
				AS.activeKeycode = KeyCode.F6;
			}
		
			return AS.activeKeycode;
		}

		//
		// Following taken from SensibleScreenshot
		//
		private static string ConvertDateString ()
		{
			string dateFormat = "yyyy-MM-dd--HH-mm-ss";

			return DateTime.Now.ToString (dateFormat);
		}

		private static int[] ConvertUT (double UT)
		{
			double time = UT;
			int[] ret = { 0, 0, 0, 0, 0 };

			ret [0] = (int)Math.Floor (time / (KSPUtil.dateTimeFormatter.Year)) + 1; //year
			time %= (KSPUtil.dateTimeFormatter.Year);
			ret [1] = (int)Math.Floor (time / KSPUtil.dateTimeFormatter.Day) + 1; //days
			time %= (KSPUtil.dateTimeFormatter.Day);
			ret [2] = (int)Math.Floor (time / (3600)); //hours
			time %= (3600);
			ret [3] = (int)Math.Floor (time / (60)); //minutes
			time %= (60);
			ret [4] = (int)Math.Floor (time); //seconds

			return ret; 
		}

		public static string AddInfo (string original, int cnt, bool sceneReady = false, bool specialScene = false, bool precrash = false)
		{
			string f = original;
			Log.detail("AddInfo: original: {0}",  original);
			if (f.Contains (":")) {
				f = f.Replace (":", "-");
			}
			if (f.Contains ("[cnt]")) {
				Log.detail("Contains [cnt]");
				f = f.Replace ("[cnt]", cnt.ToString ());
			} else
				Log.detail("Doesn't contain [cnt]");

			if (f.Contains ("[date]")) {
				f = f.Replace ("[date]", ConvertDateString ());
			}
			if (f.Contains ("[UT]")) {
				double UT = 0;
				if (Planetarium.fetch != null)
					UT = Planetarium.GetUniversalTime ();
				f = f.Replace ("[UT]", Math.Round (UT).ToString ());
			}
			if (f.Contains ("[save]")) {
				string save = "NA";
				if (HighLogic.SaveFolder != null && HighLogic.SaveFolder.Trim ().Length > 0)
					save = HighLogic.SaveFolder;
				f = f.Replace ("[save]", save);
			}
			//if (f.Contains("[version]"))
			//{
			//	string version = Versioning.GetVersionString();
			//	f = f.Replace("[version]", version);
			//}
			if (f.Contains ("[vessel]")) {
				string vessel = "NA";
				if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null)
					vessel = FlightGlobals.ActiveVessel.vesselName;
				f = f.Replace ("[vessel]", vessel);
			}
			if (f.Contains ("[body]")) {
				string body = "NA";
				if (Planetarium.fetch != null)
					body = Planetarium.fetch.CurrentMainBody.bodyName;
				f = f.Replace ("[body]", body);
			}
			if (f.Contains ("[situation]")) {
				string sit = "NA";
				if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null) {
					sit = FlightGlobals.ActiveVessel.situation.ToString ();
				}
				f = f.Replace ("[situation]", sit);
			}
			if (f.Contains ("[biome]")) {
				string biome = "NA";
				if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null)
					biome = ScienceUtil.GetExperimentBiome (FlightGlobals.ActiveVessel.mainBody, FlightGlobals.ActiveVessel.latitude, FlightGlobals.ActiveVessel.longitude);
				f = f.Replace ("[biome]", biome);
			}
			int[] times = { 0, 0, 0, 0, 0 };
			if (Planetarium.fetch != null)
				times = ConvertUT (Planetarium.GetUniversalTime ());
			if (f.Contains ("[year]")) {
				string time = times [0].ToString ("D4");
				f = f.Replace ("[year]", time);
			}
			if (f.Contains ("[day]")) {
				string time = times [1].ToString ("D2");
				f = f.Replace ("[day]", time);
			}
			if (f.Contains ("[hour]")) {
				string time = times [2].ToString ("D2");
				//if (time.Length == 1)
				//	time = "0" + time;
				f = f.Replace ("[hour]", time);
			}
			if (f.Contains ("[min]")) {
				string time = times [3].ToString ("D2");
				//if (time.Length == 1)
				//	time = "0" + time;
				f = f.Replace ("[min]", time);
			}
			if (f.Contains ("[sec]")) {
				string time = times [4].ToString ("D2");
				//if (time.Length == 1)
				//	time = "0" + time;
				f = f.Replace ("[sec]", time);
			}

			// In case they don't have anything there
			if (f == original && cnt > 0) {
				Log.detail("f == original");
				f = f + cnt.ToString ();
			}
			if (f.Contains ("[evt]")) {
				string evt = "timed";
				if (sceneReady)
					evt = "scene";
				if (specialScene)
					evt = "event";
				if (precrash)
					evt = "precrash";

				f = f.Replace ("[evt]", evt);
			}
				
			return f;
		}
	}

}