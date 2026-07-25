using ForHeavensSake.Core;
using ForHeavensSake.Core.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForHeavensSake;

public partial class FHS
{
	public static GraphicsDeviceManager Graphics;
	public static SpriteBatch SpriteBatch;
	public static Vector2 ScreenSize;
	public static Vector2 ScreenPosition;

	public static RenderTarget2D MainRender;
	public static RenderTarget2D PixelRender;
	public static RenderTarget2D FinalRender;
	
	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.SetRenderTarget(MainRender);
		GraphicsDevice.Clear(Color.CornflowerBlue * 0.05f);
		Capture();
		
		GraphicsDevice.SetRenderTarget(PixelRender);
		GraphicsDevice.Clear(Color.Transparent);
		SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
		SpriteBatch.Draw(MainRender, new Rectangle(2, 0, (int)ScreenSize.X / 2, (int)ScreenSize.Y / 2), Color.Red with { A = 0});
		SpriteBatch.Draw(MainRender, new Rectangle(-2, 0, (int)ScreenSize.X / 2, (int)ScreenSize.Y / 2), Color.Blue with { A = 0});
		SpriteBatch.Draw(MainRender, new Rectangle(0, 0, (int)ScreenSize.X / 2, (int)ScreenSize.Y / 2), Color.White with { A = 0 });
		SpriteBatch.End();
		
		GraphicsDevice.SetRenderTarget(FinalRender);
		GraphicsDevice.Clear(Color.Transparent);

		SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
		SpriteBatch.Draw(PixelRender, new Rectangle(0, 0, (int)ScreenSize.X * 2, (int)ScreenSize.Y * 2), Color.White);
		SpriteBatch.End();
		
		GraphicsDevice.SetRenderTarget(null);
		GraphicsDevice.Clear(Color.Transparent);
		SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, Assets.Effects.CRTBarrelFilter, Matrix.Identity);
		SpriteBatch.Draw(FinalRender, new Rectangle(0, 0, (int)ScreenSize.X, (int)ScreenSize.Y), Color.White);
		SpriteBatch.End();
		
		base.Draw(gameTime);
	}

	private static void Capture()
	{
		SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

		if (!InGame)
		{
			MainMenu.Draw();
		}
		else
		{
			Player.Draw();
			Tiles.Draw();
            ParticleSystem.Draw();
            HUD.Draw();
        }
		

		DrawCursor();
		
		var rand = new Random(new Random(AmbientTimer).Next(int.MaxValue));
		SpriteBatch.Draw(Assets.Textures.Noise, Vector2.Zero, new Rectangle(rand.Next((int)ScreenSize.X), rand.Next((int)ScreenSize.Y), (int)ScreenSize.X, (int)ScreenSize.Y), Color.White with { A = 0 } * 0.03f);
		
		SpriteBatch.End();
	}

	public static void DrawCursor()
	{
		SpriteBatch.Draw(Assets.Textures.Placeholder, Input.MousePosition, null, Color.White, 0, Vector2.Zero, .2f, SpriteEffects.None, 0);

		var pos = Input.MousePosition + ScreenPosition;
		var y = (int)MathF.Floor(GroundLevel - pos.Y / TileSize) + 1;
		var x = (int)(pos.X / TileSize);
		pos = new Vector2(x, GroundLevel - y) * TileSize - ScreenPosition;
		var opacity = MathF.Abs(MathF.Sin(AmbientTimer * 0.05f)) * 0.5f + 0.5f;
		
		SpriteBatch.Draw(Assets.Textures.Placeholder, pos, null, Color.White * opacity, 0, Vector2.Zero, new Vector2(0.1f, 2), SpriteEffects.None, 0);
		SpriteBatch.Draw(Assets.Textures.Placeholder, pos + new Vector2(Assets.Textures.Placeholder.Width * 2, 0), null, Color.White * opacity, 0, Vector2.Zero, new Vector2(0.1f, 2), SpriteEffects.None, 0);
		SpriteBatch.Draw(Assets.Textures.Placeholder, pos, null, Color.White * opacity, 0, Vector2.Zero, new Vector2(2, 0.1f), SpriteEffects.None, 0);
		SpriteBatch.Draw(Assets.Textures.Placeholder, pos + new Vector2(0, Assets.Textures.Placeholder.Height * 2), null, Color.White * opacity, 0, Vector2.Zero, new Vector2(2, 0.1f), SpriteEffects.None, 0);
	}
}