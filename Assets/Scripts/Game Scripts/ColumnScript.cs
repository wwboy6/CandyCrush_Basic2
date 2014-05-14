using UnityEngine;
using System.Collections;

public class ColumnScript : MonoBehaviour 
{
    internal int columnIndex = 0;    

    internal ArrayList playingObjectsScriptList;
    internal ArrayList jellyObjects;

    public PlayingObject[] playingObjectsScript;

    public int[] itemAvailability;
    public int[] jellyAvailability;

    int totalNoOfItems = 0;
    int numberOfEmptySpace = 0;

    int numberOfRows;

    void Awake()
    {
        
    }
	
	void Start () 
    {


//        Invoke("PopulateInitialColumn", .2f);

        InvokeRepeating("UpdateList", 1, 1);

    }

    void UpdateList()
    {
        if (GameManager.instance.isBusy)
            return;

        playingObjectsScript = new PlayingObject[numberOfRows];
        for (int i = 0; i < playingObjectsScript.Length; i++)
        {
            playingObjectsScript[i] = (PlayingObject)playingObjectsScriptList[i];
        }
    }

    void AddJelly(int index)
    {
        GameObject parent = null;

        jellyAvailability[index] = Mathf.Clamp(jellyAvailability[index], 0, 2);

        for (int i = 0; i < jellyAvailability[index]; i++)
        {
            GameManager.instance.totalNoOfJellies++;
            GameManager.instance.jellyText.text = "Jelly : " + GameManager.instance.totalNoOfJellies.ToString();
            GameObject temp = (GameObject)Instantiate(GameManager.instance.jellyPrefab[i], Vector3.zero, Quaternion.identity);
            temp.transform.parent = transform;
            temp.transform.localPosition = new Vector3(0, -index * GameManager.instance.gapBetweenObjects, -.5f + i * .1f);
            temp.transform.localEulerAngles = new Vector3(90, 0, 0);
            
            if (parent == null)
            {                
                jellyObjects.Add(temp);
                parent = temp;
            }
            else
            {
                temp.transform.parent = parent.transform;
            }

            parent = temp;
            
        }
        

        
    }

    void CheckForJelly(int i)
    {
        if (jellyAvailability != null)
        {
            if (jellyAvailability[i] >= 1)
            {
                AddJelly(i);
            }
            else
                jellyObjects.Add(null);
        }
        else
            jellyObjects.Add(null);
    }
	
	//TODO:debug
//	static int[,] indexMap = new int[,]{
//		{0,1,2,3,4,5,0,1},
//		{5,0,1,2,3,4,5,0},
//		{4,5,0,1,2,3,4,5},
//		{3,4,5,0,1,2,3,4},
//		{2,3,4,5,0,1,2,4}
//	};

    public void PopulateInitialColumn(int[] indexes)
	{
		numberOfRows = LevelStructure.instance.numberOfRows;
		
		playingObjectsScriptList = new ArrayList();
		jellyObjects = new ArrayList();
		
		itemAvailability = LevelStructure.instance.columnStructures[columnIndex].itemAvailability;
		
		if(LevelStructure.instance.columnStructures[columnIndex].jelly.Length > 0)
			jellyAvailability = LevelStructure.instance.columnStructures[columnIndex].jelly;
		
		for (int i = 0; i < itemAvailability.Length; i++)
        {
            
            if (itemAvailability[i] >= 1)
                totalNoOfItems++;
        }
        

        numberOfEmptySpace = numberOfRows - totalNoOfItems;


        for (int i = 0; i < numberOfRows; i++)
        {
            if (itemAvailability[i] == 0)
            {
                playingObjectsScriptList.Add(null);
                jellyObjects.Add(null);
                continue;
            }
            

//            int index = Random.Range(0, 6);
			int index = indexes[i];

			//TODO:debug
//			if (GameOperations.debug) {
//				itemAvailability[i] = 1;
//				index = indexMap[i, columnIndex];
//			}

            GameObject objectPrefab;

            if (itemAvailability[i] == 1)
            {
                objectPrefab = GameManager.instance.playingObjectPrefabs[index];                
            }
            else if (itemAvailability[i] == 2)
            {
                if (Random.value < .5f)
                    objectPrefab = GameManager.instance.horizontalPrefabs[index];
                else
                    objectPrefab = GameManager.instance.verticalPrefabs[index];
            }
            else if (itemAvailability[i] == 3)
                objectPrefab = GameManager.instance.universalPlayingObjectPrefab;
            else
                objectPrefab = GameManager.instance.playingObjectPrefabs[index];

            CheckForJelly(i);

            GameObject temp = (GameObject)Instantiate(objectPrefab, Vector3.zero, Quaternion.identity);
            temp.GetComponent<PlayingObject>().myColumnScript = this;
            temp.GetComponent<PlayingObject>().indexInColumn = i;
            playingObjectsScriptList.Add(temp.GetComponent<PlayingObject>());
            temp.transform.parent = transform;
            temp.transform.localPosition = new Vector3(0, -i * GameManager.instance.gapBetweenObjects, 0);

            GameObject temp1 = (GameObject)Instantiate(GamePrefabs.instance.playingObjectBackPrefab, temp.transform.position - new Vector3(0, 0, 1), Quaternion.identity);
            temp1.transform.localEulerAngles = new Vector3(90, 0, 0);
        }        
    }

