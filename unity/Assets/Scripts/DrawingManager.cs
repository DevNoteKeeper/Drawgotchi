using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using System.Security;

[System.Serializable]
public class PredictResponse
{
    public string label;
    public float confidence;
}

public class DrawingManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform drawingPanel;
    [SerializeField] private RawImage drawingCanvas;

    [Header("Brush")]
    [SerializeField] private Color brushColor = Color.black;
    [SerializeField] private int brushSize = 6;

    [Header("Inference")]
    [SerializeField] private string endpoint = "https://drawgotchi.onrender.com/predict";
    [SerializeField] private float unknownThreshold = 0.55f;

    private Texture2D texture;
    private Vector2? lastPosition = null;

    public GameManager gameManager;

    private void Start()
    {
        int width = Mathf.RoundToInt(drawingPanel.rect.width);   // e.g. 480
        int height = Mathf.RoundToInt(drawingPanel.rect.height); // e.g. 310

        // RGBA32 + no mipmaps for drawing canvas
        texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        drawingCanvas.texture = texture;

        ClearCanvas();

        StartCoroutine(WaitForServer());
    }

    private IEnumerator WaitForServer()
    {
        string healthUrl = endpoint.Replace("/predict", "/health");

        while(true){
            using (var req = UnityWebRequest.Get(healthUrl))
            {
                req.timeout = 5;
                yield return req.SendWebRequest();

                if(req.result == UnityWebRequest.Result.Success)
                {
                    gameManager.ToBaby();
                    yield break;
                }
            }
            yield return new WaitForSeconds(5f);
        }
    }

    public void ClearCanvas()
    {
        Pointer pointer = Pen.current ?? Mouse.current as Pointer;
        if (texture == null) return;

        Color[] pixels = new Color[texture.width * texture.height];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;

        texture.SetPixels(pixels);
        texture.Apply();
    }

    private void Update()
    {
        var pointer = Pointer.current;
        if (pointer == null)
        {
            lastPosition = null;
            return;
        }

        if (!pointer.press.isPressed)
        {
            lastPosition = null;
            return;
        }

        Vector2 screenPos = pointer.position.ReadValue();

        if (!RectTransformUtility.RectangleContainsScreenPoint(drawingPanel, screenPos, null))
        {
            lastPosition = null;
            return;
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            drawingPanel, screenPos, null, out Vector2 localPos
        );

        Vector2 currentPos = new Vector2(
            localPos.x + texture.width * 0.5f,
            localPos.y + texture.height * 0.5f
        );

        if (lastPosition.HasValue)
            DrawLine(lastPosition.Value, currentPos);
        else
            DrawPoint(currentPos);

        texture.Apply();
        lastPosition = currentPos;
    }

    private void DrawPoint(Vector2 point)
    {
        int x = Mathf.RoundToInt(point.x);
        int y = Mathf.RoundToInt(point.y);

        for (int dx = -brushSize; dx <= brushSize; dx++)
        {
            for (int dy = -brushSize; dy <= brushSize; dy++)
            {
                if (dx * dx + dy * dy > brushSize * brushSize) continue;

                int px = Mathf.Clamp(x + dx, 0, texture.width - 1);
                int py = Mathf.Clamp(y + dy, 0, texture.height - 1);
                texture.SetPixel(px, py, brushColor);
            }
        }
    }

    private void DrawLine(Vector2 from, Vector2 to)
    {
        float dist = Vector2.Distance(from, to);
        int steps = Mathf.CeilToInt(dist);

        if (steps <= 0)
        {
            DrawPoint(from);
            return;
        }

        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector2 p = Vector2.Lerp(from, to, t);
            DrawPoint(p);
        }
    }

    public void SendDrawing()
    {
        if(gameManager.State == GameState.Egg || gameManager.State == GameState.Hatching)
        {
            gameManager.UnkownDrawing("Shh..... the egg needs peace");
            ClearCanvas();
            return;
        }
        StartCoroutine(SendDrawingCoroutine());
    }

    private IEnumerator SendDrawingCoroutine()
    {
        byte[] imageData = texture.EncodeToPNG();

        using (var req = new UnityWebRequest(endpoint, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(imageData);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "image/png");

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Prediction request failed: " + req.error);
                yield break;
            }

            var json = req.downloadHandler.text;
            PredictResponse result = JsonUtility.FromJson<PredictResponse>(json);

            if (result == null)
            {
                Debug.LogError("Invalid JSON from server: " + json);
                yield break;
            }

            string finalLabel = (result.confidence < unknownThreshold) ? "unknown" : result.label;

            if (finalLabel != "unknown")
            {
                if (finalLabel == "bed")
                {
                    gameManager.Sleep(finalLabel);
                }
                else {
                    gameManager.Feed(finalLabel);
                }
            } else {
                gameManager.UnkownDrawing("What's that?! Ewwwww....");
                Debug.Log("Unrecognized drawing: " + json);
            }
            
        }

        ClearCanvas();
    }
}