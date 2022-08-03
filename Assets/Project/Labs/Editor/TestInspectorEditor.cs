using UnityEditor;
using UnityEngine;
namespace Labs
{
	[CustomEditor(typeof(TestInspector))]
	public class TestInspectorEditor : Editor
	{
		TestInspector test_inspector;
		public Editor editor;
		void OnEnable()
		{
			if (target is TestInspector cur_test_inspector)
			{
				test_inspector = cur_test_inspector;
			}
		}
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			if (!editor)
			{
				editor = CreateEditor(test_inspector.animal);
				Debug.Log(editor);
			}
			editor.OnInspectorGUI();
		}
	}
}