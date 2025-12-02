import axios from 'axios';

const apiUrl = process.env.REACT_APP_API_URL
axios.defaults.baseURL = process.env.REACT_APP_API_URL;



axios.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    console.error("Axios Error:", {
      url: error.config?.url,
      method: error.config?.method,
      status: error.response?.status,
      message: error.message,
    });

    return Promise.reject(error);
  }
);


export default {
  getTasks: async () => {
    const result = await axios.get(`/todos`)    
    return result.data;
  },

  addTask: async(name)=>{
    console.log('addTask', name)
    const result=await axios.post(`/todos`,{"name":name,"isComplete":false});
    return result.data;
  },

  setCompleted: async(id, isComplete)=>{
    console.log('setCompleted', {id, isComplete})
    const result= await axios.put(`/todo/${id}?isComplete=${isComplete}`)
    return result.data;
  },

  deleteTask:async(id)=>{
    console.log('deleteTask')
    await axios.delete(`/todo/${id}`)
  }
};
