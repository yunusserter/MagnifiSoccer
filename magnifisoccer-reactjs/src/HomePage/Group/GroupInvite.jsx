import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";

import { groupActions } from "../../_actions/groupActions";
import { Segment, Button, Form, Header, Dropdown } from "semantic-ui-react";

function GroupInvite() {
  const [submitted, setSubmitted] = useState(false);
  const invite = useSelector((state) => state.groups.invite);
  const groups = useSelector((state) => state.groups);
  const dispatch = useDispatch();

  const [inputs, setInputs] = useState({
    groupId: "",
    email: "",
  });

  useEffect(() => {
    dispatch(groupActions.getAll());
  }, []);

  function handleChange(e, data) {
    const { name, value } = data;
    setInputs((inputs) => ({ ...inputs, [name]: value }));
  }

  function handleRemoveFromGroup(e) {
    e.preventDefault();

    setSubmitted(true);
    if (inputs.email && inputs.groupId) {
      dispatch(groupActions.inviteGroup(inputs));
    }
  }

  return (
    <Segment raised>
      {groups.loading && <em>Loading groups...</em>}
      {groups.error && (
        <span className="text-danger">ERROR: {groups.error}</span>
      )}
      <Header as="h5">Invite member</Header>
      <Form>
        <Form.Field>
          <Form.Input
            placeholder="Email"
            type="email"
            name="email"
            value={inputs.email}
            onChange={handleChange}
          />
        </Form.Field>
        <Form.Field>
          <Dropdown
            hidden={inputs.email === ""}
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
          {inputs.email && inputs.groupId && (
            <Button onClick={handleRemoveFromGroup} primary>
              {invite && (
                <span className="spinner-border spinner-border-sm mr-1"></span>
              )}
              Invite
            </Button>
          )}
        </Form.Field>
      </Form>
    </Segment>
  );
}
export { GroupInvite };
