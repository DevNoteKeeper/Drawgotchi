using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DrawingManager : MonoBehaviour
{
    [SerializeField] private RectTransform drawingPanel;
    [SerializeField] private RawImage drawingCanvas;

    private Texture2D texture;
    private bool isDrawing = false;

    [SerializeField] private Color brushColor = Color.black;
    [SerializeField] private int brushSize = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int width = (int)drawingPanel.rect.width;
        int height = (int)drawingPanel.rect.height;

        texture = new Texture2D(width, height);

        ClearCanvas();

        drawingCanvas.texture = texture;
    }

    public void ClearCanvas()
    {
        Color[] pixels = new Color[texture.width * texture.height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }

        texture.SetPixels(pixels);
        texture.Apply();
    }

    // Update is called once per frame
    void Update()
    {
        var pointer = Pointer.current;
        if (pointer.press.isPressed) {
            Vector2 screenPos = pointer.position.ReadValue();

            // check whether in panel
            if(RectTransformUtility.RectangleContainsScreenPoint(drawingPanel, screenPos))
            {
                Draw(screenPos);
            }
        }

    }

    private void Draw(Vector2 screenPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            drawingPanel, screenPos, null, out Vector2 localPos
            );
        int x = (int)(localPos .x + texture.width * 0.5f);
        int y = (int)(localPos.y + texture.height * 0.5f);

        for(int dx =-brushSize; dx <= brushSize;  dx++)
            for (int dy = -brushSize; dy <= brushSize; dy++)
            {
                int px = Mathf.Clamp(x + dx, 0, texture.width - 1);
                int py = Mathf.Clamp(y + dy, 0, texture.height - 1);

                texture.SetPixel(px, py, brushColor);
            }
        texture.Apply();
    }
}
