import axios from 'axios'
import { useUserStore } from '@/stores/user'

export default (withAuth = false) => {
  const userStore = useUserStore();

  const options: any = {
    baseURL: 'https://vrefsolutions-api.azurewebsites.net/api',
    headers: {
      Authorization: withAuth ? userStore.bearerToken : '',
    },
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