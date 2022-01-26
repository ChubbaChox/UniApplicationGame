using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BZDetector : MonoBehaviour
{
    public event Action OnAmbushed;
    public LayerMask targetLayer;
    private bool ambushWaiting = false;
    public float ambushWaitFor = 1f;

    void OnCollisionEnter2D()
    {
        if (!ambushWaiting)
        {
           
            if (PlayerDetected)
            {
                if (UnityEngine.Random.Range(1, 101) <= 10)
                {
                    OnAmbushed();
                }
                else
                {
                    StartCoroutine(AmbushWaitTime());
                }
            }
        }
    }

    IEnumerator AmbushWaitTime()
    {
        ambushWaiting = true;
        yield return new WaitForSeconds(ambushWaitFor);
        ambushWaiting = false;
    }
    public void OnPlayerDetected()
    {
        OnCollisionEnter2D();
    }

    [Range(.1f, 10)]
    public float radius;

    [Header("Gizmo para")]
    public Color gizmoColor = Color.green;
    public bool showGizmos = true;

    public bool PlayerDetected { get; internal set; }

    private void Update()
    {
        var collider = Physics2D.OverlapCircle(transform.position, radius, targetLayer);
        PlayerDetected = collider != null;
        if (PlayerDetected)
            OnPlayerDetected();
    }

    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(transform.position, radius);
        }
    }
}


