import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";

import { gameActions } from "../../_actions/gameActions";
import {
  Segment,
  Grid,
  Card,
  Dimmer,
  Loader,
  Image,
  List,
  Button,
  Icon,
  Label,
  Header,
} from "semantic-ui-react";
import { Link } from "react-router-dom";

function GameList() {
  const games = useSelector((state) => state.games);
  const ratings = useSelector((state) => state.ratings);
  const user = useSelector((state) => state.authentication.user.user);
  const dispatch = useDispatch();

  useEffect(() => {
    dispatch(gameActions.getAll());
    dispatch(gameActions.getRating());
  }, []);

  function handleAgree(e, data) {
    dispatch(
      gameActions.gameInviteResponse({
        gameId: data.value.gameId,
        response: true,
      })
    );
  }
  
  function handleNegative(e, data) {
    dispatch(
      gameActions.gameInviteResponse({
        gameId: data.value.gameId,
        response: false,
      })
    );
  }

  return (
    <Segment raised>
      {games.loading && (
        <Segment basic>
          <Dimmer active inverted>
            <Loader size="medium">Loading</Loader>
          </Dimmer>
          <Image src="../../../Upload/short-paragraph.png" />
        </Segment>
      )}
      {games.error && <span className="text-danger">ERROR: {games.error}</span>}
      {games.items &&
        games.items.map((game) => (
          <Card key={game.id} fluid>
            <Grid divided>
              <Grid.Row textAlign="center" stretched></Grid.Row>
              <Grid.Row textAlign="center" stretched>
                <Grid.Column width={6}>
                  <Header size="large">TEAM: {game.homeTeam}</Header>
                </Grid.Column>
                <Grid.Column width={4}>
                  <Label size="large" color={game.winnerTeam ? "green" : null}>
                    {game.winnerTeam
                      ? "WİNNER: " + game.winnerTeam
                      : "Not yet played."}
                  </Label>
                </Grid.Column>
                <Grid.Column width={6}>
                  <Header size="large">TEAM: {game.awayTeam}</Header>
                </Grid.Column>
              </Grid.Row>

              <Grid.Row textAlign="center" stretched>
                <Grid.Column width={6}>
                  <p>
                    <Icon name="map marker alternate"></Icon>
                    {game.location}
                  </p>
                </Grid.Column>
                <Grid.Column width={5}>
                  <p>
                    <Icon name="calendar"></Icon>
                    {game.gameDate.replace("T", " ")}
                  </p>
                </Grid.Column>
                <Grid.Column width={5}>{"₺" + game.price}</Grid.Column>
              </Grid.Row>

              <Grid.Row textAlign="center" stretched>
                <Grid.Column width={6}>
                  <List>
                    {game.gamePlayers.map(
                      (player) =>
                        player.team === "1" && (
                          <List.Item key={player.userId}>
                            {player.user.firstName + " " + player.user.lastName}
                          </List.Item>
                        )
                    )}
                  </List>
                </Grid.Column>
                <Grid.Column width={2}>
                  {game.gamePlayers.map(
                    (player) =>
                      player.team === "1" && (
                        <List.Item key={player.userId}>
                          {ratings.items &&
                            ratings.items
                              .filter(
                                (rating) =>
                                  player.gameId === rating.gameId &&
                                  rating.userId === player.userId
                              )
                              .reduce((avarage, rating, index, array) => {
                                avarage += rating.ratingValue / array.length;
                                return parseFloat(avarage.toFixed(1));
                              }, null)}
                        </List.Item>
                      )
                  )}
                </Grid.Column>
                <Grid.Column width={2}>
                  {game.gamePlayers.map(
                    (player) =>
                      player.team === "2" && (
                        <List.Item key={player.userId}>
                          {ratings.items &&
                            ratings.items
                              .filter(
                                (rating) =>
                                  player.gameId === rating.gameId &&
                                  rating.userId === player.userId
                              )
                              .reduce((avarage, rating, index, array) => {
                                avarage += rating.ratingValue / array.length;
                                return parseFloat(avarage.toFixed(1));
                              }, null)}
                        </List.Item>
                      )
                  )}
                </Grid.Column>
                <Grid.Column width={6}>
                  <List>
                    {game.gamePlayers.map(
                      (player) =>
                        player.team === "2" && (
                          <List.Item key={player.userId}>
                            {player.user.firstName + " " + player.user.lastName}
                          </List.Item>
                        )
                    )}
                  </List>
                </Grid.Column>
              </Grid.Row>

              {game.winnerTeam === "" &&
                game.gamePlayers.map(
                  (player) =>
                    user.id === player.userId && (
                      <Grid.Row textAlign="center">
                        <Grid.Column>
                          <label key={player.userId}>
                            Your invitation reply:{" "}
                            {player.inviteResponse === null ? (
                              <div>
                                <Button
                                  positive
                                  value={player}
                                  onClick={handleAgree}
                                >
                                  <Icon name="check"></Icon>Agree
                                </Button>
                                <Button
                                  negative
                                  value={player}
                                  onClick={handleNegative}
                                >
                                  <Icon name="x"></Icon>Negative
                                </Button>
                              </div>
                            ) : null}
                            {player.inviteResponse === true ? (
                              <Label>Agreed</Label>
                            ) : null}
                            {player.inviteResponse === false ? (
                              <Label>Negative</Label>
                            ) : null}
                          </label>
                        </Grid.Column>
                      </Grid.Row>
                    )
                )}

              {game.winnerTeam !== "" && (
                <Grid.Row textAlign="center">
                  <Grid.Column>
                    <Label>
                      The ratings may change when players vote. If your rating
                      is 0, no votes have been cast yet.
                    </Label>
                  </Grid.Column>
                </Grid.Row>
              )}

              {game.winnerTeam !== null && (
                <Grid.Row textAlign="center">
                  <Grid.Column>
                    <Link
                      to={{
                        pathname: "/rating",
                        game: game,
                      }}
                    >
                      <Button
                        hidden={
                          ratings.items &&
                          ratings.items.find((r) => r.voterUserId === user.id)
                        }
                      >
                        Rating
                      </Button>
                    </Link>
                  </Grid.Column>
                </Grid.Row>
              )}

              {game.group.groupMembers &&
              game.group.groupMembers.find(
                (member) => member.userId === user.id && member.role === "Admin"
              ) ? (
                <Grid.Row textAlign="center">
                  <Grid.Column>
                    <Link
                      to={{
                        pathname: "/gameEdit",
                        game: game,
                      }}
                    >
                      <Button primary>
                        <Icon name="edit" />
                        Edit
                      </Button>
                    </Link>
                  </Grid.Column>
                </Grid.Row>
              ) : null}
              <Grid.Row textAlign="center">
                <Grid.Column></Grid.Column>
              </Grid.Row>
            </Grid>
          </Card>
        ))}
    </Segment>
  );
}
export { GameList };
