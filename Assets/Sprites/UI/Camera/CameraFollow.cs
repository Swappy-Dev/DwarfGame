using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -10);

    // Naudojame FixedUpdate, kad kamera judėtų IDENTIŠKAI su žaidėjo fizika
    void FixedUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}