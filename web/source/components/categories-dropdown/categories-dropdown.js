import _ from 'lodash';
import { toggle } from 'components/helpers/select';
import styles from './categories-dropdown.module.css';

const convertItemsFromFilters = (items) => {
  const map = _.keyBy(items, o => o.category_uid);
  return _.mapValues(map, o => o.values);
};
const convertItemsToFilters = (items) => {
  const entries = _.toPairs(items);
  return _.filter(entries, e => !!e[1]).map(e => ({
    category_uid: e[0],
    values: e[1],
  }));
};

export default class CategoriesDropdown extends Component {
  static propTypes = {
    categories: PropTypes.arrayOf(
      PropTypes.shape({
        category_uid: PropTypes.string,
        name: PropTypes.string,
        values: PropTypes.arrayOf(PropTypes.shape({
          name: PropTypes.string,
          option_uid: PropTypes.string,
        })),
      }),
    ),
    chosenItems: PropTypes.arrayOf(
      PropTypes.shape({
        category_uid: PropTypes.string,
        values: PropTypes.arrayOf(PropTypes.string),
      }),
    ),
    onChange: PropTypes.func,
  };

  constructor(props) {
    super(props);

    this.state = { showDropdown: false, chosenItems: {} };
  }

  componentWillMount() {
    this.setState({ chosenItems: convertItemsFromFilters(this.props.chosenItems) });
  }

  componentWillReceiveProps(nextProps) {
    this.setState({ chosenItems: convertItemsFromFilters(nextProps.chosenItems) });
  }

  onMenuItemClick = (category, option) => {
    const { chosenItems } = this.state;
    const chosenOptions = toggle(chosenItems[category.category_uid], option.option_uid);
    this.setState({
      chosenItems: {
        ...chosenItems,
        [category.category_uid]: chosenOptions.length > 0 ? chosenOptions : undefined,
      },
    });
  };

  onMenuBlur = () => {
    this.setState({ showDropdown: false });
    if (this.props.categories.length > 0) {
      this.props.onChange(convertItemsToFilters(this.state.chosenItems));
    }
  };

  getChosenOptions = category => this.state.chosenItems[category.category_uid];

  isOptionChecked = (category, option) => {
    const chosenOptions = this.getChosenOptions(category);
    if (!chosenOptions) return false;
    return chosenOptions.indexOf(option.option_uid) >= 0;
  };

  renderCategoryOptions = category => (
    <div className={styles.subMenu}>
      <ul>
        {category.options.map(option => (
          <li
            key={option.option_uid}
            onClick={() => this.onMenuItemClick(category, option)}
          >
            <div
              className={classNames(
                styles.checkMarkSize,
                { [styles.checkMarkImg]: this.isOptionChecked(category, option) },
              )}
            />
            <a>{option.name}</a>
          </li>
        ))}
      </ul>
    </div>
  );

  renderCategory = (category) => {
    const chosenOptions = this.getChosenOptions(category);
    return (
      <li key={category.category_uid} className={styles.menuItem}>
        <div
          className={classNames(
            styles.checkMarkSize, { [styles.checkMarkImg]: !!chosenOptions },
          )}
        />
        <a>{category.name}</a>
        <div className={styles.checkedItemsCount}>
          {chosenOptions && chosenOptions.length}
        </div>
        <div className={styles.arrowImg} />
        {this.renderCategoryOptions(category)}
      </li>
    );
  };

  render() {
    const { categories } = this.props;
    const { showDropdown } = this.state;

    return (
      <div
        className={styles.dropdown}
        onClick={() => categories.length > 0 && this.setState({ showDropdown: true })}
        onBlur={this.onMenuBlur}
        tabIndex='0'
      >
        <div className={styles.dropdownHeader}>
          <span>{categories.length > 0 ? 'Category' : 'No categories'}</span>
          <div className={styles.dropdownImg} />
        </div>
        <ul className={classNames(styles.menu, { [styles.show]: showDropdown })}>
          {categories.map(this.renderCategory)}
        </ul>
      </div>
    );
  }
}
