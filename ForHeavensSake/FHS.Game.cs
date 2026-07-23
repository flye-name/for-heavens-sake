using ForHeavensSake.Core;
using ForHeavensSake.Core.UI;
using Microsoft.Xna.Framework;
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
		Input.Update();
		
		if (!InGame)
			return;
		
		for (int i = 0; i < 3; i++)
			Player.UpdateMovement();
		
		Player.Update();
		
		base.Update(gameTime);
	}
}