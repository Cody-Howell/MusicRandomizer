import { useState } from 'react'
import './App.css'
import './components/AudioComponents.css'
import { AudioUploadForm, AudioList } from './components'

function App() {
  const [activeTab, setActiveTab] = useState<'upload' | 'library'>('upload')
  const [refreshTrigger, setRefreshTrigger] = useState(0)

  const handleUploadSuccess = (audioId: number) => {
    console.log('Audio uploaded successfully with ID:', audioId)
    // Trigger a refresh of the library when switching to it
    setRefreshTrigger(prev => prev + 1)
    // Optionally switch to library tab to see the uploaded audio
    setActiveTab('library')
  }

  return (
    <>
      <div className="app-header">
        <h1>Music Randomizer</h1>
        <nav className="tab-nav">
          <button 
            className={activeTab === 'upload' ? 'active' : ''} 
            onClick={() => setActiveTab('upload')}
          >
            Upload Audio
          </button>
          <button 
            className={activeTab === 'library' ? 'active' : ''} 
            onClick={() => setActiveTab('library')}
          >
            Audio Library
          </button>
        </nav>
      </div>

      <div className="app-content">
        {activeTab === 'upload' ? (
          <AudioUploadForm onUploadSuccess={handleUploadSuccess} />
        ) : (
          <AudioList key={refreshTrigger} />
        )}
      </div>
    </>
  )
}

export default App
