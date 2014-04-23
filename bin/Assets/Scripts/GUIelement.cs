using UnityEngine;
using System.Collections;

namespace HUD {
	public class GUIelement {
		public int scale = 1;
		public Color color = new Color(0, 0, 0);
		public Rect rectangle = new Rect(0, 0, 100, 100);
		public int displayTime = -1;
		public GameObject relatesTo = null;

		public string text = "NULL";
		public Texture image = null;
		public Vector4 lineDef = new Vector4(0,0,0,0);

		public enum ElementType {MESSAGE, OTHERTEXT, SCORE, HEALTH, LIVES, POSITIONINDICATOR, LINE}
		public ElementType type = ElementType.OTHERTEXT;

		public GUIelement (ElementType type, Rect rectangle) {
			this.rectangle = rectangle;
			this.type = type;

			switch (this.type) {
				case ElementType.MESSAGE :

				break;
				case ElementType.OTHERTEXT :

				break;
				case ElementType.SCORE :

				break;
				case ElementType.HEALTH :

				break;
				case ElementType.LIVES :

				break;
				case ElementType.POSITIONINDICATOR :

				break;
				case ElementType.LINE :
					lineDef.x = rectangle.x;
					lineDef.y = rectangle.y;
					lineDef.z = rectangle.width;
					lineDef.w = rectangle.height;
					displayTime = 1;
				break;
			}
		}

		public GUIelement (ElementType type, Rect rectangle, GameObject relatesTo) {
			this.rectangle = rectangle;
			this.type = type;
			this.relatesTo = relatesTo;

			switch (this.type) {
				case ElementType.MESSAGE :

				break;
				case ElementType.OTHERTEXT :

				break;
				case ElementType.SCORE :

				break;
				case ElementType.HEALTH :

				break;
				case ElementType.LIVES :

				break;
				case ElementType.POSITIONINDICATOR :

				break;
				case ElementType.LINE :
					lineDef.x = rectangle.x;
					lineDef.y = rectangle.y;
					lineDef.z = rectangle.width;
					lineDef.w = rectangle.height;
				break;
			}
		}
	}
}