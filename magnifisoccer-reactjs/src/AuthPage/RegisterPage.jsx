import React, { useState, useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { userActions } from "../_actions/userActions";
import {
  Form,
  Button,
  Segment,
  Header,
  Icon,
  Input,
  TransitionablePortal,
} from "semantic-ui-react";

function RegisterPage() {
  const [user, setUser] = useState({
    firstName: "",
    lastName: "",
    email: "",
    password: "",
    confirmPassword: "",
    agreement: false,
    open: false,
  });
  const [submitted, setSubmitted] = useState(false);
  const registering = useSelector((state) => state.registration.registering);
  const dispatch = useDispatch();

  // reset login status
  useEffect(() => {
    dispatch(userActions.logout());
  }, []);

  function handleChange(e) {
    const { name, value } = e.target;
    setUser((user) => ({ ...user, [name]: value }));
  }

  function handleCheck() {
    setUser((user) => {
      return {
        ...user,
        agreement: !user.agreement,
      };
    });
  }

  function handleSubmit(e) {
    e.preventDefault();

    setSubmitted(true);
    if (
      user.firstName &&
      user.lastName &&
      user.email &&
      user.password &&
      user.confirmPassword
    ) {
      dispatch(userActions.register(user));
    }
  }

  function handleClick() {
    setUser((user) => {
      return {
        ...user,
        open: true,
      };
    });
  }

  function handleClose() {
    setUser((user) => {
      return {
        ...user,
        open: false,
      };
    });
  }

  return (
    <Segment placeholder raised>
      <Header as="h1" icon>
        <Icon name="add user" />
        Register
        <Header.Subheader>
          After registration, an activation link will be sent to your e-mail
          address.
        </Header.Subheader>
      </Header>

      <Form onSubmit={handleSubmit}>
        <Form.Field
          required
          fluid
          placeholder="First name"
          control={Input}
          type="text"
          name="firstName"
          value={user.firstName}
          onChange={handleChange}
          label="First Name"
          error={
            submitted &&
            !user.firstName && {
              content: "Please enter first name",
              pointing: "above",
            }
          }
        />

        <Form.Field
          required
          fluid
          placeholder="First name"
          control={Input}
          type="text"
          name="lastName"
          value={user.lastName}
          onChange={handleChange}
          label="Last Name"
          error={
            submitted &&
            !user.lastName && {
              content: "Please enter last name",
              pointing: "above",
            }
          }
        />

        <Form.Field
          required
          fluid
          placeholder="E-mail"
          control={Input}
          type="email"
          name="email"
          value={user.email}
          onChange={handleChange}
          label="E-mail"
          error={
            submitted &&
            !user.email && {
              content: "Please enter a valid email address",
              pointing: "above",
            }
          }
        />

        <Form.Field
          required
          fluid
          placeholder="Password"
          control={Input}
          type="password"
          name="password"
          value={user.password}
          onChange={handleChange}
          label="Password"
          error={
            submitted &&
            !user.password && {
              content: "Please enter password",
              pointing: "above",
            }
          }
        />

        <Form.Field
          required
          fluid
          placeholder="Password"
          control={Input}
          type="password"
          name="confirmPassword"
          value={user.confirmPassword}
          onChange={handleChange}
          label="Confirm Password"
          error={
            submitted &&
            !user.confirmPassword && {
              content: "Please enter confirm password",
              pointing: "above",
            }
          }
        />

        <Form.Checkbox
          required
          label={
            <label>
              <a onClick={handleClick}>
                I agree to the Terms and Conditions and Privacy Policy
              </a>
            </label>
          }
          name="agreement"
          defaultChecked={user.agreement}
          onChange={handleCheck}
        />

        <Form.Field>
          <Button primary disabled={!user.agreement}>
            {registering && (
              <span className="spinner-border spinner-border-sm mr-1"></span>
            )}
            Register
          </Button>
        </Form.Field>

        <TransitionablePortal
        
          onClose={handleClose}
          open={user.agreement && user.open}
        >
          <Segment
            style={{
              left: "auto",
              right: "auto",
              position: "absolute",
              top: "3%",
              zIndex: 1000,
            }}
          >
            <Header>Terms and Conditions and Privacy Policy</Header>
            <p>
              Introduction Company Name we” or “us” values its visitors’
              privacy. This privacy policy is effective Date; it summarizes what
              information we might collect from a registered user or other
              visitor “you”, and what we will and will not do with it. Please
              note that this privacy policy does not govern the collection and
              use of information by companies that Company Name does not
              control, nor by individuals not employed or managed by Company
              Name. If you visit a Web site that we mention or link to, be sure
              to review its privacy policy before providing the site with
              information. What we do with your personally identifiable
              information It is always up to you whether to disclose personally
              identifiable information to us, although if you elect not to do
              so, we reserve the right not to register you as a user or provide
              you with any products or services. “Personally identifiable
              information” means information that can be used to identify you as
              an individual, such as, for example: your name, company, email
              address, phone number, billing address, and shipping address your
              Company Name user ID and password credit card information The
              above provision may not be appropriate if credit card information
              is handled by a payment processor. any account-preference
              information you provide us your computer’s domain name and IP
              address, indicating where your computer is located on the Internet
              session data for your login session, so that our computer can
              ‘talk’ to yours while you are logged in If you do provide
              personally identifiable information to us, either directly or
              through a reseller or other business partner, we will: not sell or
              rent it to a third party without your permission — although unless
              you opt out see belo, we may use your contact information to
              provide you with information we believe you need to know or may
              find useful, such as for example news about our services and
              products and modifications to the Terms of Service; take
              commercially reasonable precautions to protect the information
              from loss, misuse and unauthorized access, disclosure, alteration
              and destruction; not use or disclose the information except: as
              necessary to provide services or products you have ordered, such
              as for example by providing it to a carrier to deliver products
              you have ordered; in other ways described in this privacy policy
              or to which you have otherwise consented; in the aggregate with
              other information in such a way so that your identity cannot
              reasonably be determined for example, statistical compilations; as
              required by law, for example, in response to a subpoena or search
              warrant; to outside auditors who have agreed to keep the
              information confidential; as necessary to enforce the Terms of
              Service; as necessary to protect the rights, safety, or property
              of Company Name, its users, or others; this may include for
              example exchanging information with other organizations for fraud
              protection and/or risk reduction. Other information we collect We
              may collect other information that cannot be readily used to
              identify you, such as for example the domain name and IP address
              of your computer.
            </p>
          </Segment>
        </TransitionablePortal>
      </Form>
    </Segment>
  );
}

export { RegisterPage };
