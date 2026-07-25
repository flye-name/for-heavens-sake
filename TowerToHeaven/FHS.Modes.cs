using TowerToHeaven.Core;

namespace TowerToHeaven;

public partial class FHS
{
	public static int ModeSwitchingTimer;
	public static float ModeSwitchingProgress => ModeSwitchingTimer / (float)(Frozen ? TimeBeforeGameplay * 60 : TimeBeforePlacement * 60);
	public static float TimeBeforePlacement = 10f;
	public static float TimeBeforeGameplay = 7.5f;
	public static bool Frozen;

	public static void UpdateModes()
	{
		ModeSwitchingTimer++;
		if (ModeSwitchingTimer > (Frozen ? TimeBeforeGameplay * 60 : TimeBeforePlacement * 60))
		{
			Frozen = !Frozen;
			ModeSwitchingTimer = 0;

			Assets.Sounds.Blip.Play();
		}
	}
}