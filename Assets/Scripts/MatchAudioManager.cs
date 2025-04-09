using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MatchAudioManager : MonoBehaviour
{
    [Header("Sons d'ambiance")]
    public AudioClip ambianceFoule;
    public AudioClip acclaimations;
    public AudioClip huees;
    public AudioClip crisFouleExcitee;
    public AudioClip[] chantsSupporter;
    
    [Header("Sons de match")]
    public AudioClip siffletDebut;
    public AudioClip siffletFin;
    public AudioClip siffletMiTemps;
    public AudioClip siffletFaute;
    public AudioClip[] commentateursButMarque;
    public AudioClip[] commentateursOccasionManquee;
    public AudioClip[] commentateursJolieAction;
    
    [Header("Sons de jeu")]
    public AudioClip frappeBallon;
    public AudioClip contactJoueurs;
    public AudioClip[] encouragementCoach;
    
    [Header("Sources Audio")]
    public AudioSource sourceAmbianceFoule;
    public AudioSource sourceEffetsMatch;
    public AudioSource sourceCommentaires;
    public AudioSource sourceSifflet;
    
    private GameManager gameManager;
    private float tempsDernierChant = 0f;
    private float intervalleChantsMin = 30f;
    private float intervalleChantsMax = 120f;
    private float tempsDernierEncouragement = 0f;
    private float prochainChant;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        
        // Créer les sources audio si elles n'existent pas
        if (!sourceAmbianceFoule)
        {
            GameObject goAmbiance = new GameObject("SourceAmbianceFoule");
            goAmbiance.transform.parent = transform;
            sourceAmbianceFoule = goAmbiance.AddComponent<AudioSource>();
            sourceAmbianceFoule.loop = true;
            sourceAmbianceFoule.volume = 0.4f;
            sourceAmbianceFoule.spatialBlend = 0f;
            sourceAmbianceFoule.priority = 0;
        }
        
        if (!sourceEffetsMatch)
        {
            GameObject goEffets = new GameObject("SourceEffetsMatch");
            goEffets.transform.parent = transform;
            sourceEffetsMatch = goEffets.AddComponent<AudioSource>();
            sourceEffetsMatch.loop = false;
            sourceEffetsMatch.volume = 0.7f;
            sourceEffetsMatch.spatialBlend = 0.3f;
            sourceEffetsMatch.priority = 128;
        }
        
        if (!sourceCommentaires)
        {
            GameObject goCommentaires = new GameObject("SourceCommentaires");
            goCommentaires.transform.parent = transform;
            sourceCommentaires = goCommentaires.AddComponent<AudioSource>();
            sourceCommentaires.loop = false;
            sourceCommentaires.volume = 0.8f;
            sourceCommentaires.spatialBlend = 0f;
            sourceCommentaires.priority = 64;
        }
        
        if (!sourceSifflet)
        {
            GameObject goSifflet = new GameObject("SourceSifflet");
            goSifflet.transform.parent = transform;
            sourceSifflet = goSifflet.AddComponent<AudioSource>();
            sourceSifflet.loop = false;
            sourceSifflet.volume = 0.6f;
            sourceSifflet.spatialBlend = 0.2f;
            sourceSifflet.priority = 32;
        }
        
        // Démarrer l'ambiance de foule
        if (ambianceFoule && sourceAmbianceFoule)
        {
            sourceAmbianceFoule.clip = ambianceFoule;
            sourceAmbianceFoule.Play();
        }
        
        // Planifier le premier chant
        prochainChant = Random.Range(intervalleChantsMin, intervalleChantsMax);
        tempsDernierChant = -prochainChant; // Pour qu'un chant soit joué assez rapidement
        
        // Jouer le coup de sifflet de début
        if (siffletDebut && sourceSifflet)
        {
            sourceSifflet.clip = siffletDebut;
            sourceSifflet.Play();
        }
    }
    
    void Update()
    {
        // Vérifier s'il est temps de jouer un chant de supporter
        if (Time.time - tempsDernierChant > prochainChant && chantsSupporter != null && chantsSupporter.Length > 0)
        {
            JouerChantSupporters();
            tempsDernierChant = Time.time;
            prochainChant = Random.Range(intervalleChantsMin, intervalleChantsMax);
        }
        
        // Vérifier s'il faut jouer un encouragement du coach (tous les 30-60 secondes)
        if (Time.time - tempsDernierEncouragement > Random.Range(30f, 60f) && encouragementCoach != null && encouragementCoach.Length > 0)
        {
            JouerEncouragementCoach();
            tempsDernierEncouragement = Time.time;
        }
    }
    
    public void JouerSiffletMiTemps()
    {
        if (sourceSifflet && siffletMiTemps)
        {
            sourceSifflet.clip = siffletMiTemps;
            sourceSifflet.Play();
        }
    }
    
    public void JouerSiffletFinMatch()
    {
        if (sourceSifflet && siffletFin)
        {
            sourceSifflet.clip = siffletFin;
            sourceSifflet.Play();
        }
    }
    
    public void JouerSonButMarque(bool equipeLocale)
    {
        // Jouer les acclamations de la foule
        if (sourceEffetsMatch && acclaimations)
        {
            sourceEffetsMatch.clip = acclaimations;
            sourceEffetsMatch.volume = 1.0f;
            sourceEffetsMatch.Play();
        }
        
        // Jouer un commentaire de but
        if (sourceCommentaires && commentateursButMarque != null && commentateursButMarque.Length > 0)
        {
            StartCoroutine(JouerCommentaireApresDelai(commentateursButMarque[Random.Range(0, commentateursButMarque.Length)], 1.0f));
        }
    }
    
    public void JouerSonOccasionManquee()
    {
        // Réaction de la foule (léger "oh" de déception)
        if (sourceEffetsMatch && crisFouleExcitee)
        {
            sourceEffetsMatch.clip = crisFouleExcitee;
            sourceEffetsMatch.volume = 0.6f;
            sourceEffetsMatch.Play();
        }
        
        // Commentaire d'occasion manquée
        if (sourceCommentaires && commentateursOccasionManquee != null && commentateursOccasionManquee.Length > 0)
        {
            sourceCommentaires.clip = commentateursOccasionManquee[Random.Range(0, commentateursOccasionManquee.Length)];
            sourceCommentaires.Play();
        }
    }
    
    public void JouerSonFaute()
    {
        if (sourceSifflet && siffletFaute)
        {
            sourceSifflet.clip = siffletFaute;
            sourceSifflet.Play();
        }
        
        // Huées de la foule
        if (sourceEffetsMatch && huees)
        {
            StartCoroutine(JouerSonEffetApresDelai(huees, 0.5f));
        }
    }
    
    public void JouerSonFrappeBallon(float puissance)
    {
        if (sourceEffetsMatch && frappeBallon)
        {
            // Adapter le volume et le pitch à la puissance de la frappe
            sourceEffetsMatch.volume = Mathf.Clamp(puissance / 15f, 0.3f, 1.0f);
            sourceEffetsMatch.pitch = Mathf.Clamp(0.8f + puissance / 20f, 0.8f, 1.2f);
            sourceEffetsMatch.PlayOneShot(frappeBallon);
        }
    }
    
    public void JouerSonContactJoueurs(float intensite)
    {
        if (sourceEffetsMatch && contactJoueurs)
        {
            // Adapter le volume à l'intensité du contact
            sourceEffetsMatch.volume = Mathf.Clamp(intensite / 10f, 0.2f, 0.8f);
            sourceEffetsMatch.pitch = Random.Range(0.9f, 1.1f);
            sourceEffetsMatch.PlayOneShot(contactJoueurs);
        }
    }
    
    public void JouerSonJolieAction()
    {
        // Excitation de la foule
        if (sourceEffetsMatch && crisFouleExcitee)
        {
            sourceEffetsMatch.clip = crisFouleExcitee;
            sourceEffetsMatch.volume = 0.7f;
            sourceEffetsMatch.Play();
        }
        
        // Commentaire sur la jolie action
        if (sourceCommentaires && commentateursJolieAction != null && commentateursJolieAction.Length > 0)
        {
            StartCoroutine(JouerCommentaireApresDelai(commentateursJolieAction[Random.Range(0, commentateursJolieAction.Length)], 0.8f));
        }
    }
    
    private void JouerChantSupporters()
    {
        if (sourceEffetsMatch && chantsSupporter != null && chantsSupporter.Length > 0)
        {
            int indexChant = Random.Range(0, chantsSupporter.Length);
            sourceEffetsMatch.clip = chantsSupporter[indexChant];
            sourceEffetsMatch.volume = 0.5f;
            sourceEffetsMatch.Play();
        }
    }
    
    private void JouerEncouragementCoach()
    {
        if (sourceEffetsMatch && encouragementCoach != null && encouragementCoach.Length > 0)
        {
            int index = Random.Range(0, encouragementCoach.Length);
            // Jouer à faible volume, comme en arrière-plan
            sourceEffetsMatch.PlayOneShot(encouragementCoach[index], 0.3f);
        }
    }
    
    private IEnumerator JouerCommentaireApresDelai(AudioClip commentaire, float delai)
    {
        yield return new WaitForSeconds(delai);
        
        if (sourceCommentaires && commentaire)
        {
            sourceCommentaires.clip = commentaire;
            sourceCommentaires.Play();
        }
    }
    
    private IEnumerator JouerSonEffetApresDelai(AudioClip effet, float delai)
    {
        yield return new WaitForSeconds(delai);
        
        if (sourceEffetsMatch && effet)
        {
            sourceEffetsMatch.clip = effet;
            sourceEffetsMatch.Play();
        }
    }
    
    // Méthode pour ajuster le volume de la foule en fonction de l'intensité du match
    public void AjusterVolumeFoule(float intensite)
    {
        if (sourceAmbianceFoule)
        {
            // Intensité est une valeur entre 0 et 1
            intensite = Mathf.Clamp01(intensite);
            sourceAmbianceFoule.volume = Mathf.Lerp(0.3f, 0.7f, intensite);
        }
    }
} 