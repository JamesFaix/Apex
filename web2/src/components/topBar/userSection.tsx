import * as React from 'react';
import { User } from '../../api/model';
import { AppState } from '../../store/state';
import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import IconButton from '../controls/iconButton';
import Icons from '../../utilities/icons';
import ApiActions from '../../apiActions';

interface UserSectionProps {
    user : User,
    onLogoutClicked : () => void
}

const userSection : React.SFC<UserSectionProps> = props => {
    return props.user
        ? loggedInUserSection(props)
        : notLoggedInUserSection(props);
}

const notLoggedInUserSection : React.SFC<UserSectionProps> = props => {
    return (
        <div>
            (Not signed in)
        </div>
    );
};

const loggedInUserSection : React.SFC<UserSectionProps> = props => {
    return (
        <div>
            {props.user.name}
            <IconButton
                title="Log out"
                icon={Icons.logout}
                onClick={() => props.onLogoutClicked()}
            />
        </div>
    );
};

const mapStateToProps = (state : AppState) => {
    return {
        user: state.session.user
    };
};

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        onLogoutClicked: () => ApiActions.logout()(dispatch)
    };
}

const UserSection = connect(mapStateToProps, mapDispatchToProps)(userSection);

export default UserSection;