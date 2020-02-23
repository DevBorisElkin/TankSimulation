using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Projectile;

[RequireComponent(typeof(Rigidbody))]
public class TankController : MonoBehaviour
{
    Rigidbody rb;

    [Header("Movement related")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;
    public ForceMode BodyForceMode = ForceMode.VelocityChange;

    [Header("Body Speed limit")]
    public bool checkingMaxSpeed = true;
    public float maxSpeed = 2f;

    [Header("Gun related")]
    public GameObject gun;
    public float gunRotationSpeed = 5f;

    [Header("Reead only")]
    public bool fireAllowed = false;

    [Header("Fire related")]
    public Image cross;
    public GameObject firePoint;
    public GameObject projectilePrefab;

    public Projectile projectileForGizmos;
    Rigidbody projectileForGizmosRb;
    float gravity = -18;


    Utilities.LineDrawer lineDrawer;
    [Header("Trajectory drawing")]
    public bool draw = true;
    [Range(0.7f, 100f)]
    public float MinHeight = 20f;
    [Header("Will allow you to ignore height ceiling")]
    public bool increaseCeiling;

    void Start()
    {
        InitObject();
    }

    void InitObject()
    {
        rb = GetComponent<Rigidbody>();
        projectileForGizmosRb = projectileForGizmos.GetComponent<Rigidbody>();
        Physics.gravity = Vector3.up * gravity;
        lineDrawer = new Utilities.LineDrawer(0.5f);
    }

    private void Update()
    {
        TankMovement();
        GunMovement();
        DrawAimTrajectory();
    }

    [HideInInspector] 
    public bool turnLeft, turnRight, moveForward, moveBackwards;
    
    void TankMovement()
    {
        if (turnLeft)
        {
            transform.Rotate(Vector3.down * rotationSpeed* Time.deltaTime);
        }
        if (turnRight)
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
        if (moveForward)
        {
            rb.AddForce(transform.forward * moveSpeed * Time.deltaTime, BodyForceMode);
        }
        if (moveBackwards)
        {
            rb.AddForce(transform.forward * -1 * moveSpeed * Time.deltaTime, BodyForceMode);
        }
        CheckMaxSpeed();
    }

    void CheckMaxSpeed()
    {
        if (checkingMaxSpeed)
        {
            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }
        }
    }

    #region gunMovement

    public Ray mouseInputRay;
    RaycastHit hit;
    void GunMovement()
    {
        if (Physics.Raycast(mouseInputRay, out hit))
        {
            SetFireAllowed(true);

            if (launchData.fullSpeed)
                RotateTowardsVector3(gun, hit.point);
            else
                RotateForRequiredAngle();
        }
        else
        {
            RotateTowardsVector3(gun, mouseInputRay.direction * 10000);
            SetFireAllowed(false);
        }
    }

    void RotateForRequiredAngle()
    {
        RotateTowardsVector3(gun, launchData.initialVelocity * 100f);
    }

    
    void RotateTowardsVector3(GameObject initial, Vector3 rotateTo)
    {
        if (initial != null && rotateTo != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(rotateTo - initial.transform.position);
            initial.transform.rotation = Quaternion.Lerp(initial.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    #endregion

    
    public void ShootAtTarget()
    {
        if (!fireAllowed)
        {
            print("Fire is NOT allowed!");
        }
        else
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.transform.position, firePoint.transform.rotation);
            Projectile projectileType = projectile.GetComponent<Projectile>();

            projectileType.increaseCeiling = increaseCeiling;
            projectileForGizmos.increaseCeiling = increaseCeiling;
            projectileType.Launch(hit.point, MinHeight);
        }
    }

    void SetFireAllowed(bool allowed)
    {
        fireAllowed = allowed;
        ManageRender();
    }


    #region UI
    void ManageRender()
    {
        if (fireAllowed) cross.color = Color.green;
        else cross.color = Color.red;

        if (!fireAllowed) lineDrawer.Destroy();
    }
    public void SetAimCrossPosition(Vector3 position)
    {
        cross.gameObject.transform.position = position;
    }

    void DrawAimTrajectory()
    {
        if (draw && hit.point != Vector3.zero)
        {
            DrawPath();
        }
    }

    private LaunchData launchData;
    void DrawPath()
    {
        launchData = projectileForGizmos.GetLaunchData(hit.point, MinHeight);
        if (!launchData.fullSpeed)
        {
            Vector3 previousDrawPoint = projectileForGizmosRb.position;

            int resolution = 30;
            lineDrawer.StartCurvedLine(resolution + 1);
            for (int i = 1; i < resolution + 1; i++)
            {
                float simulationTime = i / (float)resolution * launchData.timeToTarget;
                Vector3 displacement = launchData.initialVelocity * simulationTime + Vector3.up * gravity * simulationTime * simulationTime / 2f;
                Vector3 drawPoint = projectileForGizmosRb.position + displacement;
                Debug.DrawLine(previousDrawPoint, drawPoint, Color.green);
                lineDrawer.DrawCurvedLineInGameView(previousDrawPoint, drawPoint, Color.blue);
                previousDrawPoint = drawPoint;
            }
        }
        else
        {
            lineDrawer.DrawLineInGameView(projectileForGizmosRb.position, hit.point, Color.black);
        }
        
    }
    #endregion

    public void ChangeMinHeight(float change)
    {
        if ((MinHeight + change) < 100 && (MinHeight + change) > 0.7f)
        MinHeight += change;
    }
}
