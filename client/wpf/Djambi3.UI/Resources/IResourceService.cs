using System.Collections.Generic;
using System.Windows.Media;

namespace Djambi.UI.Resources
{
    public interface IResourceService
    {
        string AssassinName { get; }
        string CorpseName { get; }
        string ChiefName { get; }
        string JournalistName { get; }
        string DiplomatName { get; }
        string UndertakerName { get; }
        string ThugName { get; }
        string SeatName { get; }

        string GameName { get; }

        string ImagesDirectory { get; }

        IEnumerable<Color> PlayerColors { get; }
        Color NeutralPlayerColor { get; }

        Color CellColor1 { get; }
        Color CellColor2 { get; }
        Color CellColorSeat { get; }

        Color BorderLabelColor { get; }
        Color BorderLabelBackgroundColor { get; }

        Color SelectionHighlightColor { get; }
        Color SelectionOptionHighlightColor { get; }
    }
}
