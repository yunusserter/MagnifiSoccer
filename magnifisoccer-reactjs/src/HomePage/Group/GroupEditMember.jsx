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

function GroupEditMember() {
  const [submitted, setSubmitted] = useState(false);
  const editing = useSelector((state) => state.groups.editing);
  const groups = useSelector((state) => state.groups);
  const dispatch = useDispatch();

  const [inputs, setInputs] = useState({
    groupId: null,
    userId: "",
    role: "",
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
    if (inputs.userId && inputs.groupId) {
      dispatch(groupActions.editMember(inputs));
    }
  }

  var group =
    groups.items && groups.items.find((group) => group.id === inputs.groupId);

  var member =
    group &&
    group.groupMembers.find((member) => member.userId === inputs.userId);

  inputs.role = member && member.role === "User" ? "Admin" : "User";

  return (
    <Segment raised>
      {groups.loading && <em>Loading groups...</em>}
      {groups.error && (
        <span className="text-danger">ERROR: {groups.error}</span>
      )}
      <Header as="h5">Edit member</Header>
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
              })))||[{key:"",value:"",text:""}]
            }
            onChange={handleChange}
          />
        </Form.Field>
        <Form.Field>
          <Dropdown
            hidden={inputs.groupId === null}
            placeholder="Please select user."
            selection
            fluid
            name="userId"
            options={
              inputs.groupId &&
              group &&
              group.groupMembers.map(({ userId, firstName, lastName }) => ({
                key: userId,
                value: userId,
                text: firstName + " " + lastName,
              }))
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
                  <Button onClick={handleSubmit} primary>
                    {editing && (
                      <span className="spinner-border spinner-border-sm mr-1"></span>
                    )}
                    Make {member.role === "User" ? "Admin" : "User"}
                  </Button>
                )
            )}
        </Form.Field>
      </Form>
    </Segment>
  );
}
export { GroupEditMember };
