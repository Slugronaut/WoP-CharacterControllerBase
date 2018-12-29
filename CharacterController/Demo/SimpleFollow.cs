using UnityEngine;

public class SimpleFollow : MonoBehaviour
{
    public Transform Target;
    public Vector3 Offset;

    void Update()
    {
        transform.position = Target.position + Offset;
    }
}
