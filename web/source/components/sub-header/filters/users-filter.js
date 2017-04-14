import AutocompleteFilter from 'components/sub-header/autocomplete-filter';
import { searchUsers } from 'store/users/actions';
import { readUser, getDisplayName } from 'store/data/user';

const readUserOption = (data) => {
  const user = readUser(data);
  return { label: getDisplayName(user), value: user.uid };
};

const UsersFilter = ({ name, filters, onFilterChange }) => (
  <AutocompleteFilter
    name={name}
    filters={filters}
    onFilterChange={onFilterChange}
    fetchPageAction={searchUsers}
    reader={readUserOption}
    placeholder='Filter by User'
    clearOption={{ label: 'All users', value: null }}
  />
);
export default UsersFilter;

UsersFilter.propTypes = {
  name: PropTypes.string.isRequired,
  filters: PropTypes.objectOf(PropTypes.string).isRequired,
  onFilterChange: PropTypes.func.isRequired,
};

export const renderUsersFilter = props => (
  <UsersFilter key={props.name} {...props} />
);
renderUsersFilter.propTypes = {
  name: PropTypes.string.isRequired,
};
