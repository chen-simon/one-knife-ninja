using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float smoothness = 0.05f;
    public Vector3 offset;

    float targetZoom;
    Camera main;

    private void Start()
    {
        transform.position = PlayerController.controller.transform.position + new Vector3(0, 5, 0);
        main = GetComponent<Camera>();
    }
    void FixedUpdate()
    {
        float x = Mathf.Lerp(transform.position.x, PlayerController.controller.transform.position.x + offset.x, smoothness);
        float y = Mathf.Lerp(transform.position.y, PlayerController.controller.transform.position.y + offset.y, smoothness);
        transform.position = new Vector3(x, y, -10);
    }

    private void Update()
    {
        if (PlayerController.controller.hiding) { targetZoom = 4; }
        else { targetZoom = 5; }

        if (main.orthographicSize != targetZoom)
        {
            main.orthographicSize = Mathf.Lerp(main.orthographicSize, targetZoom, 0.05f);
        }
    }
}
