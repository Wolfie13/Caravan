using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// _____ _   _  _____  ______ _       _____   _____________   _   _   ___   _____   _____ _   _  _____   _     _____  _    _   _   _ _   ____  _________ ___________ ___________   _____   ___  ______  ___  _   _  ___   _   _  _____ 
//|_   _| | | ||  ___| | ___ \ |     / _ \ \ / /  ___| ___ \ | | | | / _ \ /  ___| |_   _| | | ||  ___| | |   |  _  || |  | | | \ | | | | |  \/  || ___ \  ___| ___ \  ___|  _  \ /  __ \ / _ \ | ___ \/ _ \| | | |/ _ \ | \ | |/  ___|
//  | | | |_| || |__   | |_/ / |    / /_\ \ V /| |__ | |_/ / | |_| |/ /_\ \\ `--.    | | | |_| || |__   | |   | | | || |  | | |  \| | | | | .  . || |_/ / |__ | |_/ / |__ | | | | | /  \// /_\ \| |_/ / /_\ \ | | / /_\ \|  \| |\ `--. 
//  | | |  _  ||  __|  |  __/| |    |  _  |\ / |  __||    /  |  _  ||  _  | `--. \   | | |  _  ||  __|  | |   | | | || |/\| | | . ` | | | | |\/| || ___ \  __||    /|  __|| | | | | |    |  _  ||    /|  _  | | | |  _  || . ` | `--. \
//  | | | | | || |___  | |   | |____| | | || | | |___| |\ \  | | | || | | |/\__/ /   | | | | | || |___  | |___\ \_/ /\  /\  / | |\  | |_| | |  | || |_/ / |___| |\ \| |___| |/ /  | \__/\| | | || |\ \| | | \ \_/ / | | || |\  |/\__/ /
//  \_/ \_| |_/\____/  \_|   \_____/\_| |_/\_/ \____/\_| \_| \_| |_/\_| |_/\____/    \_/ \_| |_/\____/  \_____/\___/  \/  \/  \_| \_/\___/\_|  |_/\____/\____/\_| \_\____/|___/    \____/\_| |_/\_| \_\_| |_/\___/\_| |_/\_| \_/\____/ 
                                                                                                                                                                                                                                    

class CaravanAI
{

    private CaravanBoard board;
    int turnNumber;

    public CaravanAI(CaravanBoard board)
    {
        this.board = board;
        this.turnNumber = 0;
    }

    public void greedyStep()
    {
        //get all possible moves
        List<CaravanMove> possibleMoves = getAllMoves(false);
        Dictionary<int, List<int>> startingState = board.getGameState();
        float bestStateValue = -1f;
        CaravanMove bestMove = null;
        foreach (CaravanMove move in possibleMoves)
        {
            Dictionary<int, List<int>> resultantState = copyState(startingState);
            move.test(resultantState); //Modifies resultantstate with move
            float heuristicResult = heuristicForState(resultantState, false);
            if (heuristicResult > bestStateValue)
            {
                bestMove = move;
            }
        }

        bestMove.execute(board);
        turnNumber++;
    }

    private Dictionary<int, List<int>> copyState(Dictionary<int, List<int>> input)
    {
        Dictionary<int, List<int>> result = new Dictionary<int, List<int>>();
        foreach (int key in input.Keys)
        {
            List<int> copyEntry = new List<int>();
            foreach (int entry in input[key])
            {
                copyEntry.Add(entry);
            }
            result[key] = copyEntry;
        }
        return result;
    }

    private float heuristicForState(Dictionary<int, List<int>> state, bool isPlayer)
    {
        int myDeck = isPlayer ? 8 : 9;
        int myHand = isPlayer ? 6 : 7;
        int[] myCaravans = isPlayer ? new int[] { 0, 1, 2 } : new int[] { 3, 4, 5 };
        int[] theirCaravans = !isPlayer ? new int[] { 0, 1, 2 } : new int[] { 3, 4, 5 };

        float accumHeuristic = 0f;

        bool myCaravansWinning = true;
        bool theirCaravansWinning = true;
        for (int i = 0; i != myCaravans.Length; i++)
        {
            accumHeuristic += heuristicForStack(state[myCaravans[i]], state[theirCaravans[i]]);

            myCaravansWinning = myCaravansWinning && winningCaravan(state[myCaravans[i]]);
            theirCaravansWinning = theirCaravansWinning && winningCaravan(state[theirCaravans[i]]);
        }

        if (!theirCaravansWinning && myCaravansWinning)
        {
            accumHeuristic += 9000;
        }

        accumHeuristic += state[myHand].Count * 3;
        accumHeuristic += state[myDeck].Count * 1;

        return accumHeuristic;
    }

