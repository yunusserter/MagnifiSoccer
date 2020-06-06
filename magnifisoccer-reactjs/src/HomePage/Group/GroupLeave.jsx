import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";

import { groupActions } from "../../_actions/groupActions";
import { Segment, Dropdown, Button, Form, Header } from "semantic-ui-react";

function GroupLeave() {
  const [submitted, setSubmitted] = useState(false);
  const leaving = useSelector((state) => state.groups.leaving);
  const groups = useSelector((state) => state.groups);
  const dispatch = useDispatch();

  const [inputs, setInputs] = useState({
    groupId: null
  });

  useEffect(() => {
    dispatch(groupActions.getAll());
  }, []);

  function handleChange(e, data) {
    const { name, value } = data;
    setInputs((inputs) => ({ ...inputs, [name]: value }));
  }

  function handleSubmit(e) {
    e.preventDefault();

    setSubmitted(true);
    if (inputs.groupId) {
      dispatch(groupActions.leaveGroup(inputs.groupId));
    }
  }

  return (
    <Segment raised>
      {groups.loading && <em>Loading groups...</em>}
      {groups.error && (
        <span className="text-danger">ERROR: {groups.error}</span>
      )}
      <Header as="h5">Leave group</Header>
      <Form>
        <Form.Field>
          <Dropdown
            placeholder="Please select group."
            selection
            fluid
            name="groupId"
            options={
              (groups.items &&
              groups.items.map(({ id, groupName }) => ({
                key: id,
                value: id,
                text: groupName,
              })))|| [{ key: "", text: "", value: "" }]
            }
            onChange={handleChange}
          />
        </Form.Field>

        <Form.Field>
          {inputs.groupId && (
            <Button onClick={handleSubmit} primary>
              {leaving && (
                <span className="spinner-border spinner-border-sm mr-1"></span>
              )}
              Leave from group
            </Button>
          )}
        </Form.Field>
      </Form>
    </Segment>
  );
}
export { GroupLeave };
