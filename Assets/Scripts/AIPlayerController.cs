using UnityEngine;
using System.Collections;

public class AIPlayerController : MonoBehaviour
{
    [Header("Références")]
    public GameObject ballon;
    public Transform positionBase;
    
    [Header("Paramètres AI")]
    public float vitesseMax = 7f;
    public float vitesseSprint = 10f;
    public float distancePoursuiteBalle = 10f;
    public float distanceTir = 15f;
    public float distancePasse = 8f;
    public float tempsReactionMin = 0.2f;
    public float tempsReactionMax = 1.0f;
    
    [Header("Statistiques")]
    public int intelligence = 75;
    public int agressivite = 70;
    public int positionnement = 80;
    
    private Rigidbody rb;
    private Animator animator;
    private Vector3 targetPosition;
    private bool possessionBallon = false;
    private PlayerController playerController;
    private bool enPoursuiteBallon = false;
    private GameManager gameManager;
    private bool estEquipeLocale;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        gameManager = FindObjectOfType<GameManager>();
        
        if (!ballon)
        {
            ballon = GameObject.FindGameObjectWithTag("Ballon");
        }
        
        if (!positionBase)
        {
            positionBase = transform;
        }
        
        // Déterminer l'équipe du joueur basé sur le matériau du maillot
        SkinnedMeshRenderer renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (renderer && gameManager && gameManager.maillotLocal)
        {
            Material[] materials = renderer.materials;
            estEquipeLocale = (materials[0] == gameManager.maillotLocal);
        }
        
