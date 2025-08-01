import { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { userApi } from '../services/api';
import { User, Edit, Trash2, Save, X } from 'lucide-react';
import toast from 'react-hot-toast';

const Profile = () => {
  const { user, updateUser, logout } = useAuth();
  const [isEditing, setIsEditing] = useState(false);
  const [formData, setFormData] = useState({
    username: user?.username || '',
    firstName: user?.firstName || '',
    lastName: user?.lastName || '',
    password: ''
  });
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);

    try {
      const updateData = { ...formData };
      if (!updateData.password) delete updateData.password;
      
      const response = await userApi.updateUser(user.userId, updateData);
      updateUser(response.data.user);
      setIsEditing(false);
      toast.success('Profile updated successfully!');
    } catch (error) {
      toast.error('Failed to update profile');
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteAccount = async () => {
    if (!window.confirm('Are you sure you want to delete your account? This action cannot be undone.')) {
      return;
    }

    setLoading(true);
    try {
      await userApi.deleteUser(user.userId);
      toast.success('Account deleted successfully');
      logout();
    } catch (error) {
      toast.error('Failed to delete account');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  return (
    <div className="profile-container">
      <div className="profile-card">
        <div className="profile-header">
          <User size={48} />
          <h2>Profile</h2>
        </div>

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label>Username</label>
            <input
              type="text"
              name="username"
              value={formData.username}
              onChange={handleChange}
              disabled={!isEditing}
            />
          </div>
          <div className="form-group">
            <label>First Name</label>
            <input
              type="text"
              name="firstName"
              value={formData.firstName}
              onChange={handleChange}
              disabled={!isEditing}
            />
          </div>
          <div className="form-group">
            <label>Last Name</label>
            <input
              type="text"
              name="lastName"
              value={formData.lastName}
              onChange={handleChange}
              disabled={!isEditing}
            />
          </div>
          {isEditing && (
            <div className="form-group">
              <label>New Password (leave blank to keep current)</label>
              <input
                type="password"
                name="password"
                value={formData.password}
                onChange={handleChange}
              />
            </div>
          )}

          <div className="profile-actions">
            {isEditing ? (
              <>
                <button type="submit" disabled={loading} className="btn-primary">
                  <Save size={16} />
                  {loading ? 'Saving...' : 'Save Changes'}
                </button>
                <button
                  type="button"
                  onClick={() => setIsEditing(false)}
                  className="btn-secondary"
                >
                  <X size={16} />
                  Cancel
                </button>
              </>
            ) : (
              <button
                type="button"
                onClick={() => setIsEditing(true)}
                className="btn-primary"
              >
                <Edit size={16} />
                Edit Profile
              </button>
            )}
          </div>
        </form>

        <div className="danger-zone">
          <h3>Danger Zone</h3>
          <button
            onClick={handleDeleteAccount}
            disabled={loading}
            className="btn-danger"
          >
            <Trash2 size={16} />
            Delete Account
          </button>
        </div>
      </div>
    </div>
  );
};

export default Profile; 