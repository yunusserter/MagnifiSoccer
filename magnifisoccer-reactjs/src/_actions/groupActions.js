import { groupConstants } from "../_constants/groupConstants";
import { groupService } from "../_services/groupService";
import { alertActions } from "./alertActions";
import { history } from "../_helpers";

export const groupActions = {
  getById,
  getAll,
  getAllForSearch,
  newGroup,
  updateGroup,
  editMember,
  leaveGroup,
  removeGroup,
  joinGroup,
  inviteGroup,
  _removeFromGroup,
};

function getById(groupId) {
  return (dispatch) => {
    dispatch(request(groupId));

    groupService.getById(groupId).then(
      (group) => {
        dispatch(success(group));
      },
      (error) => {
        dispatch(failure(error.toString()));
      }
    );
  };

  function request(group) {
    return { type: groupConstants.GET_REQUEST, group };
  }
  function success(group) {
    return { type: groupConstants.GET_SUCCESS, group };
  }
  function failure(error) {
    return { type: groupConstants.GET_FAILURE, error };
  }
}

function getAll() {
  return (dispatch) => {
    dispatch(request());

    groupService.getAll().then(
      (groups) => dispatch(success(groups)),
      (error) => dispatch(failure(error.toString()))
    );
  };

  function request() {
    return { type: groupConstants.GETALL_REQUEST };
  }
  function success(groups) {
    return { type: groupConstants.GETALL_SUCCESS, groups };
  }
  function failure(error) {
    return { type: groupConstants.GETALL_FAILURE, error };
  }
}

function getAllForSearch() {
  return (dispatch) => {
    dispatch(request());

    groupService.getAllForSearch().then(
      (groups) => dispatch(success(groups)),
      (error) => dispatch(failure(error.toString()))
    );
  };

  function request() {
    return { type: groupConstants.GETALL_FOR_SEARCH_REQUEST };
  }
  function success(search) {
    return { type: groupConstants.GETALL_FOR_SEARCH_SUCCESS, search };
  }
  function failure(error) {
    return { type: groupConstants.GETALL_FOR_SEARCH_FAILURE, error };
  }
}

function newGroup(group) {
  return (dispatch) => {
    dispatch(request(group));

    groupService.newGroup(group).then(
      (group) => {
        dispatch(success());
        history.push("/groupList");
        dispatch(alertActions.success("Group created successfully."));
      },
      (error) => {
        dispatch(failure(error.toString()));
        dispatch(alertActions.error(error.toString()));
      }
    );
  };

  function request(group) {
    return { type: groupConstants.GROUP_CREATE_REQUEST, group };
  }
  function success(group) {
    return { type: groupConstants.GROUP_CREATE_SUCCESS, group };
  }
  function failure(error) {
    return { type: groupConstants.GROUP_CREATE_FAILURE, error };
  }
}

function updateGroup(group) {
  return (dispatch) => {
    dispatch(request(group));

    groupService.updateGroup(group).then(
      (group) => {
        dispatch(success());
        history.push("/groupList");
        dispatch(alertActions.success("Group updated successfully."));
      },
      (error) => {
        dispatch(failure(error.toString()));
        dispatch(alertActions.error(error.toString()));
      }
    );
  };

  function request(group) {
    return { type: groupConstants.GROUP_UPDATE_REQUEST, group };
  }
  function success(group) {
    return { type: groupConstants.GROUP_UPDATE_SUCCESS, group };
  }
  function failure(error) {
    return { type: groupConstants.GROUP_UPDATE_FAILURE, error };
  }
}

function editMember(input) {
  return (dispatch) => {
    dispatch(request(input));

    groupService.editMember(input).then(
      (input) => {
        dispatch(success());
        dispatch(getAll());
        dispatch(alertActions.success("Edited successfully"));
      },
      (error) => {
        dispatch(failure(error.toString()));
        dispatch(getAll());
        dispatch(alertActions.error(error.toString()));
      }
    );
  };

  function request(input) {
    return { type: groupConstants.EDIT_MEMBER_REQUEST, input };
  }
  function success(input) {
    return { type: groupConstants.EDIT_MEMBER_SUCCESS, input };
  }
  function failure(error) {
    return { type: groupConstants.EDIT_MEMBER_FAILURE, error };
  }
}

