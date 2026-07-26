using FontStashSharp;
using Microsoft.Xna.Framework;

namespace TowerToHeaven.Core.UI;

public static class HUD
{
	public static void Draw()
	{
		SpriteFontBase font = FHS.FontSystem.GetFont(44);
		var text = "BEST HEIGHT: " + FHS.Player.BestHeight / FHS.TileSize;
		var size = font.MeasureString(text);
		FHS.SpriteBatch.Draw(Assets.Textures.Noise, new Rectangle(250, 0, (int)size.X + (int)(FHS.ScreenSize.X * 0.025f) + 16, (int)size.Y * 3), Color.Black);
		
		FHS.SpriteBatch.Draw(Assets.Textures.Noise, new Rectangle(250, (int)(size.Y * 3), (int)((FHS.ScreenSize.X - 500) * (1f - MathF.Floor(FHS.ModeSwitchingProgress * 25) / 25f)), (int)(FHS.ScreenSize.Y * 0.005f)), new Rectangle(0, 0, 1, 1), Color.White);

		text = "HEIGHT: " + (FHS.GroundLevel * FHS.TileSize - (int)FHS.Player.Position.Y) / FHS.TileSize;
		FHS.SpriteBatch.DrawString(font, text, FHS.ScreenSize * new Vector2(0, 0.025f) + new Vector2(250, 0), Color.White, 0, font.MeasureString(text) / 2f * new Vector2(0, 1));
		
		Color color = FHS.Player.NewBestDelay > 0 && FHS.AmbientTimer % 10 < 5 ? Color.Transparent : Color.White;
		text = "BEST HEIGHT: " + FHS.Player.BestHeight / FHS.TileSize;
		FHS.SpriteBatch.DrawString(font, text, FHS.ScreenSize * new Vector2(0, 0.025f) + new Vector2(250, font.MeasureString(text).Y), color, 0, font.MeasureString(text) / 2f * new Vector2(0, 1));

		if (FHS.Frozen)
		{
			text = "V";
			FHS.SpriteBatch.DrawString(font, text, FHS.ScreenSize * new Vector2(0.5f, 0.2f - ((FHS.AmbientTimer * 0.025f) % 1f) * 0.025f), Color.White,  MathF.PI, font.MeasureString(text) / 2f);
			
			if (FHS.ScreenPosition.Y + FHS.ScreenSize.Y < FHS.GroundLevel * FHS.TileSize)
				FHS.SpriteBatch.DrawString(font, text, FHS.ScreenSize * new Vector2(0.5f, 0.9f + ((FHS.AmbientTimer * 0.025f) % 1f) * 0.025f), Color.White,  0, font.MeasureString(text) / 2f);
		}
	}
}