    internal void AssignNeighbours()
    {
        for (int i = 0; i < playingObjectsScriptList.Count; i++)
        {
            if (playingObjectsScriptList[i] == null)
                continue;

            if (columnIndex == 0) //left
                ((PlayingObject)playingObjectsScriptList[i]).adjacentItems[0] = null;
            else
                ((PlayingObject)playingObjectsScriptList[i]).adjacentItems[0] = ((PlayingObject)ColumnManager.instance.gameColumns[columnIndex - 1].playingObjectsScriptList[i]);

            if (columnIndex == ColumnManager.instance.gameColumns.Length - 1) // right
                ((PlayingObject)playingObjectsScriptList[i]).adjacentItems[1] = null;
            else
                ((PlayingObject)playingObjectsScriptList[i]).adjacentItems[1] = ((PlayingObject)ColumnManager.instance.gameColumns[columnIndex + 1].playingObjectsScriptList[i]);

            if (i == 0) //up
                ((PlayingObject)playingObjectsScriptList[i]).adjacentItems[2] = null;
            else
                ((PlayingObject)playingObjectsScriptList[i]).adjacentItems[2] = ((PlayingObject)playingObjectsScriptList[i - 1]);

            if (i == numberOfRows - 1) // down
                ((PlayingObject)playingObjectsScriptList[i]).adjacentItems[3] = null;
            else
                ((PlayingObject)playingObjectsScriptList[i]).adjacentItems[3] = ((PlayingObject)playingObjectsScriptList[i + 1]);
        }
    }

    internal void ChangeItem(int index, GameObject newItemPrefab,string _name)
    {
        GameObject temp = (GameObject)Instantiate(newItemPrefab, Vector3.zero, Quaternion.identity);
        temp.GetComponent<PlayingObject>().myColumnScript = this;
        temp.GetComponent<PlayingObject>().indexInColumn = index;
        playingObjectsScriptList[index] = temp.GetComponent<PlayingObject>();
        temp.transform.parent = transform;
        temp.transform.localPosition = new Vector3(0, -index * GameManager.instance.gapBetweenObjects, 0);
        temp.GetComponent<SpecialPlayingObject>().name = _name;
       // iTween.ScaleFrom(temp, Vector3.zero, .6f);
        
     //   print("new Candy Created");
    }

    internal void DeleteBrustedItems()
    {       

        for (int i = 0; i < numberOfRows; i++)
        {
            if (playingObjectsScriptList[i] != null)
            {
                if (((PlayingObject)playingObjectsScriptList[i]).brust)
                {
                    ((PlayingObject)playingObjectsScriptList[i]).DestroyMe();

                    GameObject specialObject = ((PlayingObject)playingObjectsScriptList[i]).WillFormSpecialObject();

                    if (specialObject!= null)
                    {
                        ChangeItem(i, specialObject, ((PlayingObject)playingObjectsScriptList[i]).name);
                    }
                    else
                        playingObjectsScriptList[i] = null;
                    
                    
                }
            }
        }

        int count = 0;
        for (int i = 0; i < playingObjectsScriptList.Count; i++,count++)
        {
            if ((PlayingObject)playingObjectsScriptList[i] == null && itemAvailability[count] >= 1)
            {
                playingObjectsScriptList.RemoveAt(i);
                i--;
            }
        }

        numberOfItemsToAdd = numberOfRows - playingObjectsScriptList.Count;
    }

