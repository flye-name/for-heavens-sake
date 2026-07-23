using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForHeavensSake.Core.UI;

public static class MainMenu
{
	public static void HandleInput()
	{
		if (Input.KeyboardCurrent.GetPressedKeys().Length > 0)
			FHS.InGame = true;
	}
	
	public static void Draw()
	{
		SpriteFontBase font = FHS.FontSystem.GetFont(70);
		var text = "WASD/Arrow keys to move, SPACE to jump, ESC to exit";
		FHS.SpriteBatch.DrawString(font, text, FHS.ScreenSize * new Vector2(0.5f, 0.25f), Color.White, 0, font.MeasureString(text) / 2f);
		
		text = "Press any key to start!";
		FHS.SpriteBatch.DrawString(font, text, FHS.ScreenSize * new Vector2(0.5f, 0.75f), Color.White, 0, font.MeasureString(text) / 2f);
	}
}