using UnityEngine;

public class MatchCamera : MonoBehaviour
{
    [Header("Cibles")]
    public Transform cibleBallon;
    public Transform joueurActif;
    
    [Header("Paramètres de caméra")]
    public float vitesseDeplacement = 5f;
    public float hauteurCameraDefaut = 15f;
    public float distanceCameraDefaut = 10f;
    public float hauteurCameraZoom = 8f;
    public float distanceCameraZoom = 5f;
    public float vitesseRotation = 3f;
    public float vitesseZoom = 2f;
    
    [Header("Limites")]
    public float limiteTerrainX = 45f;
    public float limiteTerrainZ = 30f;
    
    private Vector3 positionCible;
    private float hauteurCamera;
    private float distanceCamera;
    private bool vueGlobale = true;
    private Camera cam;
    private GameManager gameManager;
    
    void Start()
    {
        cam = GetComponent<Camera>();
        gameManager = FindObjectOfType<GameManager>();
        
        // Trouver le ballon s'il n'est pas assigné
        if (!cibleBallon)
        {
            GameObject ballon = GameObject.FindGameObjectWithTag("Ballon");
            if (ballon)
            {
                cibleBallon = ballon.transform;
            }
        }
        
        // Initialiser les paramètres de caméra
        hauteurCamera = hauteurCameraDefaut;
        distanceCamera = distanceCameraDefaut;
        
        // Position initiale
        if (gameManager && gameManager.centreTerrrain)
        {
            positionCible = gameManager.centreTerrrain.position;
        }
        else
        {
            positionCible = Vector3.zero;
        }
    }
    
    void LateUpdate()
    {
        if (!cibleBallon)
        {
            // Rechercher le ballon à nouveau
            GameObject ballon = GameObject.FindGameObjectWithTag("Ballon");
            if (ballon)
            {
                cibleBallon = ballon.transform;
            }
            else
            {
                return;
            }
        }
        
        // Déterminer la position cible
        Vector3 positionSuivie = cibleBallon.position;
        
        // Trouver le joueur actif (celui qui a le ballon ou le plus proche)
        TrouverJoueurActif();
        
        // Si un joueur est actif, modifier légèrement la cible pour montrer plus d'action
        if (joueurActif)
        {
            positionSuivie = Vector3.Lerp(cibleBallon.position, joueurActif.position, 0.3f);
        }
        
        // Limiter la position au terrain
        positionSuivie.x = Mathf.Clamp(positionSuivie.x, -limiteTerrainX, limiteTerrainX);
        positionSuivie.z = Mathf.Clamp(positionSuivie.z, -limiteTerrainZ, limiteTerrainZ);
        
        // Lisser le mouvement
        positionCible = Vector3.Lerp(positionCible, positionSuivie, Time.deltaTime * vitesseDeplacement);
        
        // Gérer le zoom
        // Plus de zoom si près des buts
        float distanceAuxButs = 100f;
        if (gameManager)
        {
            float distanceBut1 = Vector3.Distance(positionCible, gameManager.surfaceReparationLocale.position);
            float distanceBut2 = Vector3.Distance(positionCible, gameManager.surfaceReparationVisiteur.position);
            distanceAuxButs = Mathf.Min(distanceBut1, distanceBut2);
        }
        
        // Zoomer près des buts
        if (distanceAuxButs < 20f)
        {
            vueGlobale = false;
        }
        else if (distanceAuxButs > 30f)
        {
            vueGlobale = true;
        }
        
        // Lisser le changement de zoom
        float hauteurCible = vueGlobale ? hauteurCameraDefaut : hauteurCameraZoom;
        float distanceCible = vueGlobale ? distanceCameraDefaut : distanceCameraZoom;
        
        hauteurCamera = Mathf.Lerp(hauteurCamera, hauteurCible, Time.deltaTime * vitesseZoom);
        distanceCamera = Mathf.Lerp(distanceCamera, distanceCible, Time.deltaTime * vitesseZoom);
        
        // Calculer la position de la caméra
        Vector3 positionCamera = positionCible - Vector3.forward * distanceCamera;
        positionCamera.y = hauteurCamera;
        
        // Appliquer la position et rotation
        transform.position = positionCamera;
        transform.LookAt(positionCible);
        
        // Gérer l'inclinaison
        Vector3 rotation = transform.eulerAngles;
        rotation.x = Mathf.Lerp(40f, 30f, (hauteurCamera - hauteurCameraZoom) / (hauteurCameraDefaut - hauteurCameraZoom));
        transform.eulerAngles = rotation;
        
        // Ajuster le FOV en fonction du zoom
        float fovCible = vueGlobale ? 60f : 50f;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fovCible, Time.deltaTime * vitesseZoom);
    }
    
    private void TrouverJoueurActif()
    {
        PlayerController[] joueurs = FindObjectsOfType<PlayerController>();
        float distanceMin = float.MaxValue;
        
        foreach (PlayerController joueur in joueurs)
        {
            BallController ballon = cibleBallon.GetComponent<BallController>();
            
            // Vérifier si le joueur a la possession
            if (ballon && ballon.joueurPossession == joueur)
            {
                joueurActif = joueur.transform;
                return;
            }
            
            // Sinon chercher le joueur le plus proche
            float distance = Vector3.Distance(joueur.transform.position, cibleBallon.position);
            if (distance < distanceMin)
            {
                distanceMin = distance;
                joueurActif = joueur.transform;
            }
        }
    }
    
    // Méthode pour basculer entre les vues
    public void ChangerVue()
    {
        vueGlobale = !vueGlobale;
    }
    
    // Méthode pour suivre un joueur spécifique
    public void SuivreJoueur(Transform joueur)
    {
        joueurActif = joueur;
    }
} 