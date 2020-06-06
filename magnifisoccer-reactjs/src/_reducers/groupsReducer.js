import { groupConstants } from "../_constants/groupConstants";

export function groups(state = {}, action) {
  switch (action.type) {
    case groupConstants.GETALL_REQUEST:
      return { loading: true };
    case groupConstants.GET_SUCCESS:
      return { items: action.group };
    case groupConstants.GET_FAILURE:
      return { error: action.error };

    case groupConstants.GETALL_REQUEST:
      return { loading: true };
    case groupConstants.GETALL_SUCCESS:
      return { items: action.groups };
    case groupConstants.GETALL_FAILURE:
      return { error: action.error };

    case groupConstants.GETALL_FOR_SEARCH_REQUEST:
      return { loading: true };
    case groupConstants.GETALL_FOR_SEARCH_SUCCESS:
      return { items: action.groups };
    case groupConstants.GETALL_FOR_SEARCH_FAILURE:
      return { error: action.error };

    case groupConstants.GROUP_CREATE_REQUEST:
      return { creating: true };
    case groupConstants.GROUP_CREATE_SUCCESS:
      return {};
    case groupConstants.GROUP_CREATE_FAILURE:
      return {};

    case groupConstants.GROUP_UPDATE_REQUEST:
      return { updating: true };
    case groupConstants.GROUP_UPDATE_SUCCESS:
      return {};
    case groupConstants.GROUP_UPDATE_FAILURE:
      return {};

    case groupConstants.EDIT_MEMBER_REQUEST:
      return { editing: true };
    case groupConstants.EDIT_MEMBER_SUCCESS:
      return {};
    case groupConstants.EDIT_MEMBER_FAILURE:
      return {};

    case groupConstants.LEAVE_GROUP_REQUEST:
      return { leaving: true };
    case groupConstants.LEAVE_GROUP_SUCCESS:
      return {};
    case groupConstants.LEAVE_GROUP_FAILURE:
      return {};

    case groupConstants.REMOVE_GROUP_REQUEST:
      return { removing: true };
    case groupConstants.REMOVE_GROUP_SUCCESS:
      return {};
    case groupConstants.REMOVE_GROUP_FAILURE:
      return {};

    case groupConstants.REMOVE_FROM_GROUP_REQUEST:
      return { removing: true };
    case groupConstants.REMOVE_FROM_GROUP_SUCCESS:
      return {};
    case groupConstants.REMOVE_FROM_GROUP_FAILURE:
      return {};

    case groupConstants.GROUP_INVITE_REQUEST:
      return { invite: true };
    case groupConstants.GROUP_INVITE_SUCCESS:
      return {};
    case groupConstants.GROUP_INVITE_FAILURE:
      return {};
    default:
      return state;
  }
}
