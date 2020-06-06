import config from "config";
import { authHeader } from "../_helpers/auth-header";

export const gameService = {
  newGame,
  updateGame,
  getAll,
  editSquad,
  gameInvite,
  playerRating,
  getRating,
  gameInviteResponse,
  removeGame,
};

function newGame(game) {
  const requestOptions = {
    method: "POST",
    headers: { ...authHeader(), "Content-Type": "application/json" },
    body: JSON.stringify(game),
  };

  return fetch(`${config.apiUrl}/api/games/create`, requestOptions).then(
    handleResponse
  );
}

function updateGame(game) {
  const requestOptions = {
    method: "PUT",
    headers: { ...authHeader(), "Content-Type": "application/json" },
    body: JSON.stringify(game),
  };

  return fetch(`${config.apiUrl}/api/games/update`, requestOptions).then(
    handleResponse
  );
}

function getAll() {
  const requestOptions = {
    method: "GET",
    headers: authHeader(),
  };

  return fetch(`${config.apiUrl}/api/games`, requestOptions).then(
    handleResponse
  );
}

function editSquad(game) {
  const requestOptions = {
    method: "PUT",
    headers: { ...authHeader(), "Content-Type": "application/json" },
    body: JSON.stringify(game),
  };

  return fetch(`${config.apiUrl}/api/games/editSquad`, requestOptions).then(
    handleResponse
  );
}

function gameInvite(gameId) {
  const requestOptions = {
    method: "GET",
    headers: authHeader(),
  };

  return fetch(
    `${config.apiUrl}/api/games/invite/${gameId}`,
    requestOptions
  ).then(handleResponse);
}

function gameInviteResponse(game) {
  const requestOptions = {
    method: "POST",
    headers: { ...authHeader(), "Content-Type": "application/json" },
    body: JSON.stringify(game),
  };

  return fetch(
    `${config.apiUrl}/api/games/inviteResponse`,
    requestOptions
  ).then(handleResponse);
}

function playerRating(game) {
  const requestOptions = {
    method: "POST",
    headers: { ...authHeader(), "Content-Type": "application/json" },
    body: JSON.stringify(game),
  };

  return fetch(`${config.apiUrl}/api/games/playerRating`, requestOptions).then(
    handleResponse
  );
}

function getRating() {
  const requestOptions = {
    method: "GET",
    headers: authHeader(),
  };

  return fetch(`${config.apiUrl}/api/games/rating`, requestOptions).then(
    handleResponse
  );
}

function removeGame(game) {
  const requestOptions = {
    method: "DELETE",
    headers: { ...authHeader(), "Content-Type": "application/json" },
    body: JSON.stringify(game),
  };

  return fetch(`${config.apiUrl}/api/games/remove`, requestOptions).then(
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
