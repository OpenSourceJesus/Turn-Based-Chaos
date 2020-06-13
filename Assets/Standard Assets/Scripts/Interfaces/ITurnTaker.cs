public interface ITurnTaker
{
	int Order { get; set; }
	void TakeTurn ();
	float TurnReloadRate { get; set; }
	float TurnCooldown { get; set; }
}