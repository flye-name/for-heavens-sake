using ForHeavensSake.Core.UI;
using Microsoft.Xna.Framework.Input;

namespace ForHeavensSake.Core;

public static class Input
{
	public static bool JustPressed(Keys key) => KeyboardCurrent.IsKeyDown(key) && KeyboardPrev.IsKeyUp(key);
	
	public static KeyboardState KeyboardPrev = new();
	public static KeyboardState KeyboardCurrent = new();
	
	public static MouseState MousePrev = new();
	public static MouseState MouseCurrent = new();

	public static void Update()
	{
		KeyboardCurrent = Keyboard.GetState();
		MouseCurrent = Mouse.GetState();
			
		if (JustPressed(Keys.Escape))
		{
			Saving.Save();
			FHS.Instance.Exit();
		}

		if (!FHS.InGame)
			MainMenu.HandleInput();
		else
			FHS.Player.HandleInput();
		
		KeyboardPrev = KeyboardCurrent;
		MousePrev = MouseCurrent;
	}
}