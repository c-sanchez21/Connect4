using System;
using System.Collections.Generic;
using System.Text;

namespace Connect4
{
    class Program
    {
        const int SIZE = 8; //Size for the NxN board.
        const int maxdepth = 9; //Depth of search
        const int CONNECT = 4; //How many pieces need to be connected to win.
        static int[,] b = new int[SIZE, SIZE]; //Board Array
        static int maxTime = 0; //Max Time Allowed for computer turn.
        static DateTime startTime;

        static void Main(string[] args)
        {
            //Setup Board (Initailzie to zeros)
            Setup();            

            //Get Max Time from user
            maxTime = GetMaxTime();

            //Ask user who moves first
            bool computerMovesFirst = AIMovesFirst();
            
            if (computerMovesFirst)
                Makemove();//Computer makes first move
            Printboard();//Show board

            //Enter Game Loop
            while (true)
            {
                GetMove();
                if (CheckGameOver())
                    break;
                Console.WriteLine();
                startTime = DateTime.Now;
                Makemove();
                if (CheckGameOver())
                    break;
                Console.WriteLine();
            }
            Console.Write("Push any key to exit...");
            Console.ReadKey();
        }

        private static bool AIMovesFirst()//Ask user who moves first
        {
            Console.Write("Do you want to move first [Y/N]? ");
            ConsoleKey keyPressed = Console.ReadKey().Key;
            Console.WriteLine();
            while (keyPressed != ConsoleKey.Y && keyPressed != ConsoleKey.N)
            {
                Console.WriteLine("Invalid Key");
                Console.WriteLine();
                keyPressed = Console.ReadKey().Key;
            }
            return (keyPressed == ConsoleKey.N);
        }

        private static int GetMaxTime()//Get Max Time Allowed for AI Turn
        {
            int time = 0;
            bool invalidTime = true;
            while (invalidTime)
            {
                Console.Write("Max amount of time allowed for AI [1-30] in seconds: ");
                time = int.Parse(Console.ReadLine());
                invalidTime = (time < 1 || time > 30);
                if (invalidTime)
                    Console.WriteLine("Invalid Time");
            }
            return time;
        }

        private static void Printboard() //Show Game Board
        {
            //Print out first line of Numbers
            Console.Write("  ");
            for (int i = 1; i <= SIZE; i++)
                Console.Write(i.ToString() + " ");
            Console.WriteLine();
            
            char c = 'A';            
            int cVal = (int)'A';            

            for (int j = 0; j < SIZE; j++)
            {
                //Print Board
                Console.Write(c + " ");//Write Row Letter    
                for (int i = 0; i < SIZE; i++)
                {
                    if (b[j, i] == 2)
                        Console.Write("O ");
                    else if (b[j, i] == 1)
                        Console.Write("X ");
                    else Console.Write("- ");
                }
                Console.WriteLine();
                cVal++;
                c = (char)cVal;
            }
        }

        private static void Setup()//Initialize Board to Zeros
        {
            for (int i = 0; i < SIZE; i++)
                for (int j = 0; j < SIZE; j++)
                {
                    b[j, i] = 0;
                }
        }

        private static void GetMove() //Get Move from player
        {
            int i = 0, j = 0;
            bool invalidMove = true;
            while (invalidMove)
            {
                i = j = -1; 
                Console.Write("Enter your move: ");
                string line = Console.ReadLine().ToUpper();//Read the line and conver to uppercase
                string[] data;
                if (!String.IsNullOrEmpty(line))
                {
                    if (!line.Contains(" ") && line.Length > 1) //If move coordinates not seperated by spaces
                    {
                        data = new string[2];
                        data[0] = line.Substring(0, 1);
                        data[1] = line.Substring(1, 1);
                    }
                    else data = line.Split(' ');//Else In case move coordiantes seperated by spaces
                    j = (int)data[0].ToCharArray()[0] - (int)'A';
                    if (int.TryParse(data[1], out i))
                        i -= 1;
                    
                    //Check if coordinates inputed are in range.
                    if (j > -1 && j < SIZE && i > -1 && i < SIZE && b[j, i] == 0)
                        invalidMove = false;
                    else Console.WriteLine("Invalid Move");
                }
            }
            b[j, i] = 2;
        }

        private static int Evaluate()
        {
            int CompCount; 
            int PlayerCount;
            int blanks = 0;
            int sum = 0;
            for (int j = 0; j < SIZE; j++)
            {
                for (int i = 0; i < SIZE; i++)
                {

                    //Checks Board Horizontally For Patterns
                    //****************************************//
                    PlayerCount = 0;//Number of O's
                    CompCount = 0;//Number of X's
                    blanks = 0;//Number of Blanks

                    //Checks a chunk of the board
                    for (int x = i; x < CONNECT + i && x < SIZE;  x++)
                    {
                        if (b[j, x] == 2)
                            PlayerCount++;

                        if (b[j, x] == 1)
                            CompCount++;

                        if (b[j, x] == 0)
                            blanks++;
                    }

                    if (PlayerCount > CompCount)//Tends to make defending a higher priority
                        sum += (int)Math.Pow(10, PlayerCount + CompCount);
                    else if ((CompCount + blanks) == CONNECT) //Looks for optimal positions
                        sum += (int)Math.Pow(10, CompCount);
                    else if ((PlayerCount + blanks) == CONNECT)//Looks for disadvantageous positions
                        sum -= ((int)Math.Pow(10, PlayerCount) + 10 * PlayerCount);
                    //******************************************************//

                    //Same As Above but Checks Vertically
                    //***************************************************//
                    //TODO: Make code shorter by avoiding duplicate code...
                    //Only works when board size is n x n
                    PlayerCount = 0;
                    CompCount = 0;
                    blanks = 0;
                    for (int y = i; y < CONNECT + i && y < SIZE; y++)
                    {
                        if (b[y, j] == 2)
                            PlayerCount++;

                        if (b[y, j] == 1)
                            CompCount++;

                        if (b[y, j] == 0)
                            blanks++;
                    }
                    if (PlayerCount > CompCount)
                        sum += (int)Math.Pow(10, PlayerCount + CompCount);
                    else if ((CompCount + blanks) == CONNECT)
                        sum += (int)Math.Pow(10, CompCount);
                    else if ((PlayerCount + blanks) == CONNECT)
                        sum -= ((int)Math.Pow(10, PlayerCount) + (10 * PlayerCount));
                    //*******************************************************//
                }
            }
            return sum;
        }

