import React, { useState, useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { Button, Form, Segment, Header, Icon } from "semantic-ui-react";
import { userActions } from "../_actions/userActions";

function ForgetPasswordPage() {
  const [inputs, setInputs] = useState({
    email: "",
  });
  const [submitted, setSubmitted] = useState(false);
  const { email } = inputs;
  const forget = useSelector((state) => state.users.forget);
  const dispatch = useDispatch();

  function handleChange(e) {
    const { name, value } = e.target;
    setInputs((inputs) => ({ ...inputs, [name]: value }));
  }

  function handleSubmit(e) {
    e.preventDefault();
    setSubmitted(true);
    if (email) {
      dispatch(userActions.forgetPassword(email));
    }
  }

  return (
    <Segment placeholder raised>
      <Header as="h1" textAlign="center">        
        Forget Password        
      </Header>
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
        <Form.Field>
          <Button content="Send" primary>
            {forget && (
              <span className="spinner-border spinner-border-sm mr-1"></span>
            )}
          </Button>
        </Form.Field>
      </Form>
    </Segment>
  );
}

export { ForgetPasswordPage };
