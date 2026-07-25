using ForHeavensSake.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace ForHeavensSake;

public partial class Player
{
	public bool Disappointment() => Position.Y > LastGroundedHeight + FHS.TileSize * 7 || DamageDelay > 0 || DisappointmentDelay > 0; 
	
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