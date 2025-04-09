using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NouvelleEquipe", menuName = "IFP/Configuration d'Équipe")]
public class TeamConfig : ScriptableObject
{
    [Header("Informations d'équipe")]
    public string nomEquipe = "LFG United";
    public string abreviationEquipe = "LFG";
    public Sprite logo;
    public Color couleurPrincipale = Color.blue;
    public Color couleurSecondaire = Color.white;
    
    [Header("Joueurs")]
    public string[] nomsJoueurs = new string[11]
    {
        "Gardien LFG",
        "Défenseur LFG 1",
        "Défenseur LFG 2",
        "Défenseur LFG 3",
        "Défenseur LFG 4",
        "Milieu LFG 1",
        "Milieu LFG 2",
        "Milieu LFG 3",
        "Attaquant LFG 1",
        "Attaquant LFG 2",
        "Attaquant LFG 3"
    };
    
    public string[] positionsJoueurs = new string[11]
    {
        "Gardien",
        "Défenseur",
        "Défenseur",
        "Défenseur",
        "Défenseur",
        "Milieu",
        "Milieu",
        "Milieu",
        "Attaquant",
        "Attaquant",
        "Attaquant"
    };
    
    [Header("Statistiques d'équipe")]
    public int attaque = 80;
    public int defense = 75;
    public int milieu = 82;
    public int formationTactique = 4; // 0=433, 1=442, 2=352, 3=532, 4=4231
    
    [Header("Style de jeu")]
    [Range(0, 100)]
    public int possessionBalle = 60;
    
    [Range(0, 100)]
    public int pressionHaute = 70;
    
    [Range(0, 100)]
    public int contreAttaque = 65;
    
    // Méthodes pour obtenir les statistiques des joueurs individuels
    public Dictionary<string, int> GetStatsJoueur(int index)
    {
        if (index < 0 || index >= nomsJoueurs.Length)
            return null;
        
        Dictionary<string, int> stats = new Dictionary<string, int>();
        string position = positionsJoueurs[index];
        
        // Valeurs de base selon la position
        switch (position)
        {
            case "Gardien":
                stats.Add("Arrêts", Random.Range(70, 95));
                stats.Add("Réflexes", Random.Range(75, 90));
                stats.Add("Placement", Random.Range(70, 90));
                stats.Add("Passes", Random.Range(50, 75));
                stats.Add("Dégagements", Random.Range(65, 90));
                break;
                
            case "Défenseur":
                stats.Add("Tacles", Random.Range(75, 90));
                stats.Add("Marquage", Random.Range(70, 90));
                stats.Add("Force", Random.Range(70, 90));
                stats.Add("Interception", Random.Range(70, 90));
                stats.Add("Passes", Random.Range(60, 85));
                break;
                
            case "Milieu":
                stats.Add("Passes", Random.Range(75, 95));
                stats.Add("Vision", Random.Range(70, 90));
                stats.Add("Technique", Random.Range(70, 90));
                stats.Add("Tirs", Random.Range(65, 85));
                stats.Add("Endurance", Random.Range(75, 90));
                break;
                
            case "Attaquant":
                stats.Add("Finition", Random.Range(75, 95));
                stats.Add("Tirs", Random.Range(75, 95));
                stats.Add("Vitesse", Random.Range(75, 95));
                stats.Add("Dribbles", Random.Range(70, 90));
                stats.Add("Positionnement", Random.Range(75, 90));
                break;
        }
        
        // Ajout de facteur aléatoire pour créer des différences entre joueurs de même poste
        foreach (string key in new List<string>(stats.Keys))
        {
            stats[key] = Mathf.Clamp(stats[key] + Random.Range(-10, 10), 50, 99);
        }
        
        return stats;
    }
    
    // Obtenir la formation sous forme de vecteurs de position
    public Vector3[] GetPositionsFormation()
    {
        Vector3[] positions = new Vector3[11];
        
        // Position du gardien
        positions[0] = new Vector3(0, 0, -40);
        
        // Positions des autres joueurs selon la formation
        switch (formationTactique)
        {
            case 0: // 4-3-3
                // Défenseurs
                positions[1] = new Vector3(-15, 0, -30);
                positions[2] = new Vector3(-5, 0, -30);
                positions[3] = new Vector3(5, 0, -30);
                positions[4] = new Vector3(15, 0, -30);
                
                // Milieux
                positions[5] = new Vector3(-10, 0, -15);
                positions[6] = new Vector3(0, 0, -15);
                positions[7] = new Vector3(10, 0, -15);
                
                // Attaquants
                positions[8] = new Vector3(-15, 0, 0);
                positions[9] = new Vector3(0, 0, 0);
                positions[10] = new Vector3(15, 0, 0);
                break;
                
            case 1: // 4-4-2
                // Défenseurs
                positions[1] = new Vector3(-15, 0, -30);
                positions[2] = new Vector3(-5, 0, -30);
                positions[3] = new Vector3(5, 0, -30);
                positions[4] = new Vector3(15, 0, -30);
                
                // Milieux
                positions[5] = new Vector3(-15, 0, -15);
                positions[6] = new Vector3(-5, 0, -15);
                positions[7] = new Vector3(5, 0, -15);
                positions[8] = new Vector3(15, 0, -15);
                
                // Attaquants
                positions[9] = new Vector3(-7, 0, 0);
                positions[10] = new Vector3(7, 0, 0);
                break;
                
            case 2: // 3-5-2
                // Défenseurs
                positions[1] = new Vector3(-10, 0, -30);
                positions[2] = new Vector3(0, 0, -30);
                positions[3] = new Vector3(10, 0, -30);
                
                // Milieux
                positions[4] = new Vector3(-20, 0, -15);
                positions[5] = new Vector3(-10, 0, -15);
                positions[6] = new Vector3(0, 0, -15);
                positions[7] = new Vector3(10, 0, -15);
                positions[8] = new Vector3(20, 0, -15);
                
                // Attaquants
                positions[9] = new Vector3(-7, 0, 0);
                positions[10] = new Vector3(7, 0, 0);
                break;
                
            case 3: // 5-3-2
                // Défenseurs
                positions[1] = new Vector3(-20, 0, -30);
                positions[2] = new Vector3(-10, 0, -30);
                positions[3] = new Vector3(0, 0, -30);
                positions[4] = new Vector3(10, 0, -30);
                positions[5] = new Vector3(20, 0, -30);
                
                // Milieux
                positions[6] = new Vector3(-10, 0, -15);
                positions[7] = new Vector3(0, 0, -15);
                positions[8] = new Vector3(10, 0, -15);
                
                // Attaquants
                positions[9] = new Vector3(-7, 0, 0);
                positions[10] = new Vector3(7, 0, 0);
                break;
                
            case 4: // 4-2-3-1
                // Défenseurs
                positions[1] = new Vector3(-15, 0, -30);
                positions[2] = new Vector3(-5, 0, -30);
                positions[3] = new Vector3(5, 0, -30);
                positions[4] = new Vector3(15, 0, -30);
                
                // Milieux défensifs
                positions[5] = new Vector3(-7, 0, -20);
                positions[6] = new Vector3(7, 0, -20);
                
                // Milieux offensifs
                positions[7] = new Vector3(-15, 0, -10);
                positions[8] = new Vector3(0, 0, -10);
                positions[9] = new Vector3(15, 0, -10);
                
                // Attaquant
                positions[10] = new Vector3(0, 0, 0);
                break;
        }
        
        return positions;
    }
} 