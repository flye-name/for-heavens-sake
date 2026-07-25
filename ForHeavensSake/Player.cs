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
	public Rectangle FloorBounds => new Rectangle((int)Position.X - (int)Size.X / 2 + 4, (int)Position.Y + (int)Size.Y / 2 - 8, (int)Size.X - 8, 10);
	public Vector2 Velocity;
	public int VisualDirection = 1;
	public bool CollidingWithTileLeft;
	public bool CollidingWithTileRight;
	public bool CollidingWithTileHead;
	public bool CollidingWithTileFloor;
	public int JumpLeewayTime;
	public int JumpDelay;
	public const float Speed = 5f;
    public Vector2 Scale = new(1, 1);

    public bool Grounded() => (CollidingWithTileFloor || Position.Y > FHS.GroundLevel * FHS.TileSize) && Velocity.Y >= -0.5f;
	
	
    public void Update()
    {
	    JumpLeewayTime--;
	    JumpDelay--;

	    if (Velocity.Y < 0 && JumpDelay < 3)
		    JumpDelay = 3;
		
	    UpdateCollision();
		
	    HandleInput();
		
	    for (int i = 0; i < 3; i++)
	    {
		    UpdateMovement();
		    UpdateCamera();
	    }

	    CollidingWithTileLeft = false;
	    CollidingWithTileRight = false;
	    CollidingWithTileHead = false;
	    CollidingWithTileFloor = false;

	    Scale.X = MathHelper.Lerp(1, Scale.X, 0.9f);
	    Scale.Y = MathHelper.Lerp(1, Scale.Y, 0.9f);
    }
    
	public void HandleInput()
	{
		var jump = Input.KeyboardCurrent.IsKeyDown(Keys.W) || Input.KeyboardCurrent.IsKeyDown(Keys.Up) || Input.KeyboardCurrent.IsKeyDown(Keys.Space);
		if (jump && (Grounded() || JumpLeewayTime > 0) && !CollidingWithTileHead && JumpDelay < 0)
		{
			JumpDelay = 10;
			Assets.Sounds.Jump.Play(1, new Random(FHS.AmbientTimer).Next(100) / 100f * -0.2f, 0);
			Scale.X = 0.5f;
			Scale.Y = 2f;

            JumpLeewayTime = 0;
			Velocity.Y = -Speed;
		}

		var left = Input.KeyboardCurrent.IsKeyDown(Keys.A) || Input.KeyboardCurrent.IsKeyDown(Keys.Left);
		var right = Input.KeyboardCurrent.IsKeyDown(Keys.D) || Input.KeyboardCurrent.IsKeyDown(Keys.Right);

		if (Velocity.Y < 0.75f || Grounded())
		{
			if (left && right)
				Velocity.X = 0;
			else if (left && !CollidingWithTileLeft)
				Velocity.X = -Speed;
			else if (right && !CollidingWithTileRight)
				Velocity.X = Speed;
		}

		if (left)
			VisualDirection = -1;
		else if (right)
			VisualDirection = 1;

		if (Input.JustClickedL)
			Tiles.PlaceTile(Input.MousePosition + FHS.ScreenPosition);
		
		if (Input.JustClickedR)
			Tiles.RemoveTile(Input.MousePosition + FHS.ScreenPosition);

		if (Input.JustClickedR && Input.KeyboardCurrent.IsKeyDown(Keys.LeftControl))
		{
			Tiles.RemoveAllTiles();
		}
	}

	public void UpdateCollision()
	{
		var nextBounds = Bounds;
		nextBounds.Offset((int)Velocity.X, -5);
		nextBounds.Inflate(-2, -16);
		
		var headBounds = FloorBounds;
		headBounds.Offset(0, (int)-Size.Y + 10);
		headBounds.Inflate(-16, 20);
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
					if (hitbox.X < Position.X)
						CollidingWithTileLeft = true;
					
					if (hitbox.X > Position.X)
						CollidingWithTileRight = true;
					
					shouldMoveX = true;
				}
				if (headBounds.Intersects(hitbox) && hitbox.Y + hitbox.Height < Position.Y)
				{
					CollidingWithTileHead = true;
				}
				if (FloorBounds.Intersects(hitbox) && hitbox.Y > Position.Y)
				{
					JumpLeewayTime = 15;
					CollidingWithTileFloor = true;
					shouldMoveY = true;
				}
				
				if (shouldMoveX || shouldMoveY)
				{
					var dir = new Vector2(hitbox.Center.X, hitbox.Center.Y) - Position;
					dir.Normalize();

					var attempts = 0;
					
					while (attempts++ < 30 && (hitbox.X + hitbox.Width < Position.X || hitbox.X > Position.X + Size.X) && Bounds.Intersects(hitbox) && hitbox.Y > FloorBounds.Y)
					{
						Position.X -= dir.X;
					}

					attempts = 0;

					var floorBounds = FloorBounds;
					floorBounds.Offset(0, -2);
					while (attempts++ < 10 && floorBounds.Intersects(hitbox) && !headBounds.Intersects(hitbox) && hitbox.Y + hitbox.Height / 2 > FloorBounds.Y && (Velocity.Y == 0 || MathF.Sign(dir.Y) == MathF.Sign(Velocity.Y)))
					{
						Position.Y -= dir.Y;
					}
				}
			}
		}
	}

	public void UpdateMovement()
	{
		var outerEdges = MathF.Abs(SpawnPosition.X - (Position.X + Velocity.X)) > FHS.TileSize * 8.5f + 4;
		
		var movement = new Vector2(outerEdges ? 0 : Velocity.X, MathHelper.Clamp(Velocity.Y, float.MinValue, Grounded() ? 0 : float.MaxValue));

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

		if (Grounded() && MathF.Abs(movement.X) > 0 && FHS.AmbientTimer % 20 == 0)
			Assets.Sounds.Step.Play(1, new Random(FHS.AmbientTimer).Next(100) / 100f * -0.2f, 0);

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

		var quality = 8;
		var position = new Vector2(MathF.Floor(Position.X / quality) * quality, MathF.Floor(Position.Y / quality) * quality);
		
		FHS.SpriteBatch.Draw(texture, position - FHS.ScreenPosition, null, new Color(116, 131, 250), 0, texture.Size / 2f, Scale * 2.5f, VisualDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
		FHS.SpriteBatch.Draw(texture, position - FHS.ScreenPosition, null, Color.White, 0, texture.Size / 2f, Scale * 2f, VisualDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

	}
}