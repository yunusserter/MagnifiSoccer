import { gameConstants } from "../_constants/gameConstants";

export function ratings(state = {}, action) {
  switch (action.type) {
    case gameConstants.GET_RATING_REQUEST:
      return { loading: true };
    case gameConstants.GET_RATING_SUCCESS:
      return { items: action.ratings };
    case gameConstants.GET_RATING_FAILURE:
      return { error: action.error };
    
    default:
      return state;
  }
}
