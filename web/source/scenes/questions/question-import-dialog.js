import React, { Component } from 'react';
import { connect } from 'store';
import { eventShape, readEvent } from 'store/data/event';
import { searchEvents } from 'store/events/actions';
import { questionShape, readQuestion, editQuestion } from 'store/data/question';
import { createQuestion, searchQuestions } from 'store/questions/actions';
import { getQuestions } from 'store/questions';
import { getPinnedEvent } from 'store/events';

import ProgressIndicator from 'components/loading-spinner/loading-spinner';

import ModalView from 'components/modal-view/modal-view';
import FetchableList from 'components/fetchable/fetchable-list';
import ErrorPanel from 'components/errors/error-panel';

import { select, deselect, isSelected } from 'components/helpers/select';

import styles from './question-import-dialog.module.css';

@connect({
  questions: getQuestions,
  event: getPinnedEvent,
}, {
  createQuestion,
})
export default class QuestionImportDialog extends Component {
  static propTypes = {
    onClose: PropTypes.func.isRequired,
    onSuccess: PropTypes.func.isRequired,
    onFail: PropTypes.func.isRequired,
    questions: PropTypes.arrayOf(questionShape).isRequired,
    event: eventShape,
    createQuestion: PropTypes.func.isRequired,
  };

  componentWillMount() {
    this.state = {
      loading: false,
      event: null,
      showQuestions: false,
      questions: [],
      error: null,
    };
  }

  onClose = () => {
    this.props.onClose();
  };

  onSelectEvent = (event) => {
    if (event.uid !== this.props.event.uid) {
      this.setState({ ...this.state, event });
    }
  };

  onSelectQuestion = (q) => {
    const { questions } = this.state;
    if (isSelected(questions, q)) {
      this.setState({ ...this.state, questions: deselect(questions, q) });
    } else if (this.totalQuestionCount() < 10) {
      this.setState({ ...this.state, questions: select(questions, q) });
    }
  };

  onError = error => this.setState({ ...this.state, error });
  onFetched = () => this.setState({ ...this.state, error: null });

  onBackToEvents = () => this.setState({ ...this.state, showQuestions: false });

  chooseQuestions = () => {
    if (this.state.questions.length > 0) {
      this.setState({ ...this.state, loading: true });
      this.state.questions.reduce((acc, q) => {
        const importPromise = () => this.importQuestion(q);
        return acc ? acc.then(importPromise) : importPromise();
      }, null)
        .then(() => this.props.onSuccess())
        .catch(error => this.props.onFail(error))
      ;
    }
  };
  importQuestion = question =>
    this.props.createQuestion({
      eventUid: this.props.event.uid,
      data: editQuestion(question),
    });
  clearError = () => this.setState({ ...this.state, error: null });
  fetchQuestionsAction = () => searchQuestions(this.state.event.uid);
  chooseEvent = () => {
    if (this.state.event) {
      this.setState({ ...this.state, showQuestions: true });
    }
  };

  totalQuestionCount = () => this.props.questions.length + this.state.questions.length;

  emptyQuestionsRender = () =>
    (<div className={styles.empty}>There are no questions to import</div>);

  renderQuestion = (item, selected) => (
    <div
      className={classNames(styles.row, styles.questionRow, selected && styles.checked)}
    >
      <span>{item.text}</span>
    </div>
  );


  renderEvent = (item, selected) => (
    <div
      className={classNames(
        styles.row,
        selected && styles.active,
        this.props.event.uid === item.uid && styles.disabled,
      )}
    >
      <span>{item.name}</span>
    </div>
  );

  renderQuestionsTitle = () => (
    <div className={styles.questionsTitle}>
      <span className={styles.eventName}>{this.state.event.name}</span>
      <span className={styles.questionsCount}>
        ({this.totalQuestionCount()}/10)
      </span>
    </div>
  );

  renderError = () => {
    const { error } = this.state;
    return error && <ErrorPanel error={error} onClear={this.clearError} />;
  };

  renderEventsList = () => (
    <ModalView
      title='Сhoose an event questionnarie'
      onClose={this.onClose}
      button={{
        text: 'Continue',
        onClick: this.chooseEvent,
        active: !!this.state.event,
      }}
    >
      <FetchableList
        width={480}
        height={234}
        rowHeight={42}
        renderItem={this.renderEvent}
        selected={this.state.event ? [this.state.event] : []}
        onSelect={this.onSelectEvent}
        fetchPageAction={searchEvents}
        reader={readEvent}
        searchable
        onError={this.onError}
        onFetched={this.onFetched}
      />
      {this.renderError()}
    </ModalView>
  );

  renderQuestions = () => (
    <ModalView
      title={this.renderQuestionsTitle}
      onClose={this.onClose}
      button={{
        text: 'Import',
        onClick: this.chooseQuestions,
        active: this.state.questions.length > 0,
      }}
      leftButton={{
        text: 'Back',
        onClick: this.onBackToEvents,
      }}
    >
      <FetchableList
        width={480}
        height={234}
        rowHeight={42}
        renderItem={this.renderQuestion}
        selected={this.state.questions}
        onSelect={this.onSelectQuestion}
        fetchPageAction={this.fetchQuestionsAction}
        renderEmpty={this.emptyQuestionsRender}
        reader={readQuestion}
        searchable={false}
        onError={this.onError}
        onFetched={this.onFetched}
      />
      {this.renderError()}
    </ModalView>
  );

  render() {
    const { loading, event, showQuestions } = this.state;
    return (
      <span>
        {loading && <ProgressIndicator />}
        {(event && showQuestions) ? this.renderQuestions() : this.renderEventsList()}
      </span>
    );
  }
}
