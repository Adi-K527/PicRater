import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { pictureApi } from '../services/api';
import { Heart, ThumbsDown, MessageCircle } from 'lucide-react';
import toast from 'react-hot-toast';

const Home = () => {
  const [pictures, setPictures] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchPictures();
  }, []);

  const fetchPictures = async () => {
    try {
      const response = await pictureApi.getAllPictures();
      console.log('Pictures response:', response.data);
      setPictures(response.data.data || []);
    } catch (error) {
      console.error('Error fetching pictures:', error);
      toast.error('Failed to load pictures');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <div className="loading">Loading pictures...</div>;
  }

  return (
    <div className="home">
      <h1>Welcome to PicRater</h1>
      <p>Discover and rate amazing pictures from our community!</p>
      
      <div className="pictures-grid">
        {pictures.map((picture) => (
          <div key={picture.PicId} className="picture-card">
            <img 
              src={picture.ImageUrl} 
              alt={picture.Title}
              onError={(e) => {
                console.error('Image failed to load:', picture.ImageUrl);
                e.target.style.display = 'none';
              }}
              onLoad={() => console.log('Image loaded successfully:', picture.ImageUrl)}
            />
            <div className="picture-info">
              <h3>{picture.Title}</h3>
              <p>{picture.Description}</p>
              <div className="picture-actions">
                <button className="action-btn like-btn">
                  <Heart size={16} />
                  {picture.Likes}
                </button>
                <button className="action-btn dislike-btn">
                  <ThumbsDown size={16} />
                  {picture.Dislikes}
                </button>
                <Link to={`/picture/${picture.PicId}`} className="action-btn comment-btn">
                  <MessageCircle size={16} />
                  Comments
                </Link>
              </div>
            </div>
          </div>
        ))}
      </div>
      
      {pictures.length === 0 && (
        <div className="empty-state">
          <p>No pictures uploaded yet. Be the first to share!</p>
        </div>
      )}
    </div>
  );
};

export default Home; 