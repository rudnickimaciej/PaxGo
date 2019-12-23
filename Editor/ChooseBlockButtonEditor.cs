using UnityEditor;

[CustomEditor(typeof(ChooseBlockButton))]
public class MenuButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // // Show default inspector property editor
        // DrawDefaultInspector();
    }
}