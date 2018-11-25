import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import { UserResponse } from '../../api/model';
import ApiClient from '../../api/client';
import { Redirect } from 'react-router';
import ActionButton from '../actionButton';

export interface DashboardPageProps {
    user : UserResponse,
    api : ApiClient,
    setUser(user: UserResponse) : void,
    rulesUrl : string
}

export default class DashboardPage extends React.Component<DashboardPageProps> {

    logoutClick() {
        this.props.api
            .logout()
            .then(_ => {
                this.props.setUser(null);
            })
            .catch(reason => {
                alert("Logout failed because " + reason);
            });
    }

    render() {
        //Go to home if not logged in
        if (this.props.user === null) {
            return <Redirect to='/'/>
        }

        return (
            <div>
                <PageTitle label={"Welcome, " + this.props.user.name}/>
                <br/>
                <div className="navigationStrip">
                    <ActionButton label="Log out" action={() => this.logoutClick()}/>
                </div>
            </div>
        );
    }
}