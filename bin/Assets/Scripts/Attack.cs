using UnityEngine;
using System.Collections;

namespace MyPlayer {
	public class Attack {
		private int warmup = 0;
		private int cooldown = 60;

		private int damage = 10;

		private GameObject parent;

		public Attack (GameObject parent) {
			this.parent = parent;
		}

		public bool countdown() {
			if (warmup == 0) {
				performAttack();
			}
			if (warmup <= 0) {
				cooldown--;
				if (cooldown <= 0) {
					return true;
				}
			}
			warmup--;
			return false;
		}

		private void performAttack() {
			if (parent.GetComponent<CharacterController>().attackLanded() != null && parent.GetComponent<CharacterController>().attackLanded().GetComponent<CharacterController>() != null) {
				parent.GetComponent<CharacterController>().attackLanded().GetComponent<CharacterController>().hurt(damage, parent.transform.position - parent.GetComponent<CharacterController>().attackLanded().transform.position);
			}
		}
	}
}