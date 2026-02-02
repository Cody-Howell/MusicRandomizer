import React, { useState, useEffect } from 'react';
import { AudioPlayer } from './AudioPlayer';
import './AudioComponents.css';

interface AudioInfo {
  Id: number;
  UserGivenName: string;
  Author: string;
}

export const AudioList: React.FC = () => {
  const [audioList, setAudioList] = useState<AudioInfo[]>([]);
  const [selectedAudio, setSelectedAudio] = useState<AudioInfo | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    fetchAudioList();
  }, []);

  const fetchAudioList = async () => {
    setIsLoading(true);
    setError('');
    
    try {
      const response = await fetch('/api/audio');
      
      if (response.ok) {
        const data = await response.json();
        setAudioList(data);
      } else {
        setError('Failed to load audio list');
      }
    } catch (err) {
      setError('Error loading audio list');
    } finally {
      setIsLoading(false);
    }
  };

  const handleSelectAudio = (audio: AudioInfo) => {
    setSelectedAudio(audio);
  };

  if (isLoading) {
    return <div className="audio-list loading">Loading audio list...</div>;
  }

  if (error) {
    return (
      <div className="audio-list error">
        Error: {error}
        <button onClick={fetchAudioList}>Retry</button>
      </div>
    );
  }

  return (
    <div className="audio-list">
      <h2>Audio Library</h2>
      
      {audioList.length === 0 ? (
        <p>No audio files uploaded yet.</p>
      ) : (
        <>
          <div className="audio-items">
            {audioList.map((audio) => (
              <div
                key={audio.Id}
                className={`audio-item ${selectedAudio?.Id === audio.Id ? 'selected' : ''}`}
                onClick={() => handleSelectAudio(audio)}
              >
                <h4>{audio.UserGivenName}</h4>
                <p>by {audio.Author}</p>
              </div>
            ))}
          </div>
          
          {selectedAudio && (
            <div className="selected-audio">
              <h3>Now Playing:</h3>
              <AudioPlayer audioInfo={selectedAudio} />
            </div>
          )}
        </>
      )}
      
      <button onClick={fetchAudioList} className="refresh-btn">
        Refresh List
      </button>
    </div>
  );
};