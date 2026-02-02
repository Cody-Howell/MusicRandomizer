import React, { useState, useEffect } from 'react';
import './AudioComponents.css';

interface AudioInfo {
  Id: number;
  UserGivenName: string;
  Author: string;
}

interface AudioPlayerProps {
  audioId?: number;
  audioInfo?: AudioInfo;
}

export const AudioPlayer: React.FC<AudioPlayerProps> = ({ audioId, audioInfo: propAudioInfo }) => {
  const [audioInfo, setAudioInfo] = useState<AudioInfo | null>(propAudioInfo || null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  const [audioUrl, setAudioUrl] = useState<string | null>(null);

  useEffect(() => {
    if (audioId && !propAudioInfo) {
      fetchAudioInfo(audioId);
    }
  }, [audioId, propAudioInfo]);

  const fetchAudioInfo = async (id: number) => {
    setIsLoading(true);
    setError('');
    
    try {
      const response = await fetch(`/api/audio/${id}/info`);
      
      if (response.ok) {
        const info = await response.json();
        setAudioInfo(info);
        setAudioUrl(`/api/audio/${id}`);
      } else {
        setError('Failed to load audio information');
      }
    } catch (err) {
      setError('Error loading audio information');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    if (audioInfo && !audioUrl) {
      setAudioUrl(`/api/audio/${audioInfo.Id}`);
    }
  }, [audioInfo, audioUrl]);

  if (isLoading) {
    return <div className="audio-player loading">Loading audio...</div>;
  }

  if (error) {
    return <div className="audio-player error">Error: {error}</div>;
  }

  if (!audioInfo || !audioUrl) {
    return <div className="audio-player">No audio selected</div>;
  }

  return (
    <div className="audio-player">
      <div className="audio-info">
        <h3>{audioInfo.UserGivenName}</h3>
        <p className="author">by {audioInfo.Author}</p>
      </div>
      
      <audio
        controls
        preload="metadata"
        className="audio-element"
      >
        <source src={audioUrl} type="audio/mpeg" />
        <source src={audioUrl} type="audio/wav" />
        <source src={audioUrl} type="audio/ogg" />
        Your browser does not support the audio element.
      </audio>
    </div>
  );
};