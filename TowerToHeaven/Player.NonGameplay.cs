using TowerToHeaven.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

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
		if (MediaPlayer.State == MediaState.Stopped && !Disappointment())
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
		var texture = Assets.Textures.Player;

		var quality = 8;
		var position = new Vector2(MathF.Floor(Position.X / quality) * quality, MathF.Floor(Position.Y / quality) * quality);
		var color = DamageDelay > 0 && FHS.AmbientTimer % 3 == 0 ? Color.Red : Color.White;

		int frameX = (texture.Width / 4) * frame;

		Rectangle sourceRect = new(frameX, 0, texture.Width / 4, texture.Height);
		
		FHS.SpriteBatch.Draw(texture, position - FHS.ScreenPosition, sourceRect, color, 0, new Vector2(sourceRect.Width, sourceRect.Height) / 2f, Scale * 2f, VisualDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
	}
}