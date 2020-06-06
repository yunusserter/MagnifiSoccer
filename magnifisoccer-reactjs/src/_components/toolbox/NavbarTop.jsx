import React from "react";
import { useSelector } from "react-redux";

import { Menu, Icon, Image } from "semantic-ui-react";
import { Link } from "react-router-dom";

function NavbarTop() {
  return (
    <Menu fixed={"top"} borderless>
      <Link to="/">
        <Menu.Item>
          <Image size="tiny" src="../../../Upload/logo.png"></Image>
        </Menu.Item>
      </Link>    

      <Menu.Item
        position={"right"}
        hidden={!useSelector((state) => state.authentication.user)}
      >
        <Menu.Header>
          <Link to="/login">
            <Icon name="sign out" />
            Sign out
          </Link>
        </Menu.Header>
      </Menu.Item>
    </Menu>
  );
}

export { NavbarTop };
