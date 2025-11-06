using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; 

    private Vector3 offset; 

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Kamera takip hedefi (Target) atanmamýþ!");
            return;
        }

        offset = transform.position - target.position;
    }

 
    void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position + offset;
    }
}