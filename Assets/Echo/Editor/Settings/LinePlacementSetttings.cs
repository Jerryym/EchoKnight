using UnityEditor;

namespace Echo.Editor
{
	[FilePath("ProjectSettings/LinePlacement.asset", FilePathAttribute.Location.ProjectFolder)]
	public class LinePlacementSettings : ScriptableSingleton<LinePlacementSettings>
	{
		public Model_LinePlacement viewData = new Model_LinePlacement();

		public void Save()
		{
			EditorUtility.SetDirty(this);
			Save(true);
		}
	}
}
