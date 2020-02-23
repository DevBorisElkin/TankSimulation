using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public Vector3 offset;
    public Transform transformToFollow;


    void Update()
    {
        transform.position = transformToFollow.position + offset;
        transform.rotation = transformToFollow.rotation;
    }
}
