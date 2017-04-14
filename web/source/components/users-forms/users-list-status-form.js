import React, { Component, PropTypes } from 'react';
import {
  userShape,
  isEnabled,
  isInvited,
} from 'store/data/user';
import styles from './users-list-status-form.module.css';

export default class UserListStatusForm extends Component {
  static propTypes = {
    user: userShape.isRequired,
    enableUser: PropTypes.func.isRequired,
    disableUser: PropTypes.func.isRequired,
    canEnable: PropTypes.bool,
    enabledText: PropTypes.string,
    disabledText: PropTypes.string,
    disabled: PropTypes.bool,
  };

  static defaultProps = {
    canEnable: true,
    enabledText: 'Active',
    disabledText: 'Inactive',
    disabled: false,
  };

  enableUser = () => this.props.enableUser(this.props.user.uid);
  disableUser = () => this.props.disableUser(this.props.user.uid);

  render() {
    const { user, canEnable, enabledText, disabledText, disabled } = this.props;
    const invited = isInvited(user);
    const enabled = isEnabled(user);
    const action = enabled ? this.disableUser : this.enableUser;
    const buttonDisabled = disabled || invited || (!enabled && !canEnable);
    return (
      <div className={styles.form}>
        <span>{enabled ? enabledText : disabledText}</span>
        <button
          className={enabled ? styles.active : styles.inactive}
          onClick={action}
          disabled={buttonDisabled}
        />
      </div>
    );
  }
}
