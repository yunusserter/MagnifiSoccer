import { gameConstants } from "../_constants/gameConstants";
import { gameService } from "../_services/gameService";
import { alertActions } from "./alertActions";
import { history } from "../_helpers";

export const gameActions = {
  newGame,
  updateGame,
  getAll,
  editSquad,
  gameInvite,
  gameInviteResponse,
  playerRating,
  getRating,
  removeGame,
};

function newGame(game) {
  return (dispatch) => {
    dispatch(request(game));

    gameService.newGame(game).then(
      (game) => {
        dispatch(success());
        dispatch(alertActions.success("Game created successfully."));
      },
      (error) => {
        dispatch(failure(error.toString()));
        dispatch(alertActions.error(error.toString()));
      }
    );
  };

  function request(game) {
    return { type: gameConstants.GAME_CREATE_REQUEST, game };
  }
  function success(game) {
    return { type: gameConstants.GAME_CREATE_SUCCESS, game };
  }
  function failure(error) {
    return { type: gameConstants.GAME_CREATE_FAILURE, error };
  }
}

function updateGame(game) {
  return (dispatch) => {
    dispatch(request(game));

    gameService.updateGame(game).then(
      (game) => {
        dispatch(success());
        history.push("/gameList");
        dispatch(alertActions.success("Game updated successfully."));
      },
      (error) => {
        dispatch(failure(error.toString()));
        dispatch(alertActions.error(error.toString()));
      }
    );
  };

  function request(game) {
    return { type: gameConstants.GAME_UPDATE_REQUEST, game };
  }
  function success(game) {
    return { type: gameConstants.GAME_UPDATE_SUCCESS, game };
  }
  function failure(error) {
    return { type: gameConstants.GAME_UPDATE_FAILURE, error };
  }
}

function getAll() {
  return (dispatch) => {
    dispatch(request());

    gameService.getAll().then(
      (games) => dispatch(success(games)),
      (error) => dispatch(failure(error.toString()))
    );
  };

  function request() {
    return { type: gameConstants.GETALL_REQUEST };
  }
  function success(games) {
    return { type: gameConstants.GETALL_SUCCESS, games };
  }
  function failure(error) {
    return { type: gameConstants.GETALL_FAILURE, error };
  }
}

function editSquad(game) {
  return (dispatch) => {
    dispatch(request(game));

    gameService.editSquad(game).then(
      (game) => {
        dispatch(success());
        history.push("/gameList");
        dispatch(alertActions.success("Game updated successfully."));
      },
      (error) => {
        dispatch(failure(error.toString()));
        dispatch(alertActions.error(error.toString()));
      }
    );
  };

  function request(game) {
    return { type: gameConstants.GAME_SQUAD_REQUEST, game };
  }
  function success(game) {
    return { type: gameConstants.GAME_SQUAD_SUCCESS, game };
  }
  function failure(error) {
    return { type: gameConstants.GAME_SQUAD_FAILURE, error };
  }
}

function gameInvite(gameId) {
  return (dispatch) => {
    dispatch(request(gameId));

    gameService.gameInvite(gameId).then(
      (gameId) => {
        dispatch(success());
      },
      (error) => {
        dispatch(failure(error.toString()));
        dispatch(alertActions.error(error.toString()));
      }
    );
  };

  function request(gameId) {
    return { type: gameConstants.GAME_INVITE_REQUEST, gameId };
  }
  function success(gameId) {
    return { type: gameConstants.GAME_INVITE_SUCCESS, gameId };
  }
  function failure(error) {
    return { type: gameConstants.GAME_INVITE_FAILURE, error };
  }
}

function gameInviteResponse(game) {
  return (dispatch) => {
    dispatch(request(game));

    gameService.gameInviteResponse(game).then(
      (game) => {
        dispatch(success());
        dispatch(getAll());
      },
      (error) => {
        dispatch(failure(error.toString()));
        dispatch(alertActions.error(error.toString()));
      }
    );
  };

  function request(game) {
    return { type: gameConstants.GAME_INVITE_RESPONSE_REQUEST, game };
  }
  function success(game) {
    return { type: gameConstants.GAME_INVITE_RESPONSE_SUCCESS, game };
  }
  function failure(error) {
    return { type: gameConstants.GAME_INVITE_RESPONSE_FAILURE, error };
  }
}

function playerRating(game) {
  return (dispatch) => {
    dispatch(request(game));

    gameService.playerRating(game).then(
      (game) => {
        dispatch(success());
        history.push("/gameList");
        game.isSuccess === true
          ? dispatch(alertActions.success(game.message))
          : dispatch(alertActions.error(game.message));
      },
      (error) => {
        dispatch(failure(error.toString()));
        dispatch(alertActions.error(error.toString()));
      }
    );
  };

  function request(game) {
    return { type: gameConstants.GAME_RATING_REQUEST, game };
  }
  function success(game) {
    return { type: gameConstants.GAME_RATING_SUCCESS, game };
  }
  function failure(error) {
    return { type: gameConstants.GAME_RATING_FAILURE, error };
  }
}

function getRating() {
  return (dispatch) => {
    dispatch(request());

    gameService.getRating().then(
      (ratings) => dispatch(success(ratings)),
      (error) => dispatch(failure(error.toString()))
    );
  };

  function request() {
    return { type: gameConstants.GET_RATING_REQUEST };
  }
  function success(ratings) {
    return { type: gameConstants.GET_RATING_SUCCESS, ratings };
  }
  function failure(error) {
    return { type: gameConstants.GET_RATING_FAILURE, error };
  }
}

function removeGame(game) {
  return (dispatch) => {
    dispatch(request(game));

    gameService.removeGame(game).then(
      (game) => {
        dispatch(success());
        history.push("/gameList");
        dispatch(getAll());
        dispatch(alertActions.success("Game removed successfully."));
      },
      (error) => {
        dispatch(failure(error.toString()));
        dispatch(getAll());
        dispatch(alertActions.error(error.toString()));
      }
    );
  };

  function request(game) {
    return { type: gameConstants.REMOVE_GAME_REQUEST, game };
  }
  function success(game) {
    return { type: gameConstants.REMOVE_GAME_SUCCESS, game };
  }
  function failure(error) {
    return { type: gameConstants.REMOVE_GAME_FAILURE, error };
  }
}
