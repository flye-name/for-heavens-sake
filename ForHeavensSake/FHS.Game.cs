using ForHeavensSake.Core;
using ForHeavensSake.Core.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ForHeavensSake;

public partial class FHS
{
	public static bool InGame;
	public static Player Player = new();
	public const int GroundLevel = 40;
	public const int TileSize = 32;
	
	protected override void Update(GameTime gameTime)
	{
		Input.UpdateCurrent();

		if (!InGame)
		{
			MainMenu.HandleInput();
		}
		else
		{
			Player.Update();
		}

		if (Input.JustPressed(Keys.Escape))
		{
			Saving.Save();
			Instance.Exit();
		}

		Input.UpdatePrevious();
		
		base.Update(gameTime);
	}
}