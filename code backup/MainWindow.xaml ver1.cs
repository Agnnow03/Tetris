using System;
using System.Collections.Generic;
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
        FallingElementControl currentlyFalling;
        double squareSize=20;
        int fallingSpeed = 300;
        public enum Turn { Left, Right, None};
        private Turn fallingDirection;

        List<FallingElementControl> fallingElements = new List<FallingElementControl>();
         private void DrawFallingElement()
         {  
            
            currentlyFalling = new FallingElementControl();
            fallingElements.Add(currentlyFalling);
            int index = fallingElements.Count - 1;
            
            GameArea.Children.Add(fallingElements[index]);
            Canvas.SetTop(fallingElements[index],fallingElements[index].Position.Y);
            Canvas.SetLeft(fallingElements[index],fallingElements[index].Position.X);
            doneFalling = false;
        }
        private void MoveFallingElement()
        {

            double nextY = currentlyFalling.Position.Y;
            if ((currentlyFalling.Position.Y + currentlyFalling.SpawnContainer.Height) >= (GameArea.Height))
            {
                doneFalling = true;
                return;
            }
           /* for (int i = 0; i < fallingElements.Count - 1; i++)
            {
                if ((currentlyFalling.Position.Y + currentlyFalling.SpawnContainer.Height) >= fallingElements[i].Position.Y)
                {
                    foreach (Point subPosition in currentlyFalling.SubPositions)
                    {
                        double subPositionX = subPosition.X + currentlyFalling.Position.X;
                        double subPositionY = subPosition.Y + currentlyFalling.Position.Y;

                        if ((subPositionY + squareSize) >= fallingElements[i].Position.Y)
                        {                                                      
                                foreach (Point fallenSubPosition in fallingElements[i].SubPositions)
                                {
                                    double fallenSubPositionX = fallenSubPosition.X + fallingElements[i].Position.X;
                                    double fallenSubPositionY = fallenSubPosition.Y + fallingElements[i].Position.Y;
                                    if ((fallenSubPositionX == subPositionX)&&((subPositionY + squareSize)==fallenSubPositionY)) { //will it ever be true?
                                        doneFalling = true;
                                        return;
                                        
                                    }
                                }                           
                        }
                    }
                }
            }*/ //see if I can make only one for loop
            if (((currentlyFalling.Position.X + currentlyFalling.SpawnContainer.Width) >= GameArea.Width) && (fallingDirection == Turn.Right)) fallingDirection = Turn.None;
            if ((currentlyFalling.Position.X <= 0) && (fallingDirection == Turn.Left)) fallingDirection = Turn.None;
            for (int i = 0; i < fallingElements.Count - 1; i++)
            {
                bool heightRangeCondition = ((nextY + currentlyFalling.SpawnContainer.Height) > fallingElements[i].Position.Y) && (nextY < (fallingElements[i].Position.Y + fallingElements[i].SpawnContainer.Height));
                //wyzej blad?
                bool CanNotTurnRightCondition = ((currentlyFalling.Position.X + currentlyFalling.SpawnContainer.Width) == fallingElements[i].Position.X);

                bool CanNotTurnLeftCondition = currentlyFalling.Position.X == (fallingElements[i].Position.X + fallingElements[i].SpawnContainer.Width);

                if (CanNotTurnLeftCondition && heightRangeCondition) { fallingDirection = Turn.None; break; }
                if (CanNotTurnRightCondition && heightRangeCondition) { fallingDirection = Turn.None; break; }

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

            //CONDITIONS for turning
            currentlyFalling.Position = new Point(nextX, nextY);
            Canvas.SetTop(currentlyFalling, currentlyFalling.Position.Y);
            Canvas.SetLeft(currentlyFalling, currentlyFalling.Position.X);

            fallingDirection = Turn.None;

           
        }
        private void StartNewGame()
        {
            gameTimer.Interval = TimeSpan.FromMilliseconds(fallingSpeed);
        }
        FallingElementControl Test1 = new FallingElementControl();
        
        private void gameTimer_Tick(object sender, EventArgs e)
        {
            if(doneFalling) DrawFallingElement();
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
