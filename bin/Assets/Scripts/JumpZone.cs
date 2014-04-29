namespace custTypes {
	public class JumpZone {
		float x, y, width, height;
		bool onLeft;

		public JumpZone(float x, float y, float width, float height, bool onLeft) {
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
			this.onLeft = onLeft;
		}
	}
}