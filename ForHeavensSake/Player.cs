using System.ComponentModel.Design;
using ForHeavensSake.Core;
using ForHeavensSake.Core.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ForHeavensSake;

public class Player
{
	public Vector2 Position;
	public readonly Vector2 Size = new Vector2(64, 64);
	public Rectangle Bounds => new Rectangle((int)Position.X - (int)Size.X / 2, (int)Position.Y - (int)Size.Y / 2, (int)Size.X, (int)Size.Y);
	public Rectangle FloorBounds => new Rectangle((int)Position.X - (int)Size.X / 2 - 16, (int)Position.Y + (int)Size.Y / 2 - 5, (int)Size.X + 32, 10);
	public Vector2 Velocity;
	public int VisualDirection = 1;
	public const float Speed = 5f;

	public bool Grounded()
	{
		var grounded = false;

		if (Position.Y > FHS.GroundLevel * FHS.TileSize)
			grounded = true;
		
		return grounded;
	}
	
	public void HandleInput()
	{
		var jump = Input.KeyboardCurrent.IsKeyDown(Keys.W) || Input.KeyboardCurrent.IsKeyDown(Keys.Up) || Input.KeyboardCurrent.IsKeyDown(Keys.Space);
		if (jump && Grounded())
			Velocity.Y = -Speed;

		var left = Input.KeyboardCurrent.IsKeyDown(Keys.A) || Input.KeyboardCurrent.IsKeyDown(Keys.Left);
		var right = Input.KeyboardCurrent.IsKeyDown(Keys.D) || Input.KeyboardCurrent.IsKeyDown(Keys.Right);
		if (left && right)
			Velocity.X = 0;
		else if (left)
			Velocity.X = -Speed;
		else if (right)
			Velocity.X = Speed;

		if (left)
			VisualDirection = -1;
		else if (right)
			VisualDirection = 1;
	}
	
	public void Update()
	{
	}

	public void UpdateMovement()
	{
		var nextHitbox = Bounds;
		nextHitbox.Offset((int)Velocity.X, (int)Velocity.Y);
		
		Position += new Vector2(Velocity.X, MathHelper.Clamp(Velocity.Y, float.MinValue, Grounded() ? 0 : float.MaxValue));

		var visualPosition = Position - FHS.ScreenPosition;
		if (visualPosition.Y > FHS.ScreenSize.Y * 0.75f && Velocity.Y > 0 && FHS.ScreenPosition.Y < FHS.GroundLevel * FHS.TileSize - FHS.ScreenSize.Y + 40)
			FHS.ScreenPosition += Velocity;
		if (visualPosition.Y < FHS.ScreenSize.Y * 0.25f && Velocity.Y < 0)
			FHS.ScreenPosition += Velocity;
		
		if (visualPosition.X > FHS.ScreenSize.X * 0.75f && Velocity.X > 0)
			FHS.ScreenPosition += Velocity;
		if (visualPosition.X < FHS.ScreenSize.X * 0.25f && Velocity.X < 0)
			FHS.ScreenPosition += Velocity;
		
		FHS.ScreenPosition = Vector2.Clamp(FHS.ScreenPosition, new Vector2(float.MinValue), new Vector2(float.MaxValue, FHS.GroundLevel * FHS.TileSize - FHS.ScreenSize.Y + 40));
		
		Velocity.X = 0;
		Velocity.Y = MathHelper.Lerp(Velocity.Y, Speed * 1.5f, 0.005f);
	}
	
	public void Draw()
	{
		var texture = Assets.Placeholder;
		
		
		FHS.SpriteBatch.Draw(texture, new Vector2(600, 800) - FHS.ScreenPosition, null, new Color(116, 131, 250), 0, texture.Size / 2f, 2f, SpriteEffects.None, 0);
		
		FHS.SpriteBatch.Draw(texture, Position - FHS.ScreenPosition, null, Color.White, 0, texture.Size / 2f, 2f, VisualDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
	}
}