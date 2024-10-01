using UnityEditor;
using UnityEngine;

public class TextureViewer : EditorWindow
{
    private Texture2D texture;

    // variables for zooming
    private float zoomFactor = 1f;
    private const float minZoom = 0.7f;
    private const float maxZoom = 7f;

    // variables for resizing window
    float windowHeight;
    float windowWidth;

    // variables for dragging
    private Vector2 offset = Vector2.zero;
    private Vector2 lastMousePos;
    private bool isDragging = false;


    // Function that takes a texture and creates a window to display it
    public static void Open(Texture2D textureToDisplay)
    {
        TextureViewer window = GetWindow<TextureViewer>("Texture Viewer");
        window.texture = textureToDisplay;
        window.UpdateWindowSize();
        window.Show();
    }

    // We want the window to be a fixed size so we scale the texture and then make the window fit the ratio of width/height of the texture
    private void UpdateWindowSize()
    {
        float aspectRatio = (float)texture.width / texture.height;
        windowHeight = 400f;
        windowWidth = windowHeight * aspectRatio;
        position = new Rect(100, 100, windowWidth, windowHeight);
    }

    // Function called to redraw the texture as needed
    private void OnGUI()
    {
        HandleZoom();
        HandleDrag();
        HandleScaling();
    }


    // We check if the mouse wheel is being scrolled, if so we capture the mouse position and zoom in/out on that same pixel
    private void HandleZoom()
    {
        Event e = Event.current;

        if (e.type == EventType.ScrollWheel)
        {
            // Capture the mouse position in screen space
            Vector2 mousePos = e.mousePosition;

            // Calculate the position of the mouse in texture space before the zoom
            Vector2 mouseTexturePositionBeforeZoom = new Vector2(
                (mousePos.x - (position.width / 2 + offset.x)) / zoomFactor,
                (mousePos.y - (position.height / 2 + offset.y)) / zoomFactor
            );

            if (e.delta.y > 0)
            {
                zoomFactor *= 0.9f; // Zoom in
            }
            else if (e.delta.y < 0)
            {
                zoomFactor *= 1.1f; // Zoom out
            }

            zoomFactor = Mathf.Clamp(zoomFactor, minZoom, maxZoom);

            // Calculate the new position of the mouse in texture space after the zoom
            Vector2 mouseTexturePositionAfterZoom = new Vector2
                (
                    (mousePos.x - (position.width / 2 + offset.x)) / zoomFactor,
                    (mousePos.y - (position.height / 2 + offset.y)) / zoomFactor
                );

            // Change offset so mouse stays in same position
            offset += (mouseTexturePositionAfterZoom - mouseTexturePositionBeforeZoom) * zoomFactor;

            // Paints the texture again correctly
            Repaint();
        }
    }



    // We check that the user has left clicked and is trying to drag, we then drag the picture, clamp in
    // between the texture windows width and height with some padding WITH respect to the zoom factor
    private void HandleDrag()
    {
        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            // Start dragging
            isDragging = true;
            lastMousePos = e.mousePosition;
            e.Use();
        }

        if (e.type == EventType.MouseDrag && isDragging)
        {
            // Calculate the change in mouse position
            Vector2 delta = e.mousePosition - lastMousePos;
            lastMousePos = e.mousePosition;

            // Update the offset with the delta
            offset += delta;

            // We now clamp the height and width to stop the user from dragging endlessly with respect to the zoom factor

            if (offset.x > (windowWidth/2) * zoomFactor) // Prevent dragging to the right
            {
                offset.x = (windowWidth / 2) * zoomFactor; // Clamp to left
            }
            else if (offset.x < -(windowWidth/2) * zoomFactor) // Prevent dragging to the left
            {
                offset.x = -(windowWidth/2) * zoomFactor; // Clamp to right
            }

            // Clamp offset Y
            if (offset.y > (windowHeight/2) * zoomFactor) // Prevent dragging down
            {
                offset.y = (windowHeight / 2) * zoomFactor; // Clamp to top
            }
            else if (offset.y < -(windowHeight/2 * zoomFactor)) // Prevent dragging up
            {
                offset.y = -(windowHeight / 2 * zoomFactor); // Clamp to bottom
            }

            Repaint();
        }

        if (e.type == EventType.MouseUp && e.button == 0)
        {
            // Stop dragging
            isDragging = false;
        }
    }
    private void HandleScaling()
    {
        float textureAspect = (float)texture.width / texture.height;
        float windowAspect = position.width / position.height;

        float scaledWidth, scaledHeight;

        if (windowAspect > textureAspect)
        {
            scaledHeight = position.height * zoomFactor;
            scaledWidth = scaledHeight * textureAspect;
        }
        else
        {
            scaledWidth = position.width * zoomFactor;
            scaledHeight = scaledWidth / textureAspect;
        }

        float offsetX = (position.width - scaledWidth) / 2 + offset.x;
        float offsetY = (position.height - scaledHeight) / 2 + offset.y;

        // in the end we draw the texture
        GUI.DrawTexture(new Rect(offsetX, offsetY, scaledWidth, scaledHeight), texture);
    }
}
