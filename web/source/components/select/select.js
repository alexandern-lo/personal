import ReactSelect from 'react-select';

import styles from './select.module.css';

const clearRenderer = () => null;

const Select = props => (
  <ReactSelect
    {...props}
    arrowRenderer={() => null}
    clearRenderer={clearRenderer}
    className={classNames(props.className, styles.select)}
  />
);

export default Select;

Select.propTypes = {
  ...ReactSelect.propTypes,
};
