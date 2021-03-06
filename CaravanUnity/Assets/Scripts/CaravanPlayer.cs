﻿using UnityEngine;
using System.Collections.Generic;

public class CaravanPlayer : MonoBehaviour
{
	private CaravanBoard board = null;
    private Card selectedCard = null;

	void Start ()
    {
        board = this.GetComponent<CaravanBoard>();
	}

    private struct BoardPos
    {
        public int stack;
        public int pos;
    }

    private BoardPos getPositionForCard(Card cardToFind)
    {
        BoardPos result;
        //Error values
        result.pos = -1;
        result.stack = -1;

        //For each stack
        for (int i = 0; i != board.boardPositions.Length; i++)
        {
            //get all the cards on that board position
            Card[] cards = board.boardPositions[i].transform.GetComponentsInChildren<Card>();
            //for each card
            for (int j = 0; j != cards.Length; j++)
            {
                //see if match
	            if (cards[j] != cardToFind) continue;

	            result.stack = i;
	            result.pos = j;
	            return result;
            }
        }

        return result;
    }

	void Update ()
	{
		if (selectedCard != null)
		{
			// Comment this to disable the FOV style card movement.
			VisualiseCard();
		}

		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		// Left mouse click.
		if (Input.GetMouseButtonDown(0))
		{
			if (Physics.Raycast(ray, out hit, 100.0f))
			{
				Card foundCard = hit.collider.gameObject.GetComponent<Card>();
				BoardPos destination = getPositionForCard(foundCard);

				// Here we'll check if we have a card already.
				// If we do have a card we'll check where we're putting it.
				if (selectedCard != null)
				{
					BoardPos source = getPositionForCard(selectedCard);
					if (foundCard != null)
					{
						if (board.boardPositions[6] == hit.collider.gameObject.transform.parent.gameObject) return;

						board.makeMove(source.stack, source.pos, destination.stack, destination.pos + 1, false);
						board.makeMove(8, 0, 6, 0);
						board.makeMove(9, 0, 7, 0, false);

						selectedCard.StateChange();
						selectedCard = null;
						return;
					}

					if (hit.collider.gameObject != null)
					{
						if (selectedCard.caravanValue() == Card.CV_KING || selectedCard.caravanValue() == Card.CV_QUEEN)
						{
							selectedCard.StateChange(true);
							return;
						}

						for (int i = 0; i != 6; i++)
						{
							if (board.boardPositions[i] != hit.collider.gameObject.transform.parent.gameObject) continue;

							board.makeMove(source.stack, source.pos, i, -1, false);
							board.makeMove(8, 0, 6, -1);
							board.makeMove(9, 0, 7, 0, false);

							selectedCard.StateChange();
							selectedCard = null;
							return;
						}
					}
				}

				if (foundCard != null && selectedCard == null)
				{
					if (destination.stack < 6 || destination.stack == 7 || destination.stack == 9) return;
					selectedCard = foundCard;

					selectedCard.StateChange();
				}
			}
		}
		// When true we check our options for right mouse click.
		if (!Input.GetMouseButtonDown(1)) return;
		// If you change your mind you can drop your card. (i.e. selecting a king on start won't screw you).
		if (selectedCard != null)
		{
			selectedCard.StateChange();
			selectedCard = null;
			return;
		}

		if (Physics.Raycast(ray, out hit, 100.0f))
		{
			// check if the card is in hand or caravan.
			GameObject foundObject = hit.collider.gameObject;

			if (foundObject != null)
			{
				// Discard a card in hand.
				if (board.boardPositions[6] == foundObject.transform.parent.gameObject)
				{
					board.makeMove(8, 0, 6, -1, false); // Get a new card.

					List<Card> handCards = board.getStackById(6);
					// With all the cards in our hand find the one we want gone.
					for (int index = 0; index < handCards.Count; ++index)
					{
						if (handCards[index].gameObject == foundObject)
						{
							board.discard(6, index); // Then remove the selected card.
							return;
						}
					}
				}

				// Disband caravan.
				for (int i = 0; i != 3; i++)
				{
					if (board.boardPositions[i] == foundObject.transform.parent.gameObject)
					{
						board.disband(i);
						return;
					}
				}
			}
		}
	}

	void VisualiseCard ()
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hit, 100.0f))
		{
			// check if the card is in hand or caravan.
			GameObject foundObject = hit.collider.gameObject;

			if (foundObject != null)
			{
				// Disband caravan.
				for (int i = 0; i != 6; i++)
				{
					if (board.boardPositions[i] == foundObject.transform.parent.gameObject)
					{
						selectedCard.transform.localPosition = board.boardPositions[i].transform.localPosition;
					}
				}
			}
		}
	}
}
