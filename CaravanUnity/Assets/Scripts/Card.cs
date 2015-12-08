﻿using UnityEngine;
using System.Collections;
using System;

public class Card : MonoBehaviour
{
	[SerializeField] public int cardID;

    public int caravanValue()
    {
        return getCaravanValue(this.cardID);
    }

    public const int CV_JACK = -2;
    public const int CV_JOKER = -3;
    public const int CV_KING = -4;
    public const int CV_QUEEN = -5;

    private Color start_colour;
    private MeshRenderer mr;

    public static int getCaravanValue(int cardID) 
    {
        if (cardID < 36)
        {
            return cardID / 4 + 2;
        }
        if (cardID > 35 && cardID < 40)
        {
            return 1;
        }
        if (cardID == 40)
        {
            return CV_JOKER;
        }
        if (cardID > 40 && cardID < 45)
        {
            return 11;
        }
        if (cardID > 44 && cardID < 49)
        {
            return CV_KING;
        }
        if (cardID > 48 && cardID < 53)
        {
            return CV_QUEEN;
        }
        return -1;
    }

	// Use this for initialization
	void Start ()
	{
        mr = this.gameObject.GetComponent<MeshRenderer>();
        mr.material.mainTexture = CardManager.getCardTexture(cardID);
	}

    void OnMouseEnter()
    {        
        start_colour = mr.material.color;
        mr.material.color = new Color(0.0f, 153.0f, 0.0f);
    }

    void OnMouseExit()
    {
        mr.material.color = start_colour;
    }
}