        private static void Makemove() //AI 
        {
            int best = int.MinValue;
            int depth = maxdepth;
            int score, mi = 0, mj = 0;
            for (int j = 0; j < SIZE; j++)
                for (int i = 0; i < SIZE; i++)
                {
                    if (b[j, i] == 0)
                    {
                        b[j, i] = 1;//Make move;
                        score = Min(depth - 1,int.MinValue, int.MaxValue);
                        if (score > best)
                        {
                            mi = i;
                            mj = j;
                            best = score;
                        }
                        b[j, i] = 0; //Undo move;

                    }
                }
            int cVal = ((int)'A') + mj;
            Console.WriteLine("My move is " + (char)cVal + " " + (mi+1));
            b[mj, mi] = 1;
        }

        private static int Min(int depth, int alpha, int beta)
        {
            int v = int.MaxValue;
            if (Check4winner() != 0)  // Check for Terminal
                return (Check4winner());
            if (depth == 0 || (DateTime.Now - startTime).TotalSeconds >= maxTime) 
                return (Evaluate());
            for (int j = 0; j < SIZE; j++)
            {
                for (int i = 0; i < SIZE; i++)
                {
                    if (b[j,i] == 0)
                    {
                        b[j,i] = 2; // make move on board
                        v = Math.Min(v, Max(depth - 1, alpha, beta));
                        if (v <= alpha)
                        {
                            b[j, i] = 0;//undo move
                            return v;
                        }
                        beta = Math.Min(beta, v);                        
                        b[j,i] = 0; // undo move
                    }
                }
            }
            return (v);
        }

        private static int Max(int depth, int alpha, int beta)
        {
            int v = int.MinValue;
            if (Check4winner() != 0) 
                return (Check4winner());
            if (depth == 0 || (DateTime.Now - startTime).TotalSeconds >= maxTime) 
                return (Evaluate());
            for (int j = 0; j < SIZE; j++)
            {
                for (int i = 0; i < SIZE; i++)
                {
                    if (b[j,i] == 0)
                    {
                        b[j,i] = 1; // make move on board
                        v = Math.Max(v, Min(depth - 1, alpha, beta));
                        if (v >= beta)
                        {
                            b[j, i] = 0; //undo move
                            return v;
                        }
                        alpha = Math.Max(alpha, v);
                        b[j,i] = 0; // undo move
                    }
                }
            }
            return (v);
        }
        
        private static int Check4winner()
        {
            int emptySpacesCount = 0;
            for (int j = 0; j < SIZE; j++)
            {
                int XcomputerCount = 0;
                int XplayerCount = 0;

                int YcomputerCount = 0;
                int YplayerCount = 0;
                //Assumes NxN board else OutOfIndex crash
                for (int i = 0; i < SIZE; i++)
                {
                    //Check Horizontal
                    if (b[j, i] == 1)
                        XcomputerCount++;
                    else XcomputerCount = 0;

                    if (b[j, i] == 2)
                        XplayerCount++;
                    else XplayerCount = 0;

                    if (XcomputerCount == CONNECT)
                        return int.MaxValue;//Comptuer Wins;
                    if (XplayerCount == CONNECT)
                        return int.MinValue;//Player Wins;
                    
                    //Check Vertical
                    if (b[i, j] == 1)
                        YcomputerCount++;
                    else YcomputerCount = 0;

                    if (b[i, j] == 2)
                        YplayerCount++;
                    else YplayerCount = 0;

                    if (YcomputerCount == CONNECT)
                        return int.MaxValue;//Comptuer Wins;
                    if (YplayerCount == CONNECT)
                        return int.MinValue;//Player Wins;

                    if (b[i, j] == 0)
                        emptySpacesCount++;
                }
            }
            if(emptySpacesCount == 0)
                return 1; //Draw
            return 0; 
        }

        private static bool CheckGameOver()
        {
            Printboard();
            string s = "";
            bool gameOver = false;
            int winner = Check4winner();
            if (winner != 0)
            {
                gameOver = true;
                switch (winner)
                {
                    case int.MinValue:
                        s = "You win";
                        break;
                    case int.MaxValue:
                        s = "I win";
                        break;
                    case 1:
                        s = "Draw";
                        break;
                }
                Console.WriteLine(s);
            }
            return gameOver;
        } 
    }
}