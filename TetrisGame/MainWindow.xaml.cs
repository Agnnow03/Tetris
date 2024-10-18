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
       // private void TryToRotate()
      //  {
            
       // }
        private void TryToRotate(FallingElementControl elementToRotate)
        {
            if (rotating)
            {
                rotating = false;
                if (((currentlyFalling.Position.X + currentlyFalling.SpawnContainer.Height) >= GameArea.Width)) {  return; };
                if ((currentlyFalling.Position.Y + currentlyFalling.SpawnContainer.Width >= GameArea.Height)) {  return; };

                FallingElementControl elementToCheck = elementToRotate;
                Rotate(elementToCheck);
                if (!WillCollide(elementToCheck))
                {
                    currentlyFalling = elementToCheck;
                    currentlyFalling.RepositionSquares();
                    Canvas.SetTop(currentlyFalling, currentlyFalling.Position.Y);
                    Canvas.SetLeft(currentlyFalling, currentlyFalling.Position.X);
                }

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
                   bool rangeForAnyCollision = (elementToCheck.Position.Y +maxElementHeight >= fallingElements[i].Position.Y) && (currentlyFalling.Position.Y <= fallingElements[i].Position.Y+maxElementHeight);
                    //condition to check if there is a possible collision when falling/rotating
                    if (rangeForAnyCollision) {
                        //adding here a new condition
                        
                        foreach (Point subPosition in elementToCheck.SubPositions)
                       {
                        double subPositionX = subPosition.X + elementToCheck.Position.X;
                        double subPositionY = subPosition.Y + elementToCheck.Position.Y;

                            //TryToRotate(subPosition);
                            if ((subPositionY + squareSize) >= fallingElements[i].Position.Y)
                            {
                                foreach (Point fallenSubPosition in fallingElements[i].SubPositions)
                                {

                                    double fallenSubPositionX = fallenSubPosition.X + fallingElements[i].Position.X;
                                    double fallenSubPositionY = fallenSubPosition.Y + fallingElements[i].Position.Y;
                                    bool CanNotTurnRightCondition = ((elementToCheck.Position.X + elementToCheck.SpawnContainer.Width) == fallingElements[i].Position.X);

                                    bool CanNotTurnLeftCondition = elementToCheck.Position.X == (fallingElements[i].Position.X + fallingElements[i].SpawnContainer.Width);

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
        private void EndGameCondition(FallingElementControl elementToCheck)//niezaimplementowane
        {//add condition to end
            if (elementToCheck.Position.Y <= maxElementsStackHeight)
            {
                    gameTimer.IsEnabled = false;
                TextBox gameOverText = new TextBox();
                gameOverText.Text = gamePoints.ToString();
                gameOverText.FontSize = 30;
                gameOverText.IsReadOnly = true;
                gameOverText.BorderBrush = Brushes.Transparent;
                //BorderStyle = System.Windows.Forms.BorderStyle.None;
                GameArea.Children.Add(gameOverText);
                Canvas.SetTop(gameOverText, 20);
                Canvas.SetLeft(gameOverText, 100);
            }
        }
       
        private void MoveFallingElement()
        {
            //TESTOWO!!!!
          
            if (WillCollide(currentlyFalling)) {
                doneFalling = true;
                return;
                
            }
           
            TryToRotate(currentlyFalling);
            // if (doneFalling)
            //{
            //   return;
            // }
            double nextY = currentlyFalling.Position.Y;
            nextY += squareSize;
            //moving, after checking conditions
            double nextX = currentlyFalling.Position.X;

            // if (rotating)
            // {
            //     Rotate(currentlyFalling);
            // }



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

        }
        private void ClearRow(double yCoordinate)
        {
            gamePoints += rowPoint;
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
        private void Rotate(FallingElementControl elementToRotate)//correct to make it rotate right
        {
         
            //  double nextX=0;
            // double nextY=0;
            // double correctPosition = 0;
            //foreach (Point subElement in elementToRotate.SubPositions)
            for(int i = 0; i < elementToRotate.SubPositions.Count; i++)
            { 
                double nextX = -elementToRotate.SubPositions[i].Y + elementToRotate.SpawnContainer.Height-squareSize;
                double nextY = elementToRotate.SubPositions[i].X;
               
                elementToRotate.SubPositions[i] = new Point(nextX, nextY);
              /*  if (subElement.X > correctPosition)
                {
                    correctPosition = subElement.X;
                }*/
            }
            /* if (correctPosition > 0)
             {
                 for (int i=0;i<elementToRotate.SubPositions.Count;i++)
                 {
                     nextY += correctPosition;
                     elementToRotate.SubPositions[i] = new Point(nextX, nextY);
                 }
             }*/
            elementToRotate.CorrectWidthHeight();
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
                }
            }


        }
        private void StartNewGame()
        {
            gameTimer.Interval = TimeSpan.FromMilliseconds(fallingSpeed);
            TextBox scoreBox = new TextBox();
            scoreBox.Text = "Score:"+ gamePoints.ToString();
            scoreBox.FontSize = 20;
            scoreBox.IsReadOnly = true;
            scoreBox.BorderBrush = Brushes.Transparent;
            GameArea.Children.Add(scoreBox);
            Canvas.SetTop(scoreBox, 20);
            Canvas.SetLeft(scoreBox, 260);

        }
        FallingElementControl Test1 = new FallingElementControl();
        
        private void gameTimer_Tick(object sender, EventArgs e)
        {
            if (doneFalling)
            {
                if (currentlyFalling != null)
                {
                    EndGameCondition(currentlyFalling);
                }
                RowCompleteCheck();
                NewFallingElement();
            }

          

            MoveFallingElement();

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
            
            MoveFallingElement();
            
            
        }
        
        public MainWindow()
        {
            InitializeComponent();
            //temporary
            gameTimer.Tick += gameTimer_Tick;
        }
    }
}
