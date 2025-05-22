using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CubeView : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject canvasPrefab;

    public Rigidbody CubeRb { get; private set; }
    public Text ResultText { get; private set; }
    public Text TimerText { get; private set; }
    public Button JumpButton { get; private set; }
    public Button RestartButton { get; private set; }

    public void SetupScene()
    {
        // Create Cube
        GameObject cube = null;
        if (cubePrefab != null)
        {
            cube = Instantiate(cubePrefab, new Vector3(0f, 5f, 0f), Quaternion.identity);
        }
        else
        {
            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(0f, 5f, 0f);
        }
        cube.tag = "Player";
        CubeRb = cube.GetComponent<Rigidbody>();
        if (CubeRb == null)
            CubeRb = cube.AddComponent<Rigidbody>();

        // Create Ground
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(3f, 1f, 3f); // wider so it fills the view
        ground.AddComponent<BoxCollider>();
        ground.GetComponent<Renderer>().material = CreateStripedMaterial();
        ScrollingTexture groundScroll = ground.AddComponent<ScrollingTexture>();

        // Obstacle prefab
        Obstacle obstacleComponent = obstaclePrefab != null ? obstaclePrefab.GetComponent<Obstacle>() : null;
        if (obstacleComponent == null)
        {
            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obstacleComponent = temp.AddComponent<Obstacle>();
            temp.AddComponent<BoxCollider>();
            temp.GetComponent<Renderer>().material.color = Color.red;
            obstaclePrefab = temp;
            obstaclePrefab.SetActive(false);
        }
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
        GameObject canvasGO = null;
        if (canvasPrefab != null)
        {
            canvasGO = Instantiate(canvasPrefab);
        }
        else
        {
            canvasGO = new GameObject("Canvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            // Event System for UI interaction
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();

            // Create UI elements programmatically
            GameObject jumpGO = CreateButton("JumpButton", canvasGO.transform, "Jump", new Vector2(0f, 50f));
            JumpButton = jumpGO.GetComponent<Button>();

            GameObject restartGO = CreateButton("RestartButton", canvasGO.transform, "Restart", new Vector2(0f, 10f));
            RestartButton = restartGO.GetComponent<Button>();

            GameObject timerGO = new GameObject("TimerText");
            timerGO.transform.SetParent(canvasGO.transform);
            TimerText = timerGO.AddComponent<Text>();
            TimerText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            TimerText.alignment = TextAnchor.UpperCenter;
            TimerText.fontSize = 32;
            RectTransform timerRect = timerGO.GetComponent<RectTransform>();
            timerRect.sizeDelta = new Vector2(200f, 40f);
            timerRect.anchorMin = new Vector2(0.5f, 1f);
            timerRect.anchorMax = new Vector2(0.5f, 1f);
            timerRect.pivot = new Vector2(0.5f, 1f);
            timerRect.anchoredPosition = new Vector2(0f, -70f);

            GameObject resultGO = new GameObject("ResultText");
            resultGO.transform.SetParent(canvasGO.transform);
            ResultText = resultGO.AddComponent<Text>();
            ResultText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            ResultText.alignment = TextAnchor.UpperCenter;
            RectTransform resultRect = resultGO.GetComponent<RectTransform>();
            resultRect.sizeDelta = new Vector2(400f, 30f);
            resultRect.anchorMin = new Vector2(0.5f, 1f);
            resultRect.anchorMax = new Vector2(0.5f, 1f);
            resultRect.pivot = new Vector2(0.5f, 1f);
            resultRect.anchoredPosition = new Vector2(0f, -30f);
        }

        // Jump Button
        Transform jumpT = canvasGO.transform.Find("JumpButton");
        if (jumpT != null)
        {
            JumpButton = jumpT.GetComponent<Button>();
            Image img = JumpButton.GetComponent<Image>();
            if (img != null)
                img.color = new Color(0f, 1f, 0f, 0.5f);

            RectTransform rect = JumpButton.GetComponent<RectTransform>();
            if (rect != null)
                rect.sizeDelta = new Vector2(300f, 80f);
        }

        // Restart Button
        Transform restartT = canvasGO.transform.Find("RestartButton");
        if (restartT != null)
        {
            RestartButton = restartT.GetComponent<Button>();
            Image img = RestartButton.GetComponent<Image>();
            if (img != null)
                img.color = new Color(1f, 1f, 1f, 0.5f);

            RectTransform rect = RestartButton.GetComponent<RectTransform>();
            if (rect != null)
                rect.sizeDelta = new Vector2(300f, 80f);

            RestartButton.gameObject.SetActive(false);
        }

        // Result Text
        Transform resultT = canvasGO.transform.Find("ResultText");
        if (resultT != null)
        {
            ResultText = resultT.GetComponent<Text>();
        }

        // Timer Text
        Transform timerT = canvasGO.transform.Find("TimerText");
        if (timerT != null)
        {
            TimerText = timerT.GetComponent<Text>();
            TimerText.fontSize = 32;
        }
    }

    public void ShowResult(float time, float highScore)
    {
        if (ResultText != null)
        {
            ResultText.text = $"You lasted {time:F2} seconds Best: {highScore:F2} seconds";
        }

        if (JumpButton != null)
            JumpButton.gameObject.SetActive(false);

        if (RestartButton != null)
            RestartButton.gameObject.SetActive(true);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void UpdateTimer(float time)
    {
        if (TimerText != null)
        {
            TimerText.text = time.ToString("F2");
        }
    }

    GameObject CreateButton(string name, Transform parent, string label, Vector2 position)
    {
        GameObject btnGO = new GameObject(name);
        btnGO.transform.SetParent(parent);

        Image img = btnGO.AddComponent<Image>();
        img.color = Color.white;
        btnGO.AddComponent<Button>();

        RectTransform rect = btnGO.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(160f, 30f);
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;

        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(btnGO.transform);
        Text txt = textGO.AddComponent<Text>();
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.alignment = TextAnchor.MiddleCenter;
        txt.text = label;
        RectTransform txtRect = textGO.GetComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.offsetMin = Vector2.zero;
        txtRect.offsetMax = Vector2.zero;

        return btnGO;
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
