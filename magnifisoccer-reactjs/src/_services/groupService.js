import config from "config";
import { authHeader } from "../_helpers/auth-header";

export const groupService = {
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
  const requestOptions = {
    method: "GET",
    headers: authHeader(),
  };

  return fetch(`${config.apiUrl}/api/groups/${groupId}`, requestOptions).then(
    handleResponse
  );
}

function getAll() {
  const requestOptions = {
    method: "GET",
    headers: authHeader(),
  };

  return fetch(`${config.apiUrl}/api/groups`, requestOptions).then(
    handleResponse
  );
}

function getAllForSearch() {
  const requestOptions = {
    method: "GET",
    headers: authHeader(),
  };

  return fetch(`${config.apiUrl}/api/groups/all`, requestOptions).then(
    handleResponse
  );
}

function newGroup(group) {
  const requestOptions = {
    method: "POST",
    headers: { ...authHeader() },
    body: group,
  };

  return fetch(`${config.apiUrl}/api/groups/create`, requestOptions).then(
    handleResponse
  );
}

function updateGroup(group) {
  const requestOptions = {
    method: "PUT",
    headers: { ...authHeader() },
    body: group,
  };

  return fetch(`${config.apiUrl}/api/groups/update`, requestOptions).then(
    handleResponse
  );
}

function editMember(input) {
  const requestOptions = {
    method: "PUT",
    headers: { ...authHeader(), "Content-Type": "application/json" },
    body: JSON.stringify(input),
  };

  return fetch(`${config.apiUrl}/api/groups/editMember`, requestOptions).then(
    handleResponse
  );
}

function leaveGroup(groupId) {
  const requestOptions = {
    method: "GET",
    headers: authHeader(),
  };

  return fetch(
    `${config.apiUrl}/api/groups/leave/${groupId}`,
    requestOptions
  ).then(handleResponse);
}

function removeGroup(group) {
  const requestOptions = {
    method: "DELETE",
    headers: { ...authHeader(), "Content-Type": "application/json" },
    body: JSON.stringify(group),
  };

  return fetch(`${config.apiUrl}/api/groups/removeGroup`, requestOptions).then(
    handleResponse
  );
}

function joinGroup(group) {
  const requestOptions = {
    method: "POST",
    headers: { ...authHeader(), "Content-Type": "application/json" },
    body: JSON.stringify(group),
  };

  return fetch(`${config.apiUrl}/api/groups/join`, requestOptions).then(
    handleResponse
  );
}

function inviteGroup(group) {
  const requestOptions = {
    method: "POST",
    headers: { ...authHeader(), "Content-Type": "application/json" },
    body: JSON.stringify(group),
  };

  return fetch(`${config.apiUrl}/api/groups/invite`, requestOptions).then(
    handleResponse
  );
}

function _removeFromGroup(inputs) {
  const requestOptions = {
    method: "DELETE",
    headers: { ...authHeader(), "Content-Type": "application/json" },
    body: JSON.stringify(inputs),
  };

  return fetch(`${config.apiUrl}/api/groups/kick`, requestOptions).then(
    handleResponse
  );
}

function handleResponse(response) {
  return response.text().then((text) => {
    const data = text && JSON.parse(text);
    if (!response.ok) {
      if (response.status === 401) {
        // auto logout if 401 response returned from api
        logout();
        location.reload(true);
      }

      const error = (data && data.message) || response.statusText;
      return Promise.reject(error);
    }

    return data;
  });
}
