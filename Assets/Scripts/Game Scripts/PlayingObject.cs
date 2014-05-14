// hey hey yuki

using UnityEngine;
using System.Collections;

public enum ObjectType
{
    None,
    Horizontal,
    Vertical,
    Universal,
	Bomb
};

public class PlayingObject : MonoBehaviour 
{
    

    public ObjectType objectType = ObjectType.None;

    public GameObject horizontalPowerPrefab;
    public GameObject verticalPowerPrefab;
	public GameObject bombPowerPrefab;
    internal ColumnScript myColumnScript;
    internal int indexInColumn;
    public bool isTraced = false;
    public bool brust = false;
    public PlayingObject[] adjacentItems; //left,right,up,down

    public bool isSelected = false;
    public int itemId;
    public Vector3 initialScale;
    internal SpecialPlayingObject specialObjectScript;
    internal GameObject specialObjectToForm = null;

    string left1 = "left1";
    string left2 = "left2";
    string left3 = "left3";

    string right1 = "right1";
    string right2 = "right2";
    string right3 = "right3";

    string up1 = "up1";
    string up2 = "up2";
    string up3 = "up3";

    string down1 = "down1";
    string down2 = "down2";
    string down3 = "down3";

	string upleft = "upleft";
	string downright = "downright";
    

    void Awake()
    {
        transform.localScale = new Vector3(.7f, .7f, .7f);
        initialScale = transform.localScale;
    }
	
	void Start () 
    {
        
        specialObjectScript = GetComponent<SpecialPlayingObject>();
        itemId = GetInstanceID();
        int ind = Random.Range(0, 6);

        adjacentItems = new PlayingObject[4];
	
	}

    void CheckForSpecialCandyFormation(string objName)
    {
        if(objName == left2 && objName == left1 && objName == right1 && objName == right2)
		{
			parentCallingScript.specialObjectToForm = GameManager.instance.universalPlayingObjectPrefab;
			Debug.Log("Horizontal 5 ");
		}
        else if(objName == up2 && objName == up1 && objName == down1 && objName == down2)
		{
			parentCallingScript.specialObjectToForm = GameManager.instance.universalPlayingObjectPrefab;
			Debug.Log("Vertical 5 ");
		}
        else if ((objName == left2 && objName == left1 && objName == right1) || (objName == left1 && objName == right1 && objName == right2))
        {
            if (Random.value < .5f)
                parentCallingScript.specialObjectToForm = parentCallingScript.horizontalPowerPrefab;
            else
                parentCallingScript.specialObjectToForm = parentCallingScript.verticalPowerPrefab;

			Debug.Log("Horizontal 4 ");
        }

        else if ((objName == up2 && objName == up1 && objName == down1) || (objName == up1 && objName == down1 && objName == down2))
        {
            if (Random.value < .5f)
                parentCallingScript.specialObjectToForm = parentCallingScript.horizontalPowerPrefab;
            else
                parentCallingScript.specialObjectToForm = parentCallingScript.verticalPowerPrefab;

			Debug.Log("Vertical 4 ");
        }
		else if ((objName == up1 && objName == down1)||(objName == down1 && objName == down2)|| (objName == up2 && objName == up1)){

			if (Random.value < .5f)
				parentCallingScript.specialObjectToForm = parentCallingScript.horizontalPowerPrefab;
			else
				parentCallingScript.specialObjectToForm = parentCallingScript.verticalPowerPrefab;

			Debug.Log("vertical 3 ");
		}
		else if ((objName == left1 && objName == right1)|| ( objName == right1 && objName == right2)|| (objName == left2 && objName == left1))
		{
			if (Random.value < .5f)
				parentCallingScript.specialObjectToForm = parentCallingScript.horizontalPowerPrefab;
			else
				parentCallingScript.specialObjectToForm = parentCallingScript.verticalPowerPrefab;

			Debug.Log("Horizontal 3 ");
		}
		else if (( objName == up1 && objName == right1) || ( objName == downright && objName == down1)  || (objName == upleft && objName == left1))
		{	
			parentCallingScript.specialObjectToForm = parentCallingScript.bombPowerPrefab;
			Debug.Log("L 3 ");
		}
    }

    void AssignLRUD()
    {
        left1 = "left1";
        left2 = "left2";
        left3 = "left3";
        right1 = "right1";
        right2 = "right2";
        right3 = "right3";
        up1 = "up1";
        up2 = "up2";
        up3 = "up3";
        down1 = "down1";
        down2 = "down2";
        down3 = "down3";

		upleft = "upleft";
		downright = "downright";

        if (adjacentItems[0])
        {
            left1 = adjacentItems[0].name;
            if (adjacentItems[0].adjacentItems[0])
            {
                left2 = adjacentItems[0].adjacentItems[0].name;
                if (adjacentItems[0].adjacentItems[0].adjacentItems[0])
                    left3 = adjacentItems[0].adjacentItems[0].adjacentItems[0].name;
            }
        }

        if (adjacentItems[1])
        {
            right1 = adjacentItems[1].name;
            if (adjacentItems[1].adjacentItems[1])
            {
                right2 = adjacentItems[1].adjacentItems[1].name;
                if (adjacentItems[1].adjacentItems[1].adjacentItems[1])
                    right3 = adjacentItems[1].adjacentItems[1].adjacentItems[1].name;
            }
        }

        if (adjacentItems[2])
        {
            up1 = adjacentItems[2].name;

			if (adjacentItems[2].adjacentItems[0])
				upleft = adjacentItems[2].adjacentItems[0].name;

			if (adjacentItems[2].adjacentItems[2])
			{
                up2 = adjacentItems[2].adjacentItems[2].name;
                if (adjacentItems[2].adjacentItems[2].adjacentItems[2])
                    up3 = adjacentItems[2].adjacentItems[2].adjacentItems[2].name;
            }
        }

        if (adjacentItems[3])
        {
            down1 = adjacentItems[3].name;

			if (adjacentItems[3].adjacentItems[1])
				downright = adjacentItems[3].adjacentItems[1].name;

			if (adjacentItems[3].adjacentItems[3])
            {
                down2 = adjacentItems[3].adjacentItems[3].name;
                if (adjacentItems[3].adjacentItems[3].adjacentItems[3])
                    down3 = adjacentItems[3].adjacentItems[3].adjacentItems[3].name;
            }
        }
    }

    internal bool JustCheckIfCanBrust(string objName, int parentIndex)
    {
        AssignLRUD();

        if (parentIndex == 0)
            right1 = "right1";
        if (parentIndex == 1)
            left1 = "left1";
        if (parentIndex == 2)
            down1 = "down1";
        if (parentIndex == 3)
            up1 = "up1";

        bool canBurst = false;

        if ((objName == left1 && objName == left2) || (objName == left1 && objName == right1) || (objName == right1 && objName == right2)
            || (objName == up1 && objName == up2) || (objName == up1 && objName == down1) || (objName == down1 && objName == down2))
        {
            canBurst = true;
            GameOperations.instance.doesHaveBrustItem = true;
        }

        if (canBurst)
        {
            if (parentCallingScript)
                CheckForSpecialCandyFormation(objName);
        }

        return canBurst;
    }

    internal bool CheckIfCanBrust()
    {
        if (isDestroyed)
            return false;

        if (brust)
        {
            GameOperations.instance.doesHaveBrustItem = true;
            return true;
        }

        AssignLRUD();

        if ((name == left1 && name == left2) || (name == left1 && name == right1) || (name == right1 && name == right2)
            || (name == up1 && name == up2) || (name == up1 && name == down1) || (name == down1 && name == down2))
        {
            AssignBurst("normal");
            GameOperations.instance.doesHaveBrustItem = true;
        }

        return brust;
    }

    string brustBy = "normal";

