using UnityEngine;

public class ParallaxFollow : MonoBehaviour
{
    public Transform cam;        
    public float parallaxAmount;   

    private Vector3 lastCamPos;

    private void Start()
    {
        if (cam == null)
            cam = Camera.main.transform;

        lastCamPos = cam.position;
    }

    private void LateUpdate()
    {
        Vector3 delta = cam.position - lastCamPos;

       
        transform.position += new Vector3(delta.x * parallaxAmount, 0f, 0f);

        lastCamPos = cam.position;
    }
}


