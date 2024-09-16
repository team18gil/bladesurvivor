using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundPlane : MonoBehaviour
{
    public delegate void PlaneTriggerDelegate(BackgroundPlane plane);
    public PlaneTriggerDelegate TriggerCallback { set; private get; } = null;


    public Vector2 PlaneSize { get; private set; } = Vector2.zero;
    private void Awake()
    {
        if (TryGetComponent(out BoxCollider2D collider))
        {
            // planes' size and position was hard-coded for faster running
            PlaneSize = collider.size;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TriggerCallback.Invoke(this);
        }
    }
}
