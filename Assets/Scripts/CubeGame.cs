using UnityEngine;
using UnityEngine.UI;

public class CubeGame : MonoBehaviour
{
    public float jumpForce = 5f;

    private Rigidbody cubeRb;
    private bool gameOver = false;
    private float startTime;
    private Text resultText;
    private Button jumpButton;

    void Start()
    {
        SetupScene();
        startTime = Time.time;
    }

    void SetupScene()
    {
        // Create Cube
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(0f, 5f, 0f);
        cubeRb = cube.AddComponent<Rigidbody>();

        // Create Ground
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.transform.position = Vector3.zero;
        ground.AddComponent<BoxCollider>();

        // Camera setup
        if (Camera.main != null)
        {
            Camera.main.transform.position = new Vector3(0f, 3f, -8f);
            Camera.main.transform.LookAt(cube.transform);
        }

        // Create Canvas
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Button
        GameObject buttonGO = new GameObject("JumpButton");
        buttonGO.transform.SetParent(canvasGO.transform);
        jumpButton = buttonGO.AddComponent<Button>();
        Image btnImage = buttonGO.AddComponent<Image>();
        btnImage.color = Color.white;
        RectTransform btnRect = buttonGO.GetComponent<RectTransform>();
        btnRect.sizeDelta = new Vector2(160, 30);
        btnRect.anchoredPosition = new Vector2(0, 50);
        Text btnText = new GameObject("Text").AddComponent<Text>();
        btnText.transform.SetParent(buttonGO.transform);
        btnText.text = "Jump";
        btnText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        btnText.alignment = TextAnchor.MiddleCenter;
        btnText.color = Color.black;
        RectTransform textRect = btnText.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        jumpButton.onClick.AddListener(ApplyJump);

        // Result Text
        GameObject textGO = new GameObject("ResultText");
        textGO.transform.SetParent(canvasGO.transform);
        resultText = textGO.AddComponent<Text>();
        resultText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        resultText.fontSize = 24;
        resultText.alignment = TextAnchor.MiddleCenter;
        RectTransform resultRect = resultText.GetComponent<RectTransform>();
        resultRect.sizeDelta = new Vector2(300, 40);
        resultRect.anchoredPosition = new Vector2(0, -50);
    }

    void ApplyJump()
    {
        if (!gameOver && cubeRb != null)
        {
            cubeRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void Update()
    {
        if (gameOver || cubeRb == null) return;

        if (cubeRb.transform.position.y <= 0.5f)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        gameOver = true;
        float timeInAir = Time.time - startTime;
        if (resultText != null)
        {
            resultText.text = $"You lasted {timeInAir:F2} seconds";
        }
    }
}
