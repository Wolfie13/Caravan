using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour {

    public GameObject[] CaravanTexts;
    
    private Dictionary<int, List<int>> GameState;

	// Use this for initialization
	void Start ()
    {
        GameState = this.GetComponent<CaravanBoard>().getGameState();	
	}
	
	// Update is called once per frame
	void Update () 
    {
        GameState = this.GetComponent<CaravanBoard>().getGameState();	
        UpdateCaravanTexts(GameState);	
	}

    void UpdateCaravanTexts(Dictionary<int, List<int>> gamestate)
    {
        foreach(KeyValuePair<int, List<int>> stack in gamestate)
        {
           if(stack.Key < 6)
           {
               int caravan_value = CaravanUtil.caravanValue(stack.Value);
               CaravanTexts[stack.Key].GetComponent<TextMesh>().text = caravan_value.ToString();
           }
        
        }
    }
}
