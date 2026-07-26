using Microsoft.Xna.Framework;

namespace TowerToHeaven;

public partial class Player
{
	public Vector2 SpawnPosition;
	public Vector2 Position;
	public readonly Vector2 Size = new(64);
	public Rectangle Bounds => new((int)Position.X - (int)Size.X / 2, (int)Position.Y - (int)Size.Y / 2, (int)Size.X, (int)Size.Y);
	public Rectangle FloorBounds => new((int)Position.X - (int)Size.X / 2 + 4, (int)Position.Y + (int)Size.Y / 2 - 8, (int)Size.X - 8, 10);
	public Vector2 Velocity;
	public int VisualDirection = 1;
	public bool CollidingWithTileLeft;
	public bool CollidingWithTileRight;
	public bool CollidingWithTileHead;
	public bool CollidingWithTileFloor;
	public static bool Sticky;
	public int JumpLeewayTime;
	public int JumpDelay;
	public int DamageDelay;
	public int BestHeight;
	public int LastGroundedHeight;
	public int DisappointmentDelay;
    public int frame;
    public int frameDelay;
    public bool StepAnim;
    public int NewBestDelay;
	public bool PlayedNewBestBlip;
	
	public const float Speed = 5f;
	public Vector2 Scale = new(1, 1);

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
		Sticky = false;
		CollidingWithTileLeft = false;
		CollidingWithTileRight = false;
		CollidingWithTileHead = false;
		CollidingWithTileFloor = false;

        Scale.X = MathHelper.Lerp(1, Scale.X, 0.9f);
		Scale.Y = MathHelper.Lerp(1, Scale.Y, 0.9f);
	}
}