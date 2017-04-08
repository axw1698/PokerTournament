using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerTournament
{
    class Player7 : Player
    {
        private enum States { Evaluate, Check, Bet, Fold, RaiseCall }

        #region Variables
        List<PlayerAction> actions;
        Card[] hand;
        Dictionary<int, int[]> bettingRangeTable;
        Dictionary<int, int[]> maxRaisesTable; // raise table should hold 1. num of raise based on rank and amount to raise by
        Dictionary<int, int> cardsToDiscardTable;
        Card highCard;
        int rank;
        int safety;
        int maxBet;
        int currentBetPot; // amount of money currently being bet and is in pot
        int numRaise = 0; //number of times raised during a phase;
        States stateRound1, stateRound2;

        #endregion

        public Player7(int idNum, string name, int money) : base(idNum, name, money)
        {
            bettingRangeTable = new Dictionary<int, int[]>();
            maxRaisesTable = new Dictionary<int, int[]>();
            cardsToDiscardTable = new Dictionary<int, int>();
            rank = 0;
            safety = 0;
            maxBet = 0;
            stateRound1 = 0;
            stateRound2 = 0;
        }

        #region Custom Methods
        private void AnalyzeHand()
        {
            rank = Evaluate.RateAHand(this.Hand, out highCard);
            // May implement more later if necessary
        }

        private PlayerAction BTCheck(States currentState)
        {
            if(rank >= 2) // May change to 3 if one pair isn't good enough?
            {
                // First player
                if (!this.Dealer)
                    return BTBet(currentState);
                // Second player
                else
                    return BTRaiseCall(currentState);
            }
            else
            {
                // First player
                if (!this.Dealer)
                {
                    // FIGURE OUT WHAT STATE WE'RE GOING TO GO TO!!!!!!!! //
                    return new PlayerAction(this.Name, "Bet1", "check", 0);
                }
                // Second player but first checked
                else if (actions[actions.Count].ActionName == "check")
                {
                    stateRound1 = States.Evaluate; //Go back since the round is gonna end
                    return new PlayerAction(this.Name, "Bet1", "check", 0);
                }
                // Second player and first didn't check
                else
                {
                    //Check to see if you should fold, or if you can keep playing
                    if (ShouldFold())
                    {
                        stateRound1 = States.Evaluate; //Go back since the round is gonna end
                        return new PlayerAction(this.Name, "Bet1", "fold", 0);
                    }
                    else
                    {
                        currentState = States.RaiseCall;
                        BTRaiseCall("Bet1",currentState);
                    }
                }
            }

            // DON'T FORGET TO UPDATE STATE
            return new PlayerAction(this.Name, "<STUFF>", "<OTHER STUFF>", -1);
        }

        private PlayerAction BTBet(States currentState)
        {
            // CHECK TO SEE IF YOU SHOULD FOLD

            // DON'T FORGET TO UPDATE STATE
            return new PlayerAction(this.Name, "<STUFF>", "<OTHER STUFF>", -1);
        }

        private PlayerAction BTRaiseCall( PlayerAction prevAction ,string actionPhase, States currentState)
        {
            // DON'T FORGET TO UPDATE STATE
            if(prevAction.ActionName == "raised" && currentBetPot > maxBet) //check if other opponent raised.
            {
                 return new PlayerAction(this.Name, actionPhase, "fold", 0);
            }
            else
            {
                if(Money > safety)
                {
                    //consult the raise table
                    //If there are raisesLeft then raise based on table
                    if((maxRaisesTable[rank])[0] > numRaise)
                    {
                        //you have raise left so Raise based on table
                        return new PlayerAction(this.Name, actionPhase, "raise", (maxRaisesTable[rank])[1]);
                    }
                    else //no raises left
                    {
                        //call
                        return new PlayerAction(this.Name, actionPhase, "call", prevAction.Amount);
                    }
                }
                else
                {
                    return new PlayerAction(this.Name, actionPhase, "call", prevAction.Amount);
                }
            }

            
        }

        private bool ShouldFold()
        {
            // Do this!
            // If you have rank of 1, all 4 suits, or distant numbers
            if (rank == 1)
            {
                // Evaluate the amount of suits in five cards
                int sameSuitNum = 0;
                for (int i = 0; i < hand.Length; i++)
                {
                    for (int j = 0; i < hand.Length; j++)
                    {
                        if (i != j)
                        {
                            if (hand[i].Suit == hand[0].Suit)    // all cards compare with the first card
                            {
                                sameSuitNum++;  // if this num less  then 2 then there will be 4 suils 
                            }
                        }
                    }

                }
                bool badStraight = false;
                int numEach = 0;    // compare two cards value
                int numAll = 0;     // add up all the different between values when not fold 
                for (int i = 0; i < hand.Length - 1; i++)
                {
                    numEach = hand[i + 1].Value - hand[i].Value;
                    if (numEach > 2) // the distance between cards are too big
                    {
                        badStraight = true;
                        break;
                    }
                    else
                    {
                        numAll += numEach;  // to check the total gap amount
                    }
                }
                if (numAll > 2)
                {
                    badStraight = true; // at least two gap between cards number                   
                }

                if (badStraight == true && sameSuitNum <= 2)
                {
                    return true;  // REALLY BAD TO HAVE NO CLOSE STRAIGHT AND SAME SUIT
                }

                // If the maxbet is bigger then safety 
                if (maxBet > safety)
                {
                    return true; // Might need to add more later
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        private void CalculateSafetyAndMaxBet()
        {
            // Do stuff!
        }
        #endregion

        #region Abstract Overrides
        public override PlayerAction BettingRound1(List<PlayerAction> actions, Card[] hand)
        {
            this.actions = actions;
            this.hand = hand;
            stateRound2 = States.Evaluate;
            while (true) //Professor, I'm sorry
            {
                switch (stateRound1)
                {
                    case States.Evaluate:
                        AnalyzeHand();
                        CalculateSafetyAndMaxBet();
                        stateRound1 = States.Check;
                        break;
                    case States.Check:
                        BTCheck(stateRound1);
                        break;
                    case States.Bet:
                        BTBet(stateRound1);
                        break;
                    case States.RaiseCall:
                        BTRaiseCall(stateRound1);
                        break;
                }
            }
        }

        public override PlayerAction BettingRound2(List<PlayerAction> actions, Card[] hand)
        {
            stateRound1 = States.Evaluate;
            while (true)
            {
                switch (stateRound1)
                {
                    case States.Evaluate:
                        AnalyzeHand();
                        CalculateSafetyAndMaxBet();
                        stateRound2 = States.Bet;
                        break;
                    case States.Bet:
                        BTBet(stateRound2);
                        break;
                    case States.RaiseCall:
                        BTRaiseCall(stateRound2);
                        break;
                }
            }
        }

        public override PlayerAction Draw(Card[] hand)
        {
            // Consider high card / 1 pair case
            // Consult table to find cards to discard
            // Return basically nothing if none of those go through
            return new PlayerAction(this.Name, "<STUFF>", "<OTHER STUFF>", -1);
        }
        #endregion
    }
}
