import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { pictureApi, commentApi } from '../services/api';
import { Heart, ThumbsDown, MessageCircle, ArrowLeft } from 'lucide-react';
import toast from 'react-hot-toast';

const PictureDetail = () => {
  const { id } = useParams();
  const { user } = useAuth();
  const [picture, setPicture] = useState(null);
  const [comments, setComments] = useState([]);
  const [newComment, setNewComment] = useState('');
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchPicture();
    fetchComments();
  }, [id]);

  const fetchPicture = async () => {
    try {
      const response = await pictureApi.getPicture(id);
      setPicture(response.data.data);
    } catch (error) {
      toast.error('Failed to load picture');
    } finally {
      setLoading(false);
    }
  };

  const fetchComments = async () => {
    try {
      const response = await commentApi.getComments(id);
      setComments(response.data.data || []);
    } catch (error) {
      console.error('Failed to load comments');
    }
  };

  const handleAddComment = async (e) => {
    e.preventDefault();
    if (!newComment.trim()) return;

    try {
      await commentApi.addComment(id, {
        content: newComment,
        userId: user.userId.toString()
      });
      setNewComment('');
      fetchComments();
      toast.success('Comment added!');
    } catch (error) {
      toast.error('Failed to add comment');
    }
  };

  if (loading) {
    return <div className="loading">Loading...</div>;
  }

  if (!picture) {
    return <div className="error">Picture not found</div>;
  }

  return (
    <div className="picture-detail">
      <Link to="/" className="back-link">
        <ArrowLeft size={20} />
        Back to Home
      </Link>
      
      <div className="picture-content">
        <img src={picture.imageUrl} alt={picture.title} />
        <div className="picture-info">
          <h1>{picture.title}</h1>
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
          </div>
        </div>
      </div>

      <div className="comments-section">
        <h3>Comments</h3>
        {user && (
          <form onSubmit={handleAddComment} className="comment-form">
            <textarea
              value={newComment}
              onChange={(e) => setNewComment(e.target.value)}
              placeholder="Add a comment..."
              rows="3"
            />
            <button type="submit" className="btn-primary">
              <MessageCircle size={16} />
              Add Comment
            </button>
          </form>
        )}
        
        <div className="comments-list">
          {comments.map((comment) => (
            <div key={comment.commentId} className="comment">
              <p>{comment.content}</p>
              <small>By User {comment.userId} • {new Date(comment.commentDate).toLocaleDateString()}</small>
            </div>
          ))}
        </div>
        
        {comments.length === 0 && (
          <p>No comments yet. Be the first to comment!</p>
        )}
      </div>
    </div>
  );
};

export default PictureDetail; 