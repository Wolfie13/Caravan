using UnityEngine;
using System.Collections;
using System;

public class Card : MonoBehaviour
{
	[SerializeField] public int cardID;

    static int getCaravanValue(int cardID) 
    {
        if (cardID < 36)
        {
            return cardID / 4;
        }
        if (cardID > 35 && cardID < 40)
        {
            return 1;
        }
        return -1;
    }

    public interface CaravanCardAction
    {
        void act(CaravanBoard board, int playedAt);
    }

	// Use this for initialization
	void Start ()
	{
        CardManager mrs = GameObject.Find("Board").GetComponent<CardManager>();

        if (mrs == null)
        {
            Debug.LogError("Board doesn't exist you fool!");
            return;
        }

        MeshRenderer mr = this.gameObject.GetComponent<MeshRenderer>();
        mr.material.mainTexture = mrs.getCardTexture(cardID);
	}
}
