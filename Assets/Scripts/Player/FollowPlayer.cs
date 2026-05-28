using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform target;
    void LateUpdate()
    {
        transform.position = target.position;
    }
}
