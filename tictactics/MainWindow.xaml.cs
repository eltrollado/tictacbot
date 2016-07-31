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

namespace tictactics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TicConsole console = new TicConsole();

        bool setupMode = false;
        int setupPlayer = 1;

        Grid[] grids;
        Rectangle[][] fields;

        Game game;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void key_handler(object sender, KeyEventArgs e)
        {

            switch(e.Key)
            {
                case Key.Left:
                    game.Undo();
                    game.Undo();
                    DrawBoard();
                    break;
                case Key.Right:
                    game.Redo();
                    game.Redo();
                    DrawBoard();
                    break;
                case Key.S:
                    if (setupMode)
                    {
                        setupMode = false;
                        console.WriteLine("Game Mode");
                    }
                    else
                    {
                        setupMode = true;
                        console.WriteLine("Setup Mode");
                    }
                    break;
                case Key.D1:
                    setupPlayer = 1;
                    break;
                case Key.D2:
                    setupPlayer = 2;
                    break;
                case Key.OemTilde:
                    if (console == null)
                        console = new TicConsole();
                    if (console.IsVisible)
                        console.Hide();
                    else console.Show();
                    break;
            }


        }

        void DoSetup(int grid, int field)
        {
            game.setField(grid, field, setupPlayer);

            DrawBoard();

        }

        private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {

            Rectangle field = (Rectangle)sender;
            Grid parent = (Grid)field.Parent;

            int g = Int32.Parse(parent.Tag.ToString());
            int f = Int32.Parse(field.Tag.ToString());

            if(setupMode)
            {
                DoSetup(g, f);
                return;
            }


            if (!game.makeMove(g, f, 1))
                return;


            DrawBoard();

            Move m = game.MakeAIMove();

            DrawBoard();
        }

        private void DrawBoard()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    ColorField(i, j);
                }
            }
        }

        private void ColorField(int grid, int field)
        {
            Color c;
            int state = game.takenGrids[grid];
            int player = game.board[grid, field];
            int selected = game.selectedGrid;
            int blocked = game.blocedField;
            bool freeMove = game.isFreeMove;

            bool isAvailable = (freeMove && field != blocked) || (grid == selected && (field != blocked || game.gridCounters[grid] == 8));

            Color DrawTakenC = Color.FromRgb(175, 70, 200);
            Color DrawFreeActiveC = Color.FromRgb(231, 160, 250);
            Color DrawFreeNotActiveC = Color.FromRgb(200, 100, 220);

            Color BlockedC = Color.FromRgb(200, 200, 200);
            Color FreeActiveC = Color.FromRgb(255, 255, 255);
            Color FreeNotActiveC = Color.FromRgb(200, 200, 200);

            Color RedTakenC = Color.FromRgb(150, 0, 0);
            Color RedFreeActiveC = Color.FromRgb(255, 200, 200);
            Color RedFreeNotActiveC = Color.FromRgb(210, 100, 100);

            Color BlueTakenC = Color.FromRgb(0, 111, 111);
            Color BlueFreeNotActiveC = Color.FromRgb(0, 161, 161);
            Color BlueFreeActiveC = Color.FromRgb(100, 210, 210);

            c = DrawTakenC;

            switch (state)
            {
                case 0:
                    switch (player)
                    {
                        case 0:
                            if (isAvailable)
                                c = FreeActiveC;
                            else
                                c = FreeNotActiveC;
                            break;
                        case 1:
                            c = BlueTakenC;
                            break;
                        case 2:
                            c = RedTakenC;
                            break;
                    }
                    break;
                case 1:
                    if (player != 0)
                        c = BlueTakenC;
                    else if (isAvailable)
                        c = BlueFreeActiveC;
                    else c = BlueFreeNotActiveC;
                    break;

                case 2:
                    if (player != 0)
                        c = RedTakenC;
                    else if (isAvailable)
                        c = RedFreeActiveC;
                    else c = RedFreeNotActiveC;
                    break;

                case 4:
                    if (player != 0)
                        c = DrawTakenC;
                    else if (isAvailable)
                        c = DrawFreeActiveC;
                    else c = DrawFreeNotActiveC;
                    break;

                default:
                    c = DrawTakenC;
                    break;
            }



            fields[grid][field].Fill = new SolidColorBrush(c);
        }


        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Grid grid = (Grid)sender;
            grids = grid.Children.OfType<Grid>().ToArray();
            fields = new Rectangle[grids.Length][];

            for (int i = 0; i < grids.Length;++i)
            {
                fields[i] = grids[i].Children.OfType<Rectangle>().ToArray();
            }

            game = new Game();
            game.Output = console.WriteLine;

        }



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            console.Close();

            Application.Current.Shutdown();
        }
    }
}
