import { Field } from 'redux-form';
import Select from 'react-select';
import styles from './multiselect-field.module.css';

class Multiselect extends Component {
  static propTypes = {
    className: PropTypes.string,
    placeholder: PropTypes.string,
    options: PropTypes.arrayOf(PropTypes.shape({
      label: PropTypes.string,
      value: PropTypes.any,
    })).isRequired,
    input: PropTypes.shape({
      value: PropTypes.any,
      onChange: PropTypes.func.isRequired,
      onBlur: PropTypes.func.isRequired,
    }).isRequired,
  };

  onChange = options => this.props.input.onChange(options.map(o => (o.value ? o.value : o)));

  onBlur = () => {
    const { value, onBlur } = this.props.input;
    onBlur(value);
  };

  render() {
    const { className, placeholder, options,
      input: { value } } = this.props;
    return (
      <Select
        className={classNames(className, styles.select)}
        placeholder={placeholder}
        options={options}
        value={value}
        onChange={this.onChange}
        onBlur={this.onBlur}
        searchable={false}
        multi
      />
    );
  }
}

const MultiselectField = ({ name, ...props }) => (
  <Field
    name={name}
    component={Multiselect}
    {...props}
  />
);
export default MultiselectField;

MultiselectField.propTypes = {
  name: PropTypes.string.isRequired,
};