    private bool winningCaravan(List<int> stack)
    {
        int val = caravanValue(stack);
        return val > 20 && val < 27;
    }

    private int cardValue(List<int> stack, int idx)
    {
        if (idx < 0)
        {
            return 0;
        }
        int val = Card.getCaravanValue(stack[idx]);
        
        if (val == Card.CV_KING)
        {
            return cardValue(stack, idx - 1);
        }

        if (val == Card.CV_QUEEN)
        { 
            return (int) UnityEngine.Mathf.Floor(cardValue(stack, idx - 1) / 2);
        }

        return val;
    }

    public int caravanValue(List<int> stack)
    {
        int result = 0;
        for (int idx = 0; idx != stack.Count; idx++)
        {
            result += cardValue(stack, idx);
        }
        return result;
    }

    private float heuristicForStack(List<int> stack, List<int> opposingStack)
    {
        //if stack is below 20, scaling linearly from 0-20.
        //if more than 27 -1
        //if between 20-27, and bigger than opposition, +1
        //that comes later!!!
        return 0;
    }

    private List<CaravanMove> getAllMoves(bool isPlayer)
    {
        List<CaravanMove> result = new List<CaravanMove>();
        Dictionary<int, List<int>> gameState = board.getGameState();
        int myDeck = isPlayer ? 8 : 9;
        int myHand = isPlayer ? 6 : 7;
        int[] myCaravans = isPlayer ? new int[] {0, 1, 2} : new int[] {3, 4, 5};
        int[] allCaravans = new int[] { 0, 1, 2, 3, 4, 5 };

        //Draw from deck
        result.Add(new CaravanMove(CaravanMove.Type.Play, myDeck, 0, myHand, -1));

        //for each card in hand    
        for (int i = 0; i != gameState[myHand].Count; i++)
        {
            //play against each caravan on table
            foreach (int caravanIdx in allCaravans)
            {
                //And each card in said caravan
                for (int j = 0; j != gameState[caravanIdx].Count; j++)
                {
                    result.Add(new CaravanMove(CaravanMove.Type.Play, myHand, i, caravanIdx, j));
                }
                //Play on an empty caravan
                if (gameState[caravanIdx].Count == 0)
                {
                    result.Add(new CaravanMove(CaravanMove.Type.Play, myHand, i, caravanIdx, 0));
                }
            }
        }

        //for each card in hand
        for (int i = 0; i != gameState[myHand].Count; i++)
        {
            //remove and draw new card from deck
            result.Add(new CaravanMove(CaravanMove.Type.Discard, myHand, i, 0, 0));
        }

        //for each owned caravan on board
        foreach (int caravanIdx in myCaravans)
        {
            //remove caravan
            if (gameState[caravanIdx].Count > 0)
            {
                result.Add(new CaravanMove(CaravanMove.Type.Disband, caravanIdx, 0, 0, 0));
            }
        }

        return result;
    }


    private class CaravanMove
    {
        public CaravanMove(Type type, int sourceStack, int sourceIdx, int destStack, int destIdx)
        {
            this.type = type;
            this.sourceStack = sourceStack;
            this.sourceIdx = sourceIdx;
            this.destStack = destStack;
            this.destIdx = destIdx;
        }

        public enum Type { Discard, Play, Disband };
        private Type type;
        int sourceStack;
        int sourceIdx;
        int destStack;
        int destIdx;

        public void execute(CaravanBoard board)
        {
            UnityEngine.Debug.Log(this.type + " s:" + sourceStack + ":" + sourceIdx + " d:" + destStack + ":" + destIdx);
            switch (type)
            {
                case Type.Disband:
                    board.disband(sourceStack);
                    break;

                case Type.Play:
                    board.makeMove(sourceStack, sourceIdx, destStack, destIdx);
                    break;

                case Type.Discard:
                    board.discard(sourceStack, sourceIdx);
                    break;

            }
        }

        internal void test(Dictionary<int, List<int>> testState)
        {
            switch (type)
            {
                case Type.Disband:
                    testState[sourceStack].Clear();
                    break;

                case Type.Play:
                    List<int> source = testState[sourceStack];
                    List<int> dest = testState[destStack];

                    int cardToMove = source[sourceIdx];
                    source.RemoveAt(sourceIdx);
                    if (destIdx == -1)
                    {
                        dest.Add(cardToMove);
                    }
                    else
                    {
                        dest.Insert(destIdx, cardToMove);
                    }
                    break;

                case Type.Discard:
                    testState[sourceStack].RemoveAt(sourceIdx);
                    break;

            }
        }
    }
}