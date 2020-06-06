import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";

import { groupActions } from "../../_actions/groupActions";
import { Segment, Dropdown, Button, Form, Header } from "semantic-ui-react";

function GroupRemove() {
  const [submitted, setSubmitted] = useState(false);
  const removing = useSelector((state) => state.groups.removing);
  const groups = useSelector((state) => state.groups);
  const dispatch = useDispatch();

  const [inputs, setInputs] = useState({
    groupId: null,
  });

  useEffect(() => {
    dispatch(groupActions.getAll());
  }, []);

  function handleChange(e, data) {
    const { name, value } = data;
    setInputs((inputs) => ({ ...inputs, [name]: value }));
  }

  function handleRemoveGroup(e) {
    e.preventDefault();

    setSubmitted(true);
    if (inputs.groupId) {
      dispatch(groupActions.removeGroup(inputs));
    }
  }

  return (
    <Segment raised>
      {groups.loading && <em>Loading groups...</em>}
      {groups.error && (
        <span className="text-danger">ERROR: {groups.error}</span>
      )}
      <Header as="h5">Remove Group </Header>
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
                }))) || [{ key: "", text: "", value: "" }]
            }
            onChange={handleChange}
          />
        </Form.Field>

        <Form.Field>
          {inputs.groupId && (
            <Button onClick={handleRemoveGroup} primary>
              {removing && (
                <span className="spinner-border spinner-border-sm mr-1"></span>
              )}
              Remove group
            </Button>
          )}
        </Form.Field>
      </Form>
    </Segment>
  );
}
export { GroupRemove };
