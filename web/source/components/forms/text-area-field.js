import { Field } from 'redux-form';

import styles from './text-area-field.module.css';

export class TextArea extends Component {
  static propTypes = {
    className: PropTypes.string,
    placeholder: PropTypes.string,
    maxLength: PropTypes.number,
    value: PropTypes.string,
    onChange: PropTypes.func.isRequired,
  };

  static defaultProps = {
    maxLength: 90,
  };

  render() {
    const { value, onChange, className, placeholder, maxLength, ...props } = this.props;
    const length = value ? value.length : 0;
    return (
      <div className={classNames(className, styles.container)}>
        <textarea
          placeholder={placeholder}
          value={value}
          onChange={onChange}
          maxLength={maxLength}
          {...props}
        />
        <div className={styles.counter}>
          {length}/{maxLength}
        </div>
      </div>
    );
  }
}

const RenderTextAreaField = ({ className, input, meta, ...props }) => (
  <TextArea
    className={classNames(className, { [styles.invalid]: meta.invalid })}
    {...input}
    {...props}
  />
);

RenderTextAreaField.propTypes = {
  className: PropTypes.string,
  input: PropTypes.shape({
    value: PropTypes.string,
  }),
  meta: PropTypes.shape({
    invalid: PropTypes.bool,
  }),
  disabled: PropTypes.bool,
};

const TextAreaField = ({ name, ...props }) => (
  <Field
    name={name}
    component={RenderTextAreaField}
    {...props}
  />
);
export default TextAreaField;

TextAreaField.propTypes = {
  name: PropTypes.string.isRequired,
};
