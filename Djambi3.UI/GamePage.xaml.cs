﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Djambi.Engine;
using Djambi.Engine.Extensions;
using Djambi.Model;

namespace Djambi.UI
{
    /// <summary>
    /// Interaction logic for Page2.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        private readonly Brush _whiteBrush;
        private readonly Brush _blackBrush;
        private readonly Brush _grayBrush;
        private readonly Dictionary<PlayerColor, Brush> _playerColorBrushes;
        private readonly Brush _selectionOptionBrush;
        private readonly Brush _selectionBrush;

        private const double _selectionOpacity = 0.5;
        private const string _selectionOptionRectName = "SelectionOption";
        private const string _selectionRectName = "Selection";

        public GamePage()
        {
            InitializeComponent();

            _whiteBrush = new SolidColorBrush(Colors.White);
            _blackBrush = new SolidColorBrush(Colors.Black);
            _grayBrush = new SolidColorBrush(Colors.Gray);
            _playerColorBrushes = new Dictionary<PlayerColor, Brush>
            {
                [PlayerColor.Red] = new SolidColorBrush(Colors.Red),
                [PlayerColor.Orange] = new SolidColorBrush(Colors.Orange),
                [PlayerColor.Yellow] = new SolidColorBrush(Colors.Yellow),
                [PlayerColor.Green] = new SolidColorBrush(Colors.Green),
                [PlayerColor.Blue] = new SolidColorBrush(Colors.Blue),
                [PlayerColor.Purple] = new SolidColorBrush(Colors.Purple),
                [PlayerColor.Dead] = new SolidColorBrush(Colors.Gray)
            };
            _selectionOptionBrush = new SolidColorBrush(Colors.Yellow);
            _selectionBrush = new SolidColorBrush(Colors.Green);

            DrawBoard();
            DrawGameState();
            Controller.GetValidSelections()
                .OnValue(DrawSelectionOptions)
                .OnError(error =>
                {
                    //TODO: Display error somehow
                    throw new Exception("Whoops");
                });
        }

        #region Drawing

