using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CubeView
{
    public Rigidbody CubeRb { get; private set; }
    public Text ResultText { get; private set; }
    public Button JumpButton { get; private set; }
    public Button RestartButton { get; private set; }

    public void SetupScene()
    {
        // Create Cube
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(0f, 5f, 0f);
        CubeRb = cube.AddComponent<Rigidbody>();

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

        // Event System for UI interaction
        GameObject eventSystemGO = new GameObject("EventSystem");
        eventSystemGO.AddComponent<EventSystem>();
        eventSystemGO.AddComponent<StandaloneInputModule>();

        // Jump Button
        GameObject buttonGO = new GameObject("JumpButton");
        buttonGO.transform.SetParent(canvasGO.transform);
        JumpButton = buttonGO.AddComponent<Button>();
        Image btnImage = buttonGO.AddComponent<Image>();
        btnImage.color = Color.white;
        RectTransform btnRect = buttonGO.GetComponent<RectTransform>();
        btnRect.sizeDelta = new Vector2(160, 30);
        btnRect.anchoredPosition = new Vector2(0, 50);
        Text btnText = new GameObject("Text").AddComponent<Text>();
        btnText.transform.SetParent(buttonGO.transform);
        btnText.text = "Jump";
        btnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        btnText.alignment = TextAnchor.MiddleCenter;
        btnText.color = Color.black;
        RectTransform textRect = btnText.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        // Restart Button
        GameObject restartGO = new GameObject("RestartButton");
        restartGO.transform.SetParent(canvasGO.transform);
        RestartButton = restartGO.AddComponent<Button>();
        Image restartImage = restartGO.AddComponent<Image>();
        restartImage.color = Color.white;
        RectTransform restartRect = restartGO.GetComponent<RectTransform>();
        restartRect.sizeDelta = new Vector2(160, 30);
        restartRect.anchoredPosition = new Vector2(0, 10);
        Text restartText = new GameObject("Text").AddComponent<Text>();
        restartText.transform.SetParent(restartGO.transform);
        restartText.text = "Restart";
        restartText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        restartText.alignment = TextAnchor.MiddleCenter;
        restartText.color = Color.black;
        RectTransform restartTextRect = restartText.GetComponent<RectTransform>();
        restartTextRect.anchorMin = Vector2.zero;
        restartTextRect.anchorMax = Vector2.one;
        restartTextRect.offsetMin = Vector2.zero;
        restartTextRect.offsetMax = Vector2.zero;

        // Result Text
        GameObject textGO = new GameObject("ResultText");
        textGO.transform.SetParent(canvasGO.transform);
        ResultText = textGO.AddComponent<Text>();
        ResultText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        ResultText.fontSize = 24;
        ResultText.alignment = TextAnchor.MiddleCenter;
        RectTransform resultRect = ResultText.GetComponent<RectTransform>();
        resultRect.sizeDelta = new Vector2(300, 40);
        resultRect.anchoredPosition = new Vector2(0, -50);
    }

    public void ShowResult(float time, float highScore)
    {
        if (ResultText != null)
        {
            ResultText.text = $"You lasted {time:F2} seconds
Best: {highScore:F2} seconds";
        }
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
