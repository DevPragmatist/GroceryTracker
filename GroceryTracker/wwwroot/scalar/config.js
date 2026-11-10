const accessToken = sessionStorage.getItem('grocerytracker.accessToken');

if (!accessToken && window.location.pathname.startsWith('/scalar')) {
  window.location.replace('/');
}

export default {
  authentication: {
    preferredSecurityScheme: 'BearerAuth',
    securitySchemes: {
      BearerAuth: {
        token: accessToken || ''
      }
    }
  }
};
