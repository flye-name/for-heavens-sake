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
	
	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.SetRenderTarget(MainRender);
		GraphicsDevice.Clear(Color.Transparent);
		Capture();
		
		GraphicsDevice.SetRenderTarget(null);
		GraphicsDevice.Clear(Color.Transparent);
		SpriteBatch.Begin();
		SpriteBatch.Draw(MainRender, new Vector2(2, 0), Color.Red with { A = 0});
		SpriteBatch.Draw(MainRender, new Vector2(-2, 0), Color.Blue with { A = 0});
		SpriteBatch.Draw(MainRender, Vector2.Zero, Color.White with { A = 0 });
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
			Tiles.Draw();
			Player.Draw();
		}
		
		SpriteBatch.Draw(Assets.Textures.Placeholder, Input.MousePosition, null, Color.White, 0, Vector2.Zero, .2f, SpriteEffects.None, 0);
		
		SpriteBatch.End();
	}
}