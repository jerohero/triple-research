import axios from 'axios'

export default () => {
  const options: any = {
    baseURL: 'https://rcvfunctions.azurewebsites.net/api'
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