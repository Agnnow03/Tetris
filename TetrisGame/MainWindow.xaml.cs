using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TetrisGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Threading.DispatcherTimer gameTimer = new System.Windows.Threading.DispatcherTimer();
        bool doneFalling = true;  //why true?
        int gamePoints=0;
        int rowPoint = 10;
        double maxElementsStackHeight = 100;
        FallingElementControl currentlyFalling=null;
        Label scoreText = new Label();
        const double squareSize=20;
        const double maxElementWidth = 4 * squareSize;
        const double maxElementHeight = maxElementWidth;
        int fallingSpeed = 300;
        public enum Turn { Left, Right, None};
        bool rotating = false;
        private Turn fallingDirection;
        

        List<FallingElementControl> fallingElements = new List<FallingElementControl>();

         private void NewFallingElement()
         {
            currentlyFalling = new FallingElementControl();
            fallingElements.Add(currentlyFalling);
            GameArea.Children.Add(currentlyFalling);
            Canvas.SetTop(currentlyFalling,currentlyFalling.Position.Y);
            Canvas.SetLeft(currentlyFalling,currentlyFalling.Position.X);
            doneFalling = false;
        }
       
        private void Rotate(FallingElementControl elementToRotate)
        {
            if (rotating)
            {
                rotating = false;
                if (((elementToRotate.Position.X + elementToRotate.SpawnContainer.Height) >= GameArea.Width)) {  return; };
                if ((elementToRotate.Position.Y + elementToRotate.SpawnContainer.Width >= GameArea.Height)) {  return; };
                TryToRotate(elementToRotate);

             /*   if (!RotationWillCollide(elementToCheck))
                {
                    Rotate(currentlyFalling);
                    
                    Canvas.SetTop(currentlyFalling, currentlyFalling.Position.Y);
                    Canvas.SetLeft(currentlyFalling, currentlyFalling.Position.X);
                }
               */

            }
        }
        private void TryToTurn()
        {
            if (((currentlyFalling.Position.X + currentlyFalling.SpawnContainer.Width) >= GameArea.Width) && (fallingDirection == Turn.Right)) fallingDirection = Turn.None;
            if ((currentlyFalling.Position.X <= 0) && (fallingDirection == Turn.Left)) fallingDirection = Turn.None;

        }
        private bool WillCollide(FallingElementControl elementToCheck)
        {

            if ((elementToCheck.Position.Y + elementToCheck.SpawnContainer.Height) >= (GameArea.Height))
            {
              //  doneFalling = true;
                return true;
            }

            TryToTurn();
          
            //clockwise rotation

            for (int i = 0; i < fallingElements.Count; i++)//CUrrentlyFalling
            {
                
                if (fallingElements[i] != elementToCheck)
                {
                   bool heightRangeCondition = ((elementToCheck.Position.Y + elementToCheck.SpawnContainer.Height) >= fallingElements[i].Position.Y) && (currentlyFalling.Position.Y <= (fallingElements[i].Position.Y + fallingElements[i].SpawnContainer.Height));
                   // bool rotationRangeForCollision = (currentlyFalling);
                   //bool rangeForAnyCollision = (elementToCheck.Position.Y +maxElementHeight >= fallingElements[i].Position.Y) && (currentlyFalling.Position.Y <= fallingElements[i].Position.Y+maxElementHeight);
                    //condition to check if there is a possible collision when falling/rotating
                    if (heightRangeCondition) {
                        //adding here a new condition
                        
                        foreach (Point subPosition in elementToCheck.SubPositions)
                       {
                        double subPositionX = subPosition.X + elementToCheck.Position.X;
                        double subPositionY = subPosition.Y + elementToCheck.Position.Y;

                            
                            if ((subPositionY + squareSize) >= fallingElements[i].Position.Y)
                            {
                                foreach (Point fallenSubPosition in fallingElements[i].SubPositions)
                                {

                                    double fallenSubPositionX = fallenSubPosition.X + fallingElements[i].Position.X;
                                    double fallenSubPositionY = fallenSubPosition.Y + fallingElements[i].Position.Y;
                                    
                                    bool CanNotTurnRightCondition = ((subPositionX + squareSize) == fallenSubPositionX);
                                    bool CanNotTurnLeftCondition = subPositionX == (fallenSubPositionX + squareSize);

                                    if (CanNotTurnLeftCondition) { fallingDirection = Turn.None; }
                                    if (CanNotTurnRightCondition) { fallingDirection = Turn.None; }
                                    //if (heightRangeCondition)
                                    //{
                                        if ((fallenSubPositionX == subPositionX) && (((subPositionY + squareSize) == fallenSubPositionY) || (subPositionY == fallenSubPositionY)))
                                        {
                                           // doneFalling = true;
                                            return true;

                                        }

                                        //height condition
                                       
                                   // }
                                }
                            }
                        }
                    }
                }

            }
            return false;
        }

        private bool RotationWillCollide(FallingElementControl elementToCheck, List<Point> rotatedSubList)
        {
            double elementX = elementToCheck.Position.X;
            double elementY= elementToCheck.Position.Y;
            double rotatedHeight = elementToCheck.SpawnContainer.Width; 
            double rotatedWidth = elementToCheck.SpawnContainer.Height;
            
            for (int i = 0; i < fallingElements.Count; i++)
            {
                if (fallingElements[i] != elementToCheck)
                {
                    bool heightRangeCondition = (elementY + rotatedHeight > fallingElements[i].Position.Y)
                        && (elementY < fallingElements[i].Position.Y + fallingElements[i].SpawnContainer.Height);
                    bool widthRangeCondition = (elementX + rotatedWidth > fallingElements[i].Position.X)
                        && (elementX < fallingElements[i].Position.X + fallingElements[i].SpawnContainer.Width);
                    if (heightRangeCondition && widthRangeCondition)
                    {
                        foreach (Point subPosition in rotatedSubList)
                        {
                            double subPositionX = subPosition.X + elementX;
                            double subPositionY = subPosition.Y + elementY;

                                foreach (Point fallenSubPosition in fallingElements[i].SubPositions)// podelementy reszty elementów
                                {

                                    double fallenSubPositionX = fallenSubPosition.X + fallingElements[i].Position.X;
                                    double fallenSubPositionY = fallenSubPosition.Y + fallingElements[i].Position.Y;
                                    if((fallenSubPositionX ==subPositionX)&&(fallenSubPositionY == subPositionY))
                                    {
                                        return true;
                                    }
                                   
                                }
                            
                        }
                    }
                }

            }
            return false;
        }
        private bool EndGameCondition(FallingElementControl elementToCheck)
        {
            if ((elementToCheck!=null)&&(elementToCheck.Position.Y <= maxElementsStackHeight))
            {
                gameTimer.IsEnabled = false;
                Label gameOverText = new Label();
                gameOverText.Content = "Game over";
                gameOverText.FontSize = 30;
               
                GameArea.Children.Add(gameOverText);
                Canvas.SetTop(gameOverText, 20);
                Canvas.SetLeft(gameOverText, 100);
                return true;
            }
            else return false;
        }
       
        private bool MoveFallingElement()
        {
            if (WillCollide(currentlyFalling)) {
                doneFalling = true;
                return false;
                
            }
        
            double nextY = currentlyFalling.Position.Y;
            nextY += squareSize;
           
            double nextX = currentlyFalling.Position.X;

            switch (fallingDirection)
            {
                case Turn.Left:  //ADD IF here                                                                           
                    nextX -= squareSize;
                    break;

                case Turn.Right: //ADD IF HERE                                                                          
                    nextX += squareSize;
                    break;

                case Turn.None:
                    break;
            }

            currentlyFalling.Position = new Point(nextX, nextY);
            Canvas.SetTop(currentlyFalling, currentlyFalling.Position.Y);
            Canvas.SetLeft(currentlyFalling, currentlyFalling.Position.X);

            fallingDirection = Turn.None;
            return true;
        }
        private void ClearRow(double yCoordinate)
        {
            gamePoints += rowPoint;
            scoreText.Content =  "Score:" + gamePoints.ToString();
            List<FallingElementControl> aboveElements = new List<FallingElementControl>();
            //for storing elements that will fall down after deleting a row
            for (int i= 0; i < fallingElements.Count;i++)
            {
                double elementY = fallingElements[i].Position.Y;
                
                if (elementY <= yCoordinate)
                {
                   //bool removed = false;
                    //only checking elements that are in range of Y coordinate
                    if ((elementY + fallingElements[i].SpawnContainer.Height > yCoordinate))
                    {
                        
                        for (int j = 0; j < fallingElements[i].SubPositions.Count; j++)
                        {

                            double subBlockY = fallingElements[i].SubPositions[j].Y + fallingElements[i].Position.Y;

                            if (subBlockY == yCoordinate)
                            {
                               // removed = true;
                                fallingElements[i].SubPositions.RemoveAt(j);
                             //    for (int k = 0; k < fallingElements[i].SubPositions.Count; k++)
                              //  {
                                //  Point temp = fallingElements[i].SubPositions[k];
                                // if (temp.Y + fallingElements[i].Position.Y > yCoordinate)
                                //only moving up squares that are lower
                                // {
                                //  fallingElements[i].SubPositions[k] = new Point(temp.X, temp.Y - squareSize);

                              //   }
                              //  }

                            }
                        }
                        for (int j = 0; j < fallingElements[i].SubPositions.Count; j++)
                        {
                            double subBlockY = fallingElements[i].SubPositions[j].Y + fallingElements[i].Position.Y;

                            if (subBlockY > yCoordinate)
                            {
                                fallingElements[i].SubPositions[j] = new Point(fallingElements[i].SubPositions[j].X, fallingElements[i].SubPositions[j].Y - squareSize);
                            }
                        }

                        if (fallingElements[i].SubPositions.Count == 0)
                        {
                            fallingElements.RemoveAt(i);
                        }
                        else // if(removed)
                        {
                            fallingElements[i].SpawnContainer.Height -= squareSize;
                            fallingElements[i].Position = new Point(fallingElements[i].Position.X, fallingElements[i].Position.Y+squareSize);
                            Canvas.SetTop(fallingElements[i], fallingElements[i].Position.Y);
                            Canvas.SetLeft(fallingElements[i], fallingElements[i].Position.X);
                        }
                        //else
                        //{
                          //  aboveElements.Add(fallingElements[i]);
                       // }
                        

                    }
                    else
                    {
                        aboveElements.Add(fallingElements[i]);
                    }
                    
                   //if (!removed)
                   //{
                        //fallingElements[i].Position = new Point(fallingElements[i].Position.X, fallingElements[i].Position.Y + squareSize);
                        
                     //   Canvas.SetTop(fallingElements[i], fallingElements[i].Position.Y);
                       // Canvas.SetLeft(fallingElements[i], fallingElements[i].Position.X);
                    //}
                }
  
            }
            foreach(FallingElementControl x in aboveElements)
            {
                currentlyFalling = x;
                doneFalling = false;
                while (!doneFalling)
                {
                MoveFallingElement();
                }
               // EndGameCondition(currentlyFalling);
                currentlyFalling = null;
               // x.Position = new Point(x.Position.X, x.Position.Y + squareSize);

                //Canvas.SetTop(x, x.Position.Y);
                //Canvas.SetLeft(x, x.Position.X);
            }

        }
        private void TryToRotate(FallingElementControl elementToRotate)//correct to make it rotate right
        {
         
           List<Point> rotatedSubElements = new List<Point>();
            for(int i = 0; i < elementToRotate.SubPositions.Count; i++)
            { 
                double nextX = -elementToRotate.SubPositions[i].Y + elementToRotate.SpawnContainer.Height-squareSize;
                double nextY = elementToRotate.SubPositions[i].X;
               
               rotatedSubElements.Add(new Point(nextX, nextY));
             
            }//calculating X,Y for rotation

            if (!RotationWillCollide(elementToRotate,rotatedSubElements))
               {

                elementToRotate.SubPositions = rotatedSubElements;
                elementToRotate.RepositionSquares();
                elementToRotate.CorrectWidthHeight();
                Canvas.SetTop(elementToRotate, elementToRotate.Position.Y);
                Canvas.SetLeft(elementToRotate, elementToRotate.Position.X);
               }
            

          //  elementToRotate.SpawnContainer.Height = elementToRotate.SpawnContainer.Width;
          //  elementToRotate.SpawnContainer.Width = elementToRotate.SpawnContainer.Height;



        }
        private void RowCompleteCheck()
        {
            Dictionary<double, double> rows = new Dictionary<double, double>();
            //dictionary of y=row and sum of all widths of squares
            for (int i = 0; i < fallingElements.Count; i++)
            {
                for(int j=0; j < fallingElements[i].SubPositions.Count; j++)
                {
                    double tempY = fallingElements[i].SubPositions[j].Y + fallingElements[i].Position.Y;
                    if (rows.ContainsKey(tempY))
                    {
                        rows[tempY] += squareSize;
                        
                    }
                    else
                    {
                        rows.Add(tempY, squareSize);//adds a record of a row (of square blocks)
                    }
                }

            }
            foreach (double key in rows.Keys)//checking if one of the rows is full
            {
                if (rows[key] >= GameArea.Width)
                {
                    ClearRow(key);
                    rows[key] = 0;
                }
            }


        }
        private void StartNewGame()
        {
            gameTimer.Interval = TimeSpan.FromMilliseconds(fallingSpeed);
            
            scoreText.Content = "Score:"+ gamePoints.ToString();
            scoreText.FontSize = 20;
          
            GameArea.Children.Add(scoreText);
            Canvas.SetTop(scoreText, 20);
            Canvas.SetLeft(scoreText, 260);

        }
        FallingElementControl Test1 = new FallingElementControl();
        
        private void gameTimer_Tick(object sender, EventArgs e)
        {
            if (doneFalling)
            {
                if(EndGameCondition(currentlyFalling))
                {
                    return;
                }
                else
                {
                    RowCompleteCheck();
                    NewFallingElement();
                }
               
            }
            if (MoveFallingElement())
            {
                Rotate(currentlyFalling);
            }
         


        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            StartNewGame();
            gameTimer.IsEnabled = true;
        }
        private void Window_OnKeyDownEvent(object sender, KeyEventArgs e)
        {
            if (currentlyFalling == null) return;
            else if(e.Key == Key.A){
                fallingDirection = Turn.Left;
                
            }
            else if(e.Key == Key.D){
                fallingDirection = Turn.Right;
                
            }
            else if(e.Key == Key.R)
            {
                rotating = true;
            }
            else
            {
                return;
            }

            if (MoveFallingElement())
            {
                Rotate(currentlyFalling);
            }
            
            
        }
        
        public MainWindow()
        {
            InitializeComponent();
            //temporary
            gameTimer.Tick += gameTimer_Tick;
        }
    }
}
