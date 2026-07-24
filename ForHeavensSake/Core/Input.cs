using ForHeavensSake.Core.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ForHeavensSake.Core;

public static class Input
{
	public static bool JustPressed(Keys key) => KeyboardCurrent.IsKeyDown(key) && KeyboardPrev.IsKeyUp(key);
	public static bool JustClickedL => MouseCurrent.LeftButton == ButtonState.Pressed && MousePrev.LeftButton == ButtonState.Released;
	public static bool JustClickedR => MouseCurrent.RightButton == ButtonState.Pressed && MousePrev.RightButton == ButtonState.Released;
	
	public static KeyboardState KeyboardPrev = new();
	public static KeyboardState KeyboardCurrent = new();
	
	public static MouseState MousePrev = new();
	public static MouseState MouseCurrent = new();

	public static Vector2 MousePosition;

	public static void UpdateCurrent()
	{
		KeyboardCurrent = Keyboard.GetState();
		MouseCurrent = Mouse.GetState();

		MousePosition = new Vector2(MouseCurrent.X, MouseCurrent.Y);
	}

	public static void UpdatePrevious()
	{
		KeyboardPrev = KeyboardCurrent;
		MousePrev = MouseCurrent;
	}
}