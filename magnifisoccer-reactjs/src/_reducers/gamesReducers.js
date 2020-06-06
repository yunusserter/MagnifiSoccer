import { gameConstants } from "../_constants/gameConstants";

export function games(state = {}, action) {
  switch (action.type) {
    case gameConstants.GAME_CREATE_REQUEST:
      return { creating: true };
    case gameConstants.GAME_CREATE_SUCCESS:
      return {};
    case gameConstants.GAME_CREATE_FAILURE:
      return {};

    case gameConstants.GAME_UPDATE_REQUEST:
      return { updating: true };
    case gameConstants.GAME_UPDATE_SUCCESS:
      return {};
    case gameConstants.GAME_UPDATE_FAILURE:
      return {};

    case gameConstants.GAME_SQUAD_REQUEST:
      return { squadEditing: true };
    case gameConstants.GAME_SQUAD_SUCCESS:
      return {};
    case gameConstants.GAME_SQUAD_FAILURE:
      return {};

    case gameConstants.GAME_RATING_REQUEST:
      return { rating: true };
    case gameConstants.GAME_RATING_SUCCESS:
      return {};
    case gameConstants.GAME_RATING_FAILURE:
      return {};

    case gameConstants.GETALL_REQUEST:
      return { loading: true };
    case gameConstants.GETALL_SUCCESS:
      return { items: action.games };
    case gameConstants.GETALL_FAILURE:
      return { error: action.error };

    case gameConstants.REMOVE_GAME_REQUEST:
      return { removing: true };
    case gameConstants.REMOVE_GAME_SUCCESS:
      return {};
    case gameConstants.REMOVE_GAME_FAILURE:
      return {};

    default:
      return state;
  }
}
