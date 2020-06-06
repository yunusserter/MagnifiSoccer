import { userConstants } from "../_constants/userConstants";

export function users(state = {}, action) {
  switch (action.type) {
    case userConstants.FORGET_PASSWORD_REQUEST:
      return { forget: true };

    case userConstants.FORGET_PASSWORD_SUCCESS:
      return {};

    case userConstants.FORGET_PASSWORD_FAILURE:
      return {};

    case userConstants.GETALL_FOR_SQUAD_REQUEST:
      return {
        loading: true,
      };

    case userConstants.GETALL_FOR_SQUAD_SUCCESS:
      return {
        items: action.users,
      };

    case userConstants.GETALL_FOR_SQUAD_FAILURE:
      return {
        error: action.error,
      };

    default:
      return state;
  }
}
