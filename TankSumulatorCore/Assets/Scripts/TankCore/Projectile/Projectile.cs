using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody rb;
    Vector3 target;

    float height;
    float gravity;

    [HideInInspector]
    public bool increaseCeiling;

    [Header("Particles")]
    public bool useParticles = true;
    public GameObject SmokeLaunchParticles;
    public GameObject ExplosionParticles;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        gravity = Physics.gravity.y;
        InstantiateAtPosition(SmokeLaunchParticles);
    }

    public void Launch(Vector3 target, float height)
    {
        this.target = target;
        this.height = height;
        LaunchData data = CalculateLaunchData(target);
        if (!data.fullSpeed)
        {
            rb.useGravity = true;
            rb.velocity = data.initialVelocity;
        }
        else
        {
            Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
            transform.rotation = targetRotation;
            rb.velocity = transform.forward * 120;
            rb.useGravity = false;
        }
    }


    public LaunchData GetLaunchData(Vector3 target, float height)
    {
        gravity = Physics.gravity.y;
        this.target = target;
        this.height = height;

        return CalculateLaunchData(target);
    }

    public LaunchData CalculateLaunchData(Vector3 target)
    {
        float displacementY = this.target.y - transform.position.y;
        Vector3 displacementXZ = new Vector3(this.target.x - transform.position.x, 0, this.target.z - transform.position.z);

        if(!increaseCeiling && displacementY - height > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
            print(targetRotation.eulerAngles + " required power");
            return new LaunchData(targetRotation.eulerAngles, 10, transform.position, target, true);
        }
        else
        {
            if(increaseCeiling && displacementY - height > 0)
            {
                float add = Mathf.Abs(displacementY - height + 0.5f);
                height += add;
            }

            float time = CalculateTime(displacementY);
            Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * height);
            Vector3 velocityXZ = displacementXZ / time;

            return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(gravity), time, transform.position, target, false);
        }
    }

    float CalculateTime(float displacementY)
    {
        return Mathf.Sqrt(-2 * height / gravity) + Mathf.Sqrt(2 * (displacementY - height) / gravity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        InstantiateAtPosition(ExplosionParticles);
        Destroy(gameObject);
    }

    void InstantiateAtPosition(GameObject instantiating)
    {
        if (useParticles)
            Instantiate(instantiating, transform.position, transform.rotation);
    }

    public struct LaunchData
    {
        public readonly Vector3 initialVelocity;
        public readonly float timeToTarget;
        public readonly Vector3 startPosition;
        public readonly Vector3 target;
        public readonly bool fullSpeed;

        public LaunchData(Vector3 initialVelocity, float timeToTarget, Vector3 startPosition, Vector3 target, bool fullSpeed)
        {
            this.initialVelocity = initialVelocity;
            this.timeToTarget = timeToTarget;
            this.startPosition = startPosition;
            this.target = target;
            this.fullSpeed = fullSpeed;
        }
    }
}
