using UnityEditor;

namespace Echo.Editor
{
	[FilePath("ProjectSettings/CSVToSO.asset", FilePathAttribute.Location.ProjectFolder)]
	public class CSVToSOSettings : ScriptableSingleton<CSVToSOSettings>
	{ 
		public Model_CSVToSO viewData = new Model_CSVToSO();

		public void Save()
		{
			EditorUtility.SetDirty(this);
			Save(true);
		}
	}
}
