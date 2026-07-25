using TowerToHeaven.Core;
using TowerToHeaven.Core.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TowerToHeaven;

public partial class FHS
{
	public static bool InGame;
	public static Player Player = new();
	public const int GroundLevel = 40;
	public const int TileSize = 64;
	public static int AmbientTimer;
	
	protected override void Update(GameTime gameTime)
	{
		
		AmbientTimer++;
		
		Input.UpdateCurrent();

		if (!InGame)
		{
			MainMenu.HandleInput();
		}
		else
		{
			UpdateModes();
			
			Player.Update();
            ParticleSystem.Update();
        }

		if (Input.JustPressed(Keys.Escape))
		{
			Instance.Exit();
		}

		Input.UpdatePrevious();
		
		base.Update(gameTime);
	}
}