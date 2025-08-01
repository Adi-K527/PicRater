import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { LogOut, User, Upload, Home } from 'lucide-react';

const Navbar = () => {
  const { isAuthenticated, user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/');
  };

  return (
    <nav className="navbar">
      <div className="nav-brand">
        <Link to="/">PicRater</Link>
      </div>
      <div className="nav-links">
        <Link to="/" className="nav-link">
          <Home size={20} />
          Home
        </Link>
        {isAuthenticated ? (
          <>
            <Link to="/upload" className="nav-link">
              <Upload size={20} />
              Upload
            </Link>
            <Link to="/profile" className="nav-link">
              <User size={20} />
              Profile
            </Link>
            <button onClick={handleLogout} className="nav-link logout-btn">
              <LogOut size={20} />
              Logout
            </button>
          </>
        ) : (
          <>
            <Link to="/login" className="nav-link">Login</Link>
            <Link to="/register" className="nav-link">Register</Link>
          </>
        )}
      </div>
    </nav>
  );
};

export default Navbar; 