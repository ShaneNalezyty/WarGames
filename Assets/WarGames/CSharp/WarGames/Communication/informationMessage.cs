using UnityEngine;
using System.Collections;

namespace WarGames.Communication {
	/// <summary>
	/// InformationMessage contains an Information Package.
	/// </summary>
	/// <seealso cref="Messageable" />
	/// <seealso cref="Communication.InformationPackage"/>
	public class InformationMessage : Messageable {
        private InformationPackage informationPackage;
        public InformationMessage(InformationPackage package) {
            informationPackage = package;
        }

		public InformationPackage InformationPackage {
			get {
				return informationPackage;
			}
		}
	}
}