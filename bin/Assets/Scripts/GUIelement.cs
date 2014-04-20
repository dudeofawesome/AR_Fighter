using UnityEngine;
using System.Collections;

namespace HUD {
	public class GUIelement {
		public int scale = 1;
		public Color color = new Color(0,0,0);
		public Vector2 position = new Vector2(0,0);

		public enum ElementType {MESSAGE, OTHERTEXT, SCORE, HEALTH, LIVES, POSITIONINDICATOR, DEATHINDICATOR}

		public GUIelement (Vector2 position) {
			this.position = position;
		}
	}
}