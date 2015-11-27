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

    void step()
    {
        //get all possible moves
        List<CaravanMove> possibleMoves = getAllMoves(false);

        turnNumber++;
    }

    private float heuristicForStack(int stack, bool isPlayer)
    {
        //if stack is below 20, scaling linearly from 0-20.
        //if more than 27 -1
        //if between 20-27, and bigger than opposition, +1
        int opposingStack = stack + (isPlayer ? -3 : 3);

        return 0;
    }

    private float evaluateHeuristic(bool isPlayer)
    {
        if (isPlayer)
        {
            
        }
        return 0;
    }

    private List<CaravanMove> getAllMoves(bool isPlayer)
    {
        List<CaravanMove> result = new List<CaravanMove>();
        //Play
        //for each card in hand
            //play against each caravan on table
        //discard
        //for each card in hand
            //remove and draw new card from deck
        //disband
        //for each owned caravan on board
            //remove caravan
        return result;
    }


    private class CaravanMove
    {
        enum Type { Discard, Play, Disband };
        private Type type;
        int sourceStack;
        int sourceIdx;
        int destStack;
        int destIdx;

        public void execute(CaravanBoard board)
        {
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
    }
}