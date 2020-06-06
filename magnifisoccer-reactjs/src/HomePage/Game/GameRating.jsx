import React, { useEffect, useState, useContext } from "react";
import { useDispatch, useSelector } from "react-redux";
import { userActions } from "../../_actions/userActions";
import { gameActions } from "../../_actions/gameActions";
import { Segment, Form, Button, Input, Header, Icon } from "semantic-ui-react";

function GameRating(props) {
  const [submitted, setSubmitted] = useState(false);
  const rating = useSelector((state) => state.games.rating);
  var game = props.location.game;
  const dispatch = useDispatch();
  const currentUser = useSelector((state) => state.authentication.user.user);

  var team =
    game &&
    game.gamePlayers &&
    game.gamePlayers.find((player) => player.userId === currentUser.id).team;

  const [inputs, setInputs] = useState({
    gameId: game ? game.id : "",
    userId:
      (game &&
        game.gamePlayers &&
        game.gamePlayers
          .filter((player) => player.team === team)
          .reduce(
            (a, o) => a.concat(o.userId).filter((p) => p !== currentUser.id),
            []
          )) ||
      [],
    rating:
      (game &&
        game.gamePlayers &&
        game.gamePlayers
          .filter((player) => player.team === team)
          .reduce((a, o) => a.concat(null), [])) ||
      [],
  });

  useEffect(() => {    
    inputs.rating.pop();
  }, []);

  function handleChange(e, data) {
    if (data.value > 3 && data.value < 11) {
      let temp = { ...inputs };
      temp.rating[inputs.userId.findIndex((i) => i === data.name)] = parseInt(
        data.value
      );
      setInputs(temp);
    }
  }

  function handleSubmit(e) {
    e.preventDefault();

    setSubmitted(true);
    if (inputs.gameId) {
      dispatch(gameActions.playerRating(inputs));
    }
  }
  
  return (
    <Segment placeholder raised>
      <Form onSubmit={handleSubmit}>
        <Header as="h2" icon textAlign="center">
          <Icon name="futbol" />
          Player ratings
        </Header>
        {inputs.userId.map((userId) => (
          <Form.Field>
            <Input
              name={userId}
              type="number"
              label={{
                tag: true,
                content:
                  game.gamePlayers &&
                  game.gamePlayers.find((player) => player.userId === userId)
                    .user.firstName +
                    " " +
                    game.gamePlayers.find((player) => player.userId === userId)
                      .user.lastName,
              }}
              labelPosition="right"
              placeholder="Enter rate value(4-10)"
              onChange={handleChange}
              min={4}
              max={10}
            />
          </Form.Field>
        ))}

        <Form.Field>
          <Button
            primary
            disabled={inputs.rating.find((p) => p < 3 || p > 11) === null}
          >
            {rating && (
              <span className="spinner-border spinner-border-sm mr-1"></span>
            )}
            RATE
          </Button>
        </Form.Field>
      </Form>
    </Segment>
  );
}
export { GameRating };
