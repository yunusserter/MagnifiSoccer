import _ from "lodash";
import React, { useEffect, useState } from "react";
import { groupActions } from "../../_actions/groupActions";
import { useDispatch, useSelector } from "react-redux";
import { Search, Confirm } from "semantic-ui-react";

function SearchBox() {
  const search = useSelector((state) => state.search);
  const dispatch = useDispatch();

  const [inputs, setInputs] = useState({
    isLoading: false,
    results: [],
    value: "",
    groupId: "",
    open: false,
    confirm: false,
  });

  const { isLoading, results, value, groupId, open, confirm } = inputs;

  useEffect(() => {
    dispatch(groupActions.getAllForSearch());
  }, []);

  function show(e, { result }) {
    setInputs((input) => {
      return {
        ...input,
        open: true,
        groupId: result.id,
      };
    });
  }

  function handleConfirm() {
    setInputs((input) => {
      return {
        ...input,
        confirm: true,
        open: false,
      };
    });
    dispatch(groupActions.joinGroup(inputs));
  }

  function handleCancel(e) {
    setInputs((input) => {
      return {
        ...input,
        confirm: false,
        open: false,
        groupId: "",
      };
    });
  }

  function handleSearchChange(e, { value }) {
    setInputs((inputs) => ({ ...inputs, isLoading: true, value: value }));

    setTimeout(() => {
      const re = new RegExp(_.escapeRegExp(inputs.value), "i");
      const isMatch = (result) => re.test(result.groupName);
      setInputs((inputs) => ({
        ...inputs,
        isLoading: false,
        results: _.filter(search.items, isMatch),
      }));
    }, 300);
  }

  const inlineStyle = {
    confirm: {
      height: "auto",
      top: "auto",
      left: "auto",
      bottom: "auto",
      right: "auto",
    },
  };

  return (
    <div>
      <Search
        size={"tiny"}
        placeholder="Group Search"
        fluid
        loading={inputs.isLoading}
        onResultSelect={show}
        onSearchChange={_.debounce(handleSearchChange, 500, {
          leading: true,
        })}
        results={inputs.results.map((result) => ({
          id: result.id,
          title: result.groupName,
          description: "Description",
          image: "../../../Upload/"+result.photoUrl,
        }))}
        value={inputs.value}
      />

      <Confirm
        style={inlineStyle.confirm}
        content="Do you want to join the group?"
        open={inputs.open}
        onCancel={handleCancel}
        onConfirm={handleConfirm}
        size={"mini"}
      />
    </div>
  );
}

export { SearchBox };
