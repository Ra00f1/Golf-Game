using UnityEngine;
using UnityEngine.UI;

public class HitTheBall : MonoBehaviour
{
    public float hitForceMultiplier = 1.0f;
    public Rigidbody rb;
    public RectTransform powerBar;
    public GameObject Arrow;
    public SpriteRenderer ArrowSprite;
    public float minSpeedtoStop = 0.1f;
    public float dragSensitivity = 15.0f; // Increase to require more drag
    public float cancelThreshold = 50.0f; // Adjust this in Unity to control how close before canceling

    private Vector3 startMousePosition;
    private float hitForce;
    private float maxHitForce = 12.5f;
    private float minHitForce = 0.0f;
    private float maxPowerBar = 290.0f;
    private float minPowerBar = 0.0f;
    private float currentPowerBar;
    private bool isCharging = false;
    private bool canCharge = false;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        ArrowSprite.enabled = false;
    }
    //TODO: Make it so the player has to click on the ball to charge the shot, doesn't work yet
    void Update()
    {
        float dragDistance;

        if (Input.GetButtonDown("Fire1"))
        {
            // Check if the mouse is within cancelThreshold of the ball
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));
            float distanceToBall = Vector3.Distance(mouseWorldPosition, transform.position);

            if (distanceToBall < cancelThreshold) // Player must click near the ball
            {
                startMousePosition = Input.mousePosition; // Store initial mouse position
                hitForce = 0;
                canCharge = false; // Don't charge yet
                isCharging = true;
                Debug.Log("Holding ball...");
            }
        }

        if (isCharging && Input.GetButton("Fire1"))
        {
            dragDistance = Vector3.Distance(startMousePosition, Input.mousePosition);

            if (dragDistance < cancelThreshold)
            {
                canCharge = false; // Not enough drag to charge
                ArrowSprite.enabled = false;
                powerBar.sizeDelta = new Vector2(15, minPowerBar);
                return;
            }
            else
            {
                canCharge = true; // Now we can charge

                if (canCharge) // Start filling the power bar only if threshold is passed
                {
                    Debug.Log("Charging shot...");
                    Vector3 dragVector = startMousePosition - Input.mousePosition;
                    hitForce = Mathf.Clamp(dragVector.magnitude / dragSensitivity, minHitForce, maxHitForce);
                    hitForce = hitForce * hitForceMultiplier;

                    // Update power bar
                    currentPowerBar = Mathf.Lerp(minPowerBar, maxPowerBar, hitForce / maxHitForce);
                    powerBar.sizeDelta = new Vector2(powerBar.sizeDelta.x, currentPowerBar);

                    // Resize arrow
                    ArrowSprite.enabled = true;
                    float scaleFactor = Mathf.Lerp(0.5f, 2.0f, hitForce / maxHitForce);
                    Arrow.transform.localScale = new Vector3(scaleFactor, 1, 1);

                    // Convert drag direction to world space
                    Vector3 worldDragDirection = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y)) -
                                                 Camera.main.ScreenToWorldPoint(new Vector3(startMousePosition.x, startMousePosition.y, Camera.main.transform.position.y));

                    worldDragDirection.y = 0; // Ignore Y-axis movement
                    worldDragDirection.Normalize();

                    if (worldDragDirection.magnitude > 0.1f)
                    {
                        Quaternion cameraDirection = Quaternion.LookRotation(worldDragDirection, Vector3.up);
                        Arrow.transform.rotation = cameraDirection * Quaternion.Euler(0, +90, 0);
                    }
                }
            }
        }

        if (Input.GetButtonUp("Fire1"))
        {
            if (canCharge) // Only hit if charging was allowed
            {
                HitBall();
            }
            else
            {
                Debug.Log("Shot canceled.");
            }
            isCharging = false;
            canCharge = false;
        }

        // Stop ball if moving too slow
        if (rb.linearVelocity.magnitude < minSpeedtoStop)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }


    void HitBall()
    {
        print("Hit ball with force: " + hitForce);

        rb.isKinematic = false; // Enable physics

        hitForce = Mathf.Clamp(hitForce, minHitForce, maxHitForce); // Clamp force

        // Get the hit direction (opposite of drag)
        Vector3 hitDirection = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y)) -
                               Camera.main.ScreenToWorldPoint(new Vector3(startMousePosition.x, startMousePosition.y, Camera.main.transform.position.y));

        hitDirection.y = 0; // Ignore Y movement (we're not interested in up/down motion)
        hitDirection.Normalize();

        // Make sure to apply force in the X and Z directions only
        Vector3 directionToApply = new Vector3(hitDirection.x, 0, hitDirection.z); // Keep Y zero to avoid the ball going up/down

        // Apply the force in the opposite direction of the mouse drag
        rb.AddForce(-directionToApply * hitForce, ForceMode.Impulse);

        // Reset power bar
        powerBar.sizeDelta = new Vector2(15, minPowerBar);

        // Hide arrow
        ArrowSprite.enabled = false;
    }
}