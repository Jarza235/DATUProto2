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
        cube.tag = "Player";
        CubeRb = cube.AddComponent<Rigidbody>();

        // Create Ground
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(3f, 1f, 3f); // wider so it fills the view
        ground.AddComponent<BoxCollider>();
        ground.GetComponent<Renderer>().material = CreateStripedMaterial();
        ScrollingTexture groundScroll = ground.AddComponent<ScrollingTexture>();

        // Obstacle prefab
        GameObject obstaclePrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obstaclePrefab.GetComponent<Renderer>().material.color = Color.red;
        obstaclePrefab.AddComponent<BoxCollider>();
        Obstacle obstacleComponent = obstaclePrefab.AddComponent<Obstacle>();
        obstaclePrefab.SetActive(false);
        groundScroll.worldScrollSpeed = obstacleComponent.speed;

        // Spawner
        ObstacleSpawner spawner = new GameObject("Spawner").AddComponent<ObstacleSpawner>();
        spawner.obstaclePrefab = obstaclePrefab;
        spawner.minY = 1f;
        spawner.maxY = 9f;
        spawner.floorLimit = 0.5f;
        spawner.ceilingLimit = 9.5f;

        // Create Ceiling
        GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ceiling.transform.position = new Vector3(0f, 10f, 0f);
        ceiling.transform.rotation = Quaternion.Euler(-180f, 0f, 0f);
        ceiling.transform.localScale = new Vector3(3f, 1f, 3f); // match ground width
        ceiling.AddComponent<BoxCollider>();
        ceiling.GetComponent<Renderer>().material = CreateStripedMaterial();
        ScrollingTexture ceilingScroll = ceiling.AddComponent<ScrollingTexture>();
        ceilingScroll.worldScrollSpeed = obstacleComponent.speed;

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
        ResultText.horizontalOverflow = HorizontalWrapMode.Overflow;
        ResultText.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform resultRect = ResultText.GetComponent<RectTransform>();
        resultRect.sizeDelta = new Vector2(300, 40);
        resultRect.anchoredPosition = new Vector2(0, -50);
    }

    public void ShowResult(float time, float highScore)
    {
        if (ResultText != null)
        {
            ResultText.text = $"You lasted {time:F2} seconds Best: {highScore:F2} seconds";
        }
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    Material CreateStripedMaterial()
    {
        Material mat = new Material(Shader.Find("Unlit/Texture"));
        const int size = 64;
        Texture2D tex = new Texture2D(size, size);
        Color c1 = new Color(0.7f, 0.7f, 0.7f, 1f);
        Color c2 = new Color(0.5f, 0.5f, 0.5f, 1f);
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                bool even = (x / 8) % 2 == 0;
                tex.SetPixel(x, y, even ? c1 : c2);
            }
        }
        tex.Apply();
        tex.wrapMode = TextureWrapMode.Repeat;
        mat.mainTexture = tex;
        return mat;
    }
}
