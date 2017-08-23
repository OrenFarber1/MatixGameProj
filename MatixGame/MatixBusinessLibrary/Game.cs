using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WcfMatixServiceLibrary;

namespace MatixBusinessLibrary
{
    /// <summary>
    /// Game turn types 
    /// </summary>
    enum GameTurnType        
    {
        HorizontalPlayer,
        VerticalPlayer
    }


    /// <summary>
    /// The class Game encapsulates a game properties 
    /// </summary>
    class Game
    {
        /// <summary>
        /// Class internal logger 
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The size ...
        /// </summary>
        const int BOARD_SIZE = 8;

        /// <summary>
        /// The horizontal player
        /// </summary>
        private Player horisontalPlayer;

        /// <summary>
        /// The vertical player
        /// </summary>
        private Player verticalPlayer;

        /// <summary>
        /// Whose turn is it        
        /// </summary>
        GameTurnType currentTurn;

        /// <summary>
        /// The game board 
        /// </summary>
        MatixBoard board;

        /// <summary>
        /// Reference to the current token cell.  
        /// </summary>
        MatixCell currentToken;

        /// <summary>
        /// 
        /// </summary>
        Random random = new Random();

        /// <summary>
        /// The id of the game in the database so we can use it for updates
        /// </summary>
        long gameDatabaseId = -1;

        /// <summary>
        /// Construct a game and choose randomly who is the horizontal and vertical players and who should start the game 
        /// in case the second player is a robot the first one will start the game
        /// </summary>
        /// <param name="firstEmail"></param>
        /// <param name="firstNickname"></param>
        /// <param name="secondEmail"></param>
        /// <param name="secondNickname"></param>
        public Game(string firstEmail, string firstNickname,  PlayerType firstType, string secondEmail, string secondNickname, PlayerType secondType)
        {
            if(GenerateRandomTurn() == GameTurnType.HorizontalPlayer)
            {
                horisontalPlayer = new Player(firstEmail, firstNickname, firstType);
                verticalPlayer = new Player(secondEmail, secondNickname, secondType);
                
                if(secondType == PlayerType.Robot)
                {
                    currentTurn = GameTurnType.HorizontalPlayer;
                }
            }
            else
            {
                horisontalPlayer = new Player(secondEmail, secondNickname, secondType);
                verticalPlayer = new Player(firstEmail, firstNickname, firstType);

                if (secondType == PlayerType.Robot)
                {
                    currentTurn = GameTurnType.VerticalPlayer;
                }
            }
            
            // Generate a new board
            board = GenerateNewBoard(out currentToken);

            if (secondType == PlayerType.Human)
            {
                // Set who starts the game
                SetWhoStartsTheGameRandomly();
            }
        }

        /// <summary>
        /// Get the current board as xml string
        /// </summary>
        /// <returns>xml result</returns>
        public string GetBoardXml()
        {
            logger.Info("GetBoardXml");

            return SerializeMatixBoard(board);
        }

        /// <summary>
        /// Get whose turn is it horizontal or vertical
        /// </summary>
        /// <returns>GameTurnType</returns>
        public GameTurnType GetWhoseTurnIsIt()
        {
            return currentTurn;
        }

        public MatixCell GetCurrentToken()
        {
            return currentToken;
        }

        /// <summary>
        /// Get horizontal player email
        /// </summary>
        /// <returns></returns>
        public string GetHorizontalPlayerEmail()
        {
            return horisontalPlayer.GetEmail();
        }

        public string GetHorizontalNickname()
        {
            return horisontalPlayer.GetNickname();
        }

        public PlayerType GetHorizontalPlayerType()
        {
            return horisontalPlayer.GetPlayerType();
        }

        /// <summary>
        /// Get vertical player email
        /// </summary>
        /// <returns></returns>
        public string GetVerticalPlayerEmail()
        {
            return verticalPlayer.GetEmail();
        }

        /// <summary>
        /// Get vertical players nickname
        /// </summary>
        /// <returns></returns>
        public string GetVerticalPlayerNickname()
        {
            return verticalPlayer.GetNickname();
        }

        public PlayerType GetVerticalPlayerType()
        {
            return verticalPlayer.GetPlayerType();
        }

        /// <summary>
        /// Get game board
        /// </summary>
        /// <returns></returns>
        public MatixBoard GetMatixBoard()
        {   
            return board;
        }

        public long GameId
        {
            get
            {
                return gameDatabaseId;
            }
            set
            {
                gameDatabaseId = value;              
            }
        }
        
