import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";

import { groupActions } from "../../_actions/groupActions";
import {
  Segment,
  Dropdown,
  List,
  Button,
  Form,
  Header,
} from "semantic-ui-react";

function GroupKick() {
  const [submitted, setSubmitted] = useState(false);
  const removing = useSelector((state) => state.groups.removing);
  const groups = useSelector((state) => state.groups);
  const dispatch = useDispatch();

  const [inputs, setInputs] = useState({
    groupId: null,
    userId: null,
    role: "",
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
    if (inputs.userId && inputs.groupId) {
      dispatch(groupActions._removeFromGroup(inputs));
    }
  }

  var group =
    groups.items && groups.items.find((group) => group.id === inputs.groupId);

  return (
    <Segment raised>
      {groups.loading && <em>Loading groups...</em>}
      {groups.error && (
        <span className="text-danger">ERROR: {groups.error}</span>
      )}
      <Header as="h5">Kick member</Header>
      <Form>
        <Form.Field>
          <Dropdown
            placeholder="Please select group."
            fluid
            selection
            name="groupId"
            options={
              groups.items &&
              groups.items.map(({ id, groupName }) => ({
                key: id,
                value: id,
                text: groupName,
              }))|| [{ key: "", text: "", value: "" }]
            }
            onChange={handleChange}
          />
        </Form.Field>
        <Form.Field>
          <Dropdown
            hidden={inputs.groupId === null}
            placeholder="Please select user."
            fluid
            selection
            name="userId"
            options={
              (inputs.groupId &&
                group &&
                group.groupMembers.map(({ userId, firstName, lastName }) => ({
                  key: userId,
                  value: userId,
                  text: firstName + " " + lastName,
                }))) || [{ key: "", text: "", value: "" }]
            }
            onChange={handleChange}
          />
        </Form.Field>
        <Form.Field>
          {inputs.userId &&
            group &&
            group.groupMembers.map(
              (member) =>
                member.userId === inputs.userId && (
                  <Button onClick={handleRemoveFromGroup} primary>
                    {removing && (
                      <span className="spinner-border spinner-border-sm mr-1"></span>
                    )}
                    Kick from group
                  </Button>
                )
            )}
        </Form.Field>
      </Form>
    </Segment>
  );
}
export { GroupKick };
