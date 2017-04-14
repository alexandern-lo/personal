import React, { Component, PropTypes } from 'react';
import {
  userShape,
  isAdmin,
  isInvited,
} from 'store/data/user';

import styles from './users-list-role-form.module.css';

export default class UserListRoleForm extends Component {

  static propTypes = {
    user: userShape.isRequired,
    userGrantAdmin: PropTypes.func.isRequired,
    userRevokeAdmin: PropTypes.func.isRequired,
    disabled: PropTypes.bool,
  };

  static defaultProps = {
    disabled: false,
  };

  grantAdmin = () => this.props.userGrantAdmin(this.props.user.uid);
  revokeAdmin = () => this.props.userRevokeAdmin(this.props.user.uid);

  render() {
    const { user, disabled } = this.props;
    const invited = isInvited(user);
    const admin = isAdmin(user);
    const action = admin ? this.revokeAdmin : this.grantAdmin;
    return (
      <div className={styles.form}>
        <span>{admin ? 'Admin' : 'User'}</span>
        <button
          className={admin ? styles.admin : styles.user}
          onClick={action}
          disabled={disabled || invited}
        />
      </div>
    );
  }
}
