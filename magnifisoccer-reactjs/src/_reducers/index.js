import { combineReducers } from "redux";

import { authentication } from "./authenticationReducer";
import { registration } from "./registrationReducer";
import { users } from "./usersReducer";
import { groups } from "./groupsReducer";
import { alert } from "./alertReducer";
import { search } from "./searchReducer";
import { games } from "./gamesReducers";
import { ratings } from "./ratingsReducer";

const rootReducer = combineReducers({
  authentication,
  registration,
  users,
  groups,
  games,
  alert,
  search,
  ratings
});

export default rootReducer;
