using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TowerToHeaven.Core;

namespace TowerToHeaven;

public partial class Player
{
	public bool Disappointment() => Position.Y > LastGroundedHeight + FHS.TileSize * 7 || DisappointmentDelay > 0; 
	
	public void HandleBestHeight()
	{
		BestHeight = Math.Max(FHS.GroundLevel * FHS.TileSize - (int)Position.Y, BestHeight);

		if (BestHeight / FHS.TileSize % 100 == 0 && BestHeight / FHS.TileSize > 0)
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
			Assets.Sounds.Blip?.Play(0.5f, (1f - NewBestDelay / 60f) * 0.1f + 0.2f, 0);
		}
	}
    
	public void HandleMusic()
	{
		if (MediaPlayer.State == MediaState.Stopped && !Disappointment() && BestHeight / FHS.TileSize >= 4)
		{
			Assets.Sounds.FallInstance?.Stop();
		    
			MediaPlayer.Play(Assets.Music);
		}
	    
		if (Disappointment())
			MediaPlayer.Stop();
	}
    
	public void HandleAnimations()
    {
        if (DamageDelay > 0)
        {
            frame = 3;
        }
		else if (Grounded())
        {
            var left = Input.KeyboardCurrent.IsKeyDown(Keys.A) || Input.KeyboardCurrent.IsKeyDown(Keys.Left);
            var right = Input.KeyboardCurrent.IsKeyDown(Keys.D) || Input.KeyboardCurrent.IsKeyDown(Keys.Right);

            if (left || right)
            {
                if (StepAnim)
                {
                    frame = 1;
                }
                else
                {
                    frame = 0;
                }
            }
			else
            {
                frame = 0;
            }
		}
		else
        {
            if (Velocity.Y > 0)
            {
                frame = 2;
            }
            else
            {
                frame = 1;
            }
        }
    }

	public void Draw()
	{
		var texture = Assets.Textures.Atlas;

		var quality = 8;
		var position = new Vector2(MathF.Floor(Position.X / quality) * quality, MathF.Floor(Position.Y / quality) * quality);
		var color = DamageDelay > 0 && FHS.AmbientTimer % 3 == 0 ? Color.Red : Color.White;

		int frameX = 34 * frame;

		Rectangle sourceRect = new(174 + frameX, 34, 32, 32);

		var x = (int)position.X / FHS.TileSize;
		var y = FHS.GroundLevel - (int)position.Y / FHS.TileSize - 1;

		if (x - 1 > 0 && y > 0 && x + 1 < Tiles.MaxTilesX && y < Tiles.MaxTilesY)
		{
			for (int i = -1; i < 2; i++)
			{
				var tileUnderneath = Tiles.Grid[x + i, y];
				if (tileUnderneath != TileTypes.Inactive)
				{
					for (int k = 0; k < 8; k++)
					{
						var playerFeet = new Rectangle((int)position.X, (int)position.Y + Bounds.Height / 2, Bounds.Width, 2);
						var tileHitbox = new Rectangle(x * FHS.TileSize, (FHS.GroundLevel - y) * FHS.TileSize, FHS.TileSize, FHS.TileSize);
						if (!playerFeet.Intersects(tileHitbox))
							position.Y++;
					}
				}
			}
		}
		
		FHS.SpriteBatch.Draw(texture, position - FHS.ScreenPosition, sourceRect, color, 0, new Vector2(sourceRect.Width, sourceRect.Height) / 2f, Scale * 2f, VisualDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
	}
}