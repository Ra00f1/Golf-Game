using UnityEngine;
using UnityEngine.UI;

public class HitTheBall : MonoBehaviour
{
    public float hitForceMultiplier = 1000.0f;

    private Vector3 hitDirection;
    private float hitForce;

    public Rigidbody rb;

    private float maxHitForce = 100.0f;
    private float minHitForce = 0.0f;

    private bool isCharging = false;
    private float startY; 

    public RawImage powerBar;

    float maxPowerBar = 290.0f;
    float minPowerBar = 0.0f;
    float currentPowerBar;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero; // Ensure no movement on start
        rb.angularVelocity = Vector3.zero; // Stop any rotation
        rb.isKinematic = true; // Disable physics simulation until we hit
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            isCharging = true;
            startY = Input.mousePosition.y; // Store initial mouse position
            hitForce = 0; // Reset hit force when starting a new shot
        }

        if (isCharging && Input.GetButton("Fire1"))
        {
            float dragDistance = startY - Input.mousePosition.y; // Downward movement increases force
            hitForce = dragDistance * hitForceMultiplier;
            hitForce = Mathf.Clamp(hitForce, minHitForce, maxHitForce);

            // Update power bar
            // change the size of the power bar based on the hit force
            currentPowerBar = Mathf.Lerp(minPowerBar, maxPowerBar, hitForce / maxHitForce);
            powerBar.rectTransform.sizeDelta = new Vector2(15, currentPowerBar);
        }

        if (Input.GetButtonUp("Fire1"))
        {
            HitBall();
            isCharging = false; // Stop charging after hitting
        }
    }

    void HitBall()
    {
        print("Hit ball with force: " + hitForce);
        // Ensure physics is enabled before applying force
        rb.isKinematic = false;

        hitForce = Mathf.Clamp(hitForce, minHitForce, maxHitForce);
        hitDirection = Camera.main.transform.forward.normalized;
        rb.AddForce(hitDirection * hitForce, ForceMode.Impulse);

        // Reset power bar
        powerBar.rectTransform.sizeDelta = new Vector2(15, minPowerBar);
    }
}