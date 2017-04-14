import React, { Component, PropTypes } from 'react';
import {
  userShape,
  isInvited,
} from 'store/data/user';
import moment from 'moment';
import styles from './users-list-invitation-form.module.css';

const RESEND_TIMEOUT = 10;

export default class UserListInvitationForm extends Component {

  static propTypes = {
    user: userShape.isRequired,
    userResendInvite: PropTypes.func.isRequired,
    disabled: PropTypes.bool,
  };

  static defaultProps = {
    disabled: false,
  };

  componentDidUpdate() {
    const { user, disabled } = this.props;

    if (this.timer) {
      clearTimeout(this.timer);
      this.timer = null;
    }

    if (isInvited(user) && !disabled) {
      const timeLeft = moment.utc(user.updatedAt).add(RESEND_TIMEOUT, 'm').diff(moment());
      if (timeLeft > 0) {
        this.timer = setTimeout(() => {
          this.timer = null;
          this.forceUpdate();
        }, timeLeft);
      }
    }
  }

  componentWillUnmount() {
    if (this.timer) clearTimeout(this.timer);
  }

  resendInvite = () => this.props.userResendInvite(this.props.user.uid);

  renderButton = () => {
    const { user, disabled } = this.props;
    const timeLeft = moment().diff(moment.utc(user.updatedAt), 'minutes');
    const buttonDisabled = disabled || timeLeft < RESEND_TIMEOUT;
    return (
      <button
        className={styles.resend}
        onClick={this.resendInvite}
        disabled={buttonDisabled}
      >
        Resend
      </button>
    );
  };

  render() {
    const { user } = this.props;
    const invited = isInvited(user);
    return (
      <div className={styles.form}>
        <span>{invited ? 'Sent' : 'Accepted'}</span>
        {invited && this.renderButton()}
      </div>
    );
  }
}
