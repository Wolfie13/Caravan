using System;
using System.Collections.Generic;
using System.Linq;
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
    private bool AI_turn = false;
    private bool game_over = false;

    public const int PLAYER_HAND = 6;
    public const int AI_HAND = 7;
    public const int PLAYER_DECK = 8;
    public const int AI_DECK = 9;

    CaravanAI ai_instance = null;

    //Use this for intialization
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
            for (int j = 0; j != 5; j++)
            {
                int srcPos = decks[i].Count - 1;
                Card cardToMove = decks[i][srcPos];
                decks[i].RemoveAt(srcPos);
                hands[i].Add(cardToMove);
            }
        }

        ai_instance = new CaravanAI(this);
    }

    //Game Loop
    void Update()
    {
        if(game_over)
        {
            Application.LoadLevel(Application.loadedLevel);
        }

        if (!game_over)
        {
            if (AI_turn)
            {
                ai_instance.greedyStep();                
            }

            if (dirty)
            {
                positionCards();
                dirty = false;
            }

            int winnar = CheckWin();
            if (winnar != 0)
            {
                Debug.Log("Game Over: " + winnar);
                game_over = true;
            }
        }
    }

    private static List<int> cardStack(List<Card> cardObjects)
    {
        List<int> result = new List<int>(cardObjects.Count);

	    result.AddRange(cardObjects.Select(cardObject => cardObject.cardID));

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

    public List<Card> getStackById(int stackID)
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
            // Possible check for null.
        }

	    if (stack != null) return stack;

	    Debug.LogError("Terrible Move, Delete Yourself Today!!! " + stackID);
	    return null;
    }

    public void makeMove(int stack, int srcPos, int destination, int destPos, bool finishTurn = true)
    {
        List<Card> source = getStackById(stack);
        List<Card> dest = getStackById(destination);

        Card cardToMove = source[srcPos];
        source.RemoveAt(srcPos);
        if (destPos == -1)
        {
            dest.Add(cardToMove);
        }
        else
        {
            dest.Insert(destPos, cardToMove);
        }
        dirty = true;
        if (finishTurn)
        {
            AI_turn = !AI_turn;
        }
    }

    public void discard(int stack, int idx)
    {
        Destroy(getStackById(stack)[idx].gameObject);
        getStackById(stack).RemoveAt(idx);
        dirty = true;
        AI_turn = !AI_turn;
    }

    public void disband(int stack)
    {
        foreach (Transform child in boardPositions[stack].transform.Cast<Transform>().Where(child => child.gameObject.GetComponent<Card>() != null))
        {
	        Destroy(child.gameObject);
        }

        getStackById(stack).Clear();
        dirty = true;
        AI_turn = !AI_turn;
    }    

    void organizeStack(List<Card> stack, int num, bool hidden, bool dontSpread = false)
    {
		Transform cardParentObject = boardPositions[num].transform;

        if (hidden)
        {
			// This fixes parenting issues resulting in odd visuals and stops the player doing bad stuff.
			GameObject visualFix = new GameObject("Hidden Cards", typeof(BoxCollider));

			visualFix.transform.parent = cardParentObject;
			visualFix.transform.localPosition = Vector3.zero;
			cardParentObject = visualFix.transform;
        }

        bool topCaravan = num / 6.0f >= 0.5f;
        topCaravan &= num <= 5;

        for (int i = 0; i != stack.Count; i++)
        {
			stack[i].transform.parent = cardParentObject;
            stack[i].transform.localRotation = Quaternion.identity;
            stack[i].transform.localPosition = new Vector3(0, (i * 0.02f) + (-0.17f), (topCaravan ? 0.5f : -0.5f) * (dontSpread ? 0 : i));
        }

		if (!hidden) return;

	    Transform[] allChildren = cardParentObject.gameObject.GetComponentsInChildren<Transform>();
	    // Basically add all the positions together and divide by the child count to get the true center.
	    Vector3 childCenter = allChildren.Aggregate(Vector3.zero, (current, child) => current + child.transform.position) / allChildren.Length;

		cardParentObject.RotateAround(cardParentObject.localPosition + childCenter, Vector3.forward, 180f);
		// Resize the box collider to accommodate the cards.
		BoxCollider interactionFix = cardParentObject.GetComponent<BoxCollider>();
		interactionFix.center = new Vector3(0f, cardParentObject.localPosition.y / 2f, 0f);
		interactionFix.size = new Vector3(1.2f, 1, 2.2f);
    }

    //Moves card objects into their proper positions.
    void positionCards()
    {
        foreach (int stackNum in caravans.Keys)
        {
            organizeStack(caravans[stackNum], stackNum, false);
        }
        organizeStack(hands[0], 6, false);
        organizeStack(hands[1], 7, true, true);
        organizeStack(decks[0], 8, true, true);
        organizeStack(decks[1], 9, true, true);
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


		//What card does this add?
        newCard = getCard(1);
        newCard.gameObject.transform.parent = parentGroup.transform;

        result.Add(newCard.GetComponent<Card>());

        // Moves the whole deck of cards
		parentGroup.transform.position = new Vector3(UnityEngine.Random.Range(0, 10), 0, UnityEngine.Random.Range(0, 10));

        return Shuffle(result);
    }

    private static List<Card> Shuffle(List<Card> deck)
    {
        List<Card> result = new List<Card>(54);
		System.Random rand = new System.Random(Guid.NewGuid().GetHashCode());
        while (deck.Count != 0)
        {
            int randomCard = rand.Next(0, deck.Count);
            Card removedCard = deck[randomCard];
            deck.RemoveAt(randomCard);
            result.Add(removedCard);
        }
        return result;
    }

    //-1 = ai win, 0 = draw, 1 = player win
    private int CheckWin()
    {
        Dictionary<int, List<int>> caravans = this.getGameState();
        bool playerWin = true;
        for(int i = 0; i < 3; i++)
        {
            playerWin &= CaravanUtil.winningCaravan(caravans[i], caravans[i + 3]);
        }
        if (playerWin)
        {
            //player won
            return 1;
        }

        bool aiWin = true;
        for (int i = 0; i < 3; i++)
        {
            aiWin &= CaravanUtil.winningCaravan(caravans[i + 3], caravans[i]);
        }
        if (aiWin)
        {
            //ai won
            return -1;
        }

        return 0;
    }
}