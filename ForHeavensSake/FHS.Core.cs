using ForHeavensSake.Core.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForHeavensSake;

public partial class FHS : Game
{
	public static FHS Instance;
	
	static void Main(string[] args)
	{
		using FHS g = new();
		g.Run();
	}
	
	private FHS()
	{
		Instance = this;
		Graphics = new GraphicsDeviceManager(this);
		
		Graphics.PreferredBackBufferWidth = 1920;
		Graphics.PreferredBackBufferHeight = 1080;
		Graphics.IsFullScreen = true;
		Graphics.SynchronizeWithVerticalRetrace = true;
		
		Content.RootDirectory = "Content";
	}
	
	protected override void Initialize()
	{
		var display = Graphics.GraphicsDevice.Viewport.Bounds;
		ScreenSize = new Vector2(display.Width, display.Height);

		Player.Position = ScreenSize / 2f;
			
		base.Initialize();
	}
}