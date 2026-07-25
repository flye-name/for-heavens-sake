using FontStashSharp;
using TowerToHeaven.Core;
using Microsoft.Xna.Framework.Graphics;

namespace TowerToHeaven;

public partial class FHS
{
	public static FontSystem FontSystem;
	
	protected override void LoadContent()
	{
		SpriteBatch = new SpriteBatch(GraphicsDevice);
		MainRender = new RenderTarget2D(GraphicsDevice, (int)ScreenSize.X, (int)ScreenSize.Y);
		PixelRender = new RenderTarget2D(GraphicsDevice, (int)ScreenSize.X, (int)ScreenSize.Y);
		FinalRender = new RenderTarget2D(GraphicsDevice, (int)ScreenSize.X, (int)ScreenSize.Y);
		
		Assets.Load();
		
		base.LoadContent();
	}

	protected override void UnloadContent()
	{
		SpriteBatch.Dispose();
		MainRender.Dispose();
		PixelRender.Dispose();
		FinalRender.Dispose();
		Assets.Unload();
		
		base.UnloadContent();
	}
}