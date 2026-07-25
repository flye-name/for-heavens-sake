using Microsoft.Xna.Framework.Input;
using TowerToHeaven.Core;

namespace TowerToHeaven;

public partial class FHS
{
	public static int ModeSwitchingTimer = 10 * 60;
	public static float ModeSwitchingProgress => ModeSwitchingTimer / (float)(Frozen ? TimeBeforeGameplay * 60 : TimeBeforePlacement * 60);
	public static float TimeBeforePlacement = 15f;
	public static float TimeBeforeGameplay = 10f;
	public static bool Frozen;

	public static void UpdateModes()
	{
		ModeSwitchingTimer++;
		if (ModeSwitchingTimer > (Frozen ? TimeBeforeGameplay * 60 : TimeBeforePlacement * 60))
		{
			Frozen = !Frozen;
			ModeSwitchingTimer = 0;

			Assets.Sounds.Blip?.Play();
		}

		if (Frozen)
		{
			var up = Input.KeyboardCurrent.IsKeyDown(Keys.W) || Input.KeyboardCurrent.IsKeyDown(Keys.Up);
			var down = Input.KeyboardCurrent.IsKeyDown(Keys.S) || Input.KeyboardCurrent.IsKeyDown(Keys.Down);
			
			if (Input.MousePosition.Y < ScreenSize.Y * 0.1f || up)
				ScreenPosition.Y -= 7;
			
			if (Input.MousePosition.Y > ScreenSize.Y * 0.9f || down)
				ScreenPosition.Y += 7;
		}
	}
}