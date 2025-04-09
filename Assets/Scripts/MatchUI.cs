using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MatchUI : MonoBehaviour
{
    [Header("Panneaux")]
    public GameObject panneauScore;
    public GameObject panneauInfoJoueur;
    public GameObject panneauMiTemps;
    public GameObject panneauFinMatch;
    public GameObject panneauPause;
    
    [Header("Éléments UI Score")]
    public Text texteNomEquipeDomicile;
    public Text texteNomEquipeExterieur;
    public Text texteScoreDomicile;
    public Text texteScoreExterieur;
    public Text texteTempsMatch;
    public Image logoEquipeDomicile;
    public Image logoEquipeExterieur;
    
    [Header("Éléments UI Joueur")]
    public Text texteNomJoueur;
    public Text texteStatsJoueur;
    public Image imageJoueur;
    public Slider sliderEndurance;
    
    [Header("Éléments UI Événement")]
    public Text texteEvenement;
    public GameObject panneauEvenement;
    
    [Header("Éléments UI Mi-temps")]
    public Text texteStatsMiTemps;
    public Text texteScoreMiTemps;
    
    [Header("Éléments UI Fin Match")]
    public Text texteStatsFinMatch;
    public Text texteScoreFinMatch;
    public Text texteResultatMatch;
    
    [Header("Animations")]
    public Animation animationBut;
    public Animation animationCarton;
    public Animation animationChangement;
    
    private GameManager gameManager;
    private PlayerController joueurSelectionne;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        
        // Initialiser l'UI
        InitialiserUI();
        
        // Cacher les panneaux qui ne doivent pas être visibles au début
        if (panneauInfoJoueur) panneauInfoJoueur.SetActive(false);
        if (panneauMiTemps) panneauMiTemps.SetActive(false);
        if (panneauFinMatch) panneauFinMatch.SetActive(false);
        if (panneauPause) panneauPause.SetActive(false);
        if (panneauEvenement) panneauEvenement.SetActive(false);
    }
    
    void Update()
    {
        // Mettre à jour le temps de match
        if (texteTempsMatch && gameManager)
        {
            int minutes = Mathf.FloorToInt(gameManager.tempsEcoule / 60f);
            int secondes = Mathf.FloorToInt(gameManager.tempsEcoule % 60f);
            texteTempsMatch.text = string.Format("{0:00}:{1:00}", minutes, secondes);
        }
        
        // Vérifier si un joueur est sélectionné
        if (joueurSelectionne)
        {
            MettreAJourInfoJoueur();
        }
        
        // Gestion de la pause
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            BasculerPause();
        }
    }
    
    private void InitialiserUI()
    {
        if (!gameManager) return;
        
        // Configurer les noms d'équipes et logos
        if (texteNomEquipeDomicile && gameManager.equipeLocale)
        {
            texteNomEquipeDomicile.text = gameManager.equipeLocale.nomEquipe;
        }
        
        if (texteNomEquipeExterieur && gameManager.equipeVisiteur)
        {
            texteNomEquipeExterieur.text = gameManager.equipeVisiteur.nomEquipe;
        }
        
        if (logoEquipeDomicile && gameManager.equipeLocale && gameManager.equipeLocale.logo)
        {
            logoEquipeDomicile.sprite = gameManager.equipeLocale.logo;
        }
        
        if (logoEquipeExterieur && gameManager.equipeVisiteur && gameManager.equipeVisiteur.logo)
        {
            logoEquipeExterieur.sprite = gameManager.equipeVisiteur.logo;
        }
        
        // Initialiser les scores
        if (texteScoreDomicile)
        {
            texteScoreDomicile.text = "0";
        }
        
        if (texteScoreExterieur)
        {
            texteScoreExterieur.text = "0";
        }
    }
    
    public void MettreAJourScore(int scoreLocal, int scoreVisiteur)
    {
        if (texteScoreDomicile)
        {
            texteScoreDomicile.text = scoreLocal.ToString();
        }
        
        if (texteScoreExterieur)
        {
            texteScoreExterieur.text = scoreVisiteur.ToString();
        }
    }
    
    public void SelectionnerJoueur(PlayerController joueur)
    {
        joueurSelectionne = joueur;
        
        if (panneauInfoJoueur)
        {
            panneauInfoJoueur.SetActive(true);
            MettreAJourInfoJoueur();
        }
    }
    
    private void MettreAJourInfoJoueur()
    {
        if (!joueurSelectionne) return;
        
        if (texteNomJoueur)
        {
            texteNomJoueur.text = joueurSelectionne.nomJoueur;
        }
        
        if (texteStatsJoueur)
        {
            string stats = "Position: " + joueurSelectionne.position + "\n";
            stats += "Technique: " + joueurSelectionne.technique + "\n";
            stats += "Tacle: " + joueurSelectionne.tacle + "\n";
            
            texteStatsJoueur.text = stats;
        }
        
        if (sliderEndurance)
        {
            sliderEndurance.value = joueurSelectionne.endurance / 100f;
        }
    }
    
    public void FermerInfoJoueur()
    {
        joueurSelectionne = null;
        
        if (panneauInfoJoueur)
        {
            panneauInfoJoueur.SetActive(false);
        }
    }
    
    public void AfficherEvenement(string texte, float duree = 3f)
    {
        if (texteEvenement && panneauEvenement)
        {
            texteEvenement.text = texte;
            panneauEvenement.SetActive(true);
            
            // Cacher le panneau après la durée spécifiée
            StartCoroutine(CacherEvenementApresDelai(duree));
        }
    }
    
    private IEnumerator CacherEvenementApresDelai(float delai)
    {
        yield return new WaitForSeconds(delai);
        
        if (panneauEvenement)
        {
            panneauEvenement.SetActive(false);
        }
    }
    
    public void AfficherMiTemps(int scoreLocal, int scoreVisiteur, string statsMatch)
    {
        if (panneauMiTemps)
        {
            panneauMiTemps.SetActive(true);
            
            if (texteScoreMiTemps)
            {
                texteScoreMiTemps.text = scoreLocal + " - " + scoreVisiteur;
            }
            
            if (texteStatsMiTemps)
            {
                texteStatsMiTemps.text = statsMatch;
            }
        }
    }
    
    public void FermerMiTemps()
    {
        if (panneauMiTemps)
        {
            panneauMiTemps.SetActive(false);
        }
    }
    
    public void AfficherFinMatch(int scoreLocal, int scoreVisiteur, string statsMatch, string resultat)
    {
        if (panneauFinMatch)
        {
            panneauFinMatch.SetActive(true);
            
            if (texteScoreFinMatch)
            {
                texteScoreFinMatch.text = scoreLocal + " - " + scoreVisiteur;
            }
            
            if (texteStatsFinMatch)
            {
                texteStatsFinMatch.text = statsMatch;
            }
            
            if (texteResultatMatch)
            {
                texteResultatMatch.text = resultat;
            }
        }
    }
    
    public void BasculerPause()
    {
        if (panneauPause)
        {
            bool estEnPause = !panneauPause.activeSelf;
            panneauPause.SetActive(estEnPause);
            
            // Mettre le jeu en pause ou le reprendre
            Time.timeScale = estEnPause ? 0f : 1f;
        }
    }
    
    public void ReprendreJeu()
    {
        if (panneauPause)
        {
            panneauPause.SetActive(false);
            Time.timeScale = 1f;
        }
    }
    
    public void QuitterMatch()
    {
        // Réinitialiser le timeScale avant de quitter
        Time.timeScale = 1f;
        
        // Charger le menu principal
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuPrincipal");
    }
    
    public void AfficherAnimationBut()
    {
        if (animationBut)
        {
            animationBut.Play();
        }
    }
    
    public void AfficherAnimationCarton(bool cartonRouge)
    {
        if (animationCarton)
        {
            // Changer la couleur du carton selon le type
            if (cartonRouge)
            {
                // Logique pour changer la couleur du carton en rouge
                Transform carton = animationCarton.transform.Find("Carton");
                if (carton)
                {
                    Image imgCarton = carton.GetComponent<Image>();
                    if (imgCarton)
                    {
                        imgCarton.color = Color.red;
                    }
                }
            }
            else
            {
                // Logique pour changer la couleur du carton en jaune
                Transform carton = animationCarton.transform.Find("Carton");
                if (carton)
                {
                    Image imgCarton = carton.GetComponent<Image>();
                    if (imgCarton)
                    {
                        imgCarton.color = Color.yellow;
                    }
                }
            }
            
            animationCarton.Play();
        }
    }
    
    public void AfficherAnimationChangement(string joueurSortant, string joueurEntrant)
    {
        if (animationChangement)
        {
            // Mettre à jour les noms des joueurs dans l'animation
            Transform texteSortant = animationChangement.transform.Find("TexteJoueurSortant");
            Transform texteEntrant = animationChangement.transform.Find("TexteJoueurEntrant");
            
            if (texteSortant)
            {
                Text txt = texteSortant.GetComponent<Text>();
                if (txt)
                {
                    txt.text = joueurSortant;
                }
            }
            
            if (texteEntrant)
            {
                Text txt = texteEntrant.GetComponent<Text>();
                if (txt)
                {
                    txt.text = joueurEntrant;
                }
            }
            
            animationChangement.Play();
        }
    }
} 