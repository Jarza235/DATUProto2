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
        }

        // Jump Button
        Transform jumpT = canvasGO.transform.Find("JumpButton");
        if (jumpT != null)
        {
            JumpButton = jumpT.GetComponent<Button>();
        }

        // Restart Button
        Transform restartT = canvasGO.transform.Find("RestartButton");
        if (restartT != null)
        {
            RestartButton = restartT.GetComponent<Button>();
        }

        // Result Text
        Transform resultT = canvasGO.transform.Find("ResultText");
        if (resultT != null)
        {
            ResultText = resultT.GetComponent<Text>();
        }
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
