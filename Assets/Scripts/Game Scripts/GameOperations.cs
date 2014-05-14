using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameOperations : MonoBehaviour 
{
	public static bool debug = false;

    public static GameOperations instance;
    internal bool doesHaveBrustItem = false; 

    internal PlayingObject item1;
    internal PlayingObject item2;

    internal PlayingObject []suggestionItems;
    public float delay = .2f;
    float baseDelay;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        baseDelay = delay;
        suggestionItems = new PlayingObject[2];
        GameManager.instance.isBusy = true;
        Invoke("AssignNeighbours", .5f);
    }

    internal void FreeMachine()
    {
        GameManager.instance.objectFallingDuration = GameManager.instance.initialObjectFallingDuration;
        delay = baseDelay;
        GameManager.instance.isBusy = false;

        if (item1)
            item1.UnSelectMe();
        if (item2)
            item2.UnSelectMe();

        item1 = null;
        item2 = null;
    }
   

    internal void CheckBoardState()
    {
       // print("1");
        GameManager.instance.isBusy = true;
        suggestionItems = new PlayingObject[2];
        CancelInvoke("ShowHint");
//        doesHaveBrustItem = false;

//        for (int j = 0; j < ColumnManager.instance.gameColumns.Length; j++)
//        {
//            for (int i = 0; i < ColumnManager.instance.gameColumns[j].playingObjectsScriptList.Count; i++)
//            {
//                if(ColumnManager.instance.gameColumns[j].playingObjectsScriptList[i] != null)
//                    ((PlayingObject)ColumnManager.instance.gameColumns[j].playingObjectsScriptList[i]).CheckIfCanBrust();
//            }
//        }
       // print(doesHaveBrustItem);

		Debug.Log ("CheckBoardState "+ ((SwapTwoObject.swappingItems[0]==null) ? "null" : (SwapTwoObject.swappingItems[0].name)));
		
		ColumnManager columnManager = ColumnManager.instance;
		List<BurstEvent> burstEvents = columnManager.checkBurst();
		foreach (BurstEvent burstEvent in burstEvents) {
			foreach (PlayingObject po in burstEvent.affectedObjects) {
				if (po == SwapTwoObject.swappingItems[0] || po == SwapTwoObject.swappingItems[1]) {
					//classify the burst pattern
					ObjectType type = ObjectType.None;

					Vector2 position = columnManager.getPosOfPlayingObject(po);
					foreach (PlayingObject po2 in burstEvent.affectingObjects) {
						Vector2 position2 = columnManager.getPosOfPlayingObject(po2);
						if (position.x != position2.x && position.y != position2.y) {
							type = ObjectType.Bomb;		//false checking for both x and y => not a straight line
							break;
						}
						if (position.x != position2.x) {
							type = ObjectType.Horizontal;
							position.x = -1;	//force false checking for x and focus on y
						} else {
							type = ObjectType.Vertical;
							position.y = -1;	//force false checking for y and focus on x
						}
					}
					if (type != ObjectType.Bomb && burstEvent.affectingObjects.Count >= 5) {
						type = ObjectType.Universal;
					}

					Debug.Log ("Type:"+type);

					switch (type) {
					case ObjectType.None:	// actually this is impossible
						Debug.LogError("No special item created for " + columnManager.getPosOfPlayingObject(po));
						break;
					case ObjectType.Horizontal:
						po.specialObjectToForm = po.horizontalPowerPrefab;
						break;
					case ObjectType.Vertical:
						po.specialObjectToForm = po.verticalPowerPrefab;
						break;
					case ObjectType.Universal:
						po.specialObjectToForm = GameManager.instance.universalPlayingObjectPrefab;
						break;
					case ObjectType.Bomb:
						po.specialObjectToForm = GameManager.instance.bombPlayingObjectPrefab;
						break;
					}
					
				}

				//TODO: check affectingObject for special burst animation
				po.AssignBurst("normal");
			}
		}

		//clear swaping object for further checking
		SwapTwoObject.swappingItems[0] = null;
		SwapTwoObject.swappingItems[1] = null;

		if (burstEvents.Count > 0)
		{
            SoundFxManager.instance.whopSound.Play();
            RemoveBrustItems();
            Invoke("AddMissingItems", delay);
            delay = baseDelay;
        }
        else
        {
            GameManager.numberOfItemsPoppedInaRow = 0;
            FreeMachine();
            CheckForPossibleMove();
            Invoke("ShowHint", 5f);
        }

    }

    internal void RemoveBrustItems()
    {
        for (int i = 0; i < ColumnManager.instance.gameColumns.Length; i++)
        {
            ColumnManager.instance.gameColumns[i].DeleteBrustedItems();
        }

    }

    internal void AddMissingItems()
    {
       // print("Add");
        float delay = 0;
        for (int i = 0; i < ColumnManager.instance.gameColumns.Length; i++)
        {
            if (ColumnManager.instance.gameColumns[i].GetNumberOfItemsToAdd() > 0)
            {
                ColumnManager.instance.gameColumns[i].Invoke("AddMissingItems", delay);
                delay += .05f;
            }
        }

        Invoke("AssignNeighbours", delay + .1f);
    }

    void ShowHint()
    {       
        if (GameManager.instance.isBusy)
            return;

		if (GameOperations.instance.suggestionItems[0]) GameOperations.instance.suggestionItems[0].Animate();
		if (GameOperations.instance.suggestionItems[1])GameOperations.instance.suggestionItems[1].Animate();
    }
    

    internal void StopShowingHint()
    {
        if (GameOperations.instance.suggestionItems[0])
            GameOperations.instance.suggestionItems[0].StopAnimating();
        if (GameOperations.instance.suggestionItems[1])
            GameOperations.instance.suggestionItems[1].StopAnimating();
    }

    void CheckForPossibleMove()
    {
        if (!IsMovePossible())
        {
            print("No Moves Possible");

			//TODO:debug
//			if (debug) return;

			Application.LoadLevel(Application.loadedLevel);            
        }        
    }


    bool IsMovePossible()
    {
        
//        for (int j = 0; j < ColumnManager.instance.gameColumns.Length; j++)
//        {
//            for (int i = 0; i < ColumnManager.instance.gameColumns[j].playingObjectsScriptList.Count; i++)
//            {
//                if (ColumnManager.instance.gameColumns[j].playingObjectsScriptList[i] != null)
//                {
//                    if (((PlayingObject)ColumnManager.instance.gameColumns[j].playingObjectsScriptList[i]).isMovePossible())
//                    {
//                        return true;
//                    }
//                }
//            }
//        }
//
//        return false;

		ColumnManager columnManager = ColumnManager.instance;

		//check if any move is valid
		//Note: no need to check last row and column (0 to column-1/row-1)
		for (int j = 0; j < columnManager.gameColumns.Length - 1; j++) {
			for (int i = 0; i < columnManager.gameColumns[j].playingObjectsScriptList.Count - 1; i++) {
				PlayingObject o1 = (PlayingObject) columnManager.gameColumns[j].playingObjectsScriptList[i];
				PlayingObject o2;
				//try to swap with right and bottom and see if burst is available
				//Note: no need to check left and top
				for (int d=0; d<2; ++d) {
					if (d==0) o2 = (PlayingObject) columnManager.gameColumns[j+1].playingObjectsScriptList[i];
					else o2 = (PlayingObject) columnManager.gameColumns[j].playingObjectsScriptList[i+1];
					if (o2==null) continue;

					columnManager.Swipe(o1, o2);
					List<BurstEvent> burstEvents = columnManager.checkBurst(true);
					columnManager.Swipe(o1, o2);

					if (burstEvents.Count > 0) {
						//TODO: show other suggestion randomly
						suggestionItems[0] = o1;
						suggestionItems[1] = o2;

						Debug.Log("Suggest: "+columnManager.getPosOfPlayingObject(o1)+" "+columnManager.getPosOfPlayingObject(o2));

						return true;
					}
				}
			}
		}

		return false;
    }

    internal void AssignNeighbours()
    {
        for (int i = 0; i < ColumnManager.instance.gameColumns.Length; i++)
        {
            ColumnManager.instance.gameColumns[i].AssignNeighbours();
        }

        Invoke("CheckBoardState", GameManager.instance.objectFallingDuration);
        GameManager.instance.objectFallingDuration = GameManager.instance.initialObjectFallingDuration;
    }

   


    
}
