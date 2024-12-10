/*
	This file is part of Automated Screenshots /L Unleashed
		© 2018-2023 Lisias T : http://lisias.net <support@lisias.net>
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
using IO = System.IO;	// To be replaced by KSPe.IO someday...

namespace AutomatedScreenshots
{
	class SaveFilesHandlers
	{
		// Since it's impossible to enter any Editor without having the SaveGame fully loaded,
		// there's no risk on doing things here on the static land.
		//
		// But don't try these on anything that could be loaded before KSP fully populate the HighLogic.fetch.GameSaveFolder!
		private static string SAVEDIR => KSPe.IO.Hierarchy.SAVE.Solve(HighLogic.SaveFolder);
		private static string SAVEFILELIST = KSPe.IO.Hierarchy.SAVE.Solve(HighLogic.SaveFolder, "saveFileList.txt");

		private const int NUMFILES_OFFSET = 0;
		private const int FILESAVECNT_OFFSET = 1;
		private const int FILENAME_OFFSET = 2;

		private const int MAX_OFFSET = 2; // This should equal the highest offset above

		ushort numSaveFiles;
		int fileSaveCnt;
		private static int saveFileCnt;

		/*
		 * BackupWork
		 */
		private void BackupWork (AS asRef)
		{
			SaveFilesHandlers sfh  = new SaveFilesHandlers ();

			// SaveMode is:  OVERWRITE    APPEND   ABORT
			SaveMode s = SaveMode.OVERWRITE;

			saveFileCnt = FileSaveCnt() + 1;
			string saveFileName = AS.AddInfo (AS.configuration.savePrefix, saveFileCnt,	asRef.isSceneReady(), asRef.isSpecialScene(), asRef.isPreCrash());

			string str = GamePersistence.SaveGame (saveFileName, HighLogic.SaveFolder, s);
			Log.dbg("String: {0}", str);

			sfh.deleteOldestSaveFile (SAVEDIR, AS.configuration.numToRotate, saveFileCnt, saveFileName);

			Log.trace("backup thread terminated");
		}

		/*
		 * startBackup
		 */
		public void startBackup (AS asRef)
		{
			asRef.backupThread = new Thread (() => BackupWork(asRef));
			asRef.backupThread.Start ();
		}


		/*
		 * FileSaveCnt
		 */
		private int FileSaveCnt()
		{

			string fname = SAVEFILELIST;

			if (!IO.File.Exists (fname)) {
				return 0;
			}

			// Open the file to read from. 

			IO.StreamReader file = new IO.StreamReader (fname);
			string line = file.ReadLine ();
			// numSaveFiles = Convert.ToUInt16 (line);
			line = file.ReadLine ();
			fileSaveCnt = Convert.ToInt32 (line);
			file.Close (); file.Dispose ();
			return fileSaveCnt;
		}

		/*
		 * 
		 */
		private string[] emptyReadText()
		{
			string[] readText = new string[2];
			readText [NUMFILES_OFFSET] = "0";
			readText [FILESAVECNT_OFFSET] = "0";
			return readText;
		}

		/*
		 * readSaveFileList
		 */
		private string[] readSaveFileList (string path)
		{
			string[] readText;

			string fname = SAVEFILELIST;
			if (!IO.File.Exists (fname)) {
				Log.warn("file does not exist: {0}", fname);
				return emptyReadText();
			}

			// Open the file to read from. 
			IO.StreamReader file = new IO.StreamReader (fname);
			string line = file.ReadLine ();
			numSaveFiles = 0;
			try {
				numSaveFiles = Convert.ToUInt16 (line);
			} catch (FormatException ) {
				file.Close (); file.Dispose ();
				return emptyReadText();
			} catch (OverflowException ) {
				file.Close (); file.Dispose ();
				return emptyReadText();
			}


			line = file.ReadLine ();
			try {
				fileSaveCnt = Convert.ToInt32 (line);
			} catch (FormatException ) {
				file.Close (); file.Dispose ();
				return emptyReadText();
			} catch (OverflowException ) {
				file.Close (); file.Dispose ();
				return emptyReadText();
			}

			Log.detail("fileSaveCnt: {0}", fileSaveCnt);

			ushort cnt = 0;
			readText = new string[numSaveFiles + MAX_OFFSET];
			readText [FILESAVECNT_OFFSET] = fileSaveCnt.ToString ();
			readText [NUMFILES_OFFSET] = numSaveFiles.ToString ();

			while (numSaveFiles > 0) {
				line = file.ReadLine ();
				if (line != null && cnt < numSaveFiles ) {
					cnt++;
					readText [cnt + MAX_OFFSET - 1] = line;
				} else {
					readText [NUMFILES_OFFSET] = cnt.ToString ();
					break;
				}
			}

			file.Close ();
			file.Dispose ();

			return readText;
		}

		/*
		 * writeSaveFileList
		 */
		void writeSaveFileList (string path, string[] writeText)
		{
			string fname = SAVEFILELIST;

			try{
				IO.File.WriteAllLines (fname, writeText);	
			}
			catch (Exception e) {
				Log.err("Exception caught after WriteAllLines: {0}", e);
			}
		}

		/*
		 * deleteOldestSaveFile
		 */
		private void deleteOldestSaveFile (string path, ushort maxSaveFiles, int cnt = -1, string newFile = "")
		{
			string[] fileList = readSaveFileList (path);
		
		//	ushort numSaveFiles = 0;
		//	if (fileList.Length > 0)
		//		numSaveFiles = Convert.ToUInt16 (fileList [NUMFILES_OFFSET]);
		
			ushort x = numSaveFiles;
			if (maxSaveFiles > x)
				x = maxSaveFiles;
			string[] newList = new string[MAX_OFFSET + x + 1];

			for (int i = FILENAME_OFFSET; i <= numSaveFiles + FILENAME_OFFSET - 1; i++) 
			{

				newList [i + 1] = fileList [i];
			}
				
			if (maxSaveFiles <= numSaveFiles) {
				newList [FILENAME_OFFSET + maxSaveFiles] = "";
				for (int i = maxSaveFiles; i <= numSaveFiles; i++) {
					//
					// This will delete all files which are greater than the maxSavefile
					// useful in case the user reduces the number after it has started
					//

					string f = path + "/" + fileList [FILENAME_OFFSET + i - 1] + ".sfs";
					if (IO.File.Exists (f)) { 
						IO.File.Delete (f);
					}
                    f = path + "/" + fileList[FILENAME_OFFSET + i - 1] + ".loadmeta";
                    if (IO.File.Exists(f))
                    {
                        IO.File.Delete(f);
                    }

                }
                numSaveFiles = maxSaveFiles;

			} else {
				if (newFile != "") {
					numSaveFiles++;
				}
			}

			if (newFile != "") {

				newList [FILENAME_OFFSET] = newFile;
			}
			newList [NUMFILES_OFFSET] = numSaveFiles.ToString ();
			if (cnt > 0) {
				fileSaveCnt = cnt;
			}
			newList [FILESAVECNT_OFFSET] = fileSaveCnt.ToString();

			writeSaveFileList (path, newList);
		}

	}
}