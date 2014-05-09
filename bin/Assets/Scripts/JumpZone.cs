namespace custTypes {
	public class JumpZone {
		public float x, y, width, height;
		public bool onLeft;

		public JumpZone(float x, float y, float width, float height, bool onLeft) {
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
			this.onLeft = onLeft;
		}
	}
}