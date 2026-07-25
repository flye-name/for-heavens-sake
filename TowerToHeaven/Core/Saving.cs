using Microsoft.Xna.Framework.Storage;

namespace TowerToHeaven.Core;

public static class Saving
{
	private const string SaveDisplayName = "FHSSaveData";
	public static void Save()
	{
		IAsyncResult result;

		result = StorageDevice.BeginShowSelector(null, null);
		while (!result.IsCompleted)
		{
			Thread.Sleep(1);
		}
		StorageDevice device = StorageDevice.EndShowSelector(result);

		result = device.BeginOpenContainer(SaveDisplayName, null, null);
		while (!result.IsCompleted)
		{
			Thread.Sleep(1);
		}
		StorageContainer container = device.EndOpenContainer(result);

		using var stream = container.CreateFile("save");
		
		using (StreamWriter writer = new StreamWriter(stream))
		{
			writer.WriteLine(SaveDisplayName);
		}

		container.Dispose();
	}
}