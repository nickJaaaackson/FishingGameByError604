using UnityEngine;
using UnityEngine.Rendering;

public class CameraControll : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5;
    public Vector3 offset;
    
    private void LateUpdate()
    {
        if (target == null)
        {
             Player p = FindAnyObjectByType<Player>();  
            if (p != null)
            {
                target = p.transform;
            }
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedposition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = new Vector3(smoothedposition.x,smoothedposition.y,transform.position.z);
    }
}
