using FontStashSharp;
using Microsoft.Xna.Framework;

namespace ForHeavensSake.Core.UI;

public static class HUD
{
	public static void Draw()
	{
		SpriteFontBase font = FHS.FontSystem.GetFont(44);
		string text = "HEIGHT: " + (FHS.GroundLevel * FHS.TileSize - (int)FHS.Player.Position.Y) / FHS.TileSize;
		FHS.SpriteBatch.DrawString(font, text, FHS.ScreenSize * new Vector2(0.025f), Color.White, 0, font.MeasureString(text) / 2f * new Vector2(0, 1));
		
		Color color = FHS.Player.NewBestDelay > 0 && FHS.AmbientTimer % 10 < 5 ? Color.Transparent : Color.White;
		text = "BEST HEIGHT: " + FHS.Player.BestHeight / FHS.TileSize;
		FHS.SpriteBatch.DrawString(font, text, FHS.ScreenSize * new Vector2(0.025f) + new Vector2(0, font.MeasureString(text).Y), color, 0, font.MeasureString(text) / 2f * new Vector2(0, 1));
	}
}