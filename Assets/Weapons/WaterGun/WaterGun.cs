using UnityEngine;

public class WaterGun : Weapon
{
    public Material waterMaterial;
    public ParticleSystem muzzleWaterDroplets;

    public AnimationCurve waterStrengthDistanceCurve;
    public float maxDistance;
    public float maxWaterStrength;

    public int numberOfSegments;

    LineRenderer lr;

    bool firing = false;

    public float beamWidthMin;
    public float beamWidthMax;

    public bool IsFiring { get { return firing; } }

    public LayerMask layers;

    AudioSource audioSource;

    public ParticleSystem onHitParticleSystem;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        GameObject go = new GameObject("Line Renderer");
        go.transform.position = muzzle.transform.position;
        go.transform.SetParent(transform, false);
        lr = go.AddComponent<LineRenderer>();
        lr.positionCount = numberOfSegments;
        lr.material = waterMaterial;
        lr.startWidth = beamWidthMin;
        lr.endWidth = beamWidthMax;
    }

    private void Update()
    {
        if (firing && (!CanFire() || !Input.GetButton("Fire1")))
        {
            firing = false;
            audioSource.Stop();
            audioSource.time = 0;
            muzzleWaterDroplets.Stop();
            lr.gameObject.SetActive(false);
            onHitParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    public override void Fire()
    {
        if (!CanFire())
        {
            return;
        }

        if (!firing)
        {
            audioSource.Play();
        }

        firing = true;

        magazine.Fire();
        muzzleWaterDroplets.Play();

        lr.gameObject.SetActive(true);

        RaycastHit hit;
        Ray ray = new Ray(muzzle.position, muzzle.forward);

        float totalDistance;

        if (Physics.BoxCast(
            ray.origin,
            lr.startWidth * muzzle.localScale,
            ray.direction,
            out hit,
            muzzle.rotation,
            maxDistance,
            layers,
            QueryTriggerInteraction.Ignore))
        {
            GameObject go = hit.collider.gameObject;
            Rigidbody rb = go.GetComponentInParent<Rigidbody>();

            if (rb)
            {
                Doggo d = go.GetComponentInParent<Doggo>();
                if (d)
                {
                    PhysicsBehaviour pb = d.pb;
                    if (pb && pb.ControlledByAgent)
                    {
                        pb.DisableAgentAndSetupForPhysics();
                    }

                    d.status.AddBoost(Attributes.CLEANLINESS);
                }

                Vector3 force = waterStrengthDistanceCurve.Evaluate(hit.distance / maxDistance)
                    * maxWaterStrength * (-hit.normal + Vector3.up);

                rb.AddForceAtPosition(force, hit.point);
            }
            totalDistance = hit.distance;
        }
        else
        {
            onHitParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            totalDistance = maxDistance;
        }

        Vector3 dir = ray.direction;
        lr.positionCount = numberOfSegments;

        for (int i = 0; i < numberOfSegments; i++)
        {
            float perc = i / (float)numberOfSegments;
            float distance = perc * maxDistance;
            Vector3 pos = ray.GetPoint(perc * maxDistance);

            Vector3 beamPos = pos +
                Random.Range(0.01f, 0.015f) * Mathf.Sin(30 * (i % 2 == 0 ? 1 : -1) * Time.time) * Vector3.up;

            if (distance >= totalDistance)
            {
                lr.positionCount = i;

                break;
            }

            lr.SetPosition(i, beamPos);
        }

        onHitParticleSystem.transform.position = lr.GetPosition(lr.positionCount - 1);
        if (!onHitParticleSystem.isPlaying)
        {
            onHitParticleSystem.Play();
        }
    }
}
