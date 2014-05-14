using UnityEngine;
using System.Collections;

public class SwapSpecialObjects : MonoBehaviour
{
    SwapTwoObject swapTwoObjectScript;

    PlayingObject item1;
    PlayingObject item2;

    ObjectType type1;
    ObjectType type2;

    void Start()
    {
        swapTwoObjectScript = GetComponent<SwapTwoObject>();
    }


    internal void Swap(PlayingObject _item1, PlayingObject _item2)
    {
        item1 = _item1;
        item2 = _item2;

        type1 = item1.objectType;
        type2 = item2.objectType;

        if ((type1 == ObjectType.Horizontal || type1 == ObjectType.Vertical) && (type2 == ObjectType.Horizontal || type2 == ObjectType.Vertical))
        {
            ObjectsAreOfTypeHorizontal();
        }
        
        if((type1 == ObjectType.Universal && type2 == ObjectType.None) || (type1 == ObjectType.None && type2 == ObjectType.Universal))
        {
            ObjetsAreOfTypeUniversalAndNone();
        }

        if ((type1 == ObjectType.Universal && (type2 == ObjectType.Horizontal || type2 == ObjectType.Vertical)) 
            || (type2 == ObjectType.Universal && (type1 == ObjectType.Horizontal || type1 == ObjectType.Vertical))) 
        {
            ObjetsAreOfTypeUniversalAndHorizontal();
        }
        
    }

    void ObjetsAreOfTypeUniversalAndHorizontal()
    {
        GameOperations.instance.StopShowingHint();
        ColumnManager.instance.Swipe(item1, item2);

        for (int i = 0; i < ColumnManager.instance.gameColumns.Length; i++)
        {
            ColumnManager.instance.gameColumns[i].AssignNeighbours();
        }
        
        if(type1 == ObjectType.Universal)
            SwapUniversalAndHorizontal.instance.StartSwapProcess(item1, item2);
        else
            SwapUniversalAndHorizontal.instance.StartSwapProcess(item2, item1);
    }

    void ObjetsAreOfTypeUniversalAndNone()
    {
        GameOperations.instance.StopShowingHint();
        PlayingObject specialItemScript;
        PlayingObject otherItemScript;

        if (type1 == ObjectType.Universal)
        {
            specialItemScript = item1;
            otherItemScript = item2;
        }
        else
        {
            specialItemScript = item2;
            otherItemScript = item1;
        }

        specialItemScript.specialObjectScript.AssignBurstUniversalObject(otherItemScript);

    }

    void AssignNeighbour()
    {
        for (int i = 0; i < ColumnManager.instance.gameColumns.Length; i++)
        {
            ColumnManager.instance.gameColumns[i].AssignNeighbours();
        }
    }

    void ObjectsAreOfTypeHorizontal()
    {
        GameOperations.instance.StopShowingHint();
		ColumnManager.instance.Swipe(item1, item2);

        if (type1 == type2)
        {
            if (type1 == ObjectType.Horizontal)
                item1.objectType = ObjectType.Vertical;
            else
                item1.objectType = ObjectType.Horizontal;
        }

        item1.specialObjectScript.AssignBurst();
        item2.specialObjectScript.AssignBurst();

        GameOperations.instance.Invoke("RemoveBrustItems", 1f);
        GameOperations.instance.Invoke("AddMissingItems", 1f + .1f);
    }

}
