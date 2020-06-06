import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import { Button, Divider, Form, Grid, Segment, Label } from "semantic-ui-react";
import { userActions } from "../_actions/userActions";

function LoginPage() {
  const [inputs, setInputs] = useState({
    email: "",
    password: "",
  });
  const [submitted, setSubmitted] = useState(false);
  const { email, password } = inputs;
  const loggingIn = useSelector((state) => state.authentication.loggingIn);
  const dispatch = useDispatch();

  // reset login status
  useEffect(() => {
    dispatch(userActions.logout());
  }, []);

  function handleChange(e) {
    const { name, value } = e.target;
    setInputs((inputs) => ({ ...inputs, [name]: value }));
  }

  function handleSubmit(e) {
    e.preventDefault();

    setSubmitted(true);
    if (email && password) {
      dispatch(userActions.login(email, password));
    }
  }

  return (
    <Segment placeholder raised>
      <Grid columns={2} relaxed="very" stackable>
        <Grid.Column>
          <Form onSubmit={handleSubmit}>
            <Form.Input
            required
              icon="user"
              iconPosition="left"
              label="Email"
              placeholder="Email"
              type="email"
              name="email"
              value={email}
              onChange={handleChange}
            />
            <Form.Input
            required
              icon="lock"
              iconPosition="left"
              label="Password"
              placeholder="Password"
              type="password"
              name="password"
              value={password}
              onChange={handleChange}
            />
            <Form.Field>
              <Button content="Login" primary>
                {loggingIn && (
                  <span className="spinner-border spinner-border-sm mr-1"></span>
                )}
              </Button>
            </Form.Field>
            <Link to="/forgetPassword"  primary>
              Forgot your password?
            </Link>
          </Form>
        </Grid.Column>

        <Grid.Column verticalAlign="middle">
          <Link to="/register">
            <Button content="Sign up" icon="signup" size="big" />
          </Link>
        </Grid.Column>
      </Grid>

      <Divider vertical>Or</Divider>
    </Segment>
  );
}

export { LoginPage };
