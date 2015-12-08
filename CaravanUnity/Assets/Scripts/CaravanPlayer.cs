using UnityEngine;
using System.Collections;

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
                if (cards[j] == cardToFind)
                {
                    result.stack = i;
                    result.pos = j;
                    return result;
                }
            }
        }

        return result;
    }

    Vector3 originalPos;

	void Update ()
    {
        //CardFollowCursor();

		if (Input.GetMouseButtonDown (0))
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			if (Physics.Raycast(ray, out hit, 100f))
			{
                Card foundCard = (Card)hit.collider.gameObject.GetComponent<Card>();

				// Here we'll check if we have a card already.
				// If we do have a card we'll check where we're putting it.
                if (selectedCard != null)
                {
                    //selectedCard.transform.position = originalPos;

                    BoardPos source = getPositionForCard(selectedCard);
                    if (foundCard != null)
                    {
                        BoardPos destination = getPositionForCard(foundCard);
                        board.makeMove(source.stack, source.pos, destination.stack, destination.pos);
                                                
                        selectedCard = null;
                        return;

                    }

                    if (hit.collider.gameObject != null)
                    {
                        for (int i = 0; i != 6; i++)
                        {
                            if (board.boardPositions[i] == hit.collider.gameObject.transform.parent.gameObject)
                            {
                                board.makeMove(source.stack, source.pos, i, -1);

                                selectedCard = null;
                                return;
                            }
                        }
                    }
                }

                if (foundCard != null && selectedCard == null)
                {
                    selectedCard = foundCard;
                }

			}
		}

        if(Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.collider.gameObject != null)
                {
                    for (int i = 0; i != 3; i++)
                    {
                        if (board.boardPositions[i] == hit.collider.gameObject.transform.parent.gameObject)
                        {
                            board.disband(i);

                            selectedCard = null;
                            return;
                        }
                    }
                }
            }
        }
	}

    void CardFollowCursor()
    {
        if (selectedCard == null)
        {
            return;
        }

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 6f;

        originalPos = selectedCard.gameObject.transform.position;
        selectedCard.gameObject.transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
        selectedCard.gameObject.collider.enabled = false;
    }
}
