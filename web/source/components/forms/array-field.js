import { FieldArray } from 'redux-form';
import { AddButton } from './specialized/buttons';

import styles from './array-field.module.css';

class RenderArrayField extends Component {
  static propTypes = {
    className: PropTypes.string,

    fields: PropTypes.shape({
      length: PropTypes.number.isRequired,
      map: PropTypes.func.isRequired,
      push: PropTypes.func.isRequired,
      remove: PropTypes.func.isRequired,
    }).isRequired,
    renderFields: PropTypes.func,
    fixed: PropTypes.bool,
    min: PropTypes.number.isRequired,
    max: PropTypes.number,

    addButton: PropTypes.oneOfType([
      PropTypes.node,
      PropTypes.bool,
    ]),
    addButtonLabel: PropTypes.string,

    prop: PropTypes.string,
    initialValue: PropTypes.shape({}),

    render: PropTypes.func,
    children: PropTypes.node,
  };

  static defaultProps = {
    min: 0,
    addButton: true,
    addButtonLabel: 'Add new',
  };

  componentWillMount() {
    const { fields, min } = this.props;
    for (let i = fields.length; i < min; i += 1) {
      this.onAdd();
    }
  }

  onAdd = () => this.props.fields.push(this.props.initialValue);

  renderInputField = (name, index, all) => {
    const { fields, min, prop, render, fixed, children } = this.props;
    const onRemove = !fixed && all.length > min ? () => fields.remove(index) : null;
    const props = {
      name: prop ? `${name}.${prop}` : name,
      onRemove,
    };
    return (
      <div key={index}>
        { children
          ? React.Children.map(children, child => React.cloneElement(child, props))
          : render(props)
        }
      </div>
    );
  };

  renderAddButton = () => {
    const { fields, max, addButton, addButtonLabel } = this.props;
    if (addButton === false) return null;
    const canAdd = !max || fields.length < max;
    if (addButton !== true) {
      return (
        <addButton onClick={canAdd ? this.onAdd : null} />
      );
    }
    return (
      <AddButton
        className={classNames(styles.addButton)}
        onClick={this.onAdd}
        disabled={!canAdd}
      >
        { addButtonLabel }
      </AddButton>
    );
  };

  render() {
    const { className, fields, fixed, renderFields } = this.props;
    if (renderFields) return renderFields(fields);
    return (
      <div className={classNames(className, styles.container)}>
        {fields.length > 0 && (
          <div className={styles.fields}>
            { fields.map(this.renderInputField) }
          </div>
        )}
        { !fixed && this.renderAddButton() }
      </div>
    );
  }
}

const ArrayField = ({ name, ...props }) => (
  <FieldArray
    name={name}
    component={RenderArrayField}
    {...props}
  />
);
export default ArrayField;

ArrayField.propTypes = {
  name: PropTypes.string.isRequired,
};
