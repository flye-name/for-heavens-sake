using System.ComponentModel.Design;
using TowerToHeaven.Core;
using TowerToHeaven.Core.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TowerToHeaven;

public partial class Player
{
	public void Update()
    {
	    HandleMusic();

	    if (!FHS.Frozen)
	    {
		    HandleBestHeight();
		    HandleDelays();
		    HandleTiles();
	    }

	    HandleInput();
		
	    if (!FHS.Frozen)
	    { 
		    for (int i = 0; i < 3; i++)
		    {
			    UpdateMovement();
			    UpdateCamera();
		    }
	    }
	    
	    if (Grounded())
	    {
		    LastGroundedHeight = (int)Position.Y;
	    }
	    else if (Disappointment() && !FHS.Frozen)
	    {
		    if (DisappointmentDelay < 0 && MediaPlayer.State == MediaState.Playing)
			    Assets.Sounds.FallInstance?.Play();
		    
		    DisappointmentDelay = 100; 
	    }
	    
	    if (!FHS.Frozen)
			ResetFields();	
    }
    
	public void HandleInput()
	{
		if (!FHS.Frozen)
		{
			Jump();

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
		}
		else
		{
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
		}
		
		if (Input.JustPressed(Keys.P))
		{
			var rand = new Random((int)(Position.X + Position.Y) + FHS.AmbientTimer);
			var type = (byte)3;
			if (rand.Next(4) == 0)
				type = 2;
			else if (rand.Next(3) == 0)
				type = 1;
			Tiles.PlaceTile(Input.MousePosition + FHS.ScreenPosition, type);
		}

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

	public void Hurt(int direction)
	{
		DamageDelay = 20;
		
		Velocity.X = direction * 10;
		Velocity.Y = 10;

		Scale = new(1.2f, 0.8f);

		Assets.Sounds.Hurt?.Play();

		for (int i = 0; i < 10; i++)
		{
			var rand = new Random(FHS.AmbientTimer + i);
			var velocity = new Vector2(rand.Next(-100, 100), rand.Next(-100, 100)) * 0.1f;
			ParticleSystem.SpawnParticle(Position, velocity, 60, ParticleType.Hurt);
		}
	}
}