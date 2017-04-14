import { connect } from 'store';
import { redirect } from 'store/navigate';

import { pinQuestionById, updateQuestion } from 'store/questions/actions';
import { getPinnedQuestion, getPinnedError } from 'store/questions';
import { questionShape, editQuestion } from 'store/data/question';

import { getPinnedEvent } from 'store/events';
import { eventShape } from 'store/data/event';

import Spinner from 'components/loading-spinner/loading-spinner';
import ErrorPanel from 'components/errors/error-panel';
import Breadcrumbs from 'components/sub-header/breadcrumbs';
import EditForm from './question-edit-form';

@connect({
  question: getPinnedQuestion,
  event: getPinnedEvent,
  error: getPinnedError,
}, {
  pinQuestionById,
  updateQuestion,
  redirect,
})
export default class QuestionEditPage extends Component {
  static propTypes = {
    routeParams: PropTypes.shape({
      uid: PropTypes.string,
    }).isRequired,
    question: questionShape,
    error: PropTypes.shape({
      message: PropTypes.string,
    }),
    event: eventShape,
    pinQuestionById: PropTypes.func.isRequired,
    updateQuestion: PropTypes.func.isRequired,
    redirect: PropTypes.func.isRequired,
  };

  constructor(props) {
    super(props);
    this.state = {
      success: null,
      error: null,
    };
  }

  componentWillMount() {
    const { uid } = this.props.routeParams;
    const { event, question } = this.props;
    if (!question || question.uid !== uid) {
      this.props.pinQuestionById({ eventUid: event.uid, uid });
    }
  }

  onUpdate = data =>
    this.props.updateQuestion({
      eventUid: this.props.event.uid,
      data,
    });

  onSuccess = (r, d, { values: question }) => {
    const { event } = this.props;
    this.props.redirect({
      pathname: `/events/${event.uid}/questions`,
      state: { updated: question.text },
    });
  };

  onCancel = () => {
    const { event } = this.props;
    this.props.redirect({
      pathname: `/events/${event.uid}/questions`,
    });
  };

  render() {
    const { event, question, error } = this.props;
    if (!question) {
      return error
        ? <ErrorPanel error={error} />
        : <Spinner />;
    }
    return (
      <EditForm
        title={<Breadcrumbs event={event} question={question} action='edit' />}
        actionTitle='Save'
        initialValues={editQuestion(question)}
        onCancel={this.onCancel}
        onSubmit={this.onUpdate}
        onSubmitSuccess={this.onSuccess}
        onSubmitFail={this.onFail}
      />
    );
  }
}

