using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchInputHandler : MonoBehaviour {

    

    
    public GameObject degYaw;
    public GameObject degPitch;
    private LineRenderer yawLine;
    private LineRenderer pitchLine;
    private GameObject outterFrame;
    private GameObject innerFrame;
    private void Start()
    {
        yawLine = GameObject.Find("Stamat").GetComponent<LineRenderer>();
        outterFrame = GameObject.Find("OutterFrame");
        innerFrame = GameObject.Find("InnerFrame");
        pitchLine = innerFrame.GetComponent<LineRenderer>();
        pitchLine.SetPosition(1, new Vector3(0, 0.55f, 0));
        yawLine.SetPosition(1, Vector3.zero);
        degYaw.SetActive(false);
        degPitch.SetActive(false);
    }
    // Update is called once per frame
    Vector2 prevPressPos = new Vector3();
    Vector2 latestPressPos = new Vector2();
    private float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
    void Update() {
        if (EventSystem.current.IsPointerOverGameObject() ||
            EventSystem.current.currentSelectedGameObject != null)
        {
            //MSGField.text = "overUIelement";
        }else if (Input.touches.Length > 0)
        {
            
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                //save began touch 2d point
                prevPressPos = new Vector2(t.position.x, t.position.y);
            }
            if (t.phase == TouchPhase.Moved)
            {
                
                latestPressPos = new Vector2(t.position.x, t.position.y);
                Vector2 currentSwipe = new Vector2(latestPressPos.x - prevPressPos.x,
                                                   latestPressPos.y - prevPressPos.y);
               
                prevPressPos = latestPressPos;
                int swipeDistance = Mathf.RoundToInt(currentSwipe.magnitude);
                currentSwipe.Normalize();
                
                
                //swipe upwards
                if (currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                {
                    swipeDistance = Mathf.RoundToInt(Remap(swipeDistance, 0, 500, 1, 1));
                    degPitch.SetActive(true);
                    degYaw.SetActive(false);
                    Debug.Log("up swipe");
                    transform.GetComponent<Controller>().Up(swipeDistance);
                    yawLine.SetPosition(1, Vector3.zero);
                    pitchLine.SetPosition(1, new Vector3(-innerFrame.transform.forward.x, -innerFrame.transform.forward.y, -innerFrame.transform.forward.z) * 10);
                }
                //swipe down
                if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                {
                    swipeDistance = Mathf.RoundToInt(Remap(swipeDistance, 0, 500, 1, 1));
                    degPitch.SetActive(true);
                    degYaw.SetActive(false);
                    Debug.Log("down swipe");
                    transform.GetComponent<Controller>().Down(swipeDistance);
                    yawLine.SetPosition(1, Vector3.zero);
                    pitchLine.SetPosition(1, new Vector3(-innerFrame.transform.forward.x, -innerFrame.transform.forward.y, -innerFrame.transform.forward.z) * 10);
                }
                //swipe left
                if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                {
                    swipeDistance = Mathf.RoundToInt(Remap(swipeDistance, 0, 500, 1, 10));
                    degYaw.SetActive(true);
                    degPitch.SetActive(false);
                    Debug.Log("left swipe");
                    yawLine.SetPosition(1, new Vector3(-outterFrame.transform.forward.x, 0,-outterFrame.transform.forward.z)*10);
                    pitchLine.SetPosition(1, new Vector3(0,0.55f,0));
                    transform.GetComponent<Controller>().Left(swipeDistance);

                }
                //swipe right
                if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                {
                    swipeDistance = Mathf.RoundToInt(Remap(swipeDistance, 0, 500, 1, 10));
                    degYaw.SetActive(true);
                    degPitch.SetActive(false);
                    Debug.Log("right swipe");
                    yawLine.SetPosition(1, new Vector3(-outterFrame.transform.forward.x, 0, -outterFrame.transform.forward.z) * 10);
                    pitchLine.SetPosition(1, new Vector3(0, 0.55f, 0));
                    transform.GetComponent<Controller>().Right(swipeDistance);
                }
                
            }
            if (t.phase == TouchPhase.Ended)
            {
                degYaw.SetActive(false);
                degPitch.SetActive(false);
                yawLine.SetPosition(1, Vector3.zero);
                pitchLine.SetPosition(1, new Vector3(0, 0.55f, 0));
                prevPressPos = new Vector2();
                latestPressPos = new Vector2();
            }
        }

    }
}
