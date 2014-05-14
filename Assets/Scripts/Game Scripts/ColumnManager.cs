using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColumnManager : MonoBehaviour 
{
    public static ColumnManager instance;
    internal ColumnScript[] gameColumns;
    internal int numberOfColumns;
    

    void Awake()
    {
        instance = this;
        
    }
	
	void Start () 
    {
        numberOfColumns = LevelStructure.instance.numberOfColumns;

        gameColumns = new ColumnScript[numberOfColumns];

        for (int i = 0; i < gameColumns.Length; i++)
        {
            GameObject temp1 = new GameObject();
            gameColumns[i] = temp1.AddComponent<ColumnScript>();
            temp1.transform.parent = transform;
            temp1.name = "Column " + i.ToString();
        }

        float x = 2.5f;

        if (numberOfColumns % 2 == 1)
        {
            x = (numberOfColumns / 2) * GameManager.instance.gapBetweenObjects;
        }
        else
        {
            x = (numberOfColumns / 2) * GameManager.instance.gapBetweenObjects - GameManager.instance.gapBetweenObjects * .5f;
        }

        for (int i = 0; i < gameColumns.Length; i++)
        {
            if (i < numberOfColumns)
            {
                gameColumns[i].columnIndex = i;
                gameColumns[i].transform.localPosition = new Vector3(x - i * GameManager.instance.gapBetweenObjects, 0, 0);
            }
            else
                Destroy(gameColumns[i].gameObject);
        }

        ColumnScript[] temp = gameColumns;
        gameColumns = new ColumnScript[numberOfColumns];

        for (int i = 0; i < numberOfColumns; i++)
        {
            gameColumns[i] = temp[i];
        }


		//generate color indexes
		int[][] indexMap = new int[numberOfColumns][];
		for (int col=0; col<numberOfColumns; ++col) {
			indexMap[col] = new int[LevelStructure.instance.numberOfRows];
			for (int row=0; row<LevelStructure.instance.numberOfRows; ++row) {
				for (;;) {
					int index = Random.Range(0, 6);
					indexMap[col][row] = index;

					//deep first search from new color index
					if (
						(col > 0 && indexMap[col-1][row] == index &&
					 row > 0 && indexMap[col][row-1] == index)
						||
						(col > 0 && indexMap[col-1][row] == index
					 && (col > 1 && indexMap[col-2][row] == index ||
					    row > 0 && indexMap[col-1][row-1] == index ||
					    row < LevelStructure.instance.numberOfRows-1 && indexMap[col-1][row+1] == index))
						||
						(row > 0 && indexMap[col][row-1] == index
					 && (row > 2 && indexMap[col][row-2] == index ||
					    col > 0 && indexMap[col-1][row-1] == index))
						) {

						//color group detected, continue for rand another
						continue;
					}

					break;
				}
			}

			gameColumns[col].PopulateInitialColumn(indexMap[col]);
		}

	
	}

    
	
	
	public void Swipe(PlayingObject item1, PlayingObject item2)
	{
		ColumnScript firstColumn = item1.myColumnScript;
		ColumnScript secondColumn = item2.myColumnScript;

		item1.transform.parent = secondColumn.transform;
		item2.transform.parent = firstColumn.transform;
		
		item1.myColumnScript = secondColumn;
		item2.myColumnScript = firstColumn;
		
		firstColumn.playingObjectsScriptList.RemoveAt(item1.indexInColumn);
		firstColumn.playingObjectsScriptList.Insert(item1.indexInColumn, item2);
		
		secondColumn.playingObjectsScriptList.RemoveAt(item2.indexInColumn);
		secondColumn.playingObjectsScriptList.Insert(item2.indexInColumn, item1);
		
		int tempIndex = item1.indexInColumn;
		item1.indexInColumn = item2.indexInColumn;
		item2.indexInColumn = tempIndex;
	}

	public Vector2 getPosOfPlayingObject(PlayingObject po) {
		return new Vector2(po.myColumnScript.columnIndex, po.indexInColumn);
	}
	
	public PlayingObject getPlayingObjectOfPos(int column, int row) {
		if (column<0 || column>=gameColumns.Length || !gameColumns[column]) return null;
		if (row<0 || row>=gameColumns[column].playingObjectsScriptList.Count) return null;
		return (PlayingObject) gameColumns[column].playingObjectsScriptList[row];
	}

	public static int successfulBurstMinCount = 3;

	//returnForOne - return for any one event occurs for checking only
	public List<BurstEvent> checkBurst(bool returnForOne = false, PlayingObject[] swappingItems = null) {
		//TODO:debug
		returnForOne = false;

		List<BurstEvent> burstEvents = new List<BurstEvent>();

		//record the searched objects for minimizing the looping
		List<PlayingObject> searchedObjects = new List<PlayingObject>();

		if (swappingItems != null) {
			//check if swappingItems contain universal burst object
			for (int i=0; i<swappingItems.Length; ++i) {
				if (swappingItems[i].objectType == ObjectType.Universal) {
					BurstEvent e = new BurstEvent();
					e.affectingObjects.Add(swappingItems[i]);
					
					burstEvents.Add(e);
					if (returnForOne) return burstEvents;
					
					for (int col = 0; col < gameColumns.Length; ++col) {
						for (int row = 0; row < gameColumns[i].playingObjectsScriptList.Count; ++row) {
							PlayingObject affectedObject = (PlayingObject) gameColumns[col].playingObjectsScriptList[row];
							if (affectedObject != null && affectedObject.name == swappingItems[i].name
							    && !e.affectedObjects.Contains(affectedObject))
								e.affectedObjects.Add(affectedObject);
						}
					}
				}
			}
		}
		
		for (int j = 0; j < gameColumns.Length; j++) {
			ColumnScript column = gameColumns[j];
			for (int i = 0; i < column.playingObjectsScriptList.Count; i++) {
				PlayingObject po = (PlayingObject) column.playingObjectsScriptList[i];

				//skip searched object or index out-of-bound
				if (po != null && !searchedObjects.Contains(po)) {
					BurstEvent e = new BurstEvent();
					
					//get the group of candies with the same color by breadth first search
					
					e.affectingObjects.Add(po);

					int searchIndex = 0;

					while (e.affectingObjects.Count > searchIndex) {
						PlayingObject target = e.affectingObjects[searchIndex];
						++searchIndex;

						Vector2 tarPos = getPosOfPlayingObject(target);

						for (int adjIndex=0; adjIndex<4; ++adjIndex) {
							PlayingObject adjObject = null;
							switch (adjIndex) {
							case 0: adjObject = getPlayingObjectOfPos((int)tarPos.x+1, (int)tarPos.y); break;
							case 1: adjObject = getPlayingObjectOfPos((int)tarPos.x, (int)tarPos.y+1); break;
							case 2: adjObject = getPlayingObjectOfPos((int)tarPos.x-1, (int)tarPos.y); break;
							case 3: adjObject = getPlayingObjectOfPos((int)tarPos.x, (int)tarPos.y-1); break;
							}

							if (adjObject == null) continue;

							//TODO: better checking than string comparison
							if (!e.affectingObjects.Contains(adjObject) && adjObject.name == po.name) {

								//TODO:debug
//								Debug.Log (getPosOfPlayingObject(po)+" "+getPosOfPlayingObject(adjObject));

								//matching is found, do checking, add it to the lists

								Vector2 pos = getPosOfPlayingObject(adjObject);

//								if (!e.hasCorner) {
//
//									//check if corner exist for maybe producing object type C
//
//									for (int k=0; k<e.affectingObjects.Count; ++k) {
//										Vector2 pos2 = getPosOfPlayingObject(e.affectingObjects[k]);
//										if (Mathf.Abs(pos.x-pos2.x)==1 && Mathf.Abs(pos.y-pos2.y)==1) {
//											e.hasCorner = true;
//											break;
//										}
//									}
//								}

								//add it to the affecting and searched list
								//Note: this object is not necessary to be in successful burst event
								//		to add in searched object in order to minimize the loop

								e.affectingObjects.Add(adjObject);
								searchedObjects.Add(adjObject);

							}
						} // for (adjIndex)
					} // while (searchIndex)

					//TODO:debug use
//					if (e.affectingObjects.Count > 1) {
//						string testStr = "group:\n";
//						foreach (PlayingObject tarObject in e.affectingObjects) {
//							testStr += getPosOfPlayingObject(tarObject) + "\n";
//						}
//						Debug.Log(testStr);
//					}

					//check if that's a successful burst

					if (e.affectingObjects.Count >= successfulBurstMinCount) {

						//add to event list

						burstEvents.Add(e);
						
						//short cut for checking
						if (returnForOne) return burstEvents;

						//add the affected objects
						//TODO: specify which effect is applied on every affected objects

						foreach (PlayingObject tarObject in e.affectingObjects) {
							if (!e.affectedObjects.Contains(tarObject)) e.affectedObjects.Add(tarObject);
							
							//check the brust effect of object
							switch (tarObject.objectType) {
							case ObjectType.None: break;
							case ObjectType.Horizontal: {
								int row = tarObject.indexInColumn;
								for (int col = 0; col < gameColumns.Length; ++col) {
									PlayingObject affectedObject = (PlayingObject)gameColumns[col].playingObjectsScriptList[row];
									if (affectedObject != null && !e.affectedObjects.Contains(affectedObject))
										e.affectedObjects.Add(affectedObject);
								}
							}
								break;
							case ObjectType.Vertical: {
								for (int row=0; row<tarObject.myColumnScript.playingObjectsScriptList.Count; ++row) {
									PlayingObject affectedObject = (PlayingObject)tarObject.myColumnScript.playingObjectsScriptList[row];
									if (affectedObject != null && !e.affectedObjects.Contains(affectedObject))
										e.affectedObjects.Add(affectedObject);
								}
							}
								break;
							case ObjectType.Universal: {
								//Note: it is impossible
							}
								break;
							case ObjectType.Bomb: {
								//TODO:
							}
								break;
							}
						} // switch (objectType)

					} // if (successfulBurstMinCount)

				} // if (po)
			} // i
		} // j

		//TODO:
//		Debug.Log("=============");

		return burstEvents;
	}

}


public class BurstEvent {
//	public bool hasCorner = false;
	public List<PlayingObject> affectingObjects = new List<PlayingObject>();
	public List<PlayingObject> affectedObjects = new List<PlayingObject>();
}

