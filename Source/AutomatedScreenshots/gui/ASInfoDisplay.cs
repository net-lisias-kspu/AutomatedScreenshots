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
