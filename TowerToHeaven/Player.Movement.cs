using TowerToHeaven.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TowerToHeaven;

public partial class Player
{
	public bool Grounded() => (CollidingWithTileFloor || Position.Y > FHS.GroundLevel * FHS.TileSize) && Velocity.Y >= -0.5f;

	public void Jump()
	{
		var jump = Input.KeyboardCurrent.IsKeyDown(Keys.W) || Input.KeyboardCurrent.IsKeyDown(Keys.Up) || Input.KeyboardCurrent.IsKeyDown(Keys.Space);
		if (jump && (Grounded() || JumpLeewayTime > 0) && !CollidingWithTileHead && JumpDelay < 0)
		{
			for (int i = 0; i < 5; i++)
			{
				var rand = new Random(FHS.AmbientTimer + i);

				var velX = rand.Next(-15, 15);

				var velY = rand.Next(-20, -5);
				ParticleSystem.SpawnParticle(Position + new Vector2(Size.X * 0.5f, Size.Y * 0.7f), new Vector2(velX, velY), 30, ParticleType.FootStep);
			}
			
			JumpDelay = 10;
			Assets.Sounds.Jump?.Play(1, new Random(FHS.AmbientTimer).Next(100) / 100f * -0.2f, 0);
			Scale.X = 0.5f;
			Scale.Y = 2f;

			JumpLeewayTime = 0;
			Velocity.Y = -Speed;
		}
	}
	
	public void HandleTiles()
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
				var y = (FHS.GroundLevel - j) * FHS.TileSize;
				if (Tiles.Grid[i, j] == 0 || y > FHS.ScreenPosition.Y + FHS.ScreenSize.Y * 1.5f || y < FHS.ScreenPosition.Y - FHS.ScreenSize.Y * .5f)
					continue;


				if (Tiles.Grid[i, j] is > 3 and <= 121)
				{
					if (Tiles.Grid[i, j]++ > 120)
					{
						Tiles.RemoveTile(i, j);
					}
				}

				var shouldMoveX = false;
				var shouldMoveY = false;
				var collided = false;
				var hitbox = new Rectangle(i * FHS.TileSize, (FHS.GroundLevel - j) * FHS.TileSize, FHS.TileSize, FHS.TileSize);
				if (nextBounds.Intersects(hitbox))
				{
					if (hitbox.X < Position.X)
						CollidingWithTileLeft = true;
					
					if (hitbox.X > Position.X)
						CollidingWithTileRight = true;

					collided = true;
					shouldMoveX = true;
				}
				if (headBounds.Intersects(hitbox) && hitbox.Y + hitbox.Height < Position.Y && Tiles.Grid[i, j] < 254)
				{
					CollidingWithTileHead = true;
					collided = true;
				}
				if (FloorBounds.Intersects(hitbox) && hitbox.Y > Position.Y && Tiles.Grid[i, j] < 254)
				{
					JumpLeewayTime = 15;
					CollidingWithTileFloor = true;

					if (Tiles.Grid[i, j] == 3)
						Tiles.Grid[i, j]++;
					
					collided = true;
					shouldMoveY = true;
				}
				
				if (collided)
				{
					var dir = new Vector2(hitbox.Center.X, hitbox.Center.Y) - Position;
					dir.Normalize();

					if (Tiles.Grid[i, j] == 2 && !shouldMoveY && DamageDelay < 0)
					{
						Hurt(dir.X < 0 ? 1 : -1);
					}

					if (shouldMoveX || shouldMoveY) 
					{
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
	}

	public void UpdateMovement()
	{
		var outerEdges = MathF.Abs(SpawnPosition.X - (Position.X + Velocity.X)) > FHS.ScreenSize.X / 2f - Size.X / 2f;
		
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

		if (Grounded() && MathF.Abs(movement.X) > 0)
		{
			var rand = new Random(FHS.AmbientTimer);
			
			if (FHS.AmbientTimer % 20 == 0)
				Assets.Sounds.Step?.Play(0.2f, new Random(FHS.AmbientTimer).Next(100) / 100f * -0.2f, 0);
			if (FHS.AmbientTimer % 10 == 0)
				ParticleSystem.SpawnParticle(Position + new Vector2(Size.X * 0.5f, Size.Y * 0.7f), new Vector2(-Velocity.X, rand.Next(-4, -1)), 30, ParticleType.FootStep);
		}

		if (MathF.Abs(Velocity.X) < 5f)
			Velocity.X = 0;
		else
			Velocity.X = MathHelper.Lerp(Velocity.X, 0, 0.01f);
		
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
}