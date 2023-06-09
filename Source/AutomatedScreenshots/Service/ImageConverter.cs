/*
	This file is part of Automated Screenshots /L Unleashed
		© 2018-2023 Lisias T : http://lisias.net <support@lisias.net>

	Automated Screenshots /L Unleashed is licensed as follows:
		* GPL 3.0 : https://www.gnu.org/licenses/gpl-3.0.txt

	Automated Screenshots /L Unleashed is distributed in the hope that
	it will be useful, but WITHOUT ANY WARRANTY; without even the implied
	warranty of	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the GNU General Public License 3.0
	along with Automated Screenshots /L Unleashed.
	If not, see <https://www.gnu.org/licenses/>.

*/
using System.Collections.Generic;
using IO = System.IO;	// To be replaced by KSPe.IO someday...
using UnityEngine;
using KSPe.Annotations;


namespace AutomatedScreenshots.Service
{
	internal class ImageConverter : MonoBehaviour
	{
		private MonoBehaviour owner;
		internal static ImageConverter Create(MonoBehaviour owner)
		{
			ImageConverter r = owner.gameObject.AddComponent<ImageConverter>();
			r.owner = owner;
			return r;
		}

		private class Job
		{
			public readonly string pngFile;
			public readonly bool keep;
			public readonly string jpgFile;
			public readonly ushort jpgQuality;

			public Job(string png, bool keep, string jpg, ushort jpgQuality)
			{
				this.pngFile = png;
				this.keep = keep;
				this.jpgFile = jpg;
				this.jpgQuality = jpgQuality;
			}

			public override bool Equals(object obj)
			{
				Log.dbg("Equals {0} {1}", this, obj);
				if (null == obj) return false;
				if (!(obj is Job)) return false;
				Job o = (Job)obj;
				return o.pngFile.Equals(this.pngFile);
			}

			public override int GetHashCode()
			{
				return this.pngFile.GetHashCode();
			}
		}

		private readonly List<Job> jobs = new List<Job>();

		internal void AddJob(string pngName, bool keepOrginalPNG, string jpgName, ushort jpgQuality)
		{
			Log.dbg("Adding Job for {0}", pngName);
			Job j = new Job(pngName, keepOrginalPNG, jpgName, jpgQuality);
			lock (this.jobs)
			{ 
				if(!this.jobs.Contains(j)) this.jobs.Add(j);
			}
			this.enabled = true;
		}

		[UsedImplicitly]
		private void Start()
		{
			Log.dbg("Start");
			this.enabled = false;
		}

		[UsedImplicitly]
		private void LateUpdate()
		{
			Log.dbg("LateUpdate");
			Job j = null;
			lock (this.jobs)
			{
				this.enabled = (0 != this.jobs.Count);
				if (!this.enabled) return;
				j = this.jobs[0];
				if(!IO.File.Exists(j.pngFile)) return; // Screenshot still processing. Try again next frame.
				this.jobs.RemoveAt(0);
			}
			process(j);
		}

		private static void process(Job job)
		{
			Log.dbg("Processing {0} {1}", job.pngFile, IO.File.Exists(job.pngFile));

			convertToJPG(job.pngFile, job.jpgFile, job.jpgQuality);
			if(job.keep) return;

			Log.dbg("Deleting {0}", job.pngFile);
			IO.FileInfo file = new IO.FileInfo(job.pngFile);
			file.Delete();
		}

		private static void convertToJPG (string originalFile, string newFile, int quality)
		{
			Log.dbg("Converting screenshot to JPG. New name: {0}", newFile);

			Texture2D png = new Texture2D (1, 1);
			byte[] pngData = IO.File.ReadAllBytes (originalFile);
			png.LoadImage (pngData);
			byte[] jpgData = png.EncodeToJPG (quality);
			var file = IO.File.Open (newFile, IO.FileMode.Create);
			var binary = new IO.BinaryWriter (file);
			binary.Write (jpgData);
			file.Close ();
			Destroy (png);
			//Resources.UnloadAsset(png);
		}

	#if DEBUG
		private static readonly KSPe.Util.Log.Logger Log = KSPe.Util.Log.Logger.CreateForType<ImageConverter>(true);
		static ImageConverter()
		{
			Log.level = KSPe.Util.Log.Level.TRACE;
		}
	#endif
	}
}
