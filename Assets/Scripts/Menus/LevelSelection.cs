using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour
{
    [SerializeField] private Button[] levelsButtons;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < levelsButtons.Length; i++)
        {
            levelsButtons[i].GetComponentInChildren<Text>().text = (i+1).ToString();
            levelsButtons[i].GetComponent<LevelSelectionButton>().index = i+1;

            if (i >= 3)
            {
                levelsButtons[i].interactable = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
