using System.ComponentModel.Design;
using ForHeavensSake.Core;
using ForHeavensSake.Core.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ForHeavensSake;

public class Player
{
	public Vector2 SpawnPosition;
	public Vector2 Position;
	public readonly Vector2 Size = new Vector2(64, 64);
	public Rectangle Bounds => new Rectangle((int)Position.X - (int)Size.X / 2, (int)Position.Y - (int)Size.Y / 2, (int)Size.X, (int)Size.Y);
	public Rectangle FloorBounds => new Rectangle((int)Position.X - (int)Size.X / 2, (int)Position.Y + (int)Size.Y / 2, (int)Size.X, 4);
	public Vector2 Velocity;
	public int VisualDirection = 1;
	public bool CollidingWithTileX;
	public bool CollidingWithTileHead;
	public bool CollidingWithTileFloor;
	public int JumpLeewayTime;
	public const float Speed = 5f;

	public bool Grounded() => (CollidingWithTileFloor || Position.Y > FHS.GroundLevel * FHS.TileSize) && Velocity.Y > 0;
	
	public void HandleInput()
	{
		var jump = Input.KeyboardCurrent.IsKeyDown(Keys.W) || Input.KeyboardCurrent.IsKeyDown(Keys.Up) || Input.KeyboardCurrent.IsKeyDown(Keys.Space);
		if (jump && (Grounded() || JumpLeewayTime > 0))
		{
			JumpLeewayTime = 0;
			Velocity.Y = -Speed;
		}

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

		if (Input.JustClickedL)
			Tiles.PlaceTile(Input.MousePosition + FHS.ScreenPosition);
		
		if (Input.JustClickedR)
			Tiles.RemoveTile(Input.MousePosition + FHS.ScreenPosition);
	}
	
	public void Update()
	{
		JumpLeewayTime--;
		UpdateCollision();
		
		HandleInput();
		
		for (int i = 0; i < 3; i++)
		{
			UpdateMovement();
			UpdateCamera();
		}

		CollidingWithTileX = false;
		CollidingWithTileHead = false;
		CollidingWithTileFloor = false;
	}

	public void UpdateCollision()
	{
		var nextBounds = Bounds;
		nextBounds.Offset((int)Velocity.X, -5);
		nextBounds.Inflate(0, -16);
		
		var headBounds = FloorBounds;
		headBounds.Offset(0, (int)-Size.Y);
		headBounds.Inflate(-16, 0);
		for (int i = 0; i < Tiles.MaxTilesX; i++)
		{
			for (int j = 0; j < Tiles.MaxTilesY; j++)
			{
				if (Tiles.Grid[i, j] == 0)
					continue;

				var shouldMoveX = false;
				var shouldMoveY = false;
				var hitbox = new Rectangle(i * FHS.TileSize, (FHS.GroundLevel - j) * FHS.TileSize, FHS.TileSize, FHS.TileSize);
				if (nextBounds.Intersects(hitbox))
				{
					CollidingWithTileX = true;
					shouldMoveX = true;
				}
				if (headBounds.Intersects(hitbox))
				{
					CollidingWithTileHead = true;
				}
				if (FloorBounds.Intersects(hitbox))
				{
					JumpLeewayTime = 20;
					CollidingWithTileFloor = true;
					shouldMoveY = true;
				}
				
				if (shouldMoveX || shouldMoveY)
				{
					var dir = new Vector2(hitbox.Center.X, hitbox.Center.Y) - Position;
					dir.Normalize();
					if (shouldMoveX)
						Position.X -= dir.X;
					if (shouldMoveY)
						Position.Y -= dir.Y;
				}
			}
		}
		
		Console.WriteLine(CollidingWithTileHead);
	}

	public void UpdateMovement()
	{
		var outerEdges = MathF.Abs(SpawnPosition.X - (Position.X + Velocity.X)) > FHS.TileSize * 8.5f + 4;
		
		var movement = new Vector2(outerEdges ? 0 : Velocity.X, MathHelper.Clamp(Velocity.Y, float.MinValue, Grounded() ? 0 : float.MaxValue));

		if (CollidingWithTileX)
			movement.X = 0;

		if (CollidingWithTileFloor)
		{
			movement.Y = MathHelper.Clamp(movement.Y, float.MinValue, 0);
			Velocity.Y = MathHelper.Clamp(Velocity.Y, float.MinValue, 0);
		}

		if (CollidingWithTileHead)
		{
			movement.Y = MathHelper.Clamp(movement.Y, 0, float.MaxValue);
			Velocity.Y = MathHelper.Clamp(Velocity.Y, 0, float.MaxValue);
		}

		Position += movement;
		
		Velocity.X = 0;
		Velocity.Y = MathHelper.Lerp(Velocity.Y, Speed * 1.5f, 0.005f);
	}

	public void UpdateCamera()
	{
		var visualPosition = Position - FHS.ScreenPosition;
		if (visualPosition.Y > FHS.ScreenSize.Y * 0.75f && Velocity.Y > 0 && FHS.ScreenPosition.Y < FHS.GroundLevel * FHS.TileSize - FHS.ScreenSize.Y + 40)
			FHS.ScreenPosition.Y += Velocity.Y;
		if (visualPosition.Y < FHS.ScreenSize.Y * 0.25f && Velocity.Y < 0)
			FHS.ScreenPosition.Y += Velocity.Y;
		
		FHS.ScreenPosition = Vector2.Clamp(FHS.ScreenPosition, new Vector2(float.MinValue), new Vector2(float.MaxValue, FHS.GroundLevel * FHS.TileSize - FHS.ScreenSize.Y + 40));
	}
	
	public void Draw()
	{
		var texture = Assets.Textures.Placeholder;
		
		
		FHS.SpriteBatch.Draw(texture, new Vector2(600, 800) - FHS.ScreenPosition, null, new Color(116, 131, 250), 0, texture.Size / 2f, 2f, SpriteEffects.None, 0);
		
		FHS.SpriteBatch.Draw(texture, new Vector2(MathF.Floor(Position.X / 2) * 2, MathF.Floor(Position.Y / 2) * 2) - FHS.ScreenPosition, null, Color.White, 0, texture.Size / 2f, 2f, VisualDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
	}
}