import * as React from 'react';
import ActionButton from '../../controls/actionButton';
import PlayerActionsService, { PlayerAction, HiddenActionsState } from '../../../playerActionsService';
import { User, Game } from '../../../api/model';
import { Kernel as K } from '../../../kernel';

export interface ActionPanelProps {
    user : User,
    game : Game,
    width : string,
    height : string,
    playerActionsService : PlayerActionsService
}

export interface ActionPanelState {
    showAllActions : boolean
}

export default class ActionPanel extends React.Component<ActionPanelProps, ActionPanelState> {
    constructor(props : ActionPanelProps) {
        super(props);
        this.state = {
            showAllActions: false
        };
    }

//--- Rendering ---

    render() {
        const style = K.styles.combine([K.styles.height(this.props.height + "px"), K.styles.width(this.props.width + "px")]);
        const visibleActions = this.props.playerActionsService.getVisibleActions(this.state.showAllActions);
        return (
            <div className={K.classes.thinBorder} style={style}>
                {
                    visibleActions.map((a, i) =>
                        this.renderAction(a, i)
                    )
                }
                {this.renderExpandCollapseButton()}
            </div>
        );
    }

    private renderAction(action : PlayerAction, keyId : number) {
        return (
            <ActionButton
                key={"action" + keyId}
                label={action.name}
                onClick={() => action.onClick()}
            />
        );
    }

    private renderExpandCollapseButton() {
        const status = this.props.playerActionsService.getHiddenActionsState(this.state.showAllActions);
        switch(status) {
            case HiddenActionsState.NoneHideable:
                return undefined; //Don't show this component
            case HiddenActionsState.SomeHidden:
                return (
                    <ActionButton
                        label="..."
                        onClick={() => this.setState({showAllActions:true})}
                    />
                );
            case HiddenActionsState.HideableShown:
                return (
                    <ActionButton
                        label="<"
                        onClick={() => this.setState({showAllActions:false})}
                    />
                );
        }
    }
}