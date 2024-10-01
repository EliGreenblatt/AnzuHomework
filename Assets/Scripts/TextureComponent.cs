using UnityEditor;
using UnityEngine;

[ExecuteInEditMode] // We want our component to work when game isn't running
public class TextureComponent : MonoBehaviour
{
    [SerializeField]
    private Texture2D texture; // the 2D texture we will inspect

    public void ViewTexture()
    {
        TextureViewer.Open(texture);
    }

    public Texture2D GetTexture()
    {
        return texture;
    }
}
