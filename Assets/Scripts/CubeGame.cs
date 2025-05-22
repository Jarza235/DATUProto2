using UnityEngine;

public class CubeGame : MonoBehaviour
{
    public float jumpForce = 50f;

    private CubeModel model;
    [SerializeField]
    private CubeView view;

    const float floorLimit = 0.5f;
    const float ceilingLimit = 9.5f;

    void Start()
    {
        model = new CubeModel();
        if (view == null)
        {
            view = FindObjectOfType<CubeView>();
        }

        if (view != null)
        {
            view.SetupScene();
            model.StartGame();

            view.JumpButton.onClick.AddListener(ApplyJump);
            view.RestartButton.onClick.AddListener(view.RestartScene);
        }
    }

    void ApplyJump()
    {
        if (!model.GameOver && view.CubeRb != null)
        {
            view.CubeRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public bool GameOver => model.GameOver;

    public void TriggerGameOver()
    {
        if (model.GameOver) return;
        float timeInAir = model.EndGame();
        view.ShowResult(timeInAir, model.HighScore);
    }

    void Update()
    {
        if (model.GameOver || view.CubeRb == null) return;

        if (view.CubeRb.transform.position.y <= floorLimit || view.CubeRb.transform.position.y >= ceilingLimit)
        {
            TriggerGameOver();
        }
    }
}
