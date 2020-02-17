using UnityEngine;

public class Car2DControl : MonoBehaviour
{
    public float Accelerate;
    public float Turn;
    public Vector2 Speed;

    public int Checkpoint = 0;
    public bool Crash = false;
    public float Distance = 0;
    public float Life = 0;
    public float LifeTime = 0;
    public float MaxLifeTime = 0;

    private Rigidbody2D RB;

    public float[] Sensors;

    private Vector3 OldPosition;

    private Vector3 StartPosition;
    private Vector3 StartRotation;
    private void Start()
    {
        RB = gameObject.GetComponent<Rigidbody2D>();
        StartPosition = transform.position;
        StartRotation = transform.eulerAngles;
        OldPosition = StartPosition;

        //This settings need for the demo "Car2D"
        Physics2D.queriesHitTriggers = false;
        Physics2D.queriesStartInColliders = false;
        Physics2D.gravity = Vector2.zero;
    }

    private void Update()
    {
        Control();
        Lifer();

        Raycast();
    }

    private void Lifer()
    {
        if (!Crash)
        {
            LifeTime += Time.deltaTime;
            if (transform.InverseTransformDirection(RB.velocity).y >= 0)
                Distance += Vector3.Distance(OldPosition, transform.position);
            else
                Distance -= Vector3.Distance(OldPosition, transform.position);
            Life = Distance + LifeTime;
        }

        if (Crash)
        {
            RB.velocity = Vector2.zero;
            RB.angularVelocity = 0;
            transform.position = StartPosition;
            transform.eulerAngles = StartRotation;
            Crash = false;
            Checkpoint = 0;
            Distance = 0;
            Life = 0;
            LifeTime = 0;
            MaxLifeTime = 0;
            Accelerate = 0;
            Turn = 0;
            Speed = Vector2.zero;
        }
        OldPosition = transform.position;

        if ((LifeTime > 10 + MaxLifeTime || LifeTime > 180) && !Crash)
            Crash = true;
    }

    private void Control()
    {
        Accelerate = Mathf.Clamp(Accelerate, -1F, 1F);
        Turn = Mathf.Clamp(Turn, -1F, 1F);

        float SF = transform.InverseTransformDirection(RB.velocity).y;

        //Move
        Vector2 V = transform.up * Accelerate * 18F;
        RB.AddForceAtPosition(V, transform.TransformPoint(Vector2.down));

        //Turn
        RB.AddTorque(-Turn * SF * 5);

        //Back Wheels Rdag
        float F = Mathf.Clamp(-transform.InverseTransformDirection(RB.GetRelativePointVelocity(Vector2.down)).x, -1.8F, 1.8F);
        V = transform.TransformDirection(new Vector2(F, -Mathf.Abs(F) * 0.2F));
        RB.AddForceAtPosition(V * 18F, transform.TransformPoint(Vector2.down));

        //Speed
        Speed = RB.velocity * 3.6F;
        if (Speed.magnitude > 122.4F)
        {
            RB.velocity = RB.velocity.normalized * 34F;
            Speed = Speed.normalized * 122.4F;
        }
        Speed = transform.InverseTransformDirection(Speed);
    }

    private void Raycast()
    {
        //Raycast
        RaycastHit2D Hit;
        int i = 0;
        float HMR = 180F / (Sensors.Length - 1);
        while (i < Sensors.Length)
        {
            Hit = Physics2D.Raycast(transform.position, transform.TransformDirection(Quaternion.AngleAxis(HMR * i - 90F, Vector3.forward) * Vector3.up), 100, ~(1 << LayerMask.NameToLayer("Cars")));
            if (Hit.collider == gameObject.GetComponent<Collider2D>())
            {
                while (Hit.collider == gameObject.GetComponent<Collider2D>())
                {
                    Hit = Physics2D.Raycast(Hit.point + (Hit.point - (Vector2)transform.position).normalized * 0.01F, transform.TransformDirection(Quaternion.AngleAxis(HMR * i - 90F, Vector3.forward) * Vector3.up), 100);
                }
            }
            if (Hit.collider != null)
                Sensors[i] = (Hit.point - (Vector2)transform.position).magnitude / 100F;
            else
                Sensors[i] = 1F;
            Debug.DrawRay(transform.position, Hit.point - (Vector2)transform.position, Color.green);
            i++;
        }
    }

    void OnCollisionEnter2D()
    {
        Crash = true;
    }
}