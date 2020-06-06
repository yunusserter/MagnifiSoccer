import React from "react";
import { useSelector } from "react-redux";
import { Message } from "semantic-ui-react";

function HomePage() {
  const user = useSelector((state) => state.authentication.user.user);

  return (
    <Message>
      <Message.Header>Hi {user.firstName}</Message.Header>
      <Message.List>
        <Message.Item>
          You can create a group, invite a friend or access other group
          operations from the group menu.
        </Message.Item>
        <Message.Item>
          You can create a new event from the game menu, edit it or browse the
          games you participate in.
        </Message.Item>
      </Message.List>
    </Message>
  );
}

export { HomePage };
