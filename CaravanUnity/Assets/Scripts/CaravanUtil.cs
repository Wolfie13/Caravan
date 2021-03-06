using System.Collections.Generic;
using System.Linq;


public static class CaravanUtil
{
	public static Dictionary<int, List<int>> copyState(Dictionary<int, List<int>> input)
	{
		Dictionary<int, List<int>> result = new Dictionary<int, List<int>>();
		
		foreach (int key in input.Keys)
		{
			List<int> copyEntry = input[key].ToList();
			result[key] = copyEntry;
		}

		return result;
	}

	public static bool winningCaravan(List<int> stack)
	{
		int val = caravanValue(stack);
		return val > 20 && val < 27;
	}

    public static bool winningCaravan(List<int> stack, List<int> opposingStack)
    {
        int val1 = caravanValue(stack);
        int val2 = caravanValue(opposingStack);
        return val1 > 20 && val1 < 27 && val1 > val2;
    }
	
	public static int cardValue(List<int> stack, int idx)
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
			return -(int) UnityEngine.Mathf.Floor(cardValue(stack, idx - 1) / 2);
		}
		
		return val;
	}
	
	public static int caravanValue(List<int> stack)
	{
		int result = 0;
		for (int idx = 0; idx != stack.Count; idx++)
		{
			result += cardValue(stack, idx);
		}
		return result;
	}

	
	private static float heuristicForStack(List<int> stack, List<int> opposingStack)
	{
		int stack_value = caravanValue (stack);
		int opposing_stack_value = caravanValue (opposingStack);

		//If stack is between 20 - 27 and bigger than the opposing stack rturn large value for heuristic
        if (stack_value >= 20 && stack_value <= 27 && stack_value > opposing_stack_value)
        {
            return 500.0f;
        }
		//If stack is below 20, scale heuristic from 0 - 20
		if (stack_value <= 21) 
		{
			return (float)stack_value/20 * 10.0f;
		}
		//If stack is greater than 27 return small value for heuristic
		if (stack_value > 27) 
		{
			return -20f;
		}

        //If opposing stack is greater than 27 return large value for heuristic
        if(opposing_stack_value > 27)
        {
            return 3000.0f;
        }

		return 0;
	}
	
	public static float heuristicForState(Dictionary<int, List<int>> state, bool isPlayer)
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
		
		accumHeuristic += state[myHand].Count * 2;
		accumHeuristic += state[myDeck].Count * 4;

		return accumHeuristic;
	}
}

