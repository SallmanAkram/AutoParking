using UnityEngine;

public class FuzzyParkingPilot : MonoBehaviour
{
    [Header("--- SENSORS ---")]
    public Transform rearSensor;      // Back Center (Z = -2.2)
    public Transform backRightSensor; // Back Right Corner (Z = -2.0)
    public Transform frontSensor;     // Front Center (Z = 2.2) <--- NEW!

    [Header("--- SETTINGS ---")]
    public float maxSpeed = 3.5f;
    public float maxSteerAngle = 45f; // Increased slightly for sharper turns
    public float minGapSize = 4.0f;

    [Header("--- DEBUG ---")]
    public int currentState = 0;      
    // 0=Scan, 1=Entry, 2=Straighten, 3=Center(New), 4=Parked
    public float currentAngle = 0;

    private Rigidbody rb;
    private Vector3 gapStartPos;
    private bool isMeasuringGap = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = 5f; 
    }

    void FixedUpdate()
    {
        // 1. READ SENSORS
        float dSide = ReadSensor(backRightSensor, transform.right);
        float dRear = ReadSensor(rearSensor, -transform.forward);
        float dFront = ReadSensor(frontSensor, transform.forward); // Read Front

        // 2. GET ANGLE (-180 to +180)
        currentAngle = transform.eulerAngles.y;
        if (currentAngle > 180) currentAngle -= 360;

        switch (currentState)
        {
            // --- STAGE 0: GAP SCANNING ---
            case 0:
                Move(maxSpeed * 0.8f, 0); 
                if (dSide > 3.0f)
                {
                    if (!isMeasuringGap) { isMeasuringGap = true; gapStartPos = transform.position; }
                    Debug.DrawLine(transform.position, transform.position + Vector3.up * 5, Color.blue);
                }
                else 
                {
                    if (isMeasuringGap)
                    {
                        float gapWidth = Vector3.Distance(transform.position, gapStartPos);
                        if (gapWidth > minGapSize) { StopCar(); currentState = 1; }
                        else { isMeasuringGap = false; }
                    }
                }
                break;

            // --- STAGE 1: ENTRY TURN (-45 deg) ---
            case 1:
                float error1 = Mathf.Abs(-45f - currentAngle);
                float steer1 = FuzzySteer(error1);
                float speed1 = FuzzySpeed(error1);

                Move(-speed1, steer1); // Reverse + Right

                if (currentAngle < -40f) { StopCar(); currentState = 2; }
                break;

            // --- STAGE 2: STRAIGHTEN (0 deg) ---
            case 2:
                float error2 = Mathf.Abs(0f - currentAngle);
                float steer2 = FuzzySteer(error2);
                float speed2 = FuzzySpeed(error2);

                Move(-speed2, -steer2); // Reverse + Left

                // STOP CONDITION:
                // Stop if Parallel OR if we get too close to the back (1.0m)
                if ((currentAngle > -2f && currentAngle < 2f) || dRear < 1.0f)
                {
                    StopCar();
                    currentState = 3; // Go to Centering Phase
                }
                break;

            // --- STAGE 3: AUTO-CENTERING (The Fix) ---
// --- STAGE 3: SMART CENTERING ---
            case 3:
                // LOGIC: Only move forward if we are closer to the BACK than the FRONT.
                // We add a small buffer (0.5f) so it doesn't jitter.
                
                if (dFront > dRear + 0.5f) 
                {
                    // We have way more space in front. Pull up!
                    Move(1.0f, 0); 
                }
                else
                {
                    // We are roughly in the middle. Stop!
                    Debug.Log("PERFECTLY CENTERED. Final Park.");
                    StopCar();
                    currentState = 4;
                }
                break;
        }
    }

    // --- FUZZY LOGIC ---
    float FuzzySteer(float error)
    {
        float isBig = Trapezoid(error, 15f, 30f, 180f, 180f);
        if (isBig > 0.5f) return maxSteerAngle;
        return maxSteerAngle * 0.5f;
    }

    float FuzzySpeed(float error)
    {
        float isSmall = Trapezoid(error, 0f, 0f, 10f, 15f);
        if (isSmall > 0) return maxSpeed * 0.4f;
        return maxSpeed * 0.7f;
    }

    float Trapezoid(float x, float a, float b, float c, float d)
    {
        if (x <= a || x >= d) return 0;
        if (x >= b && x <= c) return 1;
        if (x < b) return (x - a) / (b - a);
        return (d - x) / (d - c);
    }

    // --- PHYSICS HELPER ---
    void Move(float speed, float steer)
    {
        Vector3 fwd = transform.forward * speed * 5f * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + fwd);
        
        float direction = speed >= 0 ? 1 : -1;
        float turnAmount = steer * direction * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, turnAmount, 0));
    }

    void StopCar()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    float ReadSensor(Transform eye, Vector3 dir)
    {
        RaycastHit hit;
        if (Physics.Raycast(eye.position, dir, out hit, 20f)) return hit.distance;
        return 20f;
    }
}