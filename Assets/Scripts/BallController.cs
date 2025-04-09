using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("Propriétés physiques")]
    public float masse = 0.45f;
    public float trainee = 0.2f;
    public float rebond = 0.8f;
    public float friction = 0.4f;
    
    [Header("Effets")]
    public float effetMax = 5f;
    public ParticleSystem particuleContact;
    public AudioSource sonRebond;
    
    private Rigidbody rb;
    private Renderer ballRenderer;
    private Vector3 dernierContact;
    private PlayerController joueurPossession;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ballRenderer = GetComponent<Renderer>();
        
        // Configuration physique du ballon
        rb.mass = masse;
        rb.drag = trainee;
        
        // Configurer le material physique
        PhysicMaterial physicMat = new PhysicMaterial();
        physicMat.bounciness = rebond;
        physicMat.dynamicFriction = friction;
        physicMat.staticFriction = friction * 0.5f;
        
        Collider ballCollider = GetComponent<Collider>();
        if (ballCollider)
        {
            ballCollider.material = physicMat;
        }
        
        // Appliquer le tag
        gameObject.tag = "Ballon";
    }
    
    void Update()
    {
        // Rotation du ballon basée sur sa vélocité
        if (rb.velocity.magnitude > 0.1f)
        {
            Vector3 axeRotation = Vector3.Cross(Vector3.up, rb.velocity.normalized);
            float angleRotation = rb.velocity.magnitude * 100f * Time.deltaTime;
            transform.Rotate(axeRotation, angleRotation, Space.World);
        }
        
        // Effet de traînée supplémentaire quand le ballon ralentit
        if (rb.velocity.magnitude < 2f && rb.velocity.magnitude > 0.1f)
        {
            rb.velocity *= 0.98f;
        }
    }
    
    public void AppliquerEffet(Vector3 direction, float puissance)
    {
        puissance = Mathf.Clamp(puissance, 0, effetMax);
        rb.AddTorque(direction * puissance, ForceMode.Impulse);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        // Effets de collision
        ContactPoint contact = collision.contacts[0];
        dernierContact = contact.point;
        
        // Activer les particules
        if (particuleContact != null)
        {
            particuleContact.transform.position = contact.point;
            particuleContact.Play();
        }
        
        // Jouer le son de rebond
        if (sonRebond != null)
        {
            float volume = Mathf.Clamp01(collision.relativeVelocity.magnitude / 10f);
            sonRebond.volume = volume;
            sonRebond.pitch = Random.Range(0.8f, 1.2f);
            sonRebond.Play();
        }
    }
    
    public void SetPossessionJoueur(PlayerController joueur)
    {
        joueurPossession = joueur;
    }
    
    public void RelacherBallon()
    {
        joueurPossession = null;
    }
} 