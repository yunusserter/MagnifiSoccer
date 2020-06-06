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
  Dropdown,
} from "semantic-ui-react";

function GroupUpdate() {
  const [submitted, setSubmitted] = useState(false);
  const updating = useSelector((state) => state.groups.updating);
  const groups = useSelector((state) => state.groups);
  const dispatch = useDispatch();

  const [inputs, setInputs] = useState({
    groupId: "",
    groupName: "",
    photoUrl: null,
  });

  useEffect(() => {
    dispatch(groupActions.getAll());
  }, []);

  function handleChange(e, data) {
    const { name, value } = data;
    setInputs((inputs) => ({ ...inputs, [name]: value }));
  }

  function handleSelectedPhoto(e) {
    var imgdata = new FormData();
    imgdata = e.target.files[0];
    setInputs((inputs) => ({ ...inputs, photoUrl: imgdata }));
  }

  function handleSubmit(e) {
    e.preventDefault();

    const formData = new FormData();
    for (let name in inputs) {
      inputs[name]!==(null&&"")?formData.append(name, inputs[name]):null
    }

    setSubmitted(true);
    if (inputs.groupId) {
      dispatch(groupActions.updateGroup(formData));
    }
  }

  return (
    <Segment placeholder raised>
      <Header as="h1" icon>
        <Icon name="sync" />
        Update Group
      </Header>

      <Form onSubmit={handleSubmit}>
        <Form.Field>
          <Dropdown
            placeholder="Please select group."
            selection
            name="groupId"
            options={
              (groups.items &&
                groups.items.map(({ id, groupName }) => ({
                  key: id,
                  value: id,
                  text: groupName,
                }))) ||
              []
            }
            onChange={handleChange}
          />
        </Form.Field>

        <Form.Field          
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
            {updating && (
              <span className="spinner-border spinner-border-sm mr-1"></span>
            )}
            Update
          </Button>
        </Form.Field>
      </Form>
    </Segment>
  );
}
export { GroupUpdate };
