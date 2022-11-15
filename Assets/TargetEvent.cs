using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetEvent : MonoBehaviour
{
    public Manager manager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            Vector3 top_dir = collision.transform.position - transform.position;

            //Debug.Log("dir ="+top_dir);

            //triggerFrom top , ball in!
            if(top_dir.y > 0)
            {
                manager.BallInTarget();
            }
        }
    }
}
