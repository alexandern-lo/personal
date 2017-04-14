import { connect } from 'store';
import { redirect } from 'store/navigate';

import { createCategory } from 'store/categories/actions';
import { eventShape } from 'store/data/event';
import { getPinnedEvent } from 'store/events';

import Breadcrumbs from 'components/sub-header/breadcrumbs';
import CategoryForm from './category-edit-form';

@connect({
  event: getPinnedEvent,
}, {
  createCategory,
  redirect,
})
export default class QuestionCreatePage extends Component {
  static propTypes = {
    event: eventShape,
    createCategory: PropTypes.func.isRequired,
    redirect: PropTypes.func.isRequired,
  };

  onCreate = data =>
    this.props.createCategory({ eventUid: this.props.event.uid, data });

  onSuccess = (r, d, { values: category }) => {
    const { event } = this.props;
    this.props.redirect({
      pathname: `/events/${event.uid}/categories`,
      state: { created: category.name },
    });
  };

  onCancel = () => {
    const { event } = this.props;
    this.props.redirect({
      pathname: `/events/${event.uid}/categories`,
    });
  };

  render() {
    const { event } = this.props;
    return (
      <CategoryForm
        title={<Breadcrumbs event={event} category action='Create new category' />}
        actionTitle='Create'
        onCancel={this.onCancel}
        onSubmit={this.onCreate}
        onSubmitSuccess={this.onSuccess}
        onSubmitFail={this.onFail}
      />
    );
  }
}
