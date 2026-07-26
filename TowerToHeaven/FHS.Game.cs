using TowerToHeaven.Core;
using TowerToHeaven.Core.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TowerToHeaven;

public partial class FHS
{
	public static bool InGame;
	public static int DelayBeforeMovement;
	public static float FadeIn;
	public static int EscapeCounter;
	public static bool CanMove => DelayBeforeMovement > 30;
	public static Player Player = new();
	public const int GroundLevel = 40;
	public const int TileSize = 64;
	public static int AmbientTimer;
	
	protected override void Update(GameTime gameTime)
	{
		Assets.Sounds.CRT?.Play(0.5f, 0, 0);
		Assets.Sounds.CRT?.Instances[0].Volume = FadeIn * 0.5f;
		
		AmbientTimer++;

		FadeIn = MathF.Min(FadeIn + 0.01f, 1f);
		
		Input.UpdateCurrent();

		if (!InGame)
		{
			MainMenu.UpdateSplash();
			MainMenu.HandleInput();
		}
		else
		{
			if (!CanMove)
				DelayBeforeMovement++;
			
			UpdateModes();
			
			Player.Update();
            ParticleSystem.Update();
            ProjectileManager.Update();
            Tiles.Update();
        }

		if (Input.KeyboardCurrent.IsKeyDown(Keys.Escape) && InGame)
		{
			if (EscapeCounter++ > 60)
				Instance.Exit();
		}
		else EscapeCounter = 0;

		Input.UpdatePrevious();
		
		base.Update(gameTime);
	}
}