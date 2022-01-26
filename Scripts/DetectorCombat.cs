using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectorCombat : MonoBehaviour
{
    public event Action OnAmbushed;
    public LayerMask targetLayer; //using a gizmo, if player enters the radius it will begin a OnAmbushed fight 

    public void OnPlayerDetected()
    {
        OnAmbushed();
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


