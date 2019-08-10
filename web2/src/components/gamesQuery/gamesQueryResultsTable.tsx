import * as React from 'react';
import { Game, GameStatus } from '../../api/model';
import { AppState } from '../../store/state';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import * as Actions from '../../store/actions';
import * as ThunkActions from '../../thunkActions';
import { navigateTo } from '../../history';
import Routes from '../../routes';

interface GamesQueryResultsTableProps {
    games : Game[],
    onViewGameClicked: (game: Game) => void
}

class gamesQueryResultsTable extends React.Component<GamesQueryResultsTableProps> {
    render() {
        return (
            <div>
                <table>
                    <tbody>
                        <tr>
                            <th></th>
                            <th>GameID</th>
                            <th>Description</th>
                            <th>Created by</th>
                            <th>Status</th>
                            <th># Players</th>
                            <th># Regions</th>
                            <th>Is public</th>
                            <th>Allow guests</th>
                        </tr>
                        {this.props.games
                            .map((g, i) => this.renderRow(g, i))
                        }
                    </tbody>
                </table>
            </div>
        );
    }

    private renderRow(game: Game, rowNumber: number) {
        return (
            <tr key={rowNumber}>
                <td>
                    <button
                        onClick={e => this.props.onViewGameClicked(game)}
                    >
                        Load
                    </button>
                </td>
                <td>{game.id}</td>
                <td>{game.parameters.description}</td>
                <td>{game.createdBy.userName}</td>
                <td>{game.status}</td>
                <td>{game.players.length}</td>
                <td>{game.parameters.regionCount}</td>
                <td>{game.parameters.isPublic}</td>
                <td>{game.parameters.allowGuests}</td>
            </tr>
        );
    }
}

const mapStateToProps = (state : AppState) => {
    if (state.gamesQuery && state.gamesQuery.results) {
        return {
            games: state.gamesQuery.results
        };
    } else {
        return {
            games : []
        };
    }
};

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        onViewGameClicked: (game: Game) => ThunkActions.navigateToGame(game)
    };
};

const GamesQueryResultsTable = connect(mapStateToProps, mapDispatchToProps)(gamesQueryResultsTable);

export default GamesQueryResultsTable;