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
        MeshRenderer mr = this.gameObject.GetComponent<MeshRenderer>();
        mr.material.mainTexture = CardManager.getCardTexture(cardID);
	}
}
