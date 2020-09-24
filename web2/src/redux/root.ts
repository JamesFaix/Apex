import { combineReducers } from 'redux';
import { configReducer } from './config/reducer';
import { apiClientReducer } from './apiClient/reducer';
import { ApiClientState, defaultApiClientState } from './apiClient/state';
import { ConfigState, defaultConfigState } from './config/state';
import { ApiClientAction } from './apiClient/actions';
import { ConfigAction } from './config/actions';
import { ApiClientActionTypes } from './apiClient/actionTypes';
import { ConfigActionTypes } from './config/actionTypes';
import { SessionState, defaultSessionState } from './session/state';
import { SessionActionTypes } from './session/actionTypes';
import { SessionAction } from './session/actions';
import { sessionReducer } from './session/reducer';
import { NavigationState, defaultNavigationState } from './navigation/state';
import { NavigationActionTypes } from './navigation/actionTypes';
import { NavigationAction } from './navigation/actions';
import { navigationReducer } from './navigation/reducer';
import { ActiveGameState, defaultActiveGameState } from './activeGame/state';
import { ActiveGameActionTypes } from './activeGame/actionTypes';
import { ActiveGameAction } from './activeGame/actions';
import { activeGameReducer } from './activeGame/reducer';
import { NotificationsState, defaultNotificationsState } from './notifications/state';
import { NotificationAction } from './notifications/actions';
import { NotificationActionTypes } from './notifications/actionTypes';
import { notificationsReducer } from './notifications/reducer';
import { BoardsState, defaultBoardsState } from './boards/state';
import { BoardsActionTypes } from './boards/actionTypes';
import { BoardsAction } from './boards/actions';
import { boardsReducer } from './boards/reducer';

export type RootState = {
  activeGame: ActiveGameState,
  apiClient: ApiClientState,
  boards: BoardsState,
  config: ConfigState,
  navigation: NavigationState,
  notifications: NotificationsState,
  session: SessionState
};

export type RootActionTypes =
  ActiveGameActionTypes |
  ApiClientActionTypes |
  BoardsActionTypes |
  ConfigActionTypes |
  NavigationActionTypes |
  NotificationActionTypes |
  SessionActionTypes;

export type RootAction =
  ActiveGameAction |
  ApiClientAction |
  BoardsAction |
  ConfigAction |
  NavigationAction |
  NotificationAction |
  SessionAction;

export const defaultRootState: RootState = {
  activeGame: defaultActiveGameState,
  apiClient: defaultApiClientState,
  boards: defaultBoardsState,
  config: defaultConfigState,
  navigation: defaultNavigationState,
  notifications: defaultNotificationsState,
  session: defaultSessionState,
};

export const rootReducer = combineReducers({
  activeGame: activeGameReducer,
  apiClient: apiClientReducer,
  boards: boardsReducer,
  config: configReducer,
  navigation: navigationReducer,
  notifications: notificationsReducer,
  session: sessionReducer,
});
