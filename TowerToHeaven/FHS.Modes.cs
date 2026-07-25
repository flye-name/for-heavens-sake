using Microsoft.Xna.Framework.Input;
using TowerToHeaven.Core;

namespace TowerToHeaven;

public partial class FHS
{
	public static int ModeSwitchingTimer = 10 * 60;
	public static float ModeSwitchingProgress => ModeSwitchingTimer / (Frozen ? TimeBeforeGameplay * 60 : TimeBeforePlacement * 60);
	public static float TimeBeforePlacement = 15f;
	public static float TimeBeforeGameplay = 10f;
	public static bool Frozen;

	public static void UpdateModes()
	{
		ModeSwitchingTimer++;
		var maxTime = (Frozen ? TimeBeforeGameplay * 60 : TimeBeforePlacement * 60);
		var oldProgress = (ModeSwitchingTimer - 1) / (Frozen ? TimeBeforeGameplay * 60 : TimeBeforePlacement * 60);
		if (MathF.Floor(oldProgress * 25) / 25f < MathF.Floor(ModeSwitchingProgress * 25) / 25f && ModeSwitchingTimer > maxTime - 300)
		{
			Assets.Sounds.Blip?.Play(0.4f, (Frozen ? -0.7f : -0.5f) + ModeSwitchingProgress * 0.3f, 0);
		}
		
		if (ModeSwitchingTimer > maxTime)
		{
			Frozen = !Frozen;
			ModeSwitchingTimer = 0;

			Assets.Sounds.Blip?.Play();
		}

		if (Frozen)
		{
			Player.DisappointmentDelay = 0;
			
			var up = Input.KeyboardCurrent.IsKeyDown(Keys.W) || Input.KeyboardCurrent.IsKeyDown(Keys.Up);
			var down = Input.KeyboardCurrent.IsKeyDown(Keys.S) || Input.KeyboardCurrent.IsKeyDown(Keys.Down);
			
			if (Input.MousePosition.Y < ScreenSize.Y * 0.1f || up)
				ScreenPosition.Y -= 7;
			
			if ((Input.MousePosition.Y > ScreenSize.Y * 0.9f || down) && ScreenPosition.Y + ScreenSize.Y < GroundLevel * TileSize)
				ScreenPosition.Y += 7;
		}
	}
}