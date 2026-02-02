import React, { useState } from 'react';
import './AudioComponents.css';

interface AudioUploadFormProps {
  onUploadSuccess?: (audioId: number) => void;
}

export const AudioUploadForm: React.FC<AudioUploadFormProps> = ({ onUploadSuccess }) => {
  const [formData, setFormData] = useState({
    userGivenName: '',
    author: '',
    audioFile: null as File | null,
  });
  const [isUploading, setIsUploading] = useState(false);
  const [message, setMessage] = useState('');

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0] || null;
    setFormData(prev => ({
      ...prev,
      audioFile: file
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!formData.audioFile) {
      setMessage('Please select an audio file');
      return;
    }

    if (!formData.userGivenName.trim()) {
      setMessage('Please enter a name for the audio');
      return;
    }

    if (!formData.author.trim()) {
      setMessage('Please enter an author name');
      return;
    }

    setIsUploading(true);
    setMessage('');

    try {
      const uploadData = new FormData();
      uploadData.append('audioFile', formData.audioFile);
      uploadData.append('userGivenName', formData.userGivenName.trim());
      uploadData.append('author', formData.author.trim());

      console.log('Uploading file:', formData.audioFile.name, 'Size:', formData.audioFile.size);

      const response = await fetch('/api/audio/upload', {
        method: 'POST',
        body: uploadData,
      });

      console.log('Response status:', response.status, response.statusText);

      if (response.ok) {
        const result = await response.json();
        setMessage('Audio uploaded successfully!');
        setFormData({
          userGivenName: '',
          author: '',
          audioFile: null,
        });
        // Reset file input
        const fileInput = document.querySelector('input[type="file"]') as HTMLInputElement;
        if (fileInput) fileInput.value = '';
        
        onUploadSuccess?.(result.Id);
      } else {
        let errorMessage = 'Upload failed';
        try {
          const errorText = await response.text();
          console.log('Error response text:', errorText);
          errorMessage = errorText || `HTTP ${response.status}: ${response.statusText}`;
        } catch (e) {
          errorMessage = `HTTP ${response.status}: ${response.statusText}`;
        }
        setMessage(`Upload failed: ${errorMessage}`);
      }
    } catch (error) {
      console.error('Upload error:', error);
      setMessage(`Upload error: ${error instanceof Error ? error.message : 'Unknown error'}`);
    } finally {
      setIsUploading(false);
    }
  };

  return (
    <div className="audio-upload-form">
      <h2>Upload Audio File</h2>
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label htmlFor="userGivenName">Audio Name:</label>
          <input
            type="text"
            id="userGivenName"
            name="userGivenName"
            value={formData.userGivenName}
            onChange={handleInputChange}
            placeholder="Enter a name for this audio"
            disabled={isUploading}
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="author">Author:</label>
          <input
            type="text"
            id="author"
            name="author"
            value={formData.author}
            onChange={handleInputChange}
            placeholder="Enter the author name"
            disabled={isUploading}
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="audioFile">Audio File:</label>
          <input
            type="file"
            id="audioFile"
            name="audioFile"
            onChange={handleFileChange}
            accept="audio/*"
            disabled={isUploading}
            required
          />
        </div>

        <button type="submit" disabled={isUploading}>
          {isUploading ? 'Uploading...' : 'Upload Audio'}
        </button>
      </form>

      {message && (
        <div className={`message ${message.includes('successfully') ? 'success' : 'error'}`}>
          {message}
        </div>
      )}
    </div>
  );
};