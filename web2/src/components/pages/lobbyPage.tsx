import * as React from 'react';
import { Game } from '../../api/model';
import * as Redirects from '../redirects';
import { AppState } from '../../store/state';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import { navigateTo } from '../../history';
import Routes from '../../routes';

interface LobbyPageProps {
    game : Game
}

interface LobbyPageState {

}

class lobbyPage extends React.Component<LobbyPageProps, LobbyPageState> {

    private getGameJson() {
        if (this.props.game) {
            return JSON.stringify(this.props.game);
        } else {
            return "(no game loaded)";
        }
    }

    render() {
        return (
            <div>
                <Redirects.ToHomeIfNoSession/>
                <Redirects.ToHomeIfNoGame/>
                <button
                    onClick={() => navigateTo(Routes.dashboard)}
                >
                    Home
                </button>
                {this.getGameJson()}
            </div>
        );
    }
}


const mapStateToProps = (state: AppState) => {
    if (state.currentGame) {
        return {
            game: state.currentGame.game
        };
    } else {
        return {
            game: null
        };
    }
};

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        //Switch from lobby to play
    };
};

const LobbyPage = connect(mapStateToProps, mapDispatchToProps)(lobbyPage);

export default LobbyPage;