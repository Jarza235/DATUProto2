using UnityEngine;

public class CubeGame : MonoBehaviour
{
    public float jumpForce = 50f;

    private CubeModel model;
    private CubeView view;

    void Start()
    {
        model = new CubeModel();
        view = new CubeView();
        view.SetupScene();
        model.StartGame();

        view.JumpButton.onClick.AddListener(ApplyJump);
        view.RestartButton.onClick.AddListener(view.RestartScene);
    }

    void ApplyJump()
    {
        if (!model.GameOver && view.CubeRb != null)
        {
            view.CubeRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void Update()
    {
        if (model.GameOver || view.CubeRb == null) return;

        if (view.CubeRb.transform.position.y <= 0.5f)
        {
            float timeInAir = model.EndGame();
            view.ShowResult(timeInAir, model.HighScore);
        }
    }
}
