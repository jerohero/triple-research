import axios from 'axios'

export default () => {
  const options: any = {
    baseURL: 'https://rcvfunctions.azurewebsites.net'
    // baseURL: 'http://localhost:7071/api'
  };

  const instance = axios.create(options);

  instance.interceptors.response.use(
    (response) => {
      return response;
    },
    (error) => {
      return Promise.reject(error);
    }
  );

  return instance;
};