using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private const float YMin = -50.0f;
    private const float YMax = 50.0f;

    public Transform lookAt;

    public Transform Player;

    public float extraY = 1.0f;

    public float mouseWheel = 2.0f;

    public float distance = 10.0f;
    public float currentX = 0.0f;
    public float currentY = 0.0f;
    public float sensivity = 4.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Zoom only affects distance, not rotation
        distance -= Input.GetAxis("Mouse ScrollWheel") * mouseWheel;
        distance = Mathf.Clamp(distance, 2, 10);

        if (Input.GetButton("Fire2"))
        {
            currentX -= Input.GetAxis("Mouse X") * sensivity * Time.deltaTime;
            currentY -= Input.GetAxis("Mouse Y") * sensivity * Time.deltaTime;
            currentY = Mathf.Clamp(currentY, YMin, YMax);
        }

        // Apply rotation independent of zoom
        Vector3 Direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, -currentX, 0);
        Vector3 lookat_position = lookAt.position + new Vector3(0, extraY, 0);

        transform.position = lookat_position + rotation * Direction;
        transform.LookAt(lookat_position);
    }
}