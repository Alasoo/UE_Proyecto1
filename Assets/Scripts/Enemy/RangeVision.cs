using System;
using Controller.Player;
using UnityEngine;

public class RangeVision : MonoBehaviour
{
    [field: SerializeField] public CircleCollider2D visionTrigger { get; private set; }


    public event Action<bool> OnPlayerEnter;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != PlayerStateMachine.Instance.gameObject) return;
        OnPlayerEnter?.Invoke(true);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject != PlayerStateMachine.Instance.gameObject) return;
        OnPlayerEnter?.Invoke(false);
    }

}