        StartCoroutine(ComportementAI());
    }
    
    private IEnumerator ComportementAI()
    {
        while (true)
        {
            // Temps de réaction basé sur l'intelligence
            float tempsReaction = Mathf.Lerp(tempsReactionMax, tempsReactionMin, intelligence / 100f);
            yield return new WaitForSeconds(tempsReaction);
            
            if (!ballon)
            {
                ballon = GameObject.FindGameObjectWithTag("Ballon");
                yield return new WaitForSeconds(0.5f);
                continue;
            }
            
            // Distance au ballon
            float distanceAuBallon = Vector3.Distance(transform.position, ballon.transform.position);
            
            // Logique du joueur AI
            if (possessionBallon)
            {
                DeciderAction();
            }
            else if (distanceAuBallon < distancePoursuiteBalle && PeutPoursuivreBalle())
            {
                // Poursuivre le ballon
                enPoursuiteBallon = true;
                targetPosition = ballon.transform.position;
            }
            else
            {
                // Retour à la position de base avec variation
                enPoursuiteBallon = false;
                targetPosition = positionBase.position + new Vector3(
                    Random.Range(-5f, 5f),
                    0,
                    Random.Range(-5f, 5f)
                );
            }
        }
    }
    
    void Update()
    {
        if (!ballon) return;
        
        // Calcul du mouvement vers la position cible
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; // Garder le mouvement horizontal
        
        // Vitesse basée sur la poursuite ou non
        float speed = enPoursuiteBallon ? vitesseSprint : vitesseMax;
        
        // Déplacement
        transform.position += direction * speed * Time.deltaTime;
        
        // Rotation vers la direction du mouvement
        if (direction != Vector3.zero)
        {
            transform.forward = Vector3.Slerp(transform.forward, direction, 0.2f);
            
            // Animation
            if (animator)
            {
                animator.SetBool("EnMouvement", true);
                animator.SetFloat("Vitesse", enPoursuiteBallon ? 1.5f : 1.0f);
            }
        }
        else
        {
            if (animator)
            {
                animator.SetBool("EnMouvement", false);
            }
        }
        
        // Vérifier la possession du ballon
        float distanceAuBallon = Vector3.Distance(transform.position, ballon.transform.position);
        if (distanceAuBallon < 1.0f && !possessionBallon)
        {
            possessionBallon = true;
            
            // Mettre à jour le joueur controleur
            if (playerController)
            {
                playerController.ballon = ballon;
            }
            
            // Mettre à jour le ballon
            BallController ballCtrl = ballon.GetComponent<BallController>();
            if (ballCtrl)
            {
                ballCtrl.SetPossessionJoueur(playerController);
            }
        }
    }
    
    private void DeciderAction()
    {
        if (!ballon) return;
        
        // Position sur le terrain
        float positionZ = transform.position.z;
        
        // Direction vers le but adverse
        Vector3 directionBut;
        Transform surfaceReparation;
        
        if (estEquipeLocale)
        {
            directionBut = (gameManager.surfaceReparationVisiteur.position - transform.position).normalized;
            surfaceReparation = gameManager.surfaceReparationVisiteur;
        }
        else
        {
            directionBut = (gameManager.surfaceReparationLocale.position - transform.position).normalized;
            surfaceReparation = gameManager.surfaceReparationLocale;
        }
        
        // Distance au but adverse
        float distanceAuBut = Vector3.Distance(transform.position, surfaceReparation.position);
        
        // Trouver des coéquipiers pour passer
        GameObject meilleurCoequipier = TrouverMeilleurCoequipierPourPasse();
        
        // Décider de l'action à prendre
        if (distanceAuBut < distanceTir)
        {
            // Tirer au but
            Tirer(directionBut);
        }
        else if (meilleurCoequipier != null)
        {
            // Passer à un coéquipier
            Vector3 directionPasse = (meilleurCoequipier.transform.position - transform.position).normalized;
            Passer(directionPasse);
        }
        else
        {
            // Avancer avec le ballon
            targetPosition = transform.position + directionBut * 5f;
        }
    }
    
    private void Tirer(Vector3 direction)
    {
        if (!ballon) return;
        
        possessionBallon = false;
        
        // Ajouter de la variation à la direction pour l'IA
        float precision = Mathf.Lerp(0.5f, 0.1f, playerController.technique / 100f);
        direction += new Vector3(
            Random.Range(-precision, precision),
            Random.Range(0, precision * 0.5f),
            Random.Range(-precision, precision)
        );
        
        // Appliquer la force au ballon
        Rigidbody rbBallon = ballon.GetComponent<Rigidbody>();
        if (rbBallon)
        {
            rbBallon.velocity = Vector3.zero;
            rbBallon.AddForce(direction * playerController.puissanceTir, ForceMode.Impulse);
            
            // Animation
            if (animator)
            {
                animator.SetTrigger("Tirer");
            }
        }
        
        // Reset après un court délai
        StartCoroutine(ResetPossession());
    }
    
    private void Passer(Vector3 direction)
    {
        if (!ballon) return;
        
        possessionBallon = false;
        
        // Ajouter de la variation à la direction pour l'IA
        float precision = Mathf.Lerp(0.3f, 0.05f, playerController.precisionPasse);
        direction += new Vector3(
            Random.Range(-precision, precision),
            0,
            Random.Range(-precision, precision)
        );
        
        // Appliquer la force au ballon
        Rigidbody rbBallon = ballon.GetComponent<Rigidbody>();
        if (rbBallon)
        {
            rbBallon.velocity = Vector3.zero;
            rbBallon.AddForce(direction * (playerController.puissanceTir * 0.7f), ForceMode.Impulse);
            
            // Animation
            if (animator)
            {
                animator.SetTrigger("Passer");
            }
        }
        
        // Reset après un court délai
        StartCoroutine(ResetPossession());
    }
    
    private GameObject TrouverMeilleurCoequipierPourPasse()
    {
        GameObject meilleurCoequipier = null;
        float meilleurScore = 0f;
        
        // Trouver tous les joueurs
        PlayerController[] joueurs = FindObjectsOfType<PlayerController>();
        
        foreach (PlayerController joueur in joueurs)
        {
            // Ignorer soi-même
            if (joueur.gameObject == gameObject) continue;
            
            // Vérifier si c'est un coéquipier (même matériau de maillot)
            bool estCoequipier = EstCoequipier(joueur.gameObject);
            
            if (estCoequipier)
            {
                // Calculer la distance
                float distance = Vector3.Distance(transform.position, joueur.transform.position);
                
                // Ignorer les joueurs trop loin
                if (distance > distancePasse * 2.5f) continue;
                
                // Calculer la position par rapport au but
                Vector3 directionBut;
                Transform surfaceReparation;
                
                if (estEquipeLocale)
                {
                    directionBut = (gameManager.surfaceReparationVisiteur.position - transform.position).normalized;
                    surfaceReparation = gameManager.surfaceReparationVisiteur;
                }
                else
                {
                    directionBut = (gameManager.surfaceReparationLocale.position - transform.position).normalized;
                    surfaceReparation = gameManager.surfaceReparationLocale;
                }
                
                float distanceJoueurAuBut = Vector3.Distance(joueur.transform.position, surfaceReparation.position);
                float distanceActuelleAuBut = Vector3.Distance(transform.position, surfaceReparation.position);
                
                // Préférer les joueurs plus proches du but
                float bonusPosition = distanceActuelleAuBut > distanceJoueurAuBut ? 3.0f : 0.5f;
                
                // Vérifier si le joueur est en bonne position pour recevoir une passe
                float scorePosition = bonusPosition * (1.0f - distance / (distancePasse * 3.0f));
                
                // Vérifier s'il y a des adversaires entre nous
                bool passeCoupee = false;
                Vector3 directionPasse = (joueur.transform.position - transform.position).normalized;
                
                foreach (PlayerController autreJoueur in joueurs)
                {
                    if (autreJoueur.gameObject == gameObject || autreJoueur.gameObject == joueur.gameObject) continue;
                    
                    // Si c'est un adversaire
                    if (EstCoequipier(autreJoueur.gameObject) != estCoequipier)
                    {
                        // Vérifier s'il est sur la trajectoire de la passe
                        Vector3 directionJoueur = (autreJoueur.transform.position - transform.position).normalized;
                        float angleBetween = Vector3.Angle(directionPasse, directionJoueur);
                        float distanceJoueur = Vector3.Distance(transform.position, autreJoueur.transform.position);
                        
                        if (angleBetween < 20f && distanceJoueur < distance * 0.8f)
                        {
                            passeCoupee = true;
                            break;
                        }
                    }
                }
                
                if (!passeCoupee && scorePosition > meilleurScore)
                {
                    meilleurScore = scorePosition;
                    meilleurCoequipier = joueur.gameObject;
                }
            }
        }
        
        return meilleurCoequipier;
    }
    
    private bool EstCoequipier(GameObject autreJoueur)
    {
        SkinnedMeshRenderer monRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        SkinnedMeshRenderer autreRenderer = autreJoueur.GetComponentInChildren<SkinnedMeshRenderer>();
        
        if (monRenderer && autreRenderer && monRenderer.materials.Length > 0 && autreRenderer.materials.Length > 0)
        {
            return monRenderer.materials[0] == autreRenderer.materials[0];
        }
        
        return false;
    }
    
    private bool PeutPoursuivreBalle()
    {
        // Vérifier si le joueur est bien positionné pour poursuivre le ballon
        
        // Vérifier si d'autres coéquipiers sont plus proches du ballon
        PlayerController[] joueurs = FindObjectsOfType<PlayerController>();
        int coequipiersPlus = 0;
        
        foreach (PlayerController joueur in joueurs)
        {
            if (joueur.gameObject == gameObject) continue;
            
            // Si c'est un coéquipier
            if (EstCoequipier(joueur.gameObject))
            {
                float maDistance = Vector3.Distance(transform.position, ballon.transform.position);
                float autreDistance = Vector3.Distance(joueur.transform.position, ballon.transform.position);
                
                if (autreDistance < maDistance - 2f) // Si l'autre joueur est significativement plus proche
                {
                    coequipiersPlus++;
                    if (coequipiersPlus >= 2) // Si au moins 2 coéquipiers sont plus proches
                    {
                        return false;
                    }
                }
            }
        }
        
        // Déterminer en fonction de la position sur le terrain et du rôle du joueur
        string position = playerController.position;
        if (position == "Gardien")
        {
            // Le gardien ne doit pas trop s'éloigner des buts
            return Vector3.Distance(transform.position, positionBase.position) < 10f;
        }
        else if (position == "Défenseur")
        {
            // Les défenseurs doivent rester en défense si le ballon est dans leur moitié
            if (estEquipeLocale && ballon.transform.position.z > 0)
            {
                return false;
            }
            else if (!estEquipeLocale && ballon.transform.position.z < 0)
            {
                return false;
            }
        }
        
        // Laisser d'autres facteurs comme l'agressivité déterminer
        float chancePoursuite = agressivite / 100f;
        return Random.value < chancePoursuite;
    }
    
    private IEnumerator ResetPossession()
    {
        yield return new WaitForSeconds(0.5f);
        possessionBallon = false;
    }
} 