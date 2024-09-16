using UnityEngine;

public class ObjectFollower : MonoBehaviour
{
    [SerializeField] private GameObject followTarget;
    [SerializeField] private bool followOnPhysicsUpdate = false;
    [SerializeField] private bool keepPositionZ = false;

    private float positionZ;

    private void Awake()
    {
        positionZ = transform.position.z;
    }

    private void Update()
    {
        if (!followOnPhysicsUpdate)
        {
            Follow();
        }
    }

    private void FixedUpdate()
    {
        if (followOnPhysicsUpdate)
        {
            Follow();
        }
    }

    /// <summary>
    /// called on every tick
    /// </summary>
    private void Follow()
    {
        var targetPosition = followTarget.transform.position;
        if (keepPositionZ)
        {
            targetPosition.z = positionZ;
        }
        transform.position = targetPosition;
    }
}
