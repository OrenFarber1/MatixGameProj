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

        public int SetGameAction(string email, int row, int column)
        {        

            // Validate user and directions 
            if(currentTurn == GameTurnType.HorizontalPlayer)
            {
                if(GetHorizontalPlayerEmail() != email || currentToken.Row != row)
                {
                    logger.ErrorFormat("SetGameAction Error - email: {0}, row: {1}, column: {2}", email, row, column);
                    throw new Invalid​Operation​Exception("invalid Set Game Action");
                }

                
            }
            else if (currentTurn == GameTurnType.VerticalPlayer)
            {                
                if (GetVerticalPlayerEmail() != email || currentToken.Column != column)
                {
                    logger.ErrorFormat("SetGameAction Error - email: {0}, row: {1}, column: {2}", email, row, column);
                    throw new Invalid​Operation​Exception("invalid Set Game Action");
                }
            }

            // It is valid so get the cell
            MatixCell c = board.MatixCells[row][column];

            // Set the new token
            currentToken.Token = false;
            c.Token = true;
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

            // Return the cell value
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
            const int BOARD_SIZE = 8;
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

    }
}
