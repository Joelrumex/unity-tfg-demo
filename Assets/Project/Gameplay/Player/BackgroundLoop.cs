using UnityEngine;

public class BackgroundLoop : MonoBehaviour
{
    private float width;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        width = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        float camX = cam.transform.position.x;

        // If background is far behind camera, move it forward
        if (transform.position.x + width < camX)
        {
            transform.position += new Vector3(width * 2f, 0, 0);
        }
    }
}