import { Field } from 'redux-form';
import Select from 'components/autocomplete/autocomplete-select';

import styles from './autocomplete-field.module.css';

class RenderAutocomplete extends Component {
  static propTypes = {
    className: PropTypes.string,
    input: PropTypes.shape({
      value: PropTypes.any,
      onChange: PropTypes.func.isRequired,
      onBlur: PropTypes.func.isRequired,
    }).isRequired,
    meta: PropTypes.shape({
      invalid: PropTypes.bool,
      dispatch: PropTypes.func.isRequired,
    }).isRequired,
    fetcher: PropTypes.func.isRequired,
    reader: PropTypes.func.isRequired,
  };

  onFetch = (args) => {
    const { fetcher, meta: { dispatch } } = this.props;
    return dispatch(fetcher(args));
  };

  onBlur = () => {
    const { value, onBlur } = this.props.input;
    onBlur(value);
  };

  render() {
    const {
      className,
      input,
      meta,
      ...props
    } = this.props;
    return (
      <Select
        className={classNames(className, {
          [styles.invalid]: meta.invalid,
        })}
        {...input}
        {...props}
        fetcher={this.onFetch}
        onBlur={this.onBlur}
      />
    );
  }
}

const AutocompleteField = ({ name, ...props }) => (
  <Field
    name={name}
    component={RenderAutocomplete}
    {...props}
  />
);
export default AutocompleteField;

AutocompleteField.propTypes = {
  name: PropTypes.string.isRequired,
  fetcher: PropTypes.func.isRequired,
  reader: PropTypes.func.isRequired,
};
