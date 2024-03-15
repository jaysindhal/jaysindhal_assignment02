using System;

namespace GemHuntersGame
{
    class GemHuntersGame
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Gem Hunters!");

            Game game = new Game();
            game.Start();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }

    class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    class Player
    {
        public string Name { get; }
        public Position Position { get; set; }
        public int GemCount { get; set; }

        public Player(string name, Position position)
        {
            Name = name;
            Position = position;
            GemCount = 0;
        }

        public void Move(char direction)
        {
            if (direction == 'U' && Position.Y > 0)
                Position = new Position(Position.X, Position.Y - 1);
            else if (direction == 'D' && Position.Y < 5)
                Position = new Position(Position.X, Position.Y + 1);
            else if (direction == 'L' && Position.X > 0)
                Position = new Position(Position.X - 1, Position.Y);
            else if (direction == 'R' && Position.X < 5)
                Position = new Position(Position.X + 1, Position.Y);
        }
    }

    class Cell
    {
        public string Occupant { get; set; }

        public Cell(string occupant = "-")
        {
            Occupant = occupant;
        }
    }

    class Board
    {
        public Cell[,] Grid { get; }
        private Random random;

        public Board()
        {
            Grid = new Cell[6, 6];
            random = new Random();

            // Initialize the grid with empty cells
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    Grid[i, j] = new Cell();
                }
            }

            // Place gems (G)
            for (int i = 0; i < 6; i++)
            {
                int gemX = random.Next(6);
                int gemY = random.Next(6);
                Grid[gemY, gemX].Occupant = "G";
            }

            // Place obstacles (O)
            for (int i = 0; i < 6; i++)
            {
                int obstacleX = random.Next(6);
                int obstacleY = random.Next(6);
                if (Grid[obstacleY, obstacleX].Occupant != "G")
                {
                    Grid[obstacleY, obstacleX].Occupant = "O";
                }
                else
                {
                    i--;
                }
            }
        }

        public void Display(Player player1, Player player2)
        {
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (player1.Position.X == j && player1.Position.Y == i)
                        Console.Write("P1 ");
                    else if (player2.Position.X == j && player2.Position.Y == i)
                        Console.Write("P2 ");
                    else
                        Console.Write(Grid[i, j].Occupant + " ");
                }
                Console.WriteLine();
            }
        }

        public bool IsValidMove(Player player, char direction)
        {
            int x = player.Position.X;
            int y = player.Position.Y;
            if (direction == 'U')
                y--;
            else if (direction == 'D')
                y++;
            else if (direction == 'L')
                x--;
            else if (direction == 'R')
                x++;

            if (x < 0 || x >= 6 || y < 0 || y >= 6)
                return false;
            if (Grid[y, x].Occupant == "O")
                return false; // Cannot move onto a square with an obstacle
            return true;
        }

        public void CollectGem(Player player)
        {
            int x = player.Position.X;
            int y = player.Position.Y;
            if (Grid[y, x].Occupant == "G")
            {
                player.GemCount++;
                Grid[y, x].Occupant = "-";
                Console.WriteLine($"{player.Name} collected a gem!");
            }
        }
    }

    class Game
    {
        public Board Board { get; }
        public Player Player1 { get; }
        public Player Player2 { get; }
        public Player CurrentTurn { get; set; }
        public int TotalTurns { get; set; }

        public Game()
        {
            Board = new Board();
            Player1 = new Player("P1", new Position(0, 0));
            Player2 = new Player("P2", new Position(5, 5));
            CurrentTurn = Player1;
            TotalTurns = 0;
        }

        public void Start()
        {
            while (!IsGameOver())
            {
                Board.Display(Player1, Player2);
                Console.WriteLine($"{CurrentTurn.Name}'s turn.");
                Console.WriteLine("Enter direction (U/D/L/R): ");
                char direction = ' '; // Initialize direction with a default value
                bool isValidInput = false;

                // Keep asking for input until a valid direction is entered
                while (!isValidInput)
                {
                    string input = Console.ReadLine();
                    if (input.Length == 1)
                    {
                        direction = char.ToUpper(input[0]); // Convert to uppercase
                        if (direction == 'U' || direction == 'D' || direction == 'L' || direction == 'R')
                            isValidInput = true;
                        else
                            Console.WriteLine("Invalid input. Please enter U, D, L, or R.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter a single character (U, D, L, or R).");
                    }
                }

                if (Board.IsValidMove(CurrentTurn, direction))
                {
                    Position previousPosition = new Position(CurrentTurn.Position.X, CurrentTurn.Position.Y);
                    CurrentTurn.Move(direction); // Update player's position
                    Board.CollectGem(CurrentTurn);

                    // Update grid with new player position
                    Board.Grid[previousPosition.Y, previousPosition.X].Occupant = "-";
                    if (CurrentTurn == Player1)
                        Board.Grid[CurrentTurn.Position.Y, CurrentTurn.Position.X].Occupant = "P1";
                    else
                        Board.Grid[CurrentTurn.Position.Y, CurrentTurn.Position.X].Occupant = "P2";

                    TotalTurns++;
                    SwitchTurn();
                }
                else
                {
                    Console.WriteLine("Invalid move. Please try again.");
                }
            }

            AnnounceWinner();
        }

        public void SwitchTurn()
        {
            CurrentTurn = (CurrentTurn == Player1) ? Player2 : Player1;
        }

        public bool IsGameOver()
        {
            return TotalTurns >= 30;
        }

        public void AnnounceWinner()
        {
            Board.Display(Player1, Player2);
            if (Player1.GemCount > Player2.GemCount)
                Console.WriteLine("Player 1 wins!");
            else if (Player1.GemCount < Player2.GemCount)
                Console.WriteLine("Player 2 wins!");
            else
                Console.WriteLine("It's a tie!");
        }
    }
}