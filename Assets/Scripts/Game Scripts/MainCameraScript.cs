using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainCameraScript : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        RaycastHit hit;

        foreach (Touch evt in Input.touches)
        {
            Ray ray = camera.ScreenPointToRay(Input.GetTouch(0).position);

            if (evt.phase == TouchPhase.Began)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    hit.transform.gameObject.SendMessage("OnMouseDown");
                }
            }

            if (evt.phase == TouchPhase.Moved)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    hit.transform.gameObject.SendMessage("OnMouseDrag");
                }
            }

            if (evt.phase == TouchPhase.Ended || evt.phase == TouchPhase.Canceled)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    hit.transform.gameObject.SendMessage("OnMouseUp");
                }
            }
        }
    }
}

