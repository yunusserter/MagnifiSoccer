import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";

import { groupActions } from "../../_actions/groupActions";
import { List, Image, Segment, Dimmer, Loader, Label } from "semantic-ui-react";

function GroupList() {
  const groups = useSelector((state) => state.groups);
  const dispatch = useDispatch();
  const [inputs, setInputs] = useState({
    selectedGroup: "",
  });
  const { selectedGroup } = inputs;

  function handleItemOpen(e, data) {
    setInputs((inputs) => {
      return {
        ...inputs,
        selectedGroup: data.value,
      };
    });
  }
  function handleItemClose(e) {
    setInputs((inputs) => {
      return {
        ...inputs,
        selectedGroup: "",
      };
    });
  }

  useEffect(() => {
    dispatch(groupActions.getAll());
  }, []);

  return (
    <Segment raised>
      {groups.loading && (
        <Segment basic>
          <Dimmer active inverted>
            <Loader size="medium">Loading</Loader>
          </Dimmer>
          <Image src="../../../Upload/short-paragraph.png" />
        </Segment>
      )}
      {groups.error && (
        <span className="text-danger">ERROR: {groups.error}</span>
      )}
      <List selection style={{ verticalAlign: "middle" }}>
        {groups.items &&
          groups.items.map((group) => (
            <List.Item              
              key={group.id}
              onClick={
                selectedGroup !== group.id ? handleItemOpen : handleItemClose
              }
              value={group.id}
            >
              <Image avatar src={"/Upload/" + group.photoUrl} />

              <List.Content>
                <List.Header>{group.groupName}</List.Header>
              </List.Content>
              <List.Content floated="right">
                <List.Icon
                  name={selectedGroup === group.id ? "angle up" : "angle down"}
                  size="large"
                />
              </List.Content>

              <List.List>
                {group.groupMembers &&
                  group.groupMembers.map((member) => (
                    <List.Item
                      key={member.userId}
                      hidden={selectedGroup !== group.id}
                    >
                      <Image avatar src="../../../Upload/christian.jpg" />
                      <List.Content>
                        <List.Header>
                          {member.firstName} {member.lastName}
                        </List.Header>
                      </List.Content>

                      <List.Content
                        floated="right"
                        hidden={member.role !== "Admin"}
                      >
                        <List.Header>
                          <Label>{member.role}</Label>
                        </List.Header>
                      </List.Content>
                    </List.Item>
                  ))}
              </List.List>
            </List.Item>
          ))}
      </List>
    </Segment>
  );
}
export { GroupList };
