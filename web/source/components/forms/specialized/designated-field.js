import InputField from 'components/forms/input-field';
import SelectField, { optionsShape } from 'components/forms/select-field';
import { RemoveButton } from './buttons';

import styles from './designated-field.module.css';

const DesignatedField = ({
  className,
  name,
  designationProp,
  designations,
  valueProp,
  onRemove,
  ...props
}) => (
  <div className={classNames(className, styles.container)}>
    <SelectField
      className={styles.select}
      name={`${name}.${designationProp}`}
      options={designations}
    />
    <div className={styles.separator}><div /></div>
    <InputField
      className={styles.input}
      name={`${name}.${valueProp}`}
      {...props}
    />
    {onRemove && <RemoveButton onClick={onRemove} />}
  </div>
);
export default DesignatedField;

DesignatedField.propTypes = {
  className: PropTypes.string,
  name: PropTypes.string.isRequired,
  designationProp: PropTypes.string.isRequired,
  designations: optionsShape.isRequired,
  valueProp: PropTypes.string.isRequired,
  onRemove: PropTypes.func,
};

DesignatedField.defaultProps = {
  designationProp: 'designation',
};
