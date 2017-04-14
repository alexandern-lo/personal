import _ from 'lodash';
import PreviewPanel from 'components/preview/preview-panel';
import PreviewTable from 'components/preview/preview-table';
import {
  GroupRow,
  ImageRow,
  TextRow,
} from 'components/preview/preview-rows';

import { attendeeShape } from 'store/data/attendee';

export default class AgendaPreview extends PureComponent {
  static propTypes = {
    className: PropTypes.string,
    attendee: attendeeShape,
    event: PropTypes.shape({
      name: PropTypes.string,
    }),
    onClose: PropTypes.func,
    onEdit: PropTypes.func,
    onDelete: PropTypes.func,
  };

  onEdit = () => this.props.onEdit(this.props.attendee);

  onDelete = () => this.props.onDelete(this.props.attendee);

  renderCategories = () => {
    const categoriesMap = this.props.attendee.categories.reduce((acc, val) => {
      const cat = acc[val.category_uid] || {
        name: val.categoryName,
        options: [],
      };

      return {
        ...acc,
        [val.category_uid]: {
          ...cat,
          options: [...cat.options, val.optionName],
        },
      };
    }, {});
    return _.toPairs(categoriesMap).map(e => (
      <TextRow
        key={e[0]}
        label={e[1].name}
        text={e[1].options.join('; ')}
      />
    ));
  };

  render() {
    const { className, attendee, event, onClose, onEdit, onDelete } = this.props;
    const address = [attendee.city, attendee.state, attendee.country].filter(val => val).join(', ');
    return (
      <PreviewPanel
        className={className}
        title={attendee.name}
        onClose={onClose}
        onEdit={onEdit ? this.onEdit : null}
        onDelete={onDelete ? this.onDelete : null}
      >
        <PreviewTable>
          <ImageRow imageUrl={attendee.avatarUrl} />
          <GroupRow label='Personal' />
          <TextRow label='Event' text={event.name} />
          <TextRow label='Company' text={attendee.company} />
          <TextRow label='Job title' text={attendee.title} />
          <GroupRow label='Contacts' />
          <TextRow label='Email' text={attendee.email} />
          <TextRow label='Phone' text={attendee.phone} />
          <GroupRow label='Location' />
          <TextRow label='Address' text={address} />
          <GroupRow label='Categories' />
          {this.renderCategories()}
        </PreviewTable>
      </PreviewPanel>
    );
  }
}