        /// <summary>
        /// Update the game with the player move on the board 
        /// </summary>
        /// <param name="email">The player's email</param>
        /// <param name="row">The selected token row index</param>
        /// <param name="column">the selected token column index</param>
        /// <returns>The player updated score</returns>
        public int SetGameAction(string email, int row, int column)
        {
            logger.InfoFormat("SetGameAction email: {0}, row: {1}, column: {2}", email, row, column);

            // Validate user and directions 
            if (currentTurn == GameTurnType.HorizontalPlayer)
            {
                if(GetHorizontalPlayerEmail() != email || currentToken.Row != row)
                {
                    logger.ErrorFormat("SetGameAction Error Invalid Set Game Action - email: {0}, row: {1}, column: {2}", email, row, column);
                    throw new Invalid​Operation​Exception("Invalid Set Game Action");
                }

                logger.InfoFormat("SetGameAction email: {0} - HorizontalPlayer ", email);

            }
            else if (currentTurn == GameTurnType.VerticalPlayer)
            {                
                if (GetVerticalPlayerEmail() != email || currentToken.Column != column)
                {
                    logger.ErrorFormat("SetGameAction Error Invalid Set Game Action - email: {0}, row: {1}, column: {2}", email, row, column);
                    throw new Invalid​Operation​Exception("Invalid Set Game Action");
                }

                logger.InfoFormat("SetGameAction email: {0} - VerticalPlayer ", email);

            }

            // It is valid so get the cell
            MatixCell c = board.MatixCells[row][column];

            // Set the new token
            currentToken.Token = false;
            c.Token = true;
            c.Used = true;
            currentToken = c;

            int score = 0;
            if (currentTurn == GameTurnType.HorizontalPlayer)
            {
                score = horisontalPlayer.UpdateScore(currentToken.Value);
            }
            else if (currentTurn == GameTurnType.VerticalPlayer)
            {
                score = verticalPlayer.UpdateScore(currentToken.Value);
            }

            // Return the player score 
            return score;
        }

        /// <summary>
        /// Get randomly a turn type
        /// </summary>
        /// <returns>Generated GameTurnType</returns>
        private GameTurnType GenerateRandomTurn()
        {
            int value = random.Next(0, 10);

            logger.InfoFormat("GenerateRandomTurn generated value: {0}", value);
            
            if (value % 2 != 0)
            {
                return GameTurnType.HorizontalPlayer;
            }
            else
            {
                return GameTurnType.VerticalPlayer;
            }
        }



        /// <summary>
        /// Generate how's player start the game  
        /// </summary>
        private void SetWhoStartsTheGameRandomly()
        {         
            int value = random.Next(0, 10);

            logger.InfoFormat("SetWhoStartsTheGameRandomly generated value: {0}", value);

            if (value % 2 != 0)
            {
                currentTurn = GameTurnType.HorizontalPlayer;
            }
            else
            {
                currentTurn = GameTurnType.VerticalPlayer;
            }
        }

        public GameTurnType ChangeCurrentTurn()
        {
            if (currentTurn == GameTurnType.VerticalPlayer)
            {
                currentTurn = GameTurnType.HorizontalPlayer;
            }
            else
            {
                currentTurn = GameTurnType.VerticalPlayer;
            }

            return currentTurn;
        }

        /// <summary>
        /// Generate a random board
        /// </summary>
        /// <returns>The new generated board</returns>
        private MatixBoard GenerateNewBoard(out MatixCell currentToken)
        {
     
            MatixBoard board = new MatixBoard();
            Random randNum = new Random();

            // Clear it 
            currentToken = null;

            int tokenIndex = randNum.Next(0, BOARD_SIZE * BOARD_SIZE);
            int cellCounter = 0;

            board.MatixCells = new List<List<MatixCell>>();
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                List<MatixCell> columns = new List<MatixCell>();
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    MatixCell c = new MatixCell();
                    c.Row = i;
                    c.Column = j;
                    c.Value = randNum.Next(-15, 15 + 1); 

                    if (cellCounter == tokenIndex)
                    {
                        c.Token = true;
                        c.Used = true;

                        // Save the current token 
                        currentToken = c;
                    }
                    else
                    {
                        c.Token = false;
                    }

                    cellCounter++;
                    columns.Add(c);
                }
                board.MatixCells.Add(columns);
            }

            return board;
        }


        /// <summary>
        /// Serialize MatixBoard into an XML string
        /// </summary>
        /// <param name="matixBoard"></param>
        /// <returns>XML formated string</returns>
        private string SerializeMatixBoard(MatixBoard matixBoard)
        {
            XmlSerializer s = new XmlSerializer(typeof(MatixBoard));
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            s.Serialize(sw, matixBoard);
            sw.Close();
            return sb.ToString();
        }


        public List<MatixCell> GetRowOfCells(int row)
        {
            List<MatixCell> list = new List<MatixCell>();

            for (int i = 0; i < BOARD_SIZE; i++)
            {
                MatixCell c = board.MatixCells[row][i];
                if (!c.Used && !c.Token)
                {
                    MatixCell clone = new MatixCell(c);
                    list.Add(clone);
                }
            }
            
            return list;
        }

        /// <summary>
        /// Get a list of MatixCells from the game board 
        /// </summary>
        /// <param name="column">The column of the requested cells</param>
        /// <returns></returns>
        public List<MatixCell> GetColumnOfCells(int column)
        {
            List<MatixCell> list = new List<MatixCell>();

            for (int i = 0; i < BOARD_SIZE; i++)
            {
                MatixCell c = board.MatixCells[i][column];
                if (!c.Used && !c.Token)
                {
                    MatixCell clone = new MatixCell(c);
                    list.Add(clone);
                }
            }
            
            return list;
        }
    }
}
