using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float speed = 4f;
    private float leftLimit = -12f;

    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
        if (transform.position.x < leftLimit)
        {
            gameObject.SetActive(false);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CubeGame game = FindObjectOfType<CubeGame>();
            if (game != null)
            {
                game.TriggerGameOver();
            }
        }
    }
}
