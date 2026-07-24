using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace ForHeavensSake.Core;

public static class Assets
{
	public static Texture2D LoadTexture(string path) => FHS.Instance.Content.Load<Texture2D>("Images/" + path);
	public static SoundEffect LoadSound(string path) => FHS.Instance.Content.Load<SoundEffect>("Sounds/" + path);
	public static Song LoadSong(string path) => FHS.Instance.Content.Load<Song>("Music/" + path);
	
	public static Texture2D Placeholder;

	public static SoundEffect TestSound;
	
	public static void Load()
	{
		Placeholder = LoadTexture("Placeholder");

		TestSound = LoadSound("TestSound");
		
		FHS.FontSystem = new();
		FHS.FontSystem.AddFont(File.ReadAllBytes(@"Content/Fonts/SpaceMono.ttf"));
		
		Saving.Save();
	}

	public static void Unload()
	{
		Placeholder.Dispose();
		
		TestSound.Dispose();
		
		FHS.FontSystem.Dispose();
	}
}