import { userConstants } from "../_constants/userConstants";
import { userService } from "../_services/userService";
import { alertActions } from "./alertActions";
import { history } from "../_helpers";

export const userActions = {
  login,
  logout,
  register,
  forgetPassword,
  getAllUsersForSquad
};

function login(username, password) {
  return (dispatch) => {
    dispatch(request({ username }));

    userService.login(username, password).then(
      (user) => {
        dispatch(success(user));
        history.push("/");
      },
      (error) => {
        dispatch(failure(error.toString()));
        dispatch(alertActions.error(error.toString()));
      }
    );
  };

  function request(user) {
    return { type: userConstants.LOGIN_REQUEST, user };
  }
  function success(user) {
    return { type: userConstants.LOGIN_SUCCESS, user };
  }
  function failure(error) {
    return { type: userConstants.LOGIN_FAILURE, error };
  }
}

function logout() {
  userService.logout();
  return { type: userConstants.LOGOUT };
}

function register(user) {
  return (dispatch) => {
    console.log("register");
    dispatch(request(user));

    userService.register(user).then(
      (user) => {
        dispatch(success());
        history.push("/login");
        dispatch(alertActions.success("Registration successful"));
      },
      (error) => {
        dispatch(failure(error.toString()));
        dispatch(alertActions.error(error.toString()));
      }
    );
  };

  function request(user) {
    return { type: userConstants.REGISTER_REQUEST, user };
  }
  function success(user) {
    return { type: userConstants.REGISTER_SUCCESS, user };
  }
  function failure(error) {
    return { type: userConstants.REGISTER_FAILURE, error };
  }
}

function forgetPassword(email) {
  return (dispatch) => {
    dispatch(request(email));

    userService.forgetPassword(email).then(
      (email) => {
        dispatch(success());
        history.push("/login");
        dispatch(alertActions.success(email.message.toString()));
      },
      (error) => {
        dispatch(failure(error.toString()));
        dispatch(alertActions.error("Registered e-mail not found."));
      }
    );
  };

  function request(email) {
    return { type: userConstants.FORGET_PASSWORD_REQUEST, email };
  }
  function success(email) {
    return { type: userConstants.FORGET_PASSWORD_SUCCESS, email };
  }
  function failure(error) {
    return { type: userConstants.FORGET_PASSWORD_FAILURE, error };
  }
}

function getAllUsersForSquad() {
  return (dispatch) => {
    dispatch(request());

    userService.getAllUsersForSquad().then(
      (users) => dispatch(success(users)),
      (error) => dispatch(failure(error.toString()))
    );
  };

  function request() {
    return { type: userConstants.GETALL_FOR_SQUAD_REQUEST };
  }
  function success(users) {
    return { type: userConstants.GETALL_FOR_SQUAD_SUCCESS, users };
  }
  function failure(error) {
    return { type: userConstants.GETALL_FOR_SQUAD_FAILURE, error };
  }
}