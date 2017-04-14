import React from 'react';
import { Link } from 'react-router';
import PageContentWrapper from 'components/layout/page-content-wrapper';

const NotFound = () => (
  <PageContentWrapper>
    <p>Page not found. Do you want to go to <Link to='/'>home</Link>?</p>
  </PageContentWrapper>
);
export default NotFound;
