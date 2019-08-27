import { LoginRequest, User, CreateUserRequest, Game, GamesQuery, GameParameters, CreatePlayerRequest, Event, Board, PieceKind, PlayerStatus } from "../api/model";
import { NavigationState } from "./state";
import { Point } from "../viewModel/board/model";
import { ApiResponse, ApiRequest, ApiError } from "../api/requestModel";

export enum ActionStatus {
  Pending = "PENDING",
  Success = "SUCCESS",
  Error = "ERROR"
}

export enum ActionTypes {
  Login = "LOGIN",
  Logout = "LOGOUT",
  Signup = "SIGNUP",
  LoadGame = "LOAD_GAME",
  LoadGameHistory = "LOAD_GAME_HISTORY",
  QueryGames = "QUERY_GAMES",
  UpdateGamesQuery = "UPDATE_GAMES_QUERY",
  RestoreSession = "RESTORE_SESSION",
  CreateGame = "CREATE_GAME",
  UpdateCreateGameForm = "UPDATE_CREATE_GAME_FORM",
  AddPlayer = "ADD_PLAYER",
  RemovePlayer = "REMOVE_PLAYER",
  StartGame = "START_GAME",
  SetNavigationOptions = "SET_NAV_OPTIONS",
  LoadBoard = "LOAD_BOARD",
  SelectCell = "SELECT_CELL",
  BoardZoom = "BOARD_ZOOM",
  BoardScroll = "BOARD_SCROLL",
  LoadPieceImage = "LOAD_PIECE_IMAGE",
  EndTurn = "END_TURN",
  ResetTurn = "RESET_TURN",
  ChangePlayerStatus = "CHANGE_PLAYER_STATUS",
  ApiRequest = "API_REQUEST",
  ApiResponse = "API_RESPONSE",
  ApiError = "API_ERROR"
}

export interface CustomAction {
  type: ActionTypes,
  status: ActionStatus,
}

export interface DataAction<T> extends CustomAction {
  data : T
}

function create<T>(type : ActionTypes, status : ActionStatus, data : T = undefined) {
  return {
    type: type,
    status: status,
    data: data
  };
}
const pending = <T>(type : ActionTypes, data : T = undefined) => create(type, ActionStatus.Pending, data);
const success = <T>(type : ActionTypes, data : T = undefined) => create(type, ActionStatus.Success, data);
const error = <T>(type : ActionTypes, data : T = undefined) => create(type, ActionStatus.Error, data);

export const loginRequest = (request : LoginRequest) => pending(ActionTypes.Login, request);
export const loginSuccess = (user : User) => success(ActionTypes.Login, user);
export const loginError = () => error(ActionTypes.Login);

export const logoutRequest = () => pending(ActionTypes.Logout);
export const logoutSuccess = () => success(ActionTypes.Logout);
export const logoutError = () => error(ActionTypes.Logout);

export const signupRequest = (request : CreateUserRequest) => pending(ActionTypes.Signup, request);
export const signupSuccess = (user : User) => success(ActionTypes.Signup, user);
export const signupError = () => error(ActionTypes.Signup);

export const loadGameRequest = (gameId : number) => pending(ActionTypes.LoadGame, gameId);
export const loadGameSuccess = (game : Game) => success(ActionTypes.LoadGame, game);
export const loadGameError = () => error(ActionTypes.LoadGame);

export const loadGameHistoryRequest = (gameId : number) => pending(ActionTypes.LoadGameHistory, gameId);
export const loadGameHistorySuccess = (history : Event[]) => success(ActionTypes.LoadGameHistory, history);
export const loadGameHistoryError = () => error(ActionTypes.LoadGameHistory);

export const loadBoardRequest = (regionCount : number) => pending(ActionTypes.LoadBoard, regionCount);
export const loadBoardSuccess = (board : Board) => success(ActionTypes.LoadBoard, board);
export const loadBoardError = () => error(ActionTypes.LoadBoard);

export const queryGamesRequest = (query : GamesQuery) => pending(ActionTypes.QueryGames, query);
export const queryGamesSuccess = (games : Game[]) => success(ActionTypes.QueryGames, games);
export const queryGamesError = () => error(ActionTypes.QueryGames);

export const updateGamesQuery = (query: GamesQuery) => success(ActionTypes.UpdateGamesQuery, query);

export const restoreSessionRequest = () => pending(ActionTypes.RestoreSession);
export const restoreSessionSuccess = (user: User) => success(ActionTypes.RestoreSession, user);
export const restoreSessionError = () => error(ActionTypes.RestoreSession);

export const updateCreateGameForm = (parameters : GameParameters) => success(ActionTypes.UpdateCreateGameForm, parameters);

export const createGameRequest = (parameters: GameParameters) => pending(ActionTypes.CreateGame, parameters);
export const createGameSuccess = (game : Game) => success(ActionTypes.CreateGame, game);
export const createGameError = () => error(ActionTypes.CreateGame);

export const addPlayerRequest = (request: CreatePlayerRequest) => pending(ActionTypes.AddPlayer, request);
export const addPlayerSuccess = (game : Game) => success(ActionTypes.AddPlayer, game);
export const addPlayerError = () => error(ActionTypes.AddPlayer);

export const removePlayerRequest = (playerId: number) => pending(ActionTypes.RemovePlayer, playerId);
export const removePlayerSuccess = (game : Game) => success(ActionTypes.RemovePlayer, game);
export const removePlayerError = () => error(ActionTypes.RemovePlayer);

export const startGameRequest = (gameId : number) => pending(ActionTypes.StartGame, gameId);
export const startGameSuccess = (game : Game) => success(ActionTypes.StartGame, game);
export const startGameError = () => error(ActionTypes.StartGame);

export const setNavigationOptions = (options : NavigationState) => success(ActionTypes.SetNavigationOptions, options);

export const selectCellRequest = (cellId : number) => pending(ActionTypes.SelectCell, cellId);
export const selectCellSuccess = (game : Game) => success(ActionTypes.SelectCell, game);
export const selectCellError = () => error(ActionTypes.SelectCell);

export const boardZoom = (level : number) => success(ActionTypes.BoardZoom, level);
export const boardScroll = (scrollPercent : Point) => success(ActionTypes.BoardScroll, scrollPercent);

export const loadPieceImage = (pieceKind : PieceKind, image : HTMLImageElement) => success(ActionTypes.LoadPieceImage, [pieceKind, image]);

export const endTurnRequest = () => pending(ActionTypes.EndTurn);
export const endTurnSuccess = (game : Game) => success(ActionTypes.EndTurn, game);
export const endTurnError = () => error(ActionTypes.EndTurn);

export const resetTurnRequest = () => pending(ActionTypes.ResetTurn);
export const resetTurnSuccess = (game : Game) => success(ActionTypes.ResetTurn, game);
export const resetTurnError = () => error(ActionTypes.ResetTurn);

export const changePlayerStatusRequest = () => pending(ActionTypes.ChangePlayerStatus);
export const changePlayerStatusSuccess = (game : Game) => success(ActionTypes.ChangePlayerStatus, game);
export const changePlayerStatusError = () => error(ActionTypes.ChangePlayerStatus);

export const apiRequest = (request : ApiRequest) => pending(ActionTypes.ApiRequest, request);
export const apiResponse = (response : ApiResponse) => success(ActionTypes.ApiResponse, response);
export const apiError = (apiError : ApiError) => error(ActionTypes.ApiError, apiError);