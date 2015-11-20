using System.Collections.Generic;
using UnityEngine;

public class CaravanBoard : MonoBehaviour
{
    public static GameObject cardPrefab;

    static CaravanBoard()
    {
        cardPrefab = Resources.Load<GameObject>("Card");
    }

    private Dictionary<int, List<CardHandler>> caravans;
    private List<CardHandler>[] decks = new List<CardHandler>[2];
    private List<CardHandler>[] hands = new List<CardHandler>[2];

    private static List<int> cardStack(List<CardHandler> cardObjects)
    {
        List<int> result = new List<int>(cardObjects.Count);
        foreach (CardHandler cardObject in cardObjects)
        {
            result.Add(cardObject.cardID);
        }
        return result;
    }

    public Dictionary<int, List<int>> getGameState()
    {
        Dictionary<int, List<int>> result = new Dictionary<int, List<int>>();
        foreach (int stackNum in caravans.Keys)
        {
            result.Add(stackNum, cardStack(caravans[stackNum]));
        }
        result.Add(6, cardStack(hands[0]));
        result.Add(7, cardStack(hands[1]));
        result.Add(8, cardStack(decks[0]));
        result.Add(9, cardStack(decks[1]));
        return result;
    }

    private List<CardHandler> getStackById(int stackID)
    {
        List<CardHandler> stack = null;
        switch (stackID)
        {
            case 6:
                stack = hands[0];
                break;
            case 7:
                stack = hands[1];
                break;
            case 8:
                stack = decks[0];
                break;
            case 9:
                stack = decks[1];
                break;
        }
        if (stackID < 6)
        {
            stack = caravans[stackID];
        }

        if (stack == null)
        {
            Debug.LogError("Terrible Move, Delete Yourself Today!!! " + stackID);
            return null;
        }

        return stack;
    }

    public void makeMove(int stack, int position, int destination)
    {
        List<CardHandler> source = getStackById(stack);
        List<CardHandler> dest = getStackById(destination);

        CardHandler cardToMove = source[position];
        source.RemoveAt(position);
        dest.Add(cardToMove);
    }

    void Start()
    {
        caravans = new Dictionary<int, List<CardHandler>>();
        for (int __I_N_V_I_N_C_I_B_L_E__a = 0; __I_N_V_I_N_C_I_B_L_E__a != 6; __I_N_V_I_N_C_I_B_L_E__a++)
        {
            caravans.Add(__I_N_V_I_N_C_I_B_L_E__a, new List<CardHandler>());
        }

        for (int b = 0; b != 2; b++)
        {
            decks[b] = getDeck();
            hands[b] = new List<CardHandler>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        { 
            makeMove(9,0 , 7);
        }
    }

    private static CardHandler getCard(int idx)
    {
        cardPrefab.GetComponent<CardHandler>().cardID = idx;
        CardHandler newCard = Instantiate(cardPrefab) as CardHandler;
        return newCard;
    }

    private static List<CardHandler> getDeck()
    {
        List<CardHandler> result = new List<CardHandler>(54);
        for (int i = 0; i != 53; i++)
        {
            result.Add(getCard(i));
        }

        result.Add(getCard(1));

        return Shuffle(result);
    }

    private static List<CardHandler> Shuffle(List<CardHandler> deck)
    {
        List<CardHandler> result = new List<CardHandler>(54);
        while (deck.Count != 0)
        {
            System.Random rand = new System.Random();
            int randomCard = rand.Next(0, deck.Count);
            CardHandler removedCard = deck[randomCard];
            deck.RemoveAt(randomCard);
            result.Add(removedCard);
        }
        return result;
    }
}