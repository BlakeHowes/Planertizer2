using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCon : MonoBehaviour
{
    [SerializeField]
    private float CameraSpeed;
    [SerializeField]
    private float ScrollSpeed;
    [SerializeField]
    private float ScrollRange;
    private float CameraHeightOnStart;
    private float CameraSpeedOnStart;
    [SerializeField]
    private Camera OrthographicCamera;

    private void Awake()
    {
        CameraHeightOnStart = transform.position.y;
        CameraSpeedOnStart = CameraSpeed;
    }

    void Update()
    {
        float Scroll = (Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed * Time.deltaTime);
        OrthographicCamera.orthographicSize -= Scroll;

        if (Input.GetMouseButton(2))
        {
            Vector3 PosChange = new Vector3
                (
                 Input.GetAxis("Mouse X") * CameraSpeed * Time.deltaTime,
                 0f,
                 Input.GetAxis("Mouse Y") * CameraSpeed * Time.deltaTime
                );
            CameraSpeed = CameraSpeedOnStart * ((OrthographicCamera.orthographicSize) /120f);
            transform.position -= PosChange;
        }
    }
}
