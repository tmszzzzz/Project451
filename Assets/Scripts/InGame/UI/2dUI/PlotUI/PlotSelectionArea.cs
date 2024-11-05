using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlotSelectionArea : MonoBehaviour
{
    //Use three buttons here
    [SerializeField] private List<GameObject> allButtons;

    public void ClearCurrentButtons()
    {
        foreach(GameObject button in allButtons) 
        {
            button.GetComponent<Button>().onClick.RemoveAllListeners();
            button.SetActive(false);
        }
    }

    public List<Button> NeedButtons(int buttonNum)
    {
        List<Button> neededButtons = new List<Button>();

        for (int i = 0; i < buttonNum && i < allButtons.Count; i++)
        {
            //Debug.Log("set active");
            //Debug.Log(allButtons[i].activeSelf);
            //Debug.Log(allButtons[i].activeSelf);
            neededButtons.Add(allButtons[i].GetComponent<Button>());
            allButtons[i].SetActive(true);
        }

        return neededButtons;
    }

    public void ClosePlots()
    {
        foreach(GameObject button in allButtons)
        {
            button.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
