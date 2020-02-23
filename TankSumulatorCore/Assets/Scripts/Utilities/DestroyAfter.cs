using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public float DestroyIn = 5f;
    void Start()
    {
        Invoke(nameof(DestroyCall), DestroyIn);
    }

    void DestroyCall()
    {
        Destroy(gameObject);
    }
    
}
