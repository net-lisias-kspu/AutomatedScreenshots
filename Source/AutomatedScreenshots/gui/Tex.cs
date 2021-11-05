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
using UnityEngine;
using Asset = KSPe.IO.Asset<AutomatedScreenshots.Startup>;
namespace AutomatedScreenshots
{
	internal static class Tex
	{
		private const string DIR = "Textures";

		private static Texture2D _auto36 = null;
		internal static Texture2D auto36 = _auto36 ?? (_auto36 = Asset.Texture2D.LoadFromFile(DIR, "Auto-38"));

		private static Texture2D _auto24 = null;
		internal static Texture2D auto24 = _auto24 ?? (_auto24 = Asset.Texture2D.LoadFromFile(DIR, "Auto-24"));

		private static Texture2D _autoNegative36 = null;
		internal static Texture2D autoNegative36 = _autoNegative36 ?? (_autoNegative36 = Asset.Texture2D.LoadFromFile(DIR, "Auto-negative-38"));

		private static Texture2D _autoNegative24 = null;
		internal static Texture2D autoNegative24 = _autoNegative24 ?? (_autoNegative24 = Asset.Texture2D.LoadFromFile(DIR, "Auto-negative-24"));

		private static Texture2D _autoSnapshot36 = null;
		internal static Texture2D autoSnapshot36 = _autoSnapshot36 ?? (_autoSnapshot36 = Asset.Texture2D.LoadFromFile(DIR, "Auto-snapshot-38"));

		private static Texture2D _autoSnapshot24 = null;
		internal static Texture2D autoSnapshot24 = _autoSnapshot24 ?? (_autoSnapshot24 = Asset.Texture2D.LoadFromFile(DIR, "Auto-snapshot-24"));

		private static Texture2D _autoSave36 = null;
		internal static Texture2D autoSave36 = _autoSave36 ?? (_autoSave36 = Asset.Texture2D.LoadFromFile(DIR, "Auto-save-38"));

		private static Texture2D _autoSave24 = null;
		internal static Texture2D autoSave24 = _autoSave24 ?? (_autoSave24 = Asset.Texture2D.LoadFromFile(DIR, "Auto-save-24"));

		private static Texture2D _autoSnapshotSave36 = null;
		internal static Texture2D autoSnapshotSave36 = _autoSnapshotSave36 ?? (_autoSnapshotSave36 = Asset.Texture2D.LoadFromFile(DIR, "Auto-snapshot-save-38"));

		private static Texture2D _autoSnapshotSave24 = null;
		internal static Texture2D autoSnapshotSave24 = _autoSnapshotSave24 ?? (_autoSnapshotSave24 = Asset.Texture2D.LoadFromFile(DIR, "Auto-snapshot-save-24"));

	}
}
