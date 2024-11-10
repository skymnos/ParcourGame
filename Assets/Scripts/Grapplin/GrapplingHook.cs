using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrapplingHook : MonoBehaviour
{
    private Rigidbody2D rb;
    private LineRenderer lineRenderer;
    private DistanceJoint2D distanceJoint2D;
    private List<GameObject> hookPoints = new List<GameObject>();
    private GameObject selectedHookPoint = null;
    private bool hooked;
    private float distanceMax;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        distanceJoint2D = GetComponent<DistanceJoint2D>();
        lineRenderer.enabled = false;
        distanceJoint2D.enabled = false;
    }

    private void Update()
    {
        float distance = float.MaxValue;
        foreach (GameObject hookPoint in hookPoints)
        {
            if (Vector2.Distance(transform.position, hookPoint.transform.position) < distance)
            {
                distance = Vector2.Distance(transform.position, hookPoint.transform.position);
                if (selectedHookPoint != hookPoint && selectedHookPoint != null)
                {
                    selectedHookPoint.GetComponent<SpriteRenderer>().color = Color.white;
                }
                selectedHookPoint = hookPoint;
                selectedHookPoint.GetComponent<SpriteRenderer>().color = Color.green;
            }
        }

        if(hooked)
        {
            Vector2 playerToHookpointVect = new Vector2(selectedHookPoint.transform.position.x - transform.position.x, selectedHookPoint.transform.position.y - transform.position.y);
            float distanceToHookpoint = Vector2.Distance(transform.position, selectedHookPoint.transform.position);
            if (Vector2.Angle(rb.velocity, playerToHookpointVect) < 45)
            {
                distanceJoint2D.enabled = false;
            }
            else if (Vector2.Angle(rb.velocity, playerToHookpointVect) >= 45 && distanceToHookpoint >= distanceMax)
            {
                distanceJoint2D.enabled = true;
                distanceJoint2D.connectedAnchor = selectedHookPoint.transform.position;
            }

            lineRenderer.SetPosition(0, transform.position);
        }
        else
        {
            distanceJoint2D.enabled = false;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hookable"))
        {
             hookPoints.Add(collision.gameObject);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Hookable"))
        {
            if (collision.gameObject == selectedHookPoint && !hooked)
            {
                selectedHookPoint.GetComponent<SpriteRenderer>().color = Color.white;
                selectedHookPoint = null;
            }
            hookPoints.Remove(collision.gameObject);
        }
    }

    public void Hook(InputAction.CallbackContext context)
    {
        if (selectedHookPoint == null)
        {
            return;
        }

        if (context.started)
        {
            hooked = true;
            distanceMax = Vector2.Distance(transform.position, selectedHookPoint.transform.position);
            lineRenderer.SetPosition(1, selectedHookPoint.transform.position);
            lineRenderer.enabled = true;
        }

        if (context.canceled)
        {
            lineRenderer.enabled = false;
            distanceJoint2D.enabled = false;
            hooked = false;
        }

        /*if (context.performed)
        {
            lineRenderer.enabled = true;
            distanceJoint2D.enabled = true;
            //distanceJoint2D.connectedAnchor = transform.position;
            distanceJoint2D.connectedAnchor = selectedHookPoint.transform.position;
            lineRenderer.SetPosition(1, selectedHookPoint.transform.position);
            hooked = true;
        }

        if (context.canceled)
        {
            lineRenderer.enabled = false;
            distanceJoint2D.enabled = false;
            hooked = false;
        }*/
    }
}
