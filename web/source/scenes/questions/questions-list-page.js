import _ from 'lodash';

import Breadcrumbs from 'components/sub-header/breadcrumbs';
import SubHeader from 'components/sub-header/sub-header';
import { CreateButton, ImportButton } from 'components/sub-header/buttons';
import Table from 'components/sortable-table/sortable-table';

import ErrorPanel from 'components/errors/error-panel';

import PageContentWrapper from 'components/layout/page-content-wrapper';
import PreviewPanelContainer from 'components/layout/preview-panel-container';

import ProgressIndicator from 'components/loading-spinner/loading-spinner';

import { SuccessDialog, DeleteDialog, ServerErrorDialog } from 'components/dialogs';

import { navigate } from 'store/navigate';

import { connect } from 'store';
import { questionShape } from 'store/data/question';
import { eventShape } from 'store/data/event';
import {
  fetchQuestions,
  deleteQuestions,
  moveQuestion,
} from 'store/questions/actions';
import {
  isLoading,
  getQuestions,
  getError,
} from 'store/questions';
import {
  getPinnedEvent,
} from 'store/events';

import QuestionImportDialog from './question-import-dialog';
import styles from './question-list-page.module.css';

@connect({
  loading: isLoading,
  questions: getQuestions,
  error: getError,
  event: getPinnedEvent,
}, {
  fetchQuestions,
  deleteQuestions,
  moveQuestion,
  navigate,
})
export default class QuestionsListPage extends Component {

  static propTypes = {
    location: PropTypes.shape({
      state: PropTypes.objectOf(PropTypes.string),
    }),
    loading: PropTypes.bool,
    questions: PropTypes.arrayOf(questionShape).isRequired,
    error: PropTypes.shape({}),
    event: eventShape,

    fetchQuestions: PropTypes.func.isRequired,
    deleteQuestions: PropTypes.func.isRequired,
    moveQuestion: PropTypes.func.isRequired,
    navigate: PropTypes.func.isRequired,
  };

  constructor(props) {
    super(props);
    this.state = {
      selected: [],
      askDelete: false,
      showImport: false,
      successMessage: null,
      errorMessage: null,
    };
  }

  componentDidMount = () => {
    this.props.fetchQuestions(this.props.event.uid);
    this.setState({ ...this.state, successMessage: this.getSuccessMessage() });
    this.props.location.state = {};
    return null;
  };

  onShowImport = () => this.setState({ ...this.state, showImport: true });
  onHideImport = () => this.setState({ ...this.state, showImport: false });
  onSuccessImport = () => {
    this.onHideImport();
    this.setState({ ...this.state, successMessage: 'Questions have been successfully added' });
    this.props.fetchQuestions(this.props.event.uid);
  };
  onFailImport = (error) => {
    this.onHideImport();
    this.setState({ ...this.state, errorMessage: error.message });
    this.props.fetchQuestions(this.props.event.uid);
  };

  onCreateQuestion = () => {
    const { questions, event } = this.props;
    if (questions.length >= 10) {
      this.setState({ ...this.state, error: { message: 'You cannot create more questions' } });
    } else {
      this.props.navigate(`/events/${event.uid}/questions/create`);
    }
  };

  onEdit = item => this.props.navigate(`/events/${this.props.event.uid}/questions/${item.uid}/edit`);
  onSelect = (item) => {
    const { selected } = this.state;
    if (selected.indexOf(item) === -1) {
      this.setState({ ...this.state, selected: _.concat(selected, item) });
    } else {
      this.setState({ ...this.state, selected: _.without(selected, item) });
    }
  };
  onSelectAll = () => {
    const { selected } = this.state;
    if (_.difference(this.props.questions, selected).length === 0) {
      this.setState({ ...this.state, selected: [] });
    } else {
      this.setState({ ...this.state, selected: this.props.questions.slice() });
    }
  };

  onAskDeleteQuestions = () => this.state.selected.length > 0 && this.setState({ askDelete: true });
  onDeleteQuestions = () =>
    this.props.deleteQuestions({ eventUid: this.props.event.uid, data: this.getSelectedUIDs() });
  onCancelDeleteQuestions = () => this.setState({ askDelete: false });
  onCompleteDeleteQuestions = () => {
    this.setState({ selected: [], askDelete: false });
    this.props.fetchQuestions(this.props.event.uid);
  };
  onErrorDeleteQuestions = ({ _error: error }) => this.setState({ askDelete: false, error });
  onClearError = () => this.setState({ error: null });

  onMove = ({ oldIndex, newIndex }) => {
    const question = _.find(this.props.questions, q => q.position === oldIndex);
    this.props.moveQuestion({
      eventUid: this.props.event.uid,
      uid: question.uid,
      data: newIndex,
    });
  };

  getSelectedUIDs = () => this.state.selected.map(v => v.uid);

  getSuccessMessage = () => {
    const { created, updated } = this.props.location.state || {};
    if (created) {
      return `Question ${created} has been successfully created`;
    }
    if (updated) {
      return `Question ${updated} has been successfully updated`;
    }
    return null;
  };

  clearSuccessMessage = () => this.setState({ ...this.state, successMessage: null });
  clearErrorMessage = () => this.setState({ ...this.state, errorMessage: null });

  renderAnswers = answers => (
    <ol className={styles.answers}>
      { _.map(answers, answer => (
        <li key={answer.uid}>{answer.text}</li>
      ))}
    </ol>
  );

  render() {
    const { loading, event, questions, error } = this.props;
    const { selected, askDelete, successMessage, errorMessage, showImport } = this.state;
    return (
      <PageContentWrapper>
        { loading && !successMessage && !errorMessage && <ProgressIndicator /> }
        { error && <ErrorPanel error={error} onClear={this.onClearError} /> }
        { successMessage &&
          <SuccessDialog message={successMessage} onOk={this.clearSuccessMessage} /> }
        { errorMessage &&
          <ServerErrorDialog message={errorMessage} onOk={this.clearErrorMessage} /> }
        {askDelete && selected.length > 0 &&
        <DeleteDialog
          message='Delete selected questions?'
          onSubmit={this.onDeleteQuestions}
          onCancel={this.onCancelDeleteQuestions}
          onSubmitSuccess={this.onCompleteDeleteQuestions}
          onSubmitFail={this.onErrorDeleteQuestions}
        />
        }
        {showImport &&
        <QuestionImportDialog
          onClose={this.onHideImport}
          onSuccess={this.onSuccessImport}
          onFail={this.onFailImport}
        />}
        <SubHeader>
          <Breadcrumbs event={event} question />
          <ImportButton disabled={questions.length >= 10} onClick={this.onShowImport}>
            Import personal questionnaire
          </ImportButton>
          <CreateButton withIcon disabled={questions.length >= 10} onClick={this.onCreateQuestion}>
            Create new question
          </CreateButton>
        </SubHeader>
        <PreviewPanelContainer>
          <Table
            className={styles.table}
            lockAxis='y'
            useDragHandle
            items={questions}
            onSort={() => {}}
            onEdit={this.onEdit}
            onDelete={this.onAskDeleteQuestions}
            onSelect={this.onSelect}
            onSelectAll={this.onSelectAll}
            selected={selected}
            onMove={this.onMove}
            columns={[
              { field: 'text', label: `Questions (${questions.length}/10)` },
              { field: 'answers', label: 'Answers', render: this.renderAnswers },
            ]}
            onSortEnd={this.onMove}

            context={event.uid}
          />
        </PreviewPanelContainer>
      </PageContentWrapper>
    );
  }
}
