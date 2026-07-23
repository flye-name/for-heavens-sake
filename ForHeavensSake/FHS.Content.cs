using FontStashSharp;
using ForHeavensSake.Core;
using Microsoft.Xna.Framework.Graphics;

namespace ForHeavensSake;

public partial class FHS
{
	public static FontSystem FontSystem;
	
	protected override void LoadContent()
	{
		SpriteBatch = new SpriteBatch(GraphicsDevice);
		FontSystem = new();
		
		Assets.Load();
		
		base.LoadContent();
	}

	protected override void UnloadContent()
	{
		SpriteBatch.Dispose();
		Assets.Unload();
		
		base.UnloadContent();
	}
}