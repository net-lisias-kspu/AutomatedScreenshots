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
using System.Reflection;

using UnityEngine;

namespace AutomatedScreenshots
{
	internal class HistorianSupport
	{
		private const string HISTORIAN_ASM = "Historian";
		private const string HISTORIAN_TYPE = "KSEA.Historian.Historian";
		private static HistorianSupport instance = null;
		internal static HistorianSupport Instance = instance ?? (instance = new HistorianSupport());

		private Type historian = null;
		private bool available => null == this.historian;

		private HistorianSupport()
		{
			if (KSPe.Util.SystemTools.Assembly.Finder.ExistsByName(HISTORIAN_ASM)
				&& KSPe.Util.SystemTools.TypeFinder.ExistsByQualifiedName(HISTORIAN_TYPE)
			)
				this.historian = KSPe.Util.SystemTools.TypeFinder.FindByQualifiedName(HISTORIAN_TYPE);
			Log.info("{0} was {1}found.", HISTORIAN_TYPE, (null == this.historian? "not " : ""));
		}

		internal bool set_m_Active()
		{
			if (!this.available) return false;
			try
			{
				MethodInfo myMethod = this.historian.GetMethod("set_m_Active", BindingFlags.Instance | BindingFlags.Public);
				MonoBehaviour HistorianRef = (MonoBehaviour)UnityEngine.Object.FindObjectOfType(this.historian); //assumes only one instance of class Historian exists as this command returns first instance found, also must inherit MonoBehavior for this command to work. Getting a reference to your Historian object another way would work also.
				myMethod.Invoke(HistorianRef, null);
				return true;
			}
			catch (Exception e)
			{
				Log.err("Error calling type: {0}", HISTORIAN_TYPE);
				Log.ex(this, e);
				this.historian = null;
				return false;
			}
		}
	}
}
