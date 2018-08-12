using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace Djambi.UI.Resources.HotdogTown
{
    public class ResourceService : IResourceService
    {
        public string AssassinName => ResourceStrings.Assassin;

        public string CorpseName => ResourceStrings.Corpse;

        public string ChiefName => ResourceStrings.Chief;

        public string JournalistName => ResourceStrings.Journalist;

        public string DiplomatName => ResourceStrings.Diplomat;

        public string UndertakerName => ResourceStrings.Undertaker;

        public string ThugName => ResourceStrings.Thug;

        public string SeatName => ResourceStrings.Seat;

        public string GameName => ResourceStrings.Theme;

        public string ImagesDirectory => ResourceStrings.ImageDirectory;

        public IEnumerable<Color> PlayerColors => new[]
        {
            ResourceStrings.PlayerColor1,
            ResourceStrings.PlayerColor2,
            ResourceStrings.PlayerColor3,
            ResourceStrings.PlayerColor4
        }.Select(GetColor);

        public Color NeutralPlayerColor => GetColor(ResourceStrings.PlayerColorNeutral);

        public Color CellColor1 => GetColor(ResourceStrings.CellColor1);

        public Color CellColor2 => GetColor(ResourceStrings.CellColor2);

        public Color CellColorSeat => GetColor(ResourceStrings.CellColorSeat);

        public Color BorderLabelColor => GetColor(ResourceStrings.BoardLabelColor);

        public Color BorderLabelBackgroundColor => GetColor(ResourceStrings.BoardLabelBackgroundColor);

        public Color SelectionHighlightColor => GetColor(ResourceStrings.SelectionColor);

        public Color SelectionOptionHighlightColor => GetColor(ResourceStrings.SelectionOptionColor);

        private Color GetColor(string hex) => (Color)ColorConverter.ConvertFromString(hex);
    }
}
