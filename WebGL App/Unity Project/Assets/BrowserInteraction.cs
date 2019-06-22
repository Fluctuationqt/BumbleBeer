using UnityEngine;
using System.Runtime.InteropServices;

public class BrowserInteraction : MonoBehaviour {

    [DllImport("__Internal")]
    private static extern void gameLoadedCallback();

    // Use this for initialization
    void Start () {
        gameLoadedCallback();
	}
	
}
