using UnityEngine;

public class ARTransformLocker : MonoBehaviour
{
    private Vector3 lockedPosition;
    private Quaternion lockedRotation;
    private Vector3 lockedScale;

    public void Lock(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        lockedPosition = pos;
        lockedRotation = rot;
        lockedScale = scale;
    }

    private void LateUpdate()
    {
        transform.position = lockedPosition;
        transform.rotation = lockedRotation;
        transform.localScale = lockedScale;
    }
}
