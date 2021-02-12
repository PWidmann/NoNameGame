using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInterface : MonoBehaviour
{
    public static GameInterface Instance;

    [SerializeField] GameObject pickupPanel;
    [SerializeField] GameObject pickupPanelText;

    public bool pickupPanelActive = true;
    public string pickupText = "Default";

    void Start()
    {
        if (Instance == null)
            Instance = this;
    }

    
    void Update()
    {
        if (pickupPanelActive)
        {
            pickupPanelText.GetComponent<Text>().text = pickupText;
            pickupPanel.SetActive(true);
        }
        else
        {
            pickupPanel.SetActive(false);
        }
    }
}
