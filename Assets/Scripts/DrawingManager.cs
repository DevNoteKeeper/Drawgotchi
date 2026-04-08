using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class DrawingManager : MonoBehaviour
{
    [SerializeField] private RectTransform drawingPanel;
    [SerializeField] private RawImage drawingCanvas;

    private Texture2D texture;
    private bool isDrawing = false;

    [SerializeField] private Color brushColor = Color.black;
    [SerializeField] private int brushSize = 6;

    private Vector2? lastPosition = null;

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
                RectTransformUtility.ScreenPointToLocalPointInRectangle(drawingPanel, screenPos, null, out Vector2 localPos);
                Vector2 currentPos = new Vector2(
                    localPos.x + texture.width * 0.5f,
                     localPos.y + texture.height * 0.5f
                    );

                if (lastPosition.HasValue)
                {
                    DrawLine(lastPosition.Value, currentPos);
                }
                else
                {
                    DrawPoint(currentPos);
                    texture.Apply();
                }

                lastPosition = currentPos;

            }
            else
            {
                lastPosition = null;
            }
        }
        else
        {
            lastPosition = null;
        }

    }

    private void DrawPoint(Vector2 point)
    {
        int x = (int)point.x;
        int y = (int)point.y;

        for(int dx = -brushSize; dx <= brushSize; dx++)
        {
            for(int dy = -brushSize; dy <= brushSize; dy++)
            {
                if(dx*dx+dy*dy <= brushSize * brushSize)
                {
                    int px = Mathf.Clamp(x + dx, 0, texture.width - 1);
                    int py = Mathf.Clamp(y + dy, 0, texture.height - 1);
                    texture.SetPixel(px, py, brushColor);
                }
            }
        }
    }

    private void DrawLine(Vector2 from, Vector2 to) 
    {
        float dist = Vector2.Distance(from, to);
        int steps = Mathf.CeilToInt(dist);

        for(int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector2 point = Vector2.Lerp(from, to, t);
            DrawPoint(point);
        }
        texture.Apply();
    }

    public void SendDrawing()
    {
        byte[] imageData = texture.EncodeToPNG();
        string filePath = "C:/Twente/M7/Drawgotchi/Assets/Images/image.png";
        File.WriteAllBytes(filePath, imageData);

        ClearCanvas();

        Debug.Log($"Image size: {imageData.Length} bytes");
        Debug.Log($"{drawingPanel.rect.width}, {drawingPanel.rect.height}");
        Debug.Log("Send");
    }
}
