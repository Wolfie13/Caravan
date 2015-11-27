using System.Collections.Generic;
using UnityEngine;

public class CaravanBoard : MonoBehaviour
{
    public static GameObject cardPrefab;

    static CaravanBoard()
    {
        cardPrefab = Resources.Load<GameObject>("Card");
    }

    [SerializeField]
    public GameObject[] boardPositions;

    private Dictionary<int, List<Card>> caravans;
    private List<Card>[] decks = new List<Card>[2];
    private List<Card>[] hands = new List<Card>[2];

    private bool dirty = true;

    private static List<int> cardStack(List<Card> cardObjects)
    {
        List<int> result = new List<int>(cardObjects.Count);
        foreach (Card cardObject in cardObjects)
        {
            result.Add(cardObject.cardID);
        }
        return result;
    }

    public Dictionary<int, List<int>> getGameState()
    {
        Dictionary<int, List<int>> result = new Dictionary<int, List<int>>();
        for (int i = 0; i != 10; i++ )
        {
            result.Add(i, cardStack(getStackById(i)));
        }
        return result;
    }

    private List<Card> getStackById(int stackID)
    {
        List<Card> stack = null;
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
        List<Card> source = getStackById(stack);
        List<Card> dest = getStackById(destination);

        Card cardToMove = source[position];
        source.RemoveAt(position);
        dest.Add(cardToMove);
        dirty = true;
    }

    void Start()
    {
        caravans = new Dictionary<int, List<Card>>();
        for (int i = 0; i != 6; i++)
        {
            caravans.Add(i, new List<Card>());
        }

        for (int i = 0; i != 2; i++)
        {
            decks[i] = getDeck();
            hands[i] = new List<Card>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Moved?");
            makeMove(9, 0, 7);
        }

        if (dirty)
        {
            positionCards();
            dirty = false;
        }
    }

    void organizeStack(List<Card> stack, int num)
    {
        for (int i = 0; i != stack.Count; i++)
        {
            stack[i].transform.parent = boardPositions[num].transform;
            stack[i].transform.localPosition = new Vector3(0, i * 0.02f, 0);
        }
    }

    //Moves card objects into their proper positions.
    void positionCards()
    {
        foreach (int stackNum in caravans.Keys)
        {
            organizeStack(caravans[stackNum], stackNum);
        }
        organizeStack(hands[0], 6);
        organizeStack(hands[1], 7);
        organizeStack(decks[0], 8);
        organizeStack(decks[1], 9);
    }

    private static Transform getCard(int idx)
    {
        cardPrefab.GetComponent<Card>().cardID = idx;

        GameObject newCard = (GameObject)Instantiate(cardPrefab);
        // Return a transform since it's better/cheaper.
        return newCard.transform;
    }

    private static List<Card> getDeck()
    {
        List<Card> result = new List<Card>(54);
        GameObject parentGroup = new GameObject("Cards");
        Transform newCard;

        for (int i = 0; i != 53; ++i)
        {
            newCard = getCard(i);
            newCard.gameObject.transform.parent = parentGroup.transform;

            result.Add(newCard.GetComponent<Card>());
        }

        newCard = getCard(1);
        newCard.gameObject.transform.parent = parentGroup.transform;

        result.Add(newCard.GetComponent<Card>());

        // Moves the whole deck of cards
        parentGroup.transform.position = new Vector3(Random.Range(0, 10), 0, Random.Range(0, 10));

        return Shuffle(result);
    }

    private static List<Card> Shuffle(List<Card> deck)
    {
        List<Card> result = new List<Card>(54);
        while (deck.Count != 0)
        {
            System.Random rand = new System.Random();
            int randomCard = rand.Next(0, deck.Count);
            Card removedCard = deck[randomCard];
            deck.RemoveAt(randomCard);
            result.Add(removedCard);
        }
        return result;
    }
}