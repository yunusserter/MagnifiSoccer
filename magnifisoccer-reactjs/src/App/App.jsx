import React, { useEffect } from "react";
import { Router, Route, Switch, Redirect } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";

import { history } from "../_helpers/history";
import { alertActions } from "../_actions/alertActions";
import { PrivateRoute } from "../_components/PrivateRoute";
import { HomePage } from "../HomePage/HomePage";
import { LoginPage } from "../AuthPage/LoginPage";
import { RegisterPage } from "../AuthPage/RegisterPage";
import { ForgetPasswordPage } from "../AuthPage/ForgetPasswordPage";
import { GroupList } from "../HomePage/Group/GroupList";
import { Navbar } from "../_components/toolbox/Navbar";
import { Grid, GridRow } from "semantic-ui-react";
import { GroupKick } from "../HomePage/Group/GroupKick";
import { GroupNew } from "../HomePage/Group/GroupNew";
import { GroupUpdate } from "../HomePage/Group/GroupUpdate";
import { GroupEditMember } from "../HomePage/Group/GroupEditMember";
import { GroupLeave } from "../HomePage/Group/GroupLeave";
import { GroupRemove } from "../HomePage/Group/GroupRemove";
import { GroupInvite } from "../HomePage/Group/GroupInvite";
import { NavbarTop } from "../_components/toolbox/NavbarTop";
import { GameNew } from "../HomePage/Game/GameNew";
import { GameList } from "../HomePage/Game/GameList";
import { GameEdit } from "../HomePage/Game/GameEdit";
import { GameInviteList } from "../HomePage/Game/GameInviteList";
import { GameRating } from "../HomePage/Game/GameRating";

function App() {
  const alert = useSelector((state) => state.alert);
  const dispatch = useDispatch();

  useEffect(() => {
    history.listen((location, action) => {
      // clear alert on location change
      dispatch(alertActions.clear());
    });
  }, []);

  return (
    <div className="jumbotron">
      <Router history={history}>
        <Grid>
          <Grid.Row>
            <Route exact component={NavbarTop} />
          </Grid.Row>
          <Grid.Column width={3}>
            <Route exact path={"/"} component={Navbar} />
            <Route exact path="/groupList" component={Navbar} />
            <Route exact path="/newGroup" component={Navbar} />
            <Route exact path="/updateGroup" component={Navbar} />
            <Route exact path="/newGame" component={Navbar} />
            <Route exact path="/gameList" component={Navbar} />
          </Grid.Column>

          <Grid.Column width={10} fluid="true">
            {alert.message && (
              <div className={`alert ${alert.type}`}>{alert.message}</div>
            )}

            <Switch>
              <PrivateRoute exact path="/" component={HomePage} />
              <Route path="/login" component={LoginPage} />
              <Route path="/register" component={RegisterPage} />
              <Route path="/forgetPassword" component={ForgetPasswordPage} />
              <Route path="/groupList" component={GroupList} />
              <Route path="/newGroup" component={GroupNew} />
              <Route path="/updateGroup" component={GroupUpdate} />
              <Route path="/newGame" component={GameNew} />
              <Route path="/gameList" component={GameList} />
              <Route path="/gameEdit" component={GameEdit} />
              <Route path="/rating" component={GameRating} />
              <Redirect from="*" to="/" />
            </Switch>
          </Grid.Column>
          <Grid.Column width={3}>
            <Route exact path="/groupList" component={GroupInvite} />
            <Route exact path="/groupList" component={GroupKick} />
            <Route exact path="/groupList" component={GroupEditMember} />
            <Route exact path="/groupList" component={GroupLeave} />
            <Route exact path="/groupList" component={GroupRemove} />
            <Route exact path="/gameEdit" component={GameInviteList} />
          </Grid.Column>
        </Grid>
      </Router>
    </div>
  );
}

export { App };
