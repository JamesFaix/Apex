import * as React from 'react';
import { Game, Player, PlayerKind } from '../api/model';
import TextCell from './tables/textCell';
import ThemeService from '../themes/themeService';

export interface GamePlayersPanelPlayersTableProps {
    game : Game,
    theme : ThemeService
}

export default class GamePlayersPanelPlayersTable extends React.Component<GamePlayersPanelPlayersTableProps> {

    private getPlayerNote(player : Player) {
        switch (player.kind) {
            case PlayerKind.User:
                return "";

            case PlayerKind.Guest:
                const host = this.props.game.players
                    .find(p => p.userId === player.userId
                        && p.kind === PlayerKind.User);

                return "Guest of " + host.name;

            case PlayerKind.Neutral:
                return "Neutral";

            default:
                throw "Invalid player kind.";
        }
    }

    private renderPlayerRow(player : Player, rowNumber : number) {
        const color = this.props.theme.getPlayerColor(player.colorId);
        const style = {
            boxShadow: "inset 0 0 5px 5px " + color
        };

        return (
            <tr style={style} key={"row" + rowNumber}>
                <TextCell text={player.name}/>
                <TextCell text={this.getPlayerNote(player)}/>
            </tr>
        );
    }

    render() {
        return (
            <div style={{display:"flex"}}>
                <table className="table">
                    <tbody>
                        {
                            this.props.game.players
                                .map((p, i) => this.renderPlayerRow(p, i))
                        }
                    </tbody>
                </table>
            </div>
        );
    }
}