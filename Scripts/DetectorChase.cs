using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectorChase : Chase
{
    public LayerMask targetLayer; //using a gizmo to detect and then chase the player as to get close enough to begin a fight
    Chase chase;
    public void OnPlayerDetected()
    {
        BeginChase();
    }

    [Range(.1f, 10)]
    public float radius;

    [Header("Gizmo para")]
    public Color gizmoColor = Color.red;
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