    internal void AssignBurst(string who)
    {
//		Debug.Log ("AssignBurst");
        if (brust)
            return;

        brustBy = who;
        brust = true;

        if (specialObjectScript)
        {
           // GameOperations.instance.delay = .5f;
            GetComponent<SpecialPlayingObject>().AssignBrustToItsTarget();
        }
    }

    internal bool isMovePossible()
    {
        for (int i = 0; i < 4; i++)
        {
            if (adjacentItems[i])
            {
                if (adjacentItems[i].JustCheckIfCanBrust(name, i))
                {
                    GameOperations.instance.suggestionItems[0] = this;
                    GameOperations.instance.suggestionItems[1] = adjacentItems[i];
                    return true;
                }
            }
        }

        return false;
    }

    public static PlayingObject parentCallingScript;

    internal bool isMovePossibleInDirection(int dir)
    {
        parentCallingScript = this;

        if (adjacentItems[dir])
        {
            if (adjacentItems[dir].JustCheckIfCanBrust(name, dir))
            {
                return true;
            }
        }

        return false;
    }

    bool isDestroyed = false;

    internal void DestroyMe()
    {
        if (isDestroyed)
            return;

        if (myColumnScript.jellyAvailability != null)
        {
            if (myColumnScript.jellyAvailability[indexInColumn] == 2)
            {
                myColumnScript.jellyAvailability[indexInColumn] = 1;
                Destroy(((GameObject)myColumnScript.jellyObjects[indexInColumn]).transform.GetChild(0).gameObject);
                Instantiate(GamePrefabs.instance.jellyParticles, transform.position, Quaternion.identity);
                GameManager.instance.totalNoOfJellies--;
            }
            else if (myColumnScript.jellyAvailability[indexInColumn] == 1)
            {
                myColumnScript.jellyAvailability[indexInColumn] = 0;
                Destroy(((GameObject)myColumnScript.jellyObjects[indexInColumn]));
                Instantiate(GamePrefabs.instance.jellyParticles, transform.position, Quaternion.identity);
                GameManager.instance.totalNoOfJellies--;
            }

            GameManager.instance.jellyText.text = "Jelly : " + GameManager.instance.totalNoOfJellies.ToString();

        }

        isDestroyed = true;
        GameManager.numberOfItemsPoppedInaRow++;
        GameManager.instance.AddScore();

        if (specialObjectScript)
        {
            iTween.ScaleTo(gameObject, Vector3.zero, .8f);
        }
        else
            iTween.ScaleTo(gameObject, Vector3.zero, .8f);

        if (brustBy == "smoke")
            Instantiate(GamePrefabs.instance.smokeParticles, transform.position, Quaternion.identity);
        else
            Instantiate(GamePrefabs.instance.starParticles, transform.position, Quaternion.identity);

        Invoke("Dest", 1f);

    }

    void Dest()
    {
        iTween.Stop(gameObject);
        Destroy(gameObject);
    }

    int counter = 0;

    internal void Animate()
    {
        if (counter % 2 == 0)
        {
            iTween.ScaleTo(gameObject, initialScale * 1.1f, .8f);
        }
        else
        {
            iTween.ScaleTo(gameObject, initialScale / 1.1f, .8f);
        }
        counter++;
        CancelInvoke("Animate");
        Invoke("Animate", .8f);

    }

    internal void StopAnimating()
    {
        CancelInvoke("Animate");
        if(isSelected == false)
            iTween.Stop(gameObject);

        transform.localScale = initialScale;
        
    }

    internal void SelectMe()
    {
        isSelected = true;
        transform.FindChild("Image").renderer.material.SetColor("_TintColor", new Color(1, 1, 1, .5f));
        
    }

    internal void UnSelectMe()
    {
        isSelected = false;
        transform.FindChild("Image").renderer.material.SetColor("_TintColor", new Color(.5f, .5f, .5f, .5f));
    }

    internal GameObject WillFormSpecialObject()
    {
        return specialObjectToForm;
    }

    internal void Burn()
    {
       transform.FindChild("Image").renderer.material.SetColor("_TintColor", new Color(.2f, .2f, .2f, .5f));
    }
}
