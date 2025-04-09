using UnityEngine;

public class GoalDetector : MonoBehaviour
{
    [Header("Configuration")]
    public bool butEquipeLocale = true; // Si false, alors c'est un but pour l'équipe visiteur
    public ParticleSystem effetBut;
    public AudioClip sonBut;
    public AudioClip sonCri;
    
    private GameManager gameManager;
    private AudioSource audioSource;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        audioSource = GetComponent<AudioSource>();
        if (!audioSource)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ballon"))
        {
            // Marquer le but
            if (gameManager)
            {
                // Le but est marqué contre l'équipe à qui appartient le but
                gameManager.MarquerBut(!butEquipeLocale);
                
                // Déclencher effets
                if (effetBut)
                {
                    effetBut.Play();
                }
                
                if (audioSource && sonBut)
                {
                    audioSource.clip = sonBut;
                    audioSource.Play();
                    
                    // Lancer le son de cri de foule après un bref délai
                    if (sonCri)
                    {
                        StartCoroutine(JouerSonApresDelai(sonCri, 0.5f));
                    }
                }
            }
        }
    }
    
    private System.Collections.IEnumerator JouerSonApresDelai(AudioClip clip, float delai)
    {
        yield return new WaitForSeconds(delai);
        
        // Créer un source audio temporaire pour jouer le son de la foule
        GameObject tempGO = new GameObject("TempAudio");
        AudioSource tempSource = tempGO.AddComponent<AudioSource>();
        tempSource.clip = clip;
        tempSource.spatialBlend = 0f; // Son 2D
        tempSource.volume = 0.8f;
        tempSource.Play();
        
        // Détruire l'objet temporaire une fois le son terminé
        Destroy(tempGO, clip.length + 0.1f);
    }
} 