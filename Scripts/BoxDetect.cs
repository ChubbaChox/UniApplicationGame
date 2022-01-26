using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxDetect : MonoBehaviour
{
 
    [field: SerializeField]
    public bool PlayerDetected { get; private set; }

    [Header("OverlapBox parameters")]
    [SerializeField]
    private Transform detectorOrigin;
    public Vector2 detectorSize = Vector2.one;
    public Vector2 detectorOriginOffset = Vector2.zero;

    public LayerMask detectorLayerMask;

    [Header("Gizmo parameters")]
    public Color gizmoIdleColor = Color.green;
    public Color gizmoDetectedColor = Color.red;
    public bool showGizmos = true;

    private GameObject target;

    public GameObject Target
    {
        get => target;
        private set
        {
            target = value;
            PlayerDetected = target != null;
        }
    }

    private bool ambushWaiting = false;
    public float ambushWaitFor = 1f;

    public void OnCollisionEnter2D()
    {
        if (!ambushWaiting)
        {

            if (PlayerDetected)
            {
                if (UnityEngine.Random.Range(1, 101) <= 10)
                {
                    GameController.Instance.BeginBossFight(this);
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
    public void Update()
    {
        var collider = Physics2D.OverlapBox((Vector2)detectorOrigin.position + detectorOriginOffset, detectorSize, 0, detectorLayerMask);
        PlayerDetected = collider != null;
        if (PlayerDetected)
            OnPlayerDetected();
    }

    private void OnDrawGizmos()
    {
        if (showGizmos && detectorOrigin != null)
        {
            Gizmos.color = gizmoIdleColor;
            if (PlayerDetected)
                Gizmos.color = gizmoDetectedColor;
            Gizmos.DrawCube((Vector2)detectorOrigin.position + detectorOriginOffset, detectorSize);
        }
    }
}
