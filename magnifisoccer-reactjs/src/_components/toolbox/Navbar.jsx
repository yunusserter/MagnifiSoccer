import React from "react";
import { Menu, Icon } from "semantic-ui-react";
import { Link } from "react-router-dom";
import { SearchBox } from "./SearchBox";

function Navbar() {
  return (
    <Menu vertical>
      <Menu.Item>
        <Menu.Header>
          <Icon name="group" />
          Groups
        </Menu.Header>
        <Menu.Menu>
          <Link to="/newGroup">
            <Menu.Item name="New group" />
          </Link>
        </Menu.Menu>
        <Menu.Menu>
          <Link to="/updateGroup">
            <Menu.Item name="Update group" />
          </Link>
        </Menu.Menu>
        <Menu.Menu>
          <Link to="/groupList">
            <Menu.Item name="My groups" />
          </Link>
        </Menu.Menu>
        <Menu.Menu>
          <Menu.Item>
            <SearchBox></SearchBox>
          </Menu.Item>
        </Menu.Menu>
      </Menu.Item>

      <Menu.Item>
        <Menu.Header>
          <Icon name="futbol" />
          Games
        </Menu.Header>

        <Menu.Menu>
          <Link to="/newGame">
            <Menu.Item name="New Game" />
          </Link>
        </Menu.Menu>
     
        <Menu.Menu>
          <Link to="/gameList">
            <Menu.Item name="My Games" />
          </Link>
        </Menu.Menu>
      </Menu.Item>
    </Menu>
  );
}

export { Navbar };
