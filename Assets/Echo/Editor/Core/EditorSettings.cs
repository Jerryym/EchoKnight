using UnityEditor;

namespace Echo.Editor
{
	[FilePath("ProjectSettings/EditorSettings.asset", FilePathAttribute.Location.ProjectFolder)]
	public class EditorSettings : ScriptableSingleton<EditorSettings>
	{
		public Model_PTGTool model_PTGTool = new Model_PTGTool();
		public Model_LinePlacement model_linePlacement = new Model_LinePlacement();
		public Model_CSVToSO model_csvToSO = new Model_CSVToSO();

		public void SaveSettings()
		{
			EditorUtility.SetDirty(this);
			Save(true);
		}
	}
}
