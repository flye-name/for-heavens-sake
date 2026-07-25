using Microsoft.Xna.Framework.Audio;

namespace TowerToHeaven.Core;

public class SFX
{
	public SoundEffect? Sound;
	public List<SoundEffectInstance> Instances = new();
		
	public SFX(string path, int maxInstances)
	{
		Sound = Assets.LoadSound(path);
		
		maxInstances = Math.Clamp(maxInstances, 1, 10);

		for (int i = 0; i < maxInstances; i++)
			Instances.Add(Sound.CreateInstance());
	}

	public void Play()
	{
		for (int i = 0; i < Instances.Count; i++)
		{
			if (Instances[i].State == SoundState.Playing)
				continue;
			
			Instances[i].Play();
			break;
		}
	}

	public void Play(float volume, float pitch, float pan)
	{
		for (int i = 0; i < Instances.Count; i++)
		{
			if (Instances[i].State == SoundState.Playing)
				continue;

			Instances[i].Volume = volume;
			Instances[i].Pitch = pitch;
			Instances[i].Pan = pan;
			Instances[i].Play();
			break;
		}
	}

	public void Dispose()
	{
		for (int i = 0; i < Instances.Count; i++)
			Instances[i]?.Dispose();
		
		Instances.Clear();
		
		Sound?.Dispose();
	}
}