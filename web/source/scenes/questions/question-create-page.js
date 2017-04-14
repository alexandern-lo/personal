import { connect } from 'store';
import { redirect } from 'store/navigate';

import { createQuestion } from 'store/questions/actions';
import { eventShape } from 'store/data/event';
import { getPinnedEvent } from 'store/events';

import Breadcrumbs from 'components/sub-header/breadcrumbs';
import EditForm from './question-edit-form';

@connect({
  event: getPinnedEvent,
}, {
  createQuestion,
  redirect,
})
export default class QuestionCreatePage extends Component {
  static propTypes = {
    event: eventShape,
    createQuestion: PropTypes.func.isRequired,
    redirect: PropTypes.func.isRequired,
  };

  onCreate = data =>
    this.props.createQuestion({ eventUid: this.props.event.uid, data });

  onSuccess = (r, d, { values: question }) => {
    const { event } = this.props;
    this.props.redirect({
      pathname: `/events/${event.uid}/questions`,
      state: { created: question.text },
    });
  };

  onCancel = () => {
    const { event } = this.props;
    this.props.redirect({
      pathname: `/events/${event.uid}/questions`,
    });
  };

  render() {
    const { event } = this.props;
    return (
      <EditForm
        title={<Breadcrumbs event={event} question action='Create new question' />}
        actionTitle='Create'
        onCancel={this.onCancel}
        onSubmit={this.onCreate}
        onSubmitSuccess={this.onSuccess}
        onSubmitFail={this.onFail}
      />
    );
  }
}
