using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text;

public class Controller : MonoBehaviour {

    private Transform OutterFrame;
    private Transform InnerFrame;
    private string ServerIP = "";
    public int yawAngle = 90;
    public int pitchAngle = 30;
    private string feedbackPitch = "NAN";
    private string feedbackYaw = "NAN";
    string latestFeedbackMSG = "";

    private Text MessageField;
    private Text ErrorField;
    private Slider PowerSlider;
    private Text PowerField;
    private GameObject degPitch, degYaw;
    private int score = 0;
    private GameObject yay, doh;
    private void Start()
    {
        StartCoroutine(getRobotIp());
        OutterFrame = GameObject.Find("OutterFrame").transform;
        InnerFrame = GameObject.Find("InnerFrame").transform;
        MessageField = GameObject.Find("MSGField").GetComponent<Text>();
        ErrorField = GameObject.Find("ERRField").GetComponent<Text>();
        PowerSlider = GameObject.Find("Slider").GetComponent<Slider>();
        PowerField = GameObject.Find("PowerText").GetComponent<Text>();
        degPitch = GameObject.Find("degPitch");
        degYaw = GameObject.Find("degYaw");
        yay = GameObject.Find("Yay");
        doh = GameObject.Find("Doh");
        yay.GetComponent<Image>().CrossFadeAlpha(0, 0, false);
        doh.GetComponent<Image>().CrossFadeAlpha(0, 0, false);
        degYaw.SetActive(false);
        degPitch.SetActive(false);
    }

    IEnumerator getRobotIp()
    {
        using (UnityWebRequest www = UnityWebRequest.Post("http://192.168.137.1/robot_ip_get.php", new WWWForm())) //http://192.168.137.1/r_ip
        {
            yield return www.Send();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("POST successful!");
                // Print Body
                ServerIP = www.downloadHandler.text;
                Debug.Log(ServerIP);
                Text dText = GameObject.Find("debugText").GetComponent<Text>();
                dText.text = "Connected " + ServerIP;
                dText.color = Color.green;
            }
        }
    }

    public void Right(int deg)
    {
        if(yawAngle + deg >= 0)
        {
            OutterFrame.Rotate(new Vector3(0, -deg, 0));
            yawAngle -= deg;
        }
    }

    public void Left(int deg)
    {
        if (yawAngle + deg <= 180)
        {
            OutterFrame.Rotate(new Vector3(0, deg, 0));
            yawAngle += deg;
        }
    }

    public void Up(int deg)
    {
        if ( pitchAngle + deg <= 120)
        {
            InnerFrame.Rotate(new Vector3(deg, 0, 0));
            pitchAngle += deg;
        }
    }

    public void Down(int deg)
    {
        if ( pitchAngle - deg >= 0)
        {
            InnerFrame.Rotate(new Vector3(-deg, 0, 0));
            pitchAngle -= deg;
        }
    }

    private float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    IEnumerator MoveStamat()
    {
        string request = "http://" + ServerIP + "/MoveTo";
        request += "?pitch=";
        request += pitchAngle.ToString();
        request += "&yaw=";
        request += (180-yawAngle).ToString();
        Debug.Log("Sending: " + request);

        UnityWebRequest www = UnityWebRequest.Get(request);
        //www.timeout = 2;
        yield return www.Send();
        
        if(www.isNetworkError)
        {
            ErrorField.text = "Request failed!\nCheck connection!";
            Debug.Log("Request failed");
        }else
        {
            latestFeedbackMSG = www.downloadHandler.text;
            feedbackPitch = latestFeedbackMSG.Split(',')[0]; 
            feedbackYaw = (180-int.Parse(latestFeedbackMSG.Split(',')[1])).ToString();
            
            Debug.Log("Request Completed");
            Debug.Log(latestFeedbackMSG);
        }
    }

    IEnumerator FireStamat()
    {
        GameObject.Find("Fire").GetComponent<Button>().interactable = false;
        string request = "http://" + ServerIP + "/Fire";
        request += "?power=";
        int power =  Mathf.RoundToInt(Remap(PowerSlider.value,1,10,400,3000));
        request += power.ToString();
        
        Debug.Log("Sending: " + request);

        UnityWebRequest www = UnityWebRequest.Get(request);
        //www.timeout = 2;
        yield return www.Send();

        if (www.isNetworkError)
        {
            ErrorField.text = "Request failed!\nCheck connection!";
            Debug.Log("Request failed");
        }
        else
        {
            string response = www.downloadHandler.text;
            int currentHit = 0;
            int.TryParse(response, out currentHit);
            score += currentHit;
            GameObject.Find("scoreText").GetComponent<Text>().text = "Score: " + score;
            Debug.Log("Request Completed");
            GameObject.Find("Fire").GetComponent<Button>().interactable = true;

            if (currentHit == 1)
            {
                yay.GetComponent<Image>().CrossFadeAlpha(1, 0.3f, false);
            }
            else
            {
                doh.GetComponent<Image>().CrossFadeAlpha(1, 0.3f, false);
            }
            StartCoroutine(hideHomer());
        }
    }
    IEnumerator hideHomer()
    {
        yield return new WaitForSeconds(1);
        yay.GetComponent<Image>().CrossFadeAlpha(0, 0.3f, false);
        doh.GetComponent<Image>().CrossFadeAlpha(0, 0.3f, false);
    }
    public void Fire()
    {
        if (ServerIP == "") return;
        ErrorField.text = "";
        StartCoroutine(FireStamat());
    }

    public void Move()
    {
        if (ServerIP == "") return;
        ErrorField.text = "";
        StartCoroutine(MoveStamat());
    }

	// Update is called once per frame
	void Update ()
    {
        PowerField.text = "Power :" + Mathf.RoundToInt(Remap(PowerSlider.value, 1, 10, 400, 3000)).ToString();
        MessageField.text = "Input Pitch : " + pitchAngle + " deg";
        MessageField.text += "\nInput Yaw  : " + yawAngle + " deg";
        MessageField.text += "\nReal Pitch : " + feedbackPitch + " deg";
        MessageField.text += "\nReal Yaw  : " + feedbackYaw + " deg";
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Move();
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Left(1);
            degYaw.SetActive(true);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Right(1);
            degYaw.SetActive(true);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Up(1);
            degPitch.SetActive(true);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            Down(1);
            degPitch.SetActive(true);
        }
        if(Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow))
        {
            degPitch.SetActive(false);
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            degYaw.SetActive(false);
        }

    }
}
