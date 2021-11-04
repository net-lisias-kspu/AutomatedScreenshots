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
using UnityEngine;
using KSP.IO;

namespace AutomatedScreenshots
{
	public class UICLASS: MonoBehaviour
	{
		private bool uiVisible = true;

		public UICLASS ()
		{
			Log.trace("New instance of UICLASS: UICLASS constructor");

		}
		public void Start ()
		{
	
			Log.trace("UICLASS: Start");
			DontDestroyOnLoad (this);
		}

		public void Awake ()
		{
			Log.trace("UICLASS Awake");
			GameEvents.onShowUI.Add(onShowUI);
			GameEvents.onHideUI.Add(onHideUI);
		}

		private void onShowUI ()
		{
			Log.trace("UICLASS onShowUI");
			uiVisible = true;
		}

		private void onHideUI ()
		{
			Log.trace("UICLASS onHideUI");
			uiVisible = false;
		}

		public void OnDestroy ()
		{
			GameEvents.onShowUI.Remove(onShowUI);
			GameEvents.onHideUI.Remove(onHideUI);
		}

		public bool isVisible()
		{
			return uiVisible;
		}

		public void setVisible(bool b)
		{
			uiVisible = b;
		}
	}
}

