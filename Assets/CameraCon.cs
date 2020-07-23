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

    private void Awake()
    {
        CameraHeightOnStart = transform.position.y;
    }

    void Update()
    {
        Vector3 Scroll = new Vector3(0f, Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed * Time.deltaTime, 0f);
        transform.position -= Scroll;

        if (Input.GetMouseButton(2))
        {
            Vector3 PosChange = new Vector3
                (
                 Input.GetAxis("Mouse X") * CameraSpeed * Time.deltaTime,
                 0f,
                 Input.GetAxis("Mouse Y") * CameraSpeed * Time.deltaTime
                );
            transform.position -= PosChange;
        }
    }
}
