using UnityEditor;

namespace Echo.Editor
{
	[FilePath("ProjectSettings/PTGToolSettings.asset", FilePathAttribute.Location.ProjectFolder)]
	public class PTGToolSettings : ScriptableSingleton<PTGToolSettings>
	{
		public Model_PTGTool viewData = new Model_PTGTool();

		public void Save()
		{
			EditorUtility.SetDirty(this);
			Save(true);
		}
	}
}
