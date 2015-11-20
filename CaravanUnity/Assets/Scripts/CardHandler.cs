using UnityEngine;
using System.Collections;
using System;

public class CardHandler : MonoBehaviour
{
	[SerializeField] public int cardID;
	private static Texture2D[] CARD_TEXTURES = new Texture2D[53];

    static CardHandler() {
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

    /*
     * -1: card has special behaviour 
     * 
     */
    static int getCaravanValue(int cardID) {
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
        mr.material.mainTexture = CARD_TEXTURES[cardID];
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
