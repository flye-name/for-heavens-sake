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
	

	public static Song Music;

	public static class Textures
	{
		public static Texture2D Placeholder;

		public static void LoadAssets()
		{
			Placeholder = LoadTexture("Placeholder");
		}

		public static void Dispose()
		{
			Placeholder.Dispose();
		}
	}

	public static class Sounds
	{
		public static SoundEffect TestSound;
		public static SoundEffect Step;
		public static SoundEffect MenuSelect;
		public static SoundEffect Blip;
		public static SoundEffect Jump;
		public static SoundEffect Fall;
		public static SoundEffect Hurt;
		public static SoundEffect TilePlace;
		public static SoundEffect TileBreak;

		public static void LoadAssets()
		{
			TestSound = LoadSound("TestSound");
			Step = LoadSound("Step");
			MenuSelect = LoadSound("MenuSelect");
			Blip = LoadSound("Blip");
			Jump = LoadSound("Jump");
			Fall = LoadSound("Fall");
			Hurt = LoadSound("Hurt");
			TilePlace = LoadSound("BlockPlace");
			TileBreak = LoadSound("BlockBreak");
		}

		public static void Dispose()
		{
			TestSound.Dispose();
			Step.Dispose();
			MenuSelect.Dispose();
			Blip.Dispose();
			Jump.Dispose();
			Fall.Dispose();
			Hurt.Dispose();
			TilePlace.Dispose();
			TileBreak.Dispose();
		}
	}
	
	public static void Load()
	{
		Music = LoadSong("StairwayToHeaven");
		
		Textures.LoadAssets();
		Sounds.LoadAssets();
		
		FHS.FontSystem = new();
		FHS.FontSystem.AddFont(File.ReadAllBytes(@"Content/Fonts/SpaceMono.ttf"));
		
		Saving.Save();
	}

	public static void Unload()
	{
		Music.Dispose();
		
		Textures.Dispose();
		
		Sounds.Dispose();
		
		FHS.FontSystem.Dispose();
	}
}