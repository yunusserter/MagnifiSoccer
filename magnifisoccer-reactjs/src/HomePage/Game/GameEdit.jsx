import React, { useEffect, useState, useContext } from "react";
import { useDispatch, useSelector } from "react-redux";
import { userActions } from "../../_actions/userActions";
import { gameActions } from "../../_actions/gameActions";
import { alertActions } from "../../_actions/alertActions";
import {
  Segment,
  Form,
  Button,
  Input,
  Label,
  Radio,
  Icon,
  Grid,
  Dropdown,
  Divider,
} from "semantic-ui-react";
import { DateInput, TimeInput } from "semantic-ui-calendar-react";

function GameEdit(props) {
  const [submitted, setSubmitted] = useState(false);
  const updating = useSelector((state) => state.games.updating);
  const squadEditing = useSelector((state) => state.games.squadEditing);
  var game = props.location.game;
  const users = useSelector((state) => state.users);
  const dispatch = useDispatch();

  const [calendar, setCalendar] = useState({
    date: game.gameDate.split("T", 1).toString(),
    time: game.gameDate.slice(11, 16).toString(),
  });

  const [inputs, setInputs] = useState({
    gameId: game.id,
    groupId: game.groupId,
    gameDate: game.gameDate,
    location: game.location,
    price: game.price,
    winnerTeam: game.winnerTeam,
  });

  const [temp, setTemp] = useState({
    team1UsersF:
      (game.gamePlayers &&
        game.gamePlayers.reduce(
          (a, o) =>
            o.position === 3 && o.team === "1" ? a.concat(o.userId) : a,
          []
        )) ||
      [],
    team1PositionsF:
      (game.gamePlayers &&
        game.gamePlayers.reduce(
          (a, o) => (o.position === 3 && o.team === "1" ? a.concat(3) : a),
          []
        )) ||
      [],

    team1UsersO:
      (game.gamePlayers &&
        game.gamePlayers.reduce(
          (a, o) =>
            o.position === 2 && o.team === "1" ? a.concat(o.userId) : a,
          []
        )) ||
      [],
    team1PositionsO:
      (game.gamePlayers &&
        game.gamePlayers.reduce(
          (a, o) => (o.position === 2 && o.team === "1" ? a.concat(2) : a),
          []
        )) ||
      [],

    team1UsersD:
      (game.gamePlayers &&
        game.gamePlayers.reduce(
          (a, o) =>
            o.position === 1 && o.team === "1" ? a.concat(o.userId) : a,
          []
        )) ||
      [],
    team1PositionsD:
      (game.gamePlayers &&
        game.gamePlayers.reduce(
          (a, o) => (o.position === 1 && o.team === "1" ? a.concat(1) : a),
          []
        )) ||
      [],

    team1UsersK:
      (game.gamePlayers &&
        game.gamePlayers.reduce(
          (a, o) =>
            o.position === 0 && o.team === "1" ? a.concat(o.userId) : a,
          []
        )) ||
      [],
    team1PositionsK:
      (game.gamePlayers &&
        game.gamePlayers.reduce(
          (a, o) => (o.position === 0 && o.team === "1" ? a.concat(0) : a),
          []
        )) ||
      [],

    team2UsersF:
      (game.gamePlayers &&
        game.gamePlayers.reduce(
          (a, o) =>
            o.position === 3 && o.team === "2" ? a.concat(o.userId) : a,
          []
        )) ||
      [],
    team2PositionsF:
      (game.gamePlayers &&
        game.gamePlayers.reduce(
          (a, o) => (o.position === 3 && o.team === "2" ? a.concat(3) : a),
          []
        )) ||
      [],

    team2UsersO:
      (game.gamePlayers &&
        game.gamePlayers.reduce(
          (a, o) =>
            o.position === 2 && o.team === "2" ? a.concat(o.userId) : a,
          []
        )) ||
      [],
    team2PositionsO:
      (game.gamePlayers &&
        game.gamePlayers.reduce(
          (a, o) => (o.position === 2 && o.team === "2" ? a.concat(2) : a),
          []
        )) ||
      [],

    team2UsersD:
      (game.gamePlayers &&
        game.gamePlayers.reduce(
          (a, o) =>
            o.position === 1 && o.team === "2" ? a.concat(o.userId) : a,
          []
        )) ||
      [],
    team2PositionsD:
      (game.gamePlayers &&
        game.gamePlayers.reduce(
          (a, o) => (o.position === 1 && o.team === "2" ? a.concat(1) : a),
          []
        )) ||
      [],

    team2UsersK:
      (game.gamePlayers &&
        game.gamePlayers.reduce(
          (a, o) =>
            o.position === 0 && o.team === "2" ? a.concat(o.userId) : a,
          []
        )) ||
      [],
    team2PositionsK:
      (game.gamePlayers &&
        game.gamePlayers.reduce(
          (a, o) => (o.position === 0 && o.team === "2" ? a.concat(0) : a),
          []
        )) ||
      [],
  });

  const [squad, setSquad] = useState({
    team1Users: [],
    team1Positions: [],
    team2Users: [],
    team2Positions: [],
    gameId: game.id,
    squadSize:
      temp.team1UsersF.length +
      temp.team1UsersO.length +
      temp.team1UsersD.length +
      temp.team1UsersK.length +
      temp.team2UsersF.length +
      temp.team2UsersO.length +
      temp.team2UsersD.length +
      temp.team2UsersK.length,
  });

  var members =
    users.items &&
    users.items.map((user) => ({
      key: user.userId,
      text: user.firstName+" "+user.lastName,
      value: user.userId,
    }));

  useEffect(() => {
    dispatch(userActions.getAllUsersForSquad());
  }, []);

  function handleChange(e, data) {
    const { name, value } = data;
    setInputs((inputs) => ({ ...inputs, [name]: value }));
  }

  function handleChangeSquadTeam1(e, data) {
    if (data.placeholder === "Striker") {
      setTemp((temp) => ({
        ...temp,
        team1UsersF: data.value,
        team1PositionsF: data.value.map((p) => 3),
      }));
    } else if (data.placeholder === "Midfield") {
      setTemp((temp) => ({
        ...temp,
        team1UsersO: data.value,
        team1PositionsO: data.value.map((p) => 2),
      }));
    } else if (data.placeholder === "Defance") {
      setTemp((temp) => ({
        ...temp,
        team1UsersD: data.value,
        team1PositionsD: data.value.map((p) => 1),
      }));
    } else if (data.placeholder === "Goal keeper") {
      setTemp((temp) => ({
        ...temp,
        team1UsersK: [data.value],
        team1PositionsK: [0],
      }));
    }
  }

  function handleChangeSquadTeam2(e, data) {
    if (data.placeholder === "Striker") {
      setTemp((temp) => ({
        ...temp,
        team2UsersF: data.value,
        team2PositionsF: data.value.map((p) => 3),
      }));
    } else if (data.placeholder === "Midfield") {
      setTemp((temp) => ({
        ...temp,
        team2UsersO: data.value,
        team2PositionsO: data.value.map((p) => 2),
      }));
    } else if (data.placeholder === "Defance") {
      setTemp((temp) => ({
        ...temp,
        team2UsersD: data.value,
        team2PositionsD: data.value.map((p) => 1),
      }));
    } else if (data.placeholder === "Goal keeper") {
      setTemp((temp) => ({
        ...temp,
        team2UsersK: [data.value],
        team2PositionsK: [0],
      }));
    }
  }

  function handleChangeSquadSize(e, data) {
    const { name, value } = data;
    setSquad((squad) => ({ ...squad, [name]: value }));
  }

  function handleSetSquad(e) {
    setSquad((squad) => ({
      ...squad,
      team1Users: [
        ...(temp.team1UsersF ? temp.team1UsersF : []),
        ...(temp.team1UsersO ? temp.team1UsersO : []),
        ...(temp.team1UsersD ? temp.team1UsersD : []),
        ...(temp.team1UsersK ? temp.team1UsersK : []),
      ],
      team1Positions: [
        ...(temp.team1PositionsF ? temp.team1PositionsF : []),
        ...(temp.team1PositionsO ? temp.team1PositionsO : []),
        ...(temp.team1PositionsD ? temp.team1PositionsD : []),
        ...(temp.team1PositionsK ? temp.team1PositionsK : []),
      ],
      team2Users: [
        ...(temp.team2UsersF ? temp.team2UsersF : []),
        ...(temp.team2UsersO ? temp.team2UsersO : []),
        ...(temp.team2UsersD ? temp.team2UsersD : []),
        ...(temp.team2UsersK ? temp.team2UsersK : []),
      ],
      team2Positions: [
        ...(temp.team2PositionsF ? temp.team2PositionsF : []),
        ...(temp.team2PositionsO ? temp.team2PositionsO : []),
        ...(temp.team2PositionsD ? temp.team2PositionsD : []),
        ...(temp.team2PositionsK ? temp.team2PositionsK : []),
      ],
    }));
  }

  function handleSetSquadRequest(e) {
    handleSetSquad;
    setSubmitted(true);

    if (
      squad.gameId &&
      squad.squadSize === squad.team1Users.length + squad.team2Users.length
    ) {
      dispatch(gameActions.editSquad(squad));
      if (game.winnerTeam === "") {
        dispatch(gameActions.gameInvite(squad.gameId));
      }
    } else {
      dispatch(alertActions.error("Check the number of staff."));
    }
  }

  function handleChangeForCalendar(e, data) {
    const { name, value } = data;
    setCalendar((calendar) => ({ ...calendar, [name]: value }));
  }

  function handleChangeGameDate() {
    if (calendar.time && calendar.date !== "") {
      setInputs((inputs) => {
        return {
          ...inputs,
          gameDate: calendar.date + "T" + calendar.time + ":00.000",
        };
      });
    }
  }

  function handleSubmit(e) {
    e.preventDefault();

    setSubmitted(true);
    if (inputs.groupId && inputs.gameDate && inputs.location && inputs.price) {
      dispatch(gameActions.updateGame(inputs));
    }
  }

  function handleRemoveGame(e) {
    e.preventDefault();

    setSubmitted(true);
    if (inputs.groupId && inputs.gameId) {
      dispatch(gameActions.removeGame(inputs));
    }
  }

  return (
    <Segment placeholder raised>
      <Grid>
        <Grid.Column width={8}>
          {game == null ? (
            <span className="text-danger">ERROR: Game model is null.</span>
          ) : (
            <Form onSubmit={handleSubmit}>
              <Form.Field>
                <label>Date</label>
                <DateInput
                  autoComplete="off"
                  required
                  name="date"
                  placeholder="Date"
                  value={calendar.date}
                  dateFormat="YYYY-MM-DD"
                  iconPosition="left"
                  onChange={handleChangeForCalendar}
                />
              </Form.Field>

              <Form.Field>
                <label>Time</label>
                <TimeInput
                  autoComplete="off"
                  required
                  name="time"
                  placeholder="Time"
                  value={calendar.time}
                  iconPosition="left"
                  onChange={handleChangeForCalendar}
                />
              </Form.Field>

              <Form.Field
                icon="map marker alternate"
                iconPosition="left"
                required
                fluid
                placeholder="Location"
                control={Input}
                type="text"
                name="location"
                label="Location"
                value={inputs.location}
                onChange={handleChange}
              />

              <Form.Field>
                <label>Price</label>
                <Input
                  autoComplete="off"
                  required
                  labelPosition="right"
                  type="number"
                  placeholder="Amount"
                  fluid
                  name="price"
                  value={inputs.price}
                  onChange={handleChange}
                  min={0}
                >
                  <Label basic>₺</Label>
                  <input />
                </Input>
              </Form.Field>

              <Form.Field>
                <label>Winner team</label>
                <Radio
                  label={game.homeTeam}
                  name="winnerTeam"
                  value={game.homeTeam}
                  checked={inputs.winnerTeam === game.homeTeam}
                  onChange={handleChange}
                  onClick={handleChange}
                />{" "}
                &nbsp; &nbsp; &nbsp;
                <Radio
                  label={game.awayTeam}
                  name="winnerTeam"
                  value={game.awayTeam}
                  checked={inputs.winnerTeam === game.awayTeam}
                  onChange={handleChange}
                  onClick={handleChange}
                />
              </Form.Field>

              <Form.Field>
                <Button primary onClick={handleChangeGameDate}>
                  <Icon name="edit" />
                  {updating && (
                    <span className="spinner-border spinner-border-sm mr-1"></span>
                  )}
                  Edit
                </Button>
              </Form.Field>

              <Form.Field>
                <Button negative onClick={handleRemoveGame}>
                  <Icon name="delete" />
                  Delete Game
                </Button>
              </Form.Field>
            </Form>
          )}
        </Grid.Column>
        <Grid.Column width={8}>
          <Form onSubmit={handleSetSquadRequest}>
            <Form.Field>
              <label>Squad size</label>
              <Radio
                label="5+5"
                name="squadSize"
                value={10}
                checked={squad.squadSize === 10}
                onChange={handleChangeSquadSize}
                onClick={handleChangeSquadSize}
              />
              &nbsp; &nbsp; &nbsp;
              <Radio
                label="6+6"
                name="squadSize"
                value={12}
                checked={squad.squadSize === 12}
                onChange={handleChangeSquadSize}
                onClick={handleChangeSquadSize}
              />
              &nbsp; &nbsp; &nbsp;
              <Radio
                label="7+7"
                name="squadSize"
                value={14}
                checked={squad.squadSize === 14}
                onChange={handleChangeSquadSize}
                onClick={handleChangeSquadSize}
              />
            </Form.Field>

            <Segment textAlign="center" inverted color="olive">
              <Label>Team 1</Label>
              <Dropdown
                placeholder="Goal keeper"
                fluid
                selection
                options={
                  (members &&
                    members.filter(
                      (p) =>
                        !temp.team1UsersF.includes(p.key) &&
                        !temp.team1UsersO.includes(p.key) &&
                        !temp.team1UsersD.includes(p.key) &&
                        !temp.team2UsersF.includes(p.key) &&
                        !temp.team2UsersO.includes(p.key) &&
                        !temp.team2UsersK.includes(p.key) &&
                        !temp.team2UsersD.includes(p.key)
                    )) || [{ key: "", text: "", value: "" }]
                }
                defaultValue={temp.team1UsersK[0]}
                onChange={handleChangeSquadTeam1}
              />

              <Dropdown
                placeholder="Defance"
                fluid
                multiple
                selection
                options={
                  (members &&
                    members.filter(
                      (p) =>
                        !temp.team1UsersF.includes(p.key) &&
                        !temp.team1UsersO.includes(p.key) &&
                        !temp.team1UsersK.includes(p.key) &&
                        !temp.team2UsersF.includes(p.key) &&
                        !temp.team2UsersO.includes(p.key) &&
                        !temp.team2UsersK.includes(p.key) &&
                        !temp.team2UsersD.includes(p.key)
                    )) || [{ key: "", text: "", value: "" }]
                }
                defaultValue={temp.team1UsersD}
                onChange={handleChangeSquadTeam1}
              />
              <Dropdown
                placeholder="Midfield"
                fluid
                multiple
                selection
                options={
                  (members &&
                    members.filter(
                      (p) =>
                        !temp.team1UsersF.includes(p.key) &&
                        !temp.team1UsersK.includes(p.key) &&
                        !temp.team1UsersD.includes(p.key) &&
                        !temp.team2UsersF.includes(p.key) &&
                        !temp.team2UsersO.includes(p.key) &&
                        !temp.team2UsersK.includes(p.key) &&
                        !temp.team2UsersD.includes(p.key)
                    )) || [{ key: "", text: "", value: "" }]
                }
                defaultValue={temp.team1UsersO}
                onChange={handleChangeSquadTeam1}
              />
              <Dropdown
                placeholder="Striker"
                fluid
                multiple
                selection
                options={
                  (members &&
                    members.filter(
                      (p) =>
                        !temp.team1UsersK.includes(p.key) &&
                        !temp.team1UsersO.includes(p.key) &&
                        !temp.team1UsersD.includes(p.key) &&
                        !temp.team2UsersF.includes(p.key) &&
                        !temp.team2UsersO.includes(p.key) &&
                        !temp.team2UsersK.includes(p.key) &&
                        !temp.team2UsersD.includes(p.key)
                    )) || [{ key: "", text: "", value: "" }]
                }
                defaultValue={temp.team1UsersF}
                onChange={handleChangeSquadTeam1}
              />

              <Divider horizontal inverted>
                O
              </Divider>

              <Dropdown
                placeholder="Striker"
                fluid
                multiple
                selection
                options={
                  (members &&
                    members.filter(
                      (p) =>
                        !temp.team2UsersK.includes(p.key) &&
                        !temp.team2UsersO.includes(p.key) &&
                        !temp.team2UsersD.includes(p.key) &&
                        !temp.team1UsersF.includes(p.key) &&
                        !temp.team1UsersO.includes(p.key) &&
                        !temp.team1UsersK.includes(p.key) &&
                        !temp.team1UsersD.includes(p.key)
                    )) || [{ key: "", text: "", value: "" }]
                }
                defaultValue={temp.team2UsersF}
                onChange={handleChangeSquadTeam2}
              />
              <Dropdown
                placeholder="Midfield"
                fluid
                multiple
                selection
                options={
                  (members &&
                    members.filter(
                      (p) =>
                        !temp.team2UsersK.includes(p.key) &&
                        !temp.team2UsersF.includes(p.key) &&
                        !temp.team2UsersD.includes(p.key) &&
                        !temp.team1UsersF.includes(p.key) &&
                        !temp.team1UsersO.includes(p.key) &&
                        !temp.team1UsersK.includes(p.key) &&
                        !temp.team1UsersD.includes(p.key)
                    )) || [{ key: "", text: "", value: "" }]
                }
                defaultValue={temp.team2UsersO}
                onChange={handleChangeSquadTeam2}
              />
              <Dropdown
                placeholder="Defance"
                fluid
                multiple
                selection
                options={
                  (members &&
                    members.filter(
                      (p) =>
                        !temp.team2UsersK.includes(p.key) &&
                        !temp.team2UsersO.includes(p.key) &&
                        !temp.team2UsersF.includes(p.key) &&
                        !temp.team1UsersF.includes(p.key) &&
                        !temp.team1UsersO.includes(p.key) &&
                        !temp.team1UsersK.includes(p.key) &&
                        !temp.team1UsersD.includes(p.key)
                    )) || [{ key: "", text: "", value: "" }]
                }
                defaultValue={temp.team2UsersD}
                onChange={handleChangeSquadTeam2}
              />
              <Dropdown
                placeholder="Goal keeper"
                fluid
                selection
                options={
                  (members &&
                    members.filter(
                      (p) =>
                        !temp.team2UsersF.includes(p.key) &&
                        !temp.team2UsersO.includes(p.key) &&
                        !temp.team2UsersD.includes(p.key) &&
                        !temp.team1UsersF.includes(p.key) &&
                        !temp.team1UsersO.includes(p.key) &&
                        !temp.team1UsersK.includes(p.key) &&
                        !temp.team1UsersD.includes(p.key)
                    )) || [{ key: "", text: "", value: "" }]
                }
                defaultValue={temp.team2UsersK[0]}
                onChange={handleChangeSquadTeam2}
              />
              <Label>Team 2</Label>
            </Segment>
            <Form.Field>
              <Button primary onClick={handleSetSquad}>
                {squadEditing && (
                  <span className="spinner-border spinner-border-sm mr-1"></span>
                )}
                SAVE
              </Button>
            </Form.Field>
          </Form>
        </Grid.Column>
      </Grid>
    </Segment>
  );
}
export { GameEdit };
