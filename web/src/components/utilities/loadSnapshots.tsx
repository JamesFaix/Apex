import * as React from 'react';
import { connect } from 'react-redux';
import Controller from '../../controller';
import { State } from '../../store/root';

interface LoadSnapshotsProps {
    gameId : number,
    getSnapshots : (gameId : number) => void
}

class loadSnapshots extends React.Component<LoadSnapshotsProps> {
    componentDidMount() {
        this.props.getSnapshots(this.props.gameId);
    }

    render() : JSX.Element {
        return null;
    }
}

const mapStateToProps = (_ : State) => {
    return {
        getSnapshots : (gameId : number) => Controller.Snapshots.get(gameId)
    };
}

const LoadSnapshots = connect(mapStateToProps)(loadSnapshots);
export default LoadSnapshots;