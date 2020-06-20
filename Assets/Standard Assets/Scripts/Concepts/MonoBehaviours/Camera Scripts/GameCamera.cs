using Extensions;

namespace GridGame
{
	public class GameCamera : CameraScript
	{
		public AnimatedBackground animatedBackground;

		public override void HandlePosition ()
		{
			// trs.position = GameManager.GetSingleton<Player>().trs.position.SetZ(trs.position.z);
			base.HandlePosition ();
		}

		public override void HandleViewSize ()
		{
			base.HandleViewSize ();
			animatedBackground.rawImage.uvRect = viewRect.MultiplySize(animatedBackground.scale);
		}
	}
}