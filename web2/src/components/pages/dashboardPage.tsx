import * as React from 'react';
import { Dispatch } from 'redux';
import * as ThunkActions from '../../thunkActions';
import { connect } from 'react-redux';
import { GamesQuery, Game } from '../../api/model';
import GamesQueryFilters from '../forms/gamesSearchForm';
import GamesSearchResultsTable from '../tables/gamesSearchResultsTable';
import { AppState } from '../../store/state';
import Routes from '../../routes';
import { Link } from 'react-router-dom';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';

interface DashboardPageProps {
    gamesQuery : GamesQuery,
    gamesResults : Game[],
    onSearchClicked : (query: GamesQuery) => void
}

class dashboardPage extends React.Component<DashboardPageProps>{

    render() {
        return (
            <div>
                <RedirectToLoginIfNotLoggedIn/>
                <Link to={Routes.createGame}>
                    <button>
                        Create game
                    </button>
                </Link>
                <GamesQueryFilters/>
                <button
                    onClick={() => this.props.onSearchClicked(this.props.gamesQuery)}
                >
                    Search
                </button>
                <GamesSearchResultsTable/>
            </div>
        );
    }
}

const mapStateToProps = (state: AppState) => {
    if (state.gamesQuery){
        return {
            gamesQuery: state.gamesQuery.query,
            gamesResults: state.gamesQuery.results
        };
    } else {
        return {
            gamesQuery: null,
            gamesResults: []
        };
    }
};

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        onSearchClicked: (query: GamesQuery) => ThunkActions.queryGames(query)(dispatch)
    };
};

const DashboardPage = connect(mapStateToProps, mapDispatchToProps)(dashboardPage);

export default DashboardPage;