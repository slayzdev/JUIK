using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Attributs du joueur")]
    public float vitesse = 8f;
    public float vitesseSprint = 12f;
    public float forceSaut = 7f;
    public float puissanceTir = 10f;
    public float precisionPasse = 0.8f;
    
    [Header("Stats du joueur")]
    public int endurance = 100;
    public int technique = 80;
    public int tacle = 70;
    public string nomJoueur = "Joueur LFG";
    public string position = "Milieu";
    
    [Header("Références")]
    public GameObject ballon;
    private Rigidbody rb;
    private Animator anim;
    private bool possessionBallon = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }
    
    void Update()
    {
        // Mouvement du joueur
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        bool sprint = Input.GetKey(KeyCode.LeftShift);
        float vitesseActuelle = sprint ? vitesseSprint : vitesse;
        
        Vector3 mouvement = new Vector3(horizontal, 0f, vertical) * vitesseActuelle * Time.deltaTime;
        transform.Translate(mouvement, Space.World);
        
        // Rotation du joueur dans la direction du mouvement
        if (mouvement != Vector3.zero)
        {
            transform.forward = Vector3.Slerp(transform.forward, mouvement.normalized, 0.2f);
            anim?.SetBool("EnMouvement", true);
        }
        else
        {
            anim?.SetBool("EnMouvement", false);
        }
        
        // Actions avec le ballon
        if (possessionBallon)
        {
            if (Input.GetKeyDown(KeyCode.Space)) // Tirer
            {
                TirerBallon();
            }
            
            if (Input.GetKeyDown(KeyCode.E)) // Passer
            {
                PasserBallon();
            }
        }
    }
    
    void TirerBallon()
    {
        if (ballon && possessionBallon)
        {
            Rigidbody rbBallon = ballon.GetComponent<Rigidbody>();
            if (rbBallon)
            {
                possessionBallon = false;
                Vector3 directionTir = transform.forward + new Vector3(0, 0.5f, 0);
                rbBallon.AddForce(directionTir * puissanceTir, ForceMode.Impulse);
                anim?.SetTrigger("Tirer");
            }
        }
    }
    
    void PasserBallon()
    {
        if (ballon && possessionBallon)
        {
            Rigidbody rbBallon = ballon.GetComponent<Rigidbody>();
            if (rbBallon)
            {
                possessionBallon = false;
                Vector3 directionPasse = transform.forward;
                rbBallon.AddForce(directionPasse * (puissanceTir * 0.6f), ForceMode.Impulse);
                anim?.SetTrigger("Passer");
            }
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ballon") && !possessionBallon)
        {
            possessionBallon = true;
            ballon = collision.gameObject;
            // Attacher temporairement le ballon au joueur
            Rigidbody rbBallon = ballon.GetComponent<Rigidbody>();
            rbBallon.velocity = Vector3.zero;
            rbBallon.angularVelocity = Vector3.zero;
        }
    }
} 