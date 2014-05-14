using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwapTwoObject : MonoBehaviour 
{
    internal static SwapTwoObject instance;
    Vector3 pos1;
    Vector3 pos2;

    GameObject object1;
    GameObject object2;
	
	void Start () 
    {
        instance = this;
	
	}

    int GetDirectionOfSecondObject(GameObject obj1, GameObject obj2)
    {
        int index = -1;

        if (obj1.transform.position.x == obj2.transform.position.x)
        {
            if (obj1.transform.position.y < obj2.transform.position.y)
                index = 2;
            else
                index = 3;
        }
        else
        {
            if (obj1.transform.position.x > obj2.transform.position.x)
                index = 1;
            else
                index = 0;
        }
       
        return index;
    }

	public static PlayingObject[] swappingItems = new PlayingObject[2];

    public void SwapTwoItems(PlayingObject item1, PlayingObject item2)
    {
        iTween.Defaults.easeType = iTween.EaseType.easeOutBack;
        GameManager.instance.isBusy = true;

        object1 = item1.gameObject;
        object2 = item2.gameObject;

        pos1 = item1.transform.position;
        pos2 = item2.transform.position;

        iTween.MoveTo(object1, pos2, GameManager.instance.swappingTime);
        iTween.MoveTo(object2, pos1, GameManager.instance.swappingTime);


		//burst checking
		ColumnManager columnManager = ColumnManager.instance;
		columnManager.Swipe(item1, item2);
		swappingItems[0] = item1;
		swappingItems[1] = item2;

		//TODO:
		List<BurstEvent> burstEvents = columnManager.checkBurst(true, swappingItems);
		if (burstEvents.Count > 0) {
			GameOperations.instance.StopShowingHint();
			GameOperations.instance.Invoke("AssignNeighbours", .1f);

//			foreach (BurstEvent burstEvent in burstEvents) {
//				foreach (PlayingObject po in burstEvent.affectedObjects) {
//					po
//				}
//			}
		} else {
			swappingItems[0] = null;
			swappingItems[1] = null;
			//revert the swap
			columnManager.Swipe(item1, item2);
			Invoke("ChangePositionBack", GameManager.instance.swappingTime);
		}
		
		/*
        ObjectType type1 = item1.objectType;
        ObjectType type2 = item2.objectType;


        if (type1 == ObjectType.None && type2 == ObjectType.None)
        {
            if (item1.isMovePossibleInDirection(GetDirectionOfSecondObject(object1, object2)) == false && (item2.isMovePossibleInDirection(GetDirectionOfSecondObject(object2, object1)) == false))
            {
                Invoke("ChangePositionBack", GameManager.instance.swappingTime);
                return;
            }
            else
            {
                GameOperations.instance.StopShowingHint();
				columnManager.Swipe(item1, item2);
                GameOperations.instance.Invoke("AssignNeighbours", .1f);
            }
        }
        else if ((type2 == ObjectType.None && (type1 == ObjectType.Horizontal || type1 == ObjectType.Vertical))
            || (type1 == ObjectType.None && (type2 == ObjectType.Horizontal || type2 == ObjectType.Vertical)))
        {
            if (item1.isMovePossibleInDirection(GetDirectionOfSecondObject(object1, object2)) == false && (item2.isMovePossibleInDirection(GetDirectionOfSecondObject(object2, object1)) == false))
            {
                Invoke("ChangePositionBack", GameManager.instance.swappingTime);
                return;
            }
            else
            {
                GameOperations.instance.StopShowingHint();
                columnManager.Swipe(item1, item2);
                GameOperations.instance.Invoke("AssignNeighbours", .1f);
            }
        }
        else
        {
            GetComponent<SwapSpecialObjects>().Swap(item1, item2);
        }
        */
    }

    void ChangePositionBack()
    {
        SoundFxManager.instance.wrongMoveSound.Play();
        iTween.MoveTo(object1, pos1, GameManager.instance.swappingTime * .6f);
        iTween.MoveTo(object2, pos2, GameManager.instance.swappingTime * .6f);

        GameOperations.instance.FreeMachine();
    }

    
}
