using UnityEngine;
using System.Collections;

public class SpecialPlayingObject : MonoBehaviour 
{
    
    PlayingObject myPlayingObject;
	
	void Start () 
    {
        myPlayingObject = GetComponent<PlayingObject>();

        if (myPlayingObject.objectType == ObjectType.Horizontal || myPlayingObject.objectType == ObjectType.Vertical)
        {
            string a = name.Substring(name.Length - 8, 1);
            name = "Playing Object " + a + "(Clone)";
        }
        
        Invoke("Start1", .05f);
	
	}
    void Start1()
    {
        iTween.PunchScale(gameObject, new Vector3(.5f, .5f, .5f), .8f);
    }

    internal void AssignBurst()
    {
        GameOperations.instance.delay = .5f;
        myPlayingObject.brust = true;
        Invoke("AssignBrustToItsTarget", GameManager.instance.initialObjectFallingDuration);
    }

    internal void AssignBrustToItsTarget()
    {
        myPlayingObject.brust = true;

        if (myPlayingObject.objectType == ObjectType.Horizontal)
            PowerIsHorizontal();
        if (myPlayingObject.objectType == ObjectType.Vertical)
            PowerIsVertical();

        
    }

    void PowerIsVertical()
    {
        int itemIndex = myPlayingObject.indexInColumn;

        for (int i = 0; i < myPlayingObject.myColumnScript.playingObjectsScriptList.Count; i++)
        {
            if (myPlayingObject.myColumnScript.playingObjectsScriptList[i] != null)
            {
                PlayingObject po = (PlayingObject)myPlayingObject.myColumnScript.playingObjectsScriptList[i];
                po.AssignBurst("normal");
                po.DestroyMe();
            }
        }

        CreateEffect();

    }

    void PowerIsHorizontal()
    {
        int itemIndex = myPlayingObject.indexInColumn;

        for (int i = 0; i < ColumnManager.instance.gameColumns.Length; i++)
        {
            if (ColumnManager.instance.gameColumns[i].playingObjectsScriptList[itemIndex] != null)
            {
                PlayingObject po = (PlayingObject)ColumnManager.instance.gameColumns[i].playingObjectsScriptList[itemIndex];
                po.AssignBurst("normal");
                po.DestroyMe();
            }
        }

        CreateEffect();

    }

    internal void CreateEffect()
    {
        SoundFxManager.instance.whopSound.Play();

        if (myPlayingObject.objectType == ObjectType.Horizontal)
        {
            GameObject temp1 = (GameObject)Instantiate(GamePrefabs.instance.horWavePrefab, transform.position, Quaternion.identity);
            temp1.transform.localEulerAngles = new Vector3(90, 0, 0);
            Destroy(temp1, 1f);
            iTween.MoveBy(temp1, new Vector3(7, 0, 0), 1.8f);

            GameObject temp2 = (GameObject)Instantiate(GamePrefabs.instance.horWavePrefab, transform.position, Quaternion.identity);
            temp2.transform.localEulerAngles = new Vector3(90, 0, 0);
            Destroy(temp2, 1f);
            iTween.MoveBy(temp2, new Vector3(-7, 0, 0), 1.8f);
        }
        if (myPlayingObject.objectType == ObjectType.Vertical)
        {
            GameObject temp1 = (GameObject)Instantiate(GamePrefabs.instance.verWavePrefab, transform.position, Quaternion.identity);
            temp1.transform.localEulerAngles = new Vector3(90, 0, 0);
            Destroy(temp1, 1f);
            iTween.MoveBy(temp1, new Vector3(0, 9, 0), 2f);

            GameObject temp2 = (GameObject)Instantiate(GamePrefabs.instance.verWavePrefab, transform.position, Quaternion.identity);
            temp2.transform.localEulerAngles = new Vector3(90, 0, 0);
            Destroy(temp2, 1f);
            iTween.MoveBy(temp2, new Vector3(0, -9, 0), 2f);
        }
    }

    // Universal and Normal Mixture

    PlayingObject other;
    Vector3 point1;
    Vector3 point2;
    ArrayList electricEffectList;
    float delay = .05f;

    internal void AssignBurstUniversalObject(PlayingObject _other)
    {
        electricEffectList = new ArrayList();
        other = _other;

        point1 = other.transform.position;
        

        myPlayingObject.brust = true;
        StartCoroutine(ShowEffect());        
    }
    
    IEnumerator ShowEffect()
    {
        SoundFxManager.instance.electricZapSound.Play();
        int count = 0;
        for (int i = 0; i < ColumnManager.instance.gameColumns.Length; i++)
        {
            for (int j = 0; j < ColumnManager.instance.gameColumns[i].playingObjectsScriptList.Count; j++)
            {
                if ((PlayingObject)ColumnManager.instance.gameColumns[i].playingObjectsScriptList[j])
                {
                    if (((PlayingObject)ColumnManager.instance.gameColumns[i].playingObjectsScriptList[j]).name == other.name)
                    {
                        InstanciateElectricEffect(((PlayingObject)ColumnManager.instance.gameColumns[i].playingObjectsScriptList[j]).transform,count++);
                        ((PlayingObject)ColumnManager.instance.gameColumns[i].playingObjectsScriptList[j]).Burn();
                        yield return new WaitForSeconds(delay);
                    }
                }
            }
        }

        Invoke("AssignBrustToAll", 1f);
    }

    void InstanciateElectricEffect(Transform target,int count)
    {
        if(SoundFxManager.instance.electricZapSound.isPlaying == false)
            SoundFxManager.instance.electricZapSound.Play();

        point2 = target.position;

        if (point1 == point2)
            point1 = transform.position;

        Vector3 pos = (point1 + point2) * .5f + new Vector3(0,0,1);

        GameObject temp = (GameObject)Instantiate(GamePrefabs.instance.electricFieldPrefab, pos, Quaternion.identity);
        temp.transform.localScale = new Vector3(Vector2.Distance(point1, point2), temp.transform.localScale.y, temp.transform.localScale.z);
        temp.transform.rotation = Quaternion.LookRotation(point1 - point2);

        float angleX = temp.transform.localEulerAngles.x;
        if (point2.x < point1.x)
            angleX = 180 - angleX;

        temp.transform.localEulerAngles = new Vector3(0, 0, angleX);

        electricEffectList.Add(temp);
    }

    void AssignBrustToAll()
    {
        for (int i = 0; i < electricEffectList.Count; i++)
        {
            Destroy((GameObject)electricEffectList[i]);
        }

        for (int i = 0; i < ColumnManager.instance.gameColumns.Length; i++)
        {
            ColumnManager.instance.gameColumns[i].AssignBrustToAllItemsOfName(other.name);
        }

        GameOperations.instance.Invoke("CheckBoardState", .1f);
    }
	
}
