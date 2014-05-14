using UnityEngine;
using System.Collections;

public class GamePrefabs : MonoBehaviour 
{
    public static GamePrefabs instance;
    

    public GameObject horWavePrefab;
    public GameObject verWavePrefab;
    public GameObject electricFieldPrefab;
    public GameObject playingObjectBackPrefab;
    public GameObject starParticles;
    public GameObject jellyParticles;
    public GameObject smokeParticles;


    void Awake()
    {
        instance = this;
    }

    
}
