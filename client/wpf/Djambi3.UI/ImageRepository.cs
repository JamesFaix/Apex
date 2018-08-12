using System;
using System.Windows.Media.Imaging;
using Djambi.Model;
using Djambi.UI.Resources;

namespace Djambi.UI
{
    class ImageRepository
    {
        private readonly string _ImagesDirectory;

        public ImageRepository()
        {
            _ImagesDirectory = ResourceFactory.ResourceService.ImagesDirectory;

            Assassin = GetImage("assassin");
            Chief = GetImage("chief");
            Diplomat = GetImage("diplomat");
            Journalist = GetImage("journalist");
            Thug = GetImage("thug");
            Undertaker = GetImage("undertaker");
            Corpse = GetImage("corpse");

            AppIcon = new BitmapImage(new Uri($"pack://application:,,,/Resources/{_ImagesDirectory}/chief.png", UriKind.RelativeOrAbsolute));
        }

        private BitmapImage GetImage(string imageName) =>
            new BitmapImage(new Uri($"/Djambi.UI;component/Resources/{_ImagesDirectory}/{imageName}.png", UriKind.Relative));

        public BitmapImage AppIcon { get; }

        #region Pieces

        public BitmapImage Assassin { get; }
        public BitmapImage Chief { get; }
        public BitmapImage Diplomat { get; }
        public BitmapImage Journalist { get; }
        public BitmapImage Thug { get; }
        public BitmapImage Undertaker { get; }
        public BitmapImage Corpse { get; }

        public BitmapImage GetPieceImage(Piece piece)
        {
            if (!piece.IsAlive)
            {
                return Corpse;
            }

            switch (piece.Type)
            {
                case PieceType.Assassin: return Assassin;
                case PieceType.Chief: return Chief;
                case PieceType.Diplomat: return Diplomat;
                case PieceType.Journalist: return Journalist;
                case PieceType.Thug: return Thug;
                case PieceType.Undertaker: return Undertaker;
                default:
                    throw new Exception($"Invalid {nameof(PieceType)} value ({piece.Type}).");
            }
        }

        #endregion
    }
}
