using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExitTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent onTriggerEnterHandler;

    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnterHandler?.Invoke();
    }
}
