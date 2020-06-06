import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";

import { groupActions } from "../../_actions/groupActions";
import { gameActions } from "../../_actions/gameActions";
import {
  Segment,
  Button,
  Form,
  Header,
  Icon,
  Input,
  Dropdown,
  Label,
} from "semantic-ui-react";
import { DateInput, TimeInput } from "semantic-ui-calendar-react";
import moment from "moment";

function GameNew() {
  const [submitted, setSubmitted] = useState(false);
  const creating = useSelector((state) => state.games.creating);
  const groups = useSelector((state) => state.groups);
  const dispatch = useDispatch();

  const [calendar, setCalendar] = useState({
    date: "",
    time: "",
  });

  const [inputs, setInputs] = useState({
    groupId: "",
    gameDate: "",
    location: "",
    price: 0,
  });

  useEffect(() => {
    dispatch(groupActions.getAll());
  }, []);

  function handleChange(e, data) {
    const { name, value } = data;
    setInputs((inputs) => ({ ...inputs, [name]: value }));
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
      dispatch(gameActions.newGame(inputs));
    }
  }

  return (
    <Segment placeholder raised>
      <Header as="h1" icon>
        <Icon name="plus" />
        Create Game
      </Header>

      <Form onSubmit={handleSubmit}>
        <Form.Field>
          <label>Group</label>
          <Dropdown
            placeholder="Please select group."
            fluid
            selection
            name="groupId"
            options={
              (groups.items &&
                groups.items.map(({ id, groupName }) => ({
                  key: id,
                  value: id,
                  text: groupName,
                }))) || [{ key: "", value: "", text: "" }]
            }
            onChange={handleChange}
          />
        </Form.Field>

        <Form.Field>
          <label>Date</label>
          <DateInput
            autoComplete="off"
            required
            name="date"
            placeholder="Date"
            value={calendar.date}
            dateFormat="YYYY-MM-DD"
            minDate={moment().format()}
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
          <Button
            primary
            onClick={handleChangeGameDate}
            disabled={inputs.groupId === ""}
          >
            {creating && (
              <span className="spinner-border spinner-border-sm mr-1"></span>
            )}
            Create
          </Button>
        </Form.Field>
      </Form>
    </Segment>
  );
}
export { GameNew };
