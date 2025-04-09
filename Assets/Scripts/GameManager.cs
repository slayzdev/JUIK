using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Configuration du match")]
    public int dureeMatch = 90; // en minutes
    public int tempsMiTemps = 45; // en minutes
    public float vitesseTemps = 6f; // combien de fois plus vite que le temps réel
    
    [Header("Équipes")]
    public TeamConfig equipeLocale;
    public TeamConfig equipeVisiteur;
    public Material maillotLocal;
    public Material maillotVisiteur;
    
    [Header("Terrain")]
    public Transform centreTerrrain;
    public Transform surfaceReparationLocale;
    public Transform surfaceReparationVisiteur;
    public Transform cornerHautGauche;
    public Transform cornerHautDroit;
    public Transform cornerBasGauche;
    public Transform cornerBasDroit;
    
    [Header("UI")]
    public Text texteTempPartie;
    public Text texteScoreLocale;
    public Text texteScoreVisiteur;
    public Text texteInfoMatch;
    public GameObject panneauMiTemps;
    public GameObject panneauFinMatch;
    
    // Gestion de l'état du jeu
    private float tempsEcoule = 0f;
    private int scoreLocale = 0;
    private int scoreVisiteur = 0;
    private bool miTemps = false;
    private bool finMatch = false;
    private bool matchEnCours = false;
    private GameObject ballon;
    private List<GameObject> joueursLocaux = new List<GameObject>();
    private List<GameObject> joueursVisiteurs = new List<GameObject>();
    
    void Start()
    {
        // Initialiser le ballon
        ballon = GameObject.FindGameObjectWithTag("Ballon");
        if (!ballon)
        {
            ballon = Instantiate(Resources.Load<GameObject>("Prefabs/Ballon"), centreTerrrain.position, Quaternion.identity);
        }
        
        // Initialiser les équipes
        InitialiserEquipes();
        
        // Démarrer le match
        DemarrerMatch();
    }
    
    void Update()
    {
        if (matchEnCours && !miTemps && !finMatch)
        {
            // Mettre à jour le temps
            tempsEcoule += Time.deltaTime * vitesseTemps;
            MettreAJourUI();
            
            // Vérifier les conditions de mi-temps et fin du match
            if (tempsEcoule >= tempsMiTemps * 60 && !miTemps)
            {
                miTemps = true;
                StartCoroutine(GererMiTemps());
            }
            else if (tempsEcoule >= dureeMatch * 60 && !finMatch)
            {
                finMatch = true;
                TerminerMatch();
            }
        }
    }
    
    private void InitialiserEquipes()
    {
        // Créer les joueurs de l'équipe locale
        for (int i = 0; i < 11; i++)
        {
            Vector3 position = centreTerrrain.position + new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
            GameObject joueur = Instantiate(Resources.Load<GameObject>("Prefabs/Joueur"), position, Quaternion.identity);
            
            // Configurer le joueur
            PlayerController controller = joueur.GetComponent<PlayerController>();
            if (controller)
            {
                controller.nomJoueur = equipeLocale.nomsJoueurs[i];
                controller.position = equipeLocale.positionsJoueurs[i];
                
                // Assigner les stats en fonction de la position
                switch (controller.position)
                {
                    case "Gardien":
                        controller.endurance = 80;
                        controller.technique = 60;
                        controller.tacle = 40;
                        break;
                    case "Défenseur":
                        controller.endurance = 85;
                        controller.technique = 70;
                        controller.tacle = 90;
                        break;
                    case "Milieu":
                        controller.endurance = 90;
                        controller.technique = 85;
                        controller.tacle = 75;
                        break;
                    case "Attaquant":
                        controller.endurance = 85;
                        controller.technique = 90;
                        controller.tacle = 60;
                        break;
                }
            }
            
            // Appliquer le maillot
            SkinnedMeshRenderer renderer = joueur.GetComponentInChildren<SkinnedMeshRenderer>();
            if (renderer && maillotLocal)
            {
                Material[] materials = renderer.materials;
                materials[0] = maillotLocal;
                renderer.materials = materials;
            }
            
            joueursLocaux.Add(joueur);
        }
        
        // Créer les joueurs de l'équipe visiteur avec un process similaire
        for (int i = 0; i < 11; i++)
        {
            Vector3 position = centreTerrrain.position + new Vector3(Random.Range(-10f, 10f), 0, Random.Range(5f, 15f));
            GameObject joueur = Instantiate(Resources.Load<GameObject>("Prefabs/Joueur"), position, Quaternion.identity);
            
            // Configurer le joueur
            PlayerController controller = joueur.GetComponent<PlayerController>();
            if (controller)
            {
                controller.nomJoueur = equipeVisiteur.nomsJoueurs[i];
                controller.position = equipeVisiteur.positionsJoueurs[i];
                
                // Assigner les stats comme pour l'équipe locale
                switch (controller.position)
                {
                    case "Gardien":
                        controller.endurance = 80;
                        controller.technique = 60;
                        controller.tacle = 40;
                        break;
                    case "Défenseur":
                        controller.endurance = 85;
                        controller.technique = 70;
                        controller.tacle = 90;
                        break;
                    case "Milieu":
                        controller.endurance = 90;
                        controller.technique = 85;
                        controller.tacle = 75;
                        break;
                    case "Attaquant":
                        controller.endurance = 85;
                        controller.technique = 90;
                        controller.tacle = 60;
                        break;
                }
            }
            
            // Appliquer le maillot visiteur
            SkinnedMeshRenderer renderer = joueur.GetComponentInChildren<SkinnedMeshRenderer>();
            if (renderer && maillotVisiteur)
            {
                Material[] materials = renderer.materials;
                materials[0] = maillotVisiteur;
                renderer.materials = materials;
            }
            
            joueursVisiteurs.Add(joueur);
        }
    }
    
    private void DemarrerMatch()
    {
        matchEnCours = true;
        tempsEcoule = 0f;
        miTemps = false;
        finMatch = false;
        
        // Placer le ballon au centre
        if (ballon)
        {
            ballon.transform.position = centreTerrrain.position + Vector3.up * 0.2f;
            Rigidbody rb = ballon.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
        
        // Message de début de match
        texteInfoMatch.text = "Coup d'envoi!";
        StartCoroutine(EffacerMessage(3f));
    }
    
    private IEnumerator GererMiTemps()
    {
        matchEnCours = false;
        
        // Afficher le panneau de mi-temps
        panneauMiTemps.SetActive(true);
        texteInfoMatch.text = "Mi-temps!";
        
        yield return new WaitForSeconds(5f);
        
        // Repositionner les joueurs et le ballon
        RepositionnerEquipes();
        
        panneauMiTemps.SetActive(false);
        texteInfoMatch.text = "Début de la seconde mi-temps!";
        matchEnCours = true;
        miTemps = false;
        
        yield return new WaitForSeconds(3f);
        EffacerMessage(0f);
    }
    
    private void TerminerMatch()
    {
        matchEnCours = false;
        
        // Déterminer le résultat
        string resultat;
        if (scoreLocale > scoreVisiteur)
        {
            resultat = equipeLocale.nomEquipe + " remporte le match!";
        }
        else if (scoreVisiteur > scoreLocale)
        {
            resultat = equipeVisiteur.nomEquipe + " remporte le match!";
        }
        else
        {
            resultat = "Match nul entre " + equipeLocale.nomEquipe + " et " + equipeVisiteur.nomEquipe;
        }
        
        // Afficher le résultat final
        texteInfoMatch.text = "Fin du match!\n" + resultat;
        panneauFinMatch.SetActive(true);
    }
    
    private void RepositionnerEquipes()
    {
        // Repositionner le ballon au centre
        if (ballon)
        {
            ballon.transform.position = centreTerrrain.position + Vector3.up * 0.2f;
            Rigidbody rb = ballon.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
        
        // Repositionner les joueurs (simplifié)
        foreach (GameObject joueur in joueursLocaux)
        {
            Vector3 position = centreTerrrain.position + new Vector3(Random.Range(-20f, -5f), 0, Random.Range(-10f, 10f));
            joueur.transform.position = position;
        }
        
        foreach (GameObject joueur in joueursVisiteurs)
        {
            Vector3 position = centreTerrrain.position + new Vector3(Random.Range(5f, 20f), 0, Random.Range(-10f, 10f));
            joueur.transform.position = position;
        }
    }
    
    private void MettreAJourUI()
    {
        // Mettre à jour le temps
        int minutes = Mathf.FloorToInt(tempsEcoule / 60f);
        int secondes = Mathf.FloorToInt(tempsEcoule % 60f);
        texteTempPartie.text = string.Format("{0:00}:{1:00}", minutes, secondes);
        
        // Mettre à jour le score
        texteScoreLocale.text = scoreLocale.ToString();
        texteScoreVisiteur.text = scoreVisiteur.ToString();
    }
    
    public void MarquerBut(bool equipeLocaleMarque)
    {
        if (equipeLocaleMarque)
        {
            scoreLocale++;
            texteInfoMatch.text = "BUT pour " + equipeLocale.nomEquipe + "!";
        }
        else
        {
            scoreVisiteur++;
            texteInfoMatch.text = "BUT pour " + equipeVisiteur.nomEquipe + "!";
        }
        
        MettreAJourUI();
        StartCoroutine(EffacerMessage(3f));
        
        // Repositionner le ballon et les joueurs
        StartCoroutine(RepositionnerAprésBut());
    }
    
    private IEnumerator RepositionnerAprésBut()
    {
        matchEnCours = false;
        yield return new WaitForSeconds(2f);
        
        RepositionnerEquipes();
        
        yield return new WaitForSeconds(1f);
        matchEnCours = true;
    }
    
    private IEnumerator EffacerMessage(float delai)
    {
        yield return new WaitForSeconds(delai);
        texteInfoMatch.text = "";
    }
} 