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
        string playerName;
        List<PlayerAction> actions;
        Card[] hand;
        //Dictionary<int, int[]> bettingRangeTable;
        Dictionary<int, int[]> maxRaisesTable; // raise table should hold 1. num of raise based on rank and amount to raise by
        int[,] bettingRangeTable;
        //Dictionary<int, int> cardsToDiscardTable; //Don't need?
        Card highCard;
        int rank;
        int safety;
        int maxBet;
        int currentBetPot; // amount of money currently being bet and is in pot   *************************
        int startMoney;
        int numRaise = 0; //number of times raised during a phase;
        States stateRound1, stateRound2;
        bool[] shouldDiscard;
        #endregion

        public Player7(int idNum, string name, int money) : base(idNum, name, money)
        {
            playerName = name;
            //bettingRangeTable = new Dictionary<int, int[]>();
            maxRaisesTable = new Dictionary<int, int[]>();
            bettingRangeTable = new int[,]
            {
                { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 },
                { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 },
                { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 },
                { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 },
                { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 },
                { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 },
                { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 },
                { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 },
                { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 },
                { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 }
            };
            //cardsToDiscardTable = new Dictionary<int, int>();
            rank = 0;
            safety = 0;
            maxBet = 0;
            stateRound1 = 0;            // why zero
            stateRound2 = 0;
            shouldDiscard = new bool[5];
            startMoney = this.Money;
            //Hard coded table values
            maxRaisesTable.Add(1, new int[] { 1, 1 });
            maxRaisesTable.Add(2, new int[] { 2, 2 });
            maxRaisesTable.Add(3, new int[] { 3, 3 });
            maxRaisesTable.Add(4, new int[] { 4, 4 });
            maxRaisesTable.Add(5, new int[] { 5, 5 });
            maxRaisesTable.Add(6, new int[] { 6, 6 });
            maxRaisesTable.Add(7, new int[] { 70, 7 });
            maxRaisesTable.Add(8, new int[] { 800, 8 });
            maxRaisesTable.Add(9, new int[] { 9000, 9 });
            maxRaisesTable.Add(10, new int[] { 10000, 10 });
            /*
            bettingRangeTable.Add(1, new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 });
            bettingRangeTable.Add(2, new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 });
            bettingRangeTable.Add(3, new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 });
            bettingRangeTable.Add(4, new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 });
            bettingRangeTable.Add(5, new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 });
            bettingRangeTable.Add(6, new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 });
            bettingRangeTable.Add(7, new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 });
            bettingRangeTable.Add(8, new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 });
            bettingRangeTable.Add(9, new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 });
            bettingRangeTable.Add(10, new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 });
            */
        }

        #region Custom Methods
        private void AnalyzeHand()
        {
            rank = Evaluate.RateAHand(this.Hand, out highCard);
            // May implement more later if necessary
        }

        private PlayerAction BTCheck(string actionPhase, States currentState)
        {
            if(rank >= 2) // May change to 3 if one pair isn't good enough?
            {
                // First player
                if (!this.Dealer)
                    return BTBet(actionPhase, currentState);
                // Second player
                else
                    return BTRaiseCall(actionPhase, currentState);
            }
            else
            {
                // First player
                if (!this.Dealer)
                {
                    // FIGURE OUT WHAT STATE WE'RE GOING TO GO TO!!!!!!!! //







                    Console.WriteLine(playerName + " chose the action: Check (First Player)");

                    return new PlayerAction(this.Name, actionPhase, "check", 0);
                }
                // Second player but first checked
                else if (this.actions[actions.Count - 1].ActionName == "check")
                {
                    stateRound1 = States.Evaluate; //Go back since the round is gonna end
                    // We should decided rather we should check or not 




                    Console.WriteLine(playerName + " chose the action: Check (Second Player)");

                    return new PlayerAction(this.Name, actionPhase, "check", 0);
                }
                // Second player and first didn't check
                else
                {
                    //Check to see if you should fold, or if you can keep playing
                    if (ShouldFold())
                    {
                        stateRound1 = States.Evaluate; //Go back since the round is gonna end
                        return new PlayerAction(this.Name, actionPhase, "fold", 0);
                    }
                    else
                    {
                        currentState = States.RaiseCall;
                        BTRaiseCall(actionPhase, currentState);
                    }
                }
            }


            stateRound1 = States.Evaluate;
            Console.WriteLine(playerName + " chose the action: <STUFF> (Not Coded yet)");
            return new PlayerAction(this.Name, "<STUFF>", "<OTHER STUFF>", -1);
        }

        private PlayerAction BTBet(string actionPhase, States currentState)
        {
            // CHECK TO SEE IF YOU SHOULD FOLD
            if (ShouldFold())
            {
                stateRound1 = States.Evaluate; //Go back since the round is gonna end
                return new PlayerAction(this.Name, actionPhase, "fold", 0);
            }
            else
            {
                // Update state
                stateRound1 = States.RaiseCall;
                // Bet according to table
                Console.WriteLine(playerName + " chose the action: Bet");
                return new PlayerAction(this.Name, actionPhase, "bet", bettingRangeTable[rank - 1, this.highCard.Value - 2]); // -1 and -2 to compensate for array indices
            }
        }

        private PlayerAction BTRaiseCall(string actionPhase, States currentState)
        {
            if(this.actions[actions.Count-1].ActionName == "raised" && currentBetPot > maxBet) //check if other opponent raised.
            {
                // Update state
                stateRound1 = States.Evaluate;
                Console.WriteLine(playerName + " chose the action:Fold (Opponent raised too high)");

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


                        stateRound1 = States.Evaluate;
                        Console.WriteLine(playerName + " chose the action: Raise " + numRaise);
                        return new PlayerAction(this.Name, actionPhase, "raise", (maxRaisesTable[rank])[1]);
                    }
                    else //no raises left
                    {
                        //call
                        stateRound1 = States.Evaluate;
                        Console.WriteLine(playerName + " chose the action: Call");
                        return new PlayerAction(this.Name, actionPhase, "call", this.actions[actions.Count - 1].Amount);
                    }
                }
                else
                {
                    stateRound1 = States.Evaluate;
                    Console.WriteLine(playerName + " chose the action: Call");
                    return new PlayerAction(this.Name, actionPhase, "call", this.actions[actions.Count - 1].Amount);
                }
            }

            
        }

        private bool ShouldFold()
        {
            // If you have rank of 1, all 4 suits, or distant numbers
            if (rank == 1)
            {
                // Evaluate the amount of suits in five cards
                int sameSuitNum = 0;
                for (int i = 0; i < 4; i++)
                {
                    for (int j = i + 1; j < 5; j++)
                    {
                        //if (i != j)
                        //{
                            if (hand[i].Suit == hand[j].Suit)    // all cards compare with the first card
                            {
                                sameSuitNum++;  // if this num less  then 2 then there will be 4 suils 
                            }
                        //}
                    }

                }
                bool badStraight = false;
                int goodStraight = 0;
                int numEach = 0;    // compare two cards value
                int numAll = 0;     // add up all the different between values when not fold 
                for (int i = 0; i < hand.Length - 1; i++)
                {
                    numEach = hand[i + 1].Value - hand[i].Value;
                    if (numEach > 2 && i != 0 && i != hand.Length-1) // the distance between cards are too big
                    {
                        badStraight = true;
                        break;
                    }
                    else
                    {
                        numAll += numEach;  // to check the total gap amount
                        if (numEach == 1)
                        {
                            ++goodStraight;
                            shouldDiscard[i] = shouldDiscard[i + 1] = false;
                        }
                    }
                }
                if (goodStraight == 3)
                {
                    badStraight = false;  //One away from a straight!
                    // Choose which card to discard
                    if (hand[hand.Length].Value - hand[hand.Length - 1].Value > 2) 
                        shouldDiscard[hand.Length] = true;
                    else
                        shouldDiscard[0] = true;
                }
                    //THINK ABOUT LATER -- SUITS... WHICH SHOULD WE TRY FOR?
                /*
                if (numAll > 2)
                {
                    badStraight = true; // at least two gap between cards number                   
                }
                */
                if (badStraight == true && sameSuitNum <= 2)
                {
                    Console.WriteLine(playerName + " chose the action: Fold (Really bad hand)");
                    return true;  // REALLY BAD TO HAVE NO CLOSE STRAIGHT AND SAME SUIT
                }

                // If the maxbet is bigger then safety 
                if (maxBet > safety)
                {
                    Console.WriteLine(playerName + " chose the action: Fold (Max Bet is bigger then safety");

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
            // Safety is a percentage of how much money you have... if you have more than 10% of starting money
            if (this.Money > (startMoney / 10))
                safety = (int)(this.Money * 0.35f);
            else safety = 0; // If you barely have any money left, just go for broke

            // Max bet is the most amount of money you'll bet before giving up
            // Proportional to the amount of money you have, and on your hand's rank
            maxBet = (rank / 10) * this.Money;
        }
        #endregion

        #region Abstract Overrides
        public override PlayerAction BettingRound1(List<PlayerAction> actions, Card[] hand)
        {
            // list the hand
            ListTheHand(hand);

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
                        return BTCheck("Bet1", stateRound1);
                    case States.Bet:
                        return BTBet("Bet1", stateRound1);
                    case States.RaiseCall:
                        return BTRaiseCall("Bet1", stateRound1);
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
                        ListTheHand(this.hand);//DEBUG--REMOVE LATER//
                        CalculateSafetyAndMaxBet();
                        stateRound2 = States.Bet;
                        break;
                    case States.Bet:
                        return BTBet("Bet2", stateRound2);
                    case States.RaiseCall:
                        return BTRaiseCall("Bet2", stateRound2);
                }
            }
        }

        /// <summary>
        /// DEBUG - DLETE THIS LATER
        /// </summary>
        private void ListTheHand(Card[] hand)
        {
            // evaluate the hand
            Card highCard = null;
            int rank = Evaluate.RateAHand(hand, out highCard);

            // list your hand
            Console.WriteLine("\nName: " + Name + " Your hand:   Rank: " + rank);
            for (int i = 0; i < hand.Length; i++)
            {
                Console.Write(hand[i].ToString() + " ");
            }
            Console.WriteLine();
        }

        public override PlayerAction Draw(Card[] hand)
        {
            //int[] cardDelete;
            ListTheHand(this.hand);//DEBUG--REMOVE LATER//
            // Consider high card / 1 pair case
            // Consult table to find cards to discard
            // Return basically nothing if none of those go through
            switch (rank)
            {
                case 1://HIGH CARD ONLY     // add more later
                    //cardDelete = new int[4];
                    for (int i = 0; i < 5; ++i)
                    {
                        hand[i] = null;
                        //cardDelete[i] = i+1;
                    }

                    //Console.WriteLine(playerName + "Delete 4 cards"+cardDelete[0]+ ","+ cardDelete[1]+ ","+ cardDelete[2]+ ","+ cardDelete[3]);
                    return new PlayerAction(Name, "Draw", "draw", 4);
                case 2://ONE PAIR
                    //cardDelete = new int[3];
                    for (int i = 0; i < 6; ++i)
                    {
                        if (hand[i].Value != hand[i + 1].Value)
                        {
                            hand[i] = null;
                            //cardDelete[i] = i + 1;
                        }
                    }

                    //Console.WriteLine(playerName + "Delete 3 cards" + cardDelete[0] + "," + cardDelete[1] + "," + cardDelete[2]);
                    return new PlayerAction(Name, "Draw", "draw", 3);
                case 3://TWO PAIR
                    //cardDelete = new int[2];
                    for (int i = 0; i < 5; ++i)
                    {
                        if (hand[i].Value != hand[i + 1].Value)
                        {
                            if (i > 0 && hand[i].Value != hand[i - 1].Value)
                            {
                                hand[i] = null;
                                //cardDelete[i] = i + 1;
                                break;
                            }
                        }
                    }
                    //Console.WriteLine(playerName + "Delete 2 cards" + cardDelete[0] + "," + cardDelete[1]);

                    return new PlayerAction(Name, "Draw", "draw", 1);
                    //THINK ABOUT THREE OF A KIND, FOUR, ETC
                default: //OTHER THINGS...
                    Console.WriteLine(playerName + "Delete 0 cards");
                    return new PlayerAction(Name, "Draw", "stand pat", 0);
            }
        }
        #endregion
    }
}
