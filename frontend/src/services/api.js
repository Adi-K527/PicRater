import axios from 'axios';

const API_BASE_URL = 'http://52.206.203.115:30080'; // UserService
const PIC_API_BASE_URL = 'http://52.206.203.115:30081'; // PicService

const api = axios.create({
  baseURL: API_BASE_URL,
});

const picApi = axios.create({
  baseURL: PIC_API_BASE_URL,
});

// Request interceptor to add auth token
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

picApi.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// User API
export const userApi = {
  login: (credentials) => api.post('/api/User/login', credentials),
  register: (userData) => api.post('/api/User/register', userData),
  getUser: (id) => api.get(`/api/User/${id}`),
  updateUser: (id, userData) => api.put(`/api/User/${id}`, userData),
  deleteUser: (id) => api.delete(`/api/User/${id}`),
};

// Picture API
export const pictureApi = {
  getAllPictures: () => picApi.get('/api/Pic'),
  getPicture: (id) => picApi.get(`/api/Pic/${id}`),
  uploadPicture: (formData) => picApi.post('/api/Pic', formData),
  deletePicture: (id) => picApi.delete(`/api/Pic/${id}`),
};

// Comment API
export const commentApi = {
  getComments: (picId) => picApi.get(`/api/Comment/${picId}`),
  addComment: (picId, comment) => picApi.post(`/api/Comment/${picId}`, comment),
  deleteComment: (id) => picApi.delete(`/api/Comment/${id}`),
}; 