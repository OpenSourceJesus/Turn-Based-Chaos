using Extensions;

namespace GridGame
{
	public class GameCamera : CameraScript
	{
		public override void HandlePosition ()
		{
			// trs.position = GameManager.GetSingleton<Player>().trs.position.SetZ(trs.position.z);
			base.HandlePosition ();
		}
	}
}