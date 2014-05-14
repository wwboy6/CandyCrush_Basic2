using UnityEngine;
using System.Collections;

public class LevelStructure : MonoBehaviour 
{
    public static LevelStructure instance;

    internal int numberOfColumns;
    public int numberOfRows = 5;
    internal ColumnStructure[] columnStructures;


    void Awake()
    {
        instance = this;
        numberOfColumns = transform.GetChildCount();
        columnStructures = transform.GetComponentsInChildren<ColumnStructure>();
    }

	void Start () 
    {
	
	}
	
	
}
