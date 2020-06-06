import { groupConstants } from "../_constants/groupConstants";

export function search(state = {}, action) {
  switch (action.type) {
    case groupConstants.GETALL_FOR_SEARCH_REQUEST:
      return { loading: true };
    case groupConstants.GETALL_FOR_SEARCH_SUCCESS:
      return { items: action.search };
    case groupConstants.GETALL_FOR_SEARCH_FAILURE:
      return { error: action.error };
    default:
      return state;
  }
}
