import React from 'react'
import { createRoot } from 'react-dom/client'
import App from './App.jsx'
import './styles.css'

const redirectedPath = sessionStorage.getItem('a365-spa-redirect')
if (redirectedPath) {
  sessionStorage.removeItem('a365-spa-redirect')
  window.history.replaceState(null, '', redirectedPath)
}

createRoot(document.getElementById('root')).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
)