function leaveGroup(groupId) {
  return (dispatch) => {
    dispatch(request(groupId));

    groupService.leaveGroup(groupId).then(
      (groupId) => {
        dispatch(success());
        dispatch(getAll());
        dispatch(alertActions.success("Left successfully."));
      },
      (error) => {
        dispatch(failure(error.toString()));
        dispatch(getAll());
        dispatch(alertActions.error(error.toString()));
      }
    );
  };

  function request(groupId) {
    return { type: groupConstants.LEAVE_GROUP_REQUEST, groupId };
  }
  function success(groupId) {
    return { type: groupConstants.LEAVE_GROUP_SUCCESS, groupId };
  }
  function failure(error) {
    return { type: groupConstants.LEAVE_GROUP_FAILURE, error };
  }
}

function removeGroup(group) {
  return (dispatch) => {
    dispatch(request(group));

    groupService.removeGroup(group).then(
      (group) => {
        dispatch(success());
        //history.push("/groupList");
        dispatch(getAll());
        dispatch(alertActions.success("Group removed successfully."));
      },
      (error) => {
        dispatch(failure(error.toString()));
        dispatch(getAll());
        dispatch(alertActions.error(error.toString()));
      }
    );
  };

  function request(group) {
    return { type: groupConstants.REMOVE_GROUP_REQUEST, group };
  }
  function success(group) {
    return { type: groupConstants.REMOVE_GROUP_SUCCESS, group };
  }
  function failure(error) {
    return { type: groupConstants.REMOVE_GROUP_FAILURE, error };
  }
}

function joinGroup(group) {
  return (dispatch) => {
    dispatch(request(group));

    groupService.joinGroup(group).then(
      (group) => {
        dispatch(success());
        history.push("/groupList");
        dispatch(getAll());
        dispatch(alertActions.success("Group joined successfully."));
      },
      (error) => {
        dispatch(failure(error.toString()));
        dispatch(getAll());
        dispatch(alertActions.error(error.toString()));
      }
    );
  };

  function request() {
    return { type: groupConstants.GROUP_JOIN_REQUEST };
  }
  function success(groups) {
    return { type: groupConstants.GROUP_JOIN_SUCCESS, groups };
  }
  function failure(error) {
    return { type: groupConstants.GROUP_JOIN_FAILURE, error };
  }
}

function inviteGroup(group) {
  return (dispatch) => {
    dispatch(request(group));

    groupService.inviteGroup(group).then(
      (group) => {
        dispatch(success());
        history.push("/groupList");
        dispatch(getAll());
        dispatch(alertActions.success("Invited successfully."));
      },
      (error) => {
        dispatch(failure(error.toString()));
        dispatch(getAll());
        dispatch(alertActions.error(error.toString()));
      }
    );
  };

  function request() {
    return { type: groupConstants.GROUP_INVITE_REQUEST };
  }
  function success(groups) {
    return { type: groupConstants.GROUP_INVITE_SUCCESS, groups };
  }
  function failure(error) {
    return { type: groupConstants.GROUP_INVITE_FAILURE, error };
  }
}

function _removeFromGroup(input) {
  return (dispatch) => {
    dispatch(request(input));

    groupService._removeFromGroup(input).then(
      (input) => {
        dispatch(success());
        dispatch(getAll());
        dispatch(alertActions.success("Remove successful"));
      },
      (error) => {
        dispatch(failure(error.toString()));
        dispatch(getAll());
        dispatch(alertActions.error(error.toString()));
      }
    );
  };

  function request(input) {
    return { type: groupConstants.REMOVE_FROM_GROUP_REQUEST, input };
  }
  function success(input) {
    return { type: groupConstants.REMOVE_FROM_GROUP_SUCCESS, input };
  }
  function failure(error) {
    return { type: groupConstants.REMOVE_FROM_GROUP_FAILURE, error };
  }
}