using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TowerToHeaven.Core.UI;

public static class MainMenu
{
	public static int SplashTime;
	public static bool SplashOver => SplashTime > 600;
	public static float VisualProgress => MathHelper.Clamp((SplashTime - 280) / 150f, 0, 1);
	public static float ChoppyProgress => MathF.Floor(VisualProgress * 12) / 12f;
	
	public static void UpdateSplash()
	{
		SplashTime++;
		
		if (SplashTime is > 40 and < 430 && Input.KeyboardCurrent.IsKeyDown(Keys.Escape))
		{
			SplashTime = 430;
		}
		
		if (SplashTime == 20)
			Assets.Sounds.Cartridge?.Play(1, 1.2f, 0);
		
		var oldProgress = MathHelper.Clamp((SplashTime - 281) / 150f, 0, 1);
		var oldChoppyProgress = MathF.Floor(oldProgress * 12) / 12f;
		
		if (oldChoppyProgress < ChoppyProgress)
			Assets.Sounds.Blip?.Play(1, -0.2f, 0);
		
		if (SplashTime is > 430 and < 530 && SplashTime % 30 == 0)
			Assets.Sounds.MenuSelect?.Play();
	}
	
	public static void HandleInput()
	{
		if (Input.KeyboardCurrent.GetPressedKeys().Length > 0)
		{
			if (SplashOver)
			{
				FHS.InGame = true;
				Assets.Sounds.MenuSelect?.Play(1, 0.2f, 0);
			}
		}
	}
	
	public static void Draw()
	{
		var texture = Assets.Textures.Atlas;
		if (!SplashOver)
		{
			if (SplashTime > 515 || (SplashTime is > 445 and < 500 && SplashTime % 30 < 15))
				return;
			
			var progress = MathHelper.Clamp((SplashTime - 100) / 250f, 0, 1);
			var width = 630;
			var frame = new Rectangle((int)(width / 2f * (1f - ChoppyProgress)), 68, (int)(width * ChoppyProgress), 500);
			FHS.SpriteBatch.Draw(texture, FHS.ScreenSize / 2f, frame, Color.White with { A = 0 }, 0, new Vector2(frame.Width, frame.Height) / 2f, 1.5f, SpriteEffects.None,0);
			return;
		}
		
		SpriteFontBase font = FHS.FontSystem.GetFont(55);
		var text = "WASD/Arrow keys to move, SPACE to jump, hold ESC to exit.";
		FHS.SpriteBatch.DrawString(font, text, FHS.ScreenSize * new Vector2(0.5f, 0.25f), Color.White, 0, font.MeasureString(text) / 2f);
		
		text = "Left Click to place tiles, Right Click to remove them.";
		FHS.SpriteBatch.DrawString(font, text, FHS.ScreenSize * new Vector2(0.5f, 0.25f) + new Vector2(0, font.MeasureString(text).Y * 1.5f), Color.White, 0, font.MeasureString(text) / 2f);

		text = "Press any key to start!";
		FHS.SpriteBatch.DrawString(font, text, FHS.ScreenSize * new Vector2(0.5f, 0.75f), Color.White, 0, font.MeasureString(text) / 2f);
	}
}