import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { pictureApi } from '../services/api';
import { Upload as UploadIcon } from 'lucide-react';
import toast from 'react-hot-toast';

const Upload = () => {
  const [formData, setFormData] = useState({
    title: '',
    description: ''
  });
  const [file, setFile] = useState(null);
  const [loading, setLoading] = useState(false);
  const { user } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!file) {
      toast.error('Please select an image');
      return;
    }

    setLoading(true);
    const data = new FormData();
    data.append('file', file);
    data.append('title', formData.title);
    data.append('description', formData.description);
    data.append('userId', user.userId.toString());

    // Debug: Log what's being sent
    console.log('File:', file);
    console.log('Title:', formData.title);
    console.log('Description:', formData.description);
    console.log('UserId:', user.userId.toString());

    try {
      await pictureApi.uploadPicture(data);
      toast.success('Picture uploaded successfully!');
      navigate('/');
    } catch (error) {
      console.error('Upload error:', error);
      toast.error('Failed to upload picture');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleFileChange = (e) => {
    setFile(e.target.files[0]);
  };

  return (
    <div className="upload-container">
      <div className="upload-card">
        <h2>Upload Picture</h2>
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label>Title</label>
            <input
              type="text"
              name="title"
              value={formData.title}
              onChange={handleChange}
              required
            />
          </div>
          <div className="form-group">
            <label>Description</label>
            <textarea
              name="description"
              value={formData.description}
              onChange={handleChange}
              rows="3"
            />
          </div>
          <div className="form-group">
            <label>Image</label>
            <input
              type="file"
              accept="image/*"
              onChange={handleFileChange}
              required
            />
          </div>
          <button type="submit" disabled={loading} className="btn-primary">
            <UploadIcon size={16} />
            {loading ? 'Uploading...' : 'Upload Picture'}
          </button>
        </form>
      </div>
    </div>
  );
};

export default Upload; 