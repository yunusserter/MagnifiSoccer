import React from "react";
import { Segment, List, Icon } from "semantic-ui-react";

function GameInviteList(props) {
  var game = props.location.game;

  return (
    <Segment placeholder raised>
        <label>Invite List</label>
      <List>
        {game.gamePlayers &&
          game.gamePlayers.map((player) => (
            <List.Item key={player.userId}>
                 {player.inviteResponse===null ? (
                <Icon name="wait" />
              ):null}
              {player.inviteResponse===true ? (
                <Icon name="check" />
              ):null}
               {player.inviteResponse===false ? (
                <Icon name="x" />
              ):null}
              {player.user.firstName + " " + player.user.lastName}
            </List.Item>
          ))}
      </List>
    </Segment>
  );
}
export { GameInviteList };
