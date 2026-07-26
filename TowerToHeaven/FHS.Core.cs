using TowerToHeaven.Core.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TowerToHeaven.Core;

namespace TowerToHeaven;

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

		Player.Position = new Vector2(ScreenSize.X / 2f, GroundLevel * TileSize);
		ScreenPosition = new Vector2(ScreenPosition.X, GroundLevel * TileSize);
		Player.SpawnPosition = Player.Position;

		Window.Title = "Tower to Heaven";
		
		Tiles.Init();
			
		base.Initialize();
	}
}