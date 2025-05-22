using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ScrollingTexture : MonoBehaviour
{
    public float worldScrollSpeed = 4f;
    public Vector2 scrollDirection = Vector2.left;

    private Renderer rend;
    private Vector2 offset;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend.material != null)
        {
            // Tile the texture based on object scale so pattern repeats across width
            Vector2 scale = rend.material.mainTextureScale;
            scale.x = transform.localScale.x;
            rend.material.mainTextureScale = scale;
        }
    }

    void Update()
    {
        if (rend == null || rend.material == null) return;

        float uvWidth = 10f * transform.localScale.x; // world units that correspond to 1 offset unit
        if (uvWidth <= 0f) return;
        float delta = (worldScrollSpeed / uvWidth) * Time.deltaTime;
        offset += scrollDirection.normalized * delta;
        rend.material.mainTextureOffset = offset;
    }
}
