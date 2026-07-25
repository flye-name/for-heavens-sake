using System.ComponentModel.Design;
using ForHeavensSake.Core;
using ForHeavensSake.Core.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

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
	public int DamageDelay;
	public int BestHeight;
	public int LastGroundedHeight;
	public int DisappointmentDelay;
	public int NewBestDelay;
	public bool PlayedNewBestBlip;
	
	public const float Speed = 5f;
    public Vector2 Scale = new(1, 1);

    public bool Disappointment() => Position.Y > LastGroundedHeight + FHS.TileSize * 7 || DamageDelay > 0 || DisappointmentDelay > 0; 
    
    public bool Grounded() => (CollidingWithTileFloor || Position.Y > FHS.GroundLevel * FHS.TileSize) && Velocity.Y >= -0.5f;
	
	
    public void Update()
    {
	    HandleBestHeight();

	    HandleMusic();
	    
	    HandleDelays();
		
	    HandleTiles();
		
	    HandleInput();
		
	    for (int i = 0; i < 3; i++)
	    {
		    UpdateMovement();
		    UpdateCamera();
	    }
	    
	    if (Grounded())
	    {
		    LastGroundedHeight = (int)Position.Y;
	    }
	    else if (Disappointment())
	    {
		    if (DisappointmentDelay < 0 && MediaPlayer.State == MediaState.Playing)
			    Assets.Sounds.FallInstance.Play();
		    
		    DisappointmentDelay = 100; 
	    }
	    
	    ResetFields();
    }

    public void HandleBestHeight()
    {
	    BestHeight = Math.Max(FHS.GroundLevel * FHS.TileSize - (int)Position.Y, BestHeight);

	    if (BestHeight / FHS.TileSize % 100 == 0)
	    {
		    if (NewBestDelay <= 0 && !PlayedNewBestBlip)
		    {
			    NewBestDelay = 60;
			    PlayedNewBestBlip = true;
		    }
	    }
	    else 
		    PlayedNewBestBlip = false;

	    if (NewBestDelay > 0 && NewBestDelay % 10 == 0)
	    {
		    Assets.Sounds.Blip.Play(0.5f, (1f - NewBestDelay / 60f) * 0.1f + 0.2f, 0);
	    }
    }
    
    public void HandleMusic()
    {
	    if (MediaPlayer.State == MediaState.Stopped && !Disappointment())
	    {
		    Assets.Sounds.FallInstance.Stop();
		    
		    MediaPlayer.Play(Assets.Music);
	    }
	    
	    if (Disappointment())
		    MediaPlayer.Stop();
    }

    public void HandleDelays()
    {
	    DisappointmentDelay--;
	    JumpLeewayTime--;
	    JumpDelay--;
	    DamageDelay--;
	    NewBestDelay--;

	    if (Velocity.Y < 0 && JumpDelay < 3)
		    JumpDelay = 3;
    }
    
    public void ResetFields()
    {

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
			for (int i = 0; i < 5; i++)
			{
				var rand = new Random(FHS.AmbientTimer + i);

				var velX = rand.Next(-15, 15);

				var velY = rand.Next(-20, -5);
				ParticleSystem.SpawnParticle(Position + new Vector2(Size.X * 0.5f, Size.Y * 0.7f), new Vector2(velX, velY), 30, ParticleType.FootStep);
			}
			
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
		{
			var rand = new Random((int)(Position.X + Position.Y) + FHS.AmbientTimer);
			var type = (byte)3;
			if (rand.Next(4) == 0)
				type = 2;
			else if (rand.Next(3) == 0)
				type = 1;
			Tiles.PlaceTile(Input.MousePosition + FHS.ScreenPosition, type);
		}

		if (Input.JustClickedR)
			Tiles.RemoveTile(Input.MousePosition + FHS.ScreenPosition);

		if (Input.JustPressed(Keys.G))
			Hurt(1);
		if (Input.JustPressed(Keys.F))
			Hurt(-1);
		if (Input.KeyboardCurrent.IsKeyDown(Keys.T))
		{
			Position.Y -= 100;
			FHS.ScreenPosition.Y -= 100;
		}
		if (Input.KeyboardCurrent.IsKeyDown(Keys.R))
		{
			Position.X += 10;
		}
		if (Input.KeyboardCurrent.IsKeyDown(Keys.E))
		{
			Position.X -= 10;
		}

		if (Input.JustClickedR && Input.KeyboardCurrent.IsKeyDown(Keys.LeftControl))
		{
			Tiles.RemoveAllTiles();
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
				if (Tiles.Grid[i, j] == 0)
					continue;


				if (Tiles.Grid[i, j] is > 3 and < 121)
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
				if (headBounds.Intersects(hitbox) && hitbox.Y + hitbox.Height < Position.Y)
				{
					CollidingWithTileHead = true;
					collided = true;
				}
				if (FloorBounds.Intersects(hitbox) && hitbox.Y > Position.Y)
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
				Assets.Sounds.Step.Play(0.2f, new Random(FHS.AmbientTimer).Next(100) / 100f * -0.2f, 0);
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

	public void Hurt(int direction)
	{
		DamageDelay = 20;
		
		Velocity.X = direction * 10;
		Velocity.Y = 10;

		Scale = new(1.2f, 0.8f);

		Assets.Sounds.Hurt.Play();

		for (int i = 0; i < 10; i++)
		{
			var rand = new Random(FHS.AmbientTimer + i);
			var velocity = new Vector2(rand.Next(-100, 100), rand.Next(-100, 100)) * 0.1f;
			ParticleSystem.SpawnParticle(Position, velocity, 60, ParticleType.Hurt);
		}
	}
	
	public void Draw()
	{
		var texture = Assets.Textures.Placeholder;

		var quality = 8;
		var position = new Vector2(MathF.Floor(Position.X / quality) * quality, MathF.Floor(Position.Y / quality) * quality);
		var color = DamageDelay > 0 && FHS.AmbientTimer % 3 == 0 ? Color.Red : Color.White;
		
		FHS.SpriteBatch.Draw(texture, position - FHS.ScreenPosition, null, new Color(116, 131, 250), 0, texture.Size / 2f, Scale * 2.5f, VisualDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
		FHS.SpriteBatch.Draw(texture, position - FHS.ScreenPosition, null, color, 0, texture.Size / 2f, Scale * 2f, VisualDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
	}
}