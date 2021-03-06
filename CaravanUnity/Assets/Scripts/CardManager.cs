﻿using UnityEngine;
using System;

public class CardManager : MonoBehaviour
{
	private static readonly Texture2D[] CARD_TEXTURES = new Texture2D[53];

	// Use this for initialization
	public void Start()
    {
		if (!Application.isPlaying) return;

        Texture2D[] LoadedCards = Resources.LoadAll<Texture2D>("Cards/");
        foreach (Texture2D tex in LoadedCards)
        {
            int idx = Int32.Parse(tex.name);
            if (idx >= 0 || idx < 53)
            {
                CARD_TEXTURES[idx - 1] = tex;
            }
        }
	}

    public static Texture2D getCardTexture(int cardID)
    {
        if (cardID > CARD_TEXTURES.Length && cardID < 0)
        {
            return null;
        }
        return CARD_TEXTURES[cardID];
    }
}
