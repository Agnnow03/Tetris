using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TetrisGame
{
    /// <summary>
    /// Interaction logic for FallingElementCOntrol.xaml
    /// </summary>
    public partial class FallingElementControl : UserControl
    {
        public Point Position { get; set; }
        private const double startingPositionX=120;
        private const double startingPositionY=0;
        public FallingElementControl()
        {            
            InitializeComponent();
            CreateFallingElement(); //will it work?
            this.Position = new Point(startingPositionX, startingPositionY);            
        }

        private int SquareSize = 20;
        private SolidColorBrush SquareColor = Brushes.Green;

        private List<UIElement> squares = new List<UIElement>();
        private List<Point> points = new List<Point>();  //WILL IT WORK?
        public List<Point> SubPositions { get { return points; } }
        
        public void CorrectWidthHeight()
        {
            double maxHeight = points[0].Y;  //remember to add squareSize
            double maxWidth = points[0].X;
            for (int i = 1; i < 4; i++)
            {
                if (maxHeight < points[i].Y) maxHeight = points[i].Y;
                if (maxWidth < points[i].X) maxWidth = points[i].X;
            }
            maxHeight += SquareSize;
            maxWidth += SquareSize;
            SpawnContainer.Height = maxHeight;
            SpawnContainer.Width = maxWidth;
        }
        private void CreateFallingElement()
        { //
          //space in which it initializes 80x80 block
          //FIRST choose on of four possible positions in X/Y

            //first block

            points.Add(new Point { X = 0, Y = 0 });

            squares.Add(new Rectangle() //czy dziala?
            {
                Height = SquareSize,
                Width = SquareSize,
                Fill = SquareColor
            });

            SpawnContainer.Children.Add(squares[0]);
            Canvas.SetTop(squares[0], points[0].Y);
            Canvas.SetLeft(squares[0], points[0].X);
            

            for (int i = 1; i < 4; i++)  //create other blocks
            {
                  Random randomize = new Random();
                  // randomizes next to which square to PLACE A NEW SQUARE
                  int squareNum = randomize.Next(0, squares.Count);

                  double SquareX;
                  double SquareY;
                  //randomize 
                  bool IsRepeated = false; //checks for repeats
                  do
                  {
                      int randomizedPartX;
                      //randomize X
                      if (points[squareNum].X >= (SpawnContainer.Width - SquareSize))
                      {
                          randomizedPartX = (SquareSize * (randomize.Next(0, 2) - 1)); //-1 or 0
                      }
                      else if (points[squareNum].X > 0)
                      {
                          randomizedPartX = (SquareSize * (randomize.Next(0, 3) - 1)); //-1 or 0 or 1
                      }
                      else
                          randomizedPartX = (SquareSize * randomize.Next(0, 2)); //0 or 1

                      SquareX = points[squareNum].X + randomizedPartX;

                      int randomizedPartY;
                      //randomize Y
                      if (points[squareNum].Y >= (SpawnContainer.Height - SquareSize))
                      {
                          randomizedPartY = (SquareSize * (randomize.Next(0, 2) - 1)); //-1 or 0
                      }
                      else if (points[squareNum].Y > 0)
                      {
                          randomizedPartY = (SquareSize * (randomize.Next(0, 3) - 1)); //-1 or 0 or 1
                      }
                      else
                          randomizedPartY = (SquareSize * randomize.Next(0, 2)); //0 or 1

                      SquareY = points[squareNum].Y + randomizedPartY;

                      //check for repeats of X & Y coordinates
                      for (int j = 0; j < squares.Count; j++)
                      {
                          if ((points[j].X == SquareX) && (points[j].Y == SquareY)) 
                          {
                              IsRepeated = true;
                              break;
                          }
                          else
                          {
                              IsRepeated = false;
                          }
                      }

                  } while (IsRepeated);  //repeats if there is a repeat
                                         //creating a new square

                //TEMPORARY

                points.Add(new Point { X = SquareX, Y = SquareY });




                squares.Add(new Rectangle()
                {
                    Height = SquareSize,
                    Width = SquareSize,
                    Fill = SquareColor
                });

                SpawnContainer.Children.Add(squares[i]);
                Canvas.SetTop(squares[i], points[i].Y);
                Canvas.SetLeft(squares[i], points[i].X);

            } //later check for errors


            this.CorrectWidthHeight();
            
        }
    }
}
