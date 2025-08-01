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
          <div key={picture.picId} className="picture-card">
            <img 
              src={picture.imageUrl} 
              alt={picture.title}
              onError={(e) => {
                console.error('Image failed to load:', picture.imageUrl);
                e.target.style.display = 'none';
              }}
              onLoad={() => console.log('Image loaded successfully:', picture.imageUrl)}
            />
            <div className="picture-info">
              <h3>{picture.title}</h3>
              <p>{picture.description}</p>
              <div className="picture-actions">
                <button className="action-btn like-btn">
                  <Heart size={16} />
                  {picture.likes}
                </button>
                <button className="action-btn dislike-btn">
                  <ThumbsDown size={16} />
                  {picture.dislikes}
                </button>
                <Link to={`/picture/${picture.picId}`} className="action-btn comment-btn">
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