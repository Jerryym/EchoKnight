using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Echo.EditorTool
{
    public class RPGEditorToolWindow : EditorWindow
    {
        [MenuItem("Tools/RPG EditorTool")]
        public static void ShowWindow()
        {
            RPGEditorToolWindow wnd = GetWindow<RPGEditorToolWindow>();
            wnd.titleContent = new GUIContent("RPG EditorTool");
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
			//加载UXML
			VisualTreeAsset uxmlAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/View/Layout/RPGEditorToolWindow.uxml");
        	uxmlAsset.CloneTree(root);

			//加载uss
			StyleSheet ussAsset = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/View/Styles/RPGEditorToolWindow.uss");
			root.styleSheets.Add(ussAsset);
        }
    }
}

