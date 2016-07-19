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
            if (e.Key == Key.S)
            {
                setupMode = !setupMode;
            }

            if (e.Key == Key.D1)
            {
                setupPlayer = 1;
            }

            if (e.Key == Key.D2)
            {
                setupPlayer = 2;
            }

            if (e.Key == Key.Left)
            {
                game.Undo();
                HighlightGrid(game.Undo());
            }

            if (e.Key == Key.Right)
            {
                game.Redo();
                HighlightGrid(game.Redo());
            }

        }

        void DoSetup(int grid, int field)
        {
            if (game.setField(grid, field, setupPlayer))
                ColorField(grid, field, setupPlayer);
            else ColorField(grid, field, 4);
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


            ColorField(g, f, 1);

            Move m = game.MakeAIMove();

            ColorField(m.g, m.f, 2);


            HighlightGrid(game.selectedGrid);
        }

        private void ColorField(int grid, int field, int player)
        {
            Color c;
            int state = game.takenGrids[grid];

            if (state == 0)
            {
                switch(player){
                    case 1: c = Color.FromRgb(0,111,111);
                        break;
                    case 2: c = Color.FromRgb(150,0,0);
                        break;
                    case 3: c = Color.FromRgb(200, 200, 200);
                        break;
                    default: c = Color.FromRgb(255, 255, 255);
                        break;
                }
            }
            else
            {
                if(player == 1 || player == 2)
                {
                    switch (state)
                    {
                        case 1: c = Color.FromRgb(0, 111, 111);
                            break;
                        case 2: c = Color.FromRgb(150, 0, 0);
                            break;
                        case 3: c= Color.FromRgb(160, 43, 210);
                            break;
                        default: c = Color.FromRgb(255, 255, 255);
                            break;
                    }

                }
                else if (player == 4)
                {
                    switch (state)
                    {
                        case 1: c = Color.FromRgb(100, 210, 210);
                            break;
                        case 2: c = Color.FromRgb(255, 200, 200);
                            break;
                        case 3: c = Color.FromRgb(210, 150, 255);
                            break;
                        default: c = Color.FromRgb(255, 255, 255);
                            break;
                    }

                }
                else
                {
                    switch (state)
                    {
                        case 1: c = Color.FromRgb(0, 161, 161);
                            break;
                        case 2: c = Color.FromRgb(210, 100, 100);
                            break;
                        case 3: c = Color.FromRgb(210, 93, 255);
                            break;
                        default: c = Color.FromRgb(255, 255, 255);
                            break;
                    }
                }
            }


            fields[grid][field].Fill = new SolidColorBrush(c);
        }

        private void HighlightGrid(int grid)
        {
            for (int i = 0; i < 9; ++i)
            {
                int state = game.takenGrids[i];

                for (int j = 0; j < 9; j++)
                {
                    if (game.board[i, j] == 0)
                        if ((i == grid && j != game.blocedField) || game.isFreeMove || (game.gridCounters[i] == 8 && i == grid))
                            // field available
                            ColorField(i, j, 4);
                            // not available
                        else ColorField(i, j, 3);
                }

                
            }


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

        }
    }
}
