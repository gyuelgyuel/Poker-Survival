using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Option : MonoBehaviour
{

    public GameObject optionUI;
    public GameObject gameStartUI;

    private int CurrPanel;
    private string[] PanelNames = { "환경설정", "조작방법", "도감" };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrPanel = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStartUI.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Escape))
            OpenAndClose();
        
    }

    public void OpenAndClose() {
        optionUI.SetActive(!optionUI.activeSelf);
        Time.timeScale = optionUI.activeSelf ? 0f : 1f;
    }

    public void SwapPanel(int i) {
        if (i == CurrPanel) return; 
        optionUI.transform.GetChild(0).GetComponent<Text>().text = PanelNames[i];
        optionUI.transform.GetChild(CurrPanel + 1).gameObject.SetActive(false);
        optionUI.transform.GetChild(i + 1).gameObject.SetActive(true);

        CurrPanel = i;

    }


    
}
