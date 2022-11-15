using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutSizeDetect : MonoBehaviour
{
    public Manager manager;

    public void OnTriggerExit2D(Collider2D collision)
    {
        manager.ResetModel();
    }
}
