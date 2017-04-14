import AutocompleteFilter from 'components/sub-header/autocomplete-filter';
import { searchTenants } from 'store/tenants/actions';
import { readTenant } from 'store/data/tenant';

const readTenantOption = (data) => {
  const { uid, name } = readTenant(data);
  return { label: name, value: uid };
};

const TenantsFilter = ({ name, filters, onFilterChange }) => (
  <AutocompleteFilter
    name={name}
    filters={filters}
    onFilterChange={onFilterChange}
    fetchPageAction={searchTenants}
    reader={readTenantOption}
    placeholder='Filter by Tenant'
    clearOption={{ label: 'All tenants', value: null }}
  />
);
export default TenantsFilter;

TenantsFilter.propTypes = {
  name: PropTypes.string.isRequired,
  filters: PropTypes.objectOf(PropTypes.string).isRequired,
  onFilterChange: PropTypes.func.isRequired,
};

export const renderTenantsFilter = props => (
  <TenantsFilter key={props.name} {...props} />
);
renderTenantsFilter.propTypes = {
  name: PropTypes.string.isRequired,
};
