import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";

import { groupActions } from "../../_actions/groupActions";
import {
  Segment,
  Button,
  Form,
  Header,
  Icon,
  Input,
  Modal,
  Label,
} from "semantic-ui-react";

function GroupNew() {
  const [submitted, setSubmitted] = useState(false);
  const creating = useSelector((state) => state.groups.creating);
  const dispatch = useDispatch();

  const [inputs, setInputs] = useState({
    groupName: "",
    photoUrl: null,
  });

  function handleChange(e, data) {
    const { name, value } = data;
    setInputs((inputs) => ({ ...inputs, [name]: value }));
  }

  function handleSelectedPhoto(e) {
    var imgdata=new FormData();
    imgdata=e.target.files[0];
    setInputs((inputs) => ({ ...inputs, photoUrl: imgdata }));
  }

  function handleSubmit(e) {
    e.preventDefault();

    const formData = new FormData();
    for (let name in inputs) {
      formData.append(name, inputs[name]);
    }
    
    setSubmitted(true);
    if (inputs.groupName && inputs.photoUrl) {
      dispatch(groupActions.newGroup(formData));
    }
  }

  return (
    <Segment placeholder raised>
      <Header as="h1" icon>
        <Icon name="plus" />
        Create Group
      </Header>

      <Form onSubmit={handleSubmit}>
        <Form.Field
          required
          fluid
          placeholder="Group Name"
          control={Input}
          type="text"
          name="groupName"
          value={inputs.groupName}
          onChange={handleChange}
          label="Group Name"
        />

        <Form.Field>
          <label>Photo Url</label>
          <input type="file" onChange={handleSelectedPhoto} />
        </Form.Field>

        <Form.Field>
          <Button primary>
            {creating && (
              <span className="spinner-border spinner-border-sm mr-1"></span>
            )}
            Create
          </Button>
        </Form.Field>
      </Form>
    </Segment>
  );
}
export { GroupNew };
