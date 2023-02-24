using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    Camera cam;

    private void Update()
    {
        if(cam == null)
        {
            cam = FindObjectOfType<Camera>();
        }
        if (cam == null)
            return;
        transform.LookAt(cam.transform);
    }
}
