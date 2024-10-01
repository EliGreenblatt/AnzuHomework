using UnityEngine;
using UnityEditor;

// This script is a custom editor that edits the component in the hierarchy.
[CustomEditor(typeof(TextureComponent))]
public class TextureComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // First draw the original component in the inspector

        DrawDefaultInspector(); 

        // Grab a reference to the component
        TextureComponent component = (TextureComponent)target;

        // We only enable the button if there is a texture chosen, if not the button is disabled
        GUI.enabled = (component.GetTexture() != null);

        // Check if button is pressed
        if (GUILayout.Button("Open Texture Viewer"))  
        {
            component.ViewTexture();
        }
    }
}