        private void DrawBoard()
        {
            for (var i = 0; i < Constants.BoardSize; i++)
            {
                gridBoard.RowDefinitions.Add(new RowDefinition
                {
                    Name = $"{nameof(gridBoard)}Row{i}"
                });
                gridBoard.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Name = $"{nameof(gridBoard)}Column{i}"
                });
            }

            for (var r = 0; r < Constants.BoardSize; r++)
            {
                for (var c = 0; c < Constants.BoardSize; c++)
                {
                    var rect = new Rectangle
                    {
                        Fill = GetCellBrush(r, c),
                        Stretch = Stretch.Uniform
                    };
                    rect.InputBindings.Add(GetCellClickedInputBinding(Location.Create(c + 1, r + 1)));

                    gridBoard.Children.Add(rect);
                    Grid.SetColumn(rect, c);
                    Grid.SetRow(rect, r);
                }
            }
        }

        private Brush GetCellBrush(int row, int column)
        {
            //-1 because WPF grids are 0-based, but the game is 1-based
            if (row == Constants.BoardCenter - 1
            && column == Constants.BoardCenter - 1)
            {
                return _grayBrush;
            }
            else
            {
                return (row + column) % 2 == 0
                    ? _blackBrush
                    : _whiteBrush;
            }
        }

        private void DrawGameState()
        {
            var state = Controller.GameState;
            DrawPieces(state);
            DrawTurnCycle(state);
            DrawPlayers(state);
        }

        private void DrawPieces(GameState state)
        {
            var piecesWithColors = state.Pieces
                .LeftOuterJoin(state.Players,
                    piece => piece.PlayerId,
                    player => player.Id,
                    (piece, player) => new
                    {
                        Piece = piece,
                        Color = player.Color
                    },
                    piece => new
                    {
                        Piece = piece,
                        Color = PlayerColor.Dead
                    });

            foreach (var piece in piecesWithColors)
            {
                var ell = new Ellipse
                {
                    Fill = _playerColorBrushes[piece.Color],
                    Stretch = Stretch.Uniform,
                };
                var clickBinding = GetCellClickedInputBinding(piece.Piece.Location);
                ell.InputBindings.Add(clickBinding);

                gridBoard.Children.Add(ell);
                //-1 because grid is 0-based but game is 1-based
                Grid.SetColumn(ell, piece.Piece.Location.X - 1);
                Grid.SetRow(ell, piece.Piece.Location.Y - 1);
                Grid.SetZIndex(ell, 1);

                var image = new Image
                {
                    Source = new BitmapImage(new Uri(GetPieceImagePath(piece.Piece), UriKind.Relative)),
                    Height = 30,
                    Width = 30
                };
                image.InputBindings.Add(clickBinding);

                gridBoard.Children.Add(image);
                Grid.SetColumn(image, piece.Piece.Location.X - 1);
                Grid.SetRow(image, piece.Piece.Location.Y - 1);
                Grid.SetZIndex(image, 2);
            }
        }

        private string GetPieceImagePath(Piece piece)
        {
            if (!piece.IsAlive)
            {
                return "Images/corpse.png";
            }

            switch (piece.Type)
            {
                case PieceType.Assassin:
                    return "Images/assassin.png";

                case PieceType.Chief:
                    return "Images/chief.png";

                case PieceType.Diplomat:
                    return "Images/diplomat.png";

                case PieceType.Militant:
                    return "Images/militant.png";

                case PieceType.Necromobile:
                    return "Images/necromobile.png";

                case PieceType.Reporter:
                    return "Images/reporter.png";

                default:
                    throw new Exception("Invalid PieceType");
            }
        }

        private void DrawTurnCycle(GameState state)
        {
            var turnDetails = state.TurnCycle
                .Join(state.Players,
                    turnPlayerId => turnPlayerId,
                    player => player.Id,
                    (t, p) => new
                    {
                        Name = p.Name,
                        Color = p.Color
                    })
                .ToList();

            for (var i = 0; i < turnDetails.Count; i++)
            {
                gridTurnCycle.RowDefinitions.Add(new RowDefinition
                {
                    Name = $"{nameof(gridTurnCycle)}Row{i}"
                });

                var turn = turnDetails[i];

                var rect = new Rectangle
                {
                    Fill = _playerColorBrushes[turn.Color],
                    Stretch = Stretch.Uniform
                };

                gridTurnCycle.Children.Add(rect);
                Grid.SetColumn(rect, 0);
                Grid.SetRow(rect, i);

                var indexLabel = new Label
                {
                    Content = (i+1).ToString()
                };

                gridTurnCycle.Children.Add(indexLabel);
                Grid.SetColumn(indexLabel, 0);
                Grid.SetRow(indexLabel, i);

                var nameLabel = new Label
                {
                    Content = turn.Name
                };

                gridTurnCycle.Children.Add(nameLabel);
                Grid.SetColumn(nameLabel, 1);
                Grid.SetRow(nameLabel, i);
            }
        }

        private void DrawPlayers(GameState state)
        {
            var players = state.Players;

            for (var i = 0; i < players.Count; i++)
            {
                gridPlayers.RowDefinitions.Add(new RowDefinition
                {
                    Name = $"{nameof(gridPlayers)}Row{i}"
                });

                var player = players[i];

                var rect = new Rectangle
                {
                    Fill = _playerColorBrushes[player.Color],
                    Stretch = Stretch.Uniform
                };

                gridPlayers.Children.Add(rect);
                Grid.SetColumn(rect, 0);
                Grid.SetRow(rect, i);
                
                var nameLabel = new Label
                {
                    Content = GetPlayerLabelText(player)
                };

                gridPlayers.Children.Add(nameLabel);
                Grid.SetColumn(nameLabel, 1);
                Grid.SetRow(nameLabel, i);
            }
        }

        private string GetPlayerLabelText(Player player)
        {
            /*
             * Formatting will look like one of the following
             *     Player1
             *     Player2 (Neutral)
             *     Player3 (Eliminated)
             *     Player4 (Neutral, Eliminated)
             */

            var sb = new StringBuilder(player.Name);

            if (!player.IsAlive || player.IsVirtual)
            {
                sb.Append(" (");

                if (player.IsVirtual)
                {
                    sb.Append("Neutral");

                    if (!player.IsAlive)
                    {
                        sb.Append(", ");
                    }
                }
                if (!player.IsAlive)
                {
                    sb.Append("Eliminated");
                }

                sb.Append(")");
            }

            return sb.ToString();
        }

        private void DrawSelectionOptions(IEnumerable<Selection> selectionOptions)
        {
            foreach (var s in selectionOptions)
            {
                var rect = new Rectangle
                {
                    Name = _selectionOptionRectName,
                    Fill = _selectionOptionBrush,
                    Opacity = _selectionOpacity,
                    Stretch = Stretch.Uniform
                };

                gridBoard.Children.Add(rect);
                Grid.SetColumn(rect, s.Location.X - 1);
                Grid.SetRow(rect, s.Location.Y - 1);
            }
        }

        private void ClearSelectionOptions()
        {
            var selectionOptions = gridBoard.Children
                .OfType<Rectangle>()
                .Where(r => r.Name == _selectionOptionRectName)
                .ToList();

            foreach (var rect in selectionOptions)
            {
                gridBoard.Children.Remove(rect);
            }
        }

        private void ClearSelections()
        {
            var selections = gridBoard.Children
                .OfType<Rectangle>()
                .Where(r => r.Name == _selectionRectName)
                .ToList();

            foreach (var rect in selections)
            {
                gridBoard.Children.Remove(rect);
            }
        }

        private void DrawSelection(Selection selection)
        {
            var rect = new Rectangle
            {
                Name = _selectionRectName,
                Fill = _selectionBrush,
                Opacity = _selectionOpacity,
                Stretch = Stretch.Uniform
            };

            gridBoard.Children.Add(rect);
            Grid.SetColumn(rect, selection.Location.X - 1);
            Grid.SetRow(rect, selection.Location.Y - 1);
        }

        #endregion

        #region Event handlers

        private void btnQuitToMenu_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to quit and return to the main menu?",
                "Quit Game",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                this.NavigationService.Navigate(new Uri("MainMenuPage.xaml", UriKind.Relative));
            }
        }

        private InputBinding GetCellClickedInputBinding(Location location)
        {
            var gest = new MouseGesture(MouseAction.LeftClick);
            return new InputBinding(new CellClickedCommand(this, location), gest);
        } 

        public void OnSelectionMade()
        {
            ClearSelectionOptions();
            DrawSelection(Controller.TurnState.Selections.Last());
            Controller.GetValidSelections()
                .OnValue(DrawSelectionOptions)
                .OnError(error =>
                {
                    //TODO: Display error somehow
                    throw new Exception("Whoops");
                });
        }

        #endregion
    }
}