    void ArrangeColumnItems()
    {
        for (int i = numberOfRows - 1; i >= 0; i--)
        {
            if (itemAvailability[i] >= 1 && playingObjectsScriptList[i] == null)
            {
                for (int j = i; j >= 0; j--)
                {
                    if (playingObjectsScriptList[j] != null)
                    {                        
                        playingObjectsScriptList[i] = playingObjectsScriptList[j];
                        playingObjectsScriptList[j] = null;
                        break;
                    }
                }
            }
        }
    }

    int numberOfItemsToAdd;

    internal int GetNumberOfItemsToAdd()
    {
        return numberOfRows - playingObjectsScriptList.Count;
    }

    internal void AddMissingItems()
    {
		Debug.Log ("AddMissingItems");

        numberOfItemsToAdd = numberOfRows - playingObjectsScriptList.Count;
        if (numberOfItemsToAdd == 0)
            return;
        
        for (int i = 0; i < numberOfItemsToAdd; i++)
        {
            GameObject temp = (GameObject)Instantiate(GameManager.instance.playingObjectPrefabs[Random.Range(0, 6)], Vector3.zero, Quaternion.identity);
            temp.GetComponent<PlayingObject>().myColumnScript = this;
            playingObjectsScriptList.Insert(0, temp.GetComponent<PlayingObject>()); //numberOfEmptySpace
            temp.transform.parent = transform;
            temp.transform.localPosition = new Vector3(0, (i + 1) * GameManager.instance.gapBetweenObjects, 0);
        }
        
        ArrangeColumnItems();

        for (int i = 0; i < playingObjectsScriptList.Count; i++)
        {
            if ((PlayingObject)playingObjectsScriptList[i] != null)
                ((PlayingObject)playingObjectsScriptList[i]).indexInColumn = i;
        }

        // iTween.Defaults.easeType = iTween.EaseType.bounce;
        iTween.Defaults.easeType = GameManager.instance.objectfallingEase;

        fallingDuration = GameManager.instance.initialObjectFallingDuration * (.9f + numberOfItemsToAdd / 10f);

        GameManager.instance.objectFallingDuration = Mathf.Max(GameManager.instance.objectFallingDuration, fallingDuration);

        SoundFxManager.instance.PlayFallingSound();
        StartMovingDownOldPart();
        StartMovingDownNewPart();
        SoundFxManager.instance.Invoke("StopFallingSound", fallingDuration * .8f);
       // Invoke("PlayColumnFallSound", fallingDuration * .2f);
    }

    void PlayColumnFallSound()
    {
        SoundFxManager.instance.columnFallSound.Play();
    }

    float fallingDuration;

    void StartMovingDownOldPart()
    {
       
        for (int i = numberOfItemsToAdd; i < playingObjectsScriptList.Count; i++)
        {
            if(itemAvailability[i] >= 1)
                iTween.MoveTo(((PlayingObject)playingObjectsScriptList[i]).gameObject, new Vector3(transform.position.x, -i * GameManager.instance.gapBetweenObjects + transform.position.y, 0), fallingDuration);
        }
    }

    void StartMovingDownNewPart()
    {       

        for (int i = 0; i < numberOfItemsToAdd; i++)
        {
            if (itemAvailability[i] >= 1)
                iTween.MoveTo(((PlayingObject)playingObjectsScriptList[i]).gameObject, new Vector3(transform.position.x, -i * GameManager.instance.gapBetweenObjects + transform.position.y, 0), fallingDuration);
        }

       
    }

    internal void UnCheckTracedAttribute()
    {
        for (int i = 0; i < playingObjectsScriptList.Count; i++)
        {
            ((PlayingObject)playingObjectsScriptList[i]).isTraced = false;           
        }
    }

    internal void UnCheckBrustAttribute()
    {
        for (int i = 0; i < playingObjectsScriptList.Count; i++)
        {
            ((PlayingObject)playingObjectsScriptList[i]).brust = false;
        }
    }

    internal void AssignBrustToAllItemsOfName(string itemName)
    {
        for (int i = 0; i < playingObjectsScriptList.Count; i++)
        {
            if ((PlayingObject)playingObjectsScriptList[i])
            {
                if (((PlayingObject)playingObjectsScriptList[i]).name == itemName)
                    ((PlayingObject)playingObjectsScriptList[i]).AssignBurst("smoke");
            }
        }
    }

    

    
    
	
	
}
