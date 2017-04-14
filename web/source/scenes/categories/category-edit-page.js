import { connect } from 'store';
import { redirect } from 'store/navigate';

import { pinCategoryById, updateCategory } from 'store/categories/actions';
import { getPinnedCategory, getPinnedError } from 'store/categories';
import { categoryShape, editCategory } from 'store/data/category';

import { getPinnedEvent } from 'store/events';
import { eventShape } from 'store/data/event';

import Spinner from 'components/loading-spinner/loading-spinner';
import ErrorPanel from 'components/errors/error-panel';
import Breadcrumbs from 'components/sub-header/breadcrumbs';
import EditForm from './category-edit-form';

@connect({
  category: getPinnedCategory,
  event: getPinnedEvent,
  error: getPinnedError,
}, {
  pinCategoryById,
  updateCategory,
  redirect,
})
export default class CategoryEditPage extends Component {
  static propTypes = {
    routeParams: PropTypes.shape({
      uid: PropTypes.string,
    }).isRequired,
    event: eventShape,
    category: categoryShape,
    error: PropTypes.shape({
      message: PropTypes.string,
    }),
    pinCategoryById: PropTypes.func.isRequired,
    updateCategory: PropTypes.func.isRequired,
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
    const { event, category } = this.props;
    if (!category || category.uid !== uid) {
      this.props.pinCategoryById({ eventUid: event.uid, categoryUid: uid });
    }
  }

  onUpdate = data =>
    this.props.updateCategory({
      eventUid: this.props.event.uid,
      data,
    });

  onSuccess = (r, d, { values: category }) => {
    const { event } = this.props;
    this.props.redirect({
      pathname: `/events/${event.uid}/categories`,
      state: { updated: category.name },
    });
  };

  onCancel = () => {
    const { event } = this.props;
    this.props.redirect({
      pathname: `/events/${event.uid}/categories`,
    });
  };

  render() {
    const { event, category, error } = this.props;
    if (!category) {
      return error
        ? <ErrorPanel error={error} />
        : <Spinner />;
    }
    return (
      <EditForm
        title={<Breadcrumbs event={event} category={category} action='edit' />}
        actionTitle='Save'
        initialValues={editCategory(category)}
        onCancel={this.onCancel}
        onSubmit={this.onUpdate}
        onSubmitSuccess={this.onSuccess}
        onSubmitFail={this.onFail}
      />
    );
  }
}

