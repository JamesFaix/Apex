import React, { FC, useState, ChangeEvent } from 'react';
import {
  FormControl, FormLabel, Button, TextField, FormControlLabel, FormGroup,
} from '@material-ui/core';
import { createAccount } from '../../controllers/userController';

type FormState = {
  username: string,
  password1: string,
  password2: string
};

const defaultState: FormState = {
  username: '',
  password1: '',
  password2: '',
};

const CreateAccountForm: FC = () => {
  const [state, setState] = useState(defaultState);

  const onUsernameChanged = (e: ChangeEvent<HTMLInputElement>) => setState({
    ...state,
    username: e.target.value,
  });

  const onPassword1Changed = (e: ChangeEvent<HTMLInputElement>) => setState({
    ...state,
    password1: e.target.value,
  });

  const onPassword2Changed = (e: ChangeEvent<HTMLInputElement>) => setState({
    ...state,
    password2: e.target.value,
  });

  const onSubmitClicked = () => {
    createAccount({
      name: state.username,
      password: state.password1,
    });
  };

  const passwordsDontMatch = (state.password1 !== state.password2)
    && (state.password2 !== '');

  const controlStyle = {
    padding: '10px',
  };

  return (
    <div>
      <FormControl component="fieldset">
        <FormLabel>Create account</FormLabel>
        <FormGroup>
          <FormControlLabel
            value={state.username}
            label="Username"
            labelPlacement="start"
            control={(
              <TextField
                onChange={onUsernameChanged}
                style={controlStyle}
              />
            )}
          />
          <FormControlLabel
            value={state.password1}
            label="Password"
            labelPlacement="start"
            control={(
              <TextField
                type="password"
                onChange={onPassword1Changed}
                error={passwordsDontMatch}
                helperText={passwordsDontMatch ? 'Passwords do not match' : undefined}
                style={controlStyle}
              />
            )}
          />
          <FormControlLabel
            value={state.password2}
            label="Confirm password"
            labelPlacement="start"
            control={(
              <TextField
                type="password"
                onChange={onPassword2Changed}
                error={passwordsDontMatch}
                helperText={passwordsDontMatch ? 'Passwords do not match' : undefined}
                style={controlStyle}
              />
            )}
          />
          <Button
            onClick={onSubmitClicked}
            style={controlStyle}
          >
            Submit
          </Button>
        </FormGroup>
      </FormControl>
    </div>
  );
};

export default CreateAccountForm;
