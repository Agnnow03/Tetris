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
        FallingElementControl currentlyFalling;
        double squareSize=20;
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
        private void TryToRotate()
        {
            if (((currentlyFalling.Position.X + currentlyFalling.SpawnContainer.Height) >= GameArea.Width) &&(rotating==true)) rotating=false;
            if ((currentlyFalling.Position.Y+currentlyFalling.SpawnContainer.Width >=GameArea.Height) && (rotating == true)) rotating = false;

        }
        private void TryToTurn()
        {
            if (((currentlyFalling.Position.X + currentlyFalling.SpawnContainer.Height) >= GameArea.Width) && (fallingDirection == Turn.Right)) fallingDirection = Turn.None;
            if ((currentlyFalling.Position.X <= 0) && (fallingDirection == Turn.Left)) fallingDirection = Turn.None;

        }
        private bool EndGameCondition()
        {//add condition to end
            return false;
        }
       
        private void MoveFallingElement()
        {
           
           if ((currentlyFalling.Position.Y + currentlyFalling.SpawnContainer.Height) >= (GameArea.Height))
            {
                doneFalling = true;
                return;
            }
           double nextY = currentlyFalling.Position.Y;

            TryToTurn();

            for (int i = 0; i < fallingElements.Count ; i++)//CUrrentlyFalling
            {
                bool heightRangeCondition = ((currentlyFalling.Position.Y + currentlyFalling.SpawnContainer.Height) >= fallingElements[i].Position.Y) && (currentlyFalling.Position.Y <= (fallingElements[i].Position.Y + fallingElements[i].SpawnContainer.Height));
                if (fallingElements[i] != currentlyFalling)
                {

                    foreach (Point subPosition in currentlyFalling.SubPositions)
                    {
                        double subPositionX = subPosition.X + currentlyFalling.Position.X;
                        double subPositionY = subPosition.Y + currentlyFalling.Position.Y;

                        if (heightRangeCondition)
                        {
                            if ((subPositionY + squareSize) >= fallingElements[i].Position.Y)
                            {
                                foreach (Point fallenSubPosition in fallingElements[i].SubPositions)
                                {
                                    double fallenSubPositionX = fallenSubPosition.X + fallingElements[i].Position.X;
                                    double fallenSubPositionY = fallenSubPosition.Y + fallingElements[i].Position.Y;
                                    if ((fallenSubPositionX == subPositionX) && (((subPositionY + squareSize) == fallenSubPositionY) || (subPositionY == fallenSubPositionY)))
                                    {
                                        doneFalling = true;
                                        return;

                                    }//height condition
                                    bool CanNotTurnRightCondition = ((currentlyFalling.Position.X + currentlyFalling.SpawnContainer.Width) == fallingElements[i].Position.X);

                                    bool CanNotTurnLeftCondition = currentlyFalling.Position.X == (fallingElements[i].Position.X + fallingElements[i].SpawnContainer.Width);

                                    if (CanNotTurnLeftCondition) { fallingDirection = Turn.None; break; }
                                    if (CanNotTurnRightCondition) { fallingDirection = Turn.None; break; }
                                }
                            }
                        }
                    }
                }

            }
            
           
            
            nextY += squareSize;
            //moving, after checking conditions
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
                currentlyFalling = null;
               // x.Position = new Point(x.Position.X, x.Position.Y + squareSize);

                //Canvas.SetTop(x, x.Position.Y);
                //Canvas.SetLeft(x, x.Position.X);
            }

        }
        private void Rotate()
        {
            double nextX=0;
            double nextY=0;
            double correctPosition = 0;
            foreach (Point subElement in currentlyFalling.SubPositions)
            {
                nextX = subElement.Y;
                nextY = -subElement.X;
                if (subElement.X > correctPosition)
                {
                    correctPosition = subElement.X;
                }
            }
            if (correctPosition > 0)
            {
                for (int i=0;i<currentlyFalling.SubPositions.Count;i++)
                {
                    nextY += correctPosition;
                    currentlyFalling.SubPositions[i] = new Point(nextX, nextY);
                }
            }
            currentlyFalling.CorrectWidthHeight();
            Canvas.SetTop(currentlyFalling, currentlyFalling.Position.Y);
            Canvas.SetLeft(currentlyFalling, currentlyFalling.Position.X);
            rotating = false;
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
        }
        FallingElementControl Test1 = new FallingElementControl();
        
        private void gameTimer_Tick(object sender, EventArgs e)
        {
            if (doneFalling)
            {
                RowCompleteCheck();
                NewFallingElement();
            }
            // TryToRotate();//clockwise rotation
            // if (rotating)
            //  {
            //     Rotate();

            //  }
            MoveFallingElement();

            if (EndGameCondition())
            {
                gameTimer.IsEnabled = false;
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
