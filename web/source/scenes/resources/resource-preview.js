import { resourceShape, getDisplayType } from 'store/data/resource';
import { getDisplayName as getUserName } from 'store/data/user';

import PreviewPanel from 'components/preview/preview-panel';
import PreviewTable from 'components/preview/preview-table';
import {
  GroupRow,
  TextRow,
  FileUrlRow,
} from 'components/preview/preview-rows';

export const renderOwner = (owner) => {
  if (!owner) return null;
  const name = getUserName(owner);
  if (owner.email) {
    return <a href={`mailto:${owner.email}`}>{name}</a>;
  }
  return name;
};

export default class ResourcePreview extends Component {
  static propTypes = {
    resource: resourceShape.isRequired,
    onClose: PropTypes.func.isRequired,
    onEdit: PropTypes.func.isRequired,
    onDelete: PropTypes.func.isRequired,
  };

  onEdit = () => this.props.onEdit(this.props.resource);

  onDelete = () => this.props.onDelete(this.props.resource);

  render() {
    const { resource, onClose, onEdit, onDelete } = this.props;
    return (
      <PreviewPanel
        title={resource.name}
        onClose={onClose}
        onDelete={onDelete ? this.onDelete : null}
        onEdit={onEdit ? this.onEdit : null}
      >
        <PreviewTable>
          <GroupRow label='General' />
          <TextRow label='Name' text={resource.name} />
          <TextRow label='Owner' text={renderOwner(resource.user)} />
          <TextRow label='Type' text={getDisplayType(resource)} />
          <FileUrlRow label='Url' url={resource.url} />
          <TextRow label='Description' text={resource.description} />
        </PreviewTable>
      </PreviewPanel>
    );
  }
}
