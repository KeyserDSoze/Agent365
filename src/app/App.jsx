import { useEffect, useState } from 'react'
import { BrowserRouter, Route, Routes } from 'react-router-dom'
import SiteShell from './SiteShell.jsx'
import MarkdownViewer from './MarkdownViewer.jsx'
import KnowledgeIndex from './KnowledgeIndex.jsx'
import HomePage from './pages/HomePage.jsx'
import ArchitecturePage from './pages/ArchitecturePage.jsx'
import CapabilitiesPage from './pages/CapabilitiesPage.jsx'
import AssetsPage from './pages/AssetsPage.jsx'
import AcademyPage from './pages/AcademyPage.jsx'
import LabsPage from './pages/LabsPage.jsx'
import DeveloperPage from './pages/DeveloperPage.jsx'
import OperationsPage from './pages/OperationsPage.jsx'
import SourcesPage from './pages/SourcesPage.jsx'
import NotFoundPage from './pages/NotFoundPage.jsx'
import JourneyPage from './pages/JourneyPage.jsx'

export default function App() {
  const [lang, setLang] = useState(() => localStorage.getItem('a365-lang') || 'it')
  const [theme, setTheme] = useState(() => localStorage.getItem('a365-theme') || 'dark')
  const basename = import.meta.env.BASE_URL.replace(/\/$/, '')

  useEffect(() => {
    document.documentElement.dataset.theme = theme
    localStorage.setItem('a365-theme', theme)
  }, [theme])

  useEffect(() => {
    document.documentElement.lang = lang
    localStorage.setItem('a365-lang', lang)
  }, [lang])

  return (
    <BrowserRouter basename={basename}>
      <Routes>
        <Route element={<SiteShell lang={lang} setLang={setLang} theme={theme} setTheme={setTheme} />}>
          <Route index element={<HomePage lang={lang} />} />
          <Route path="journey" element={<JourneyPage lang={lang} />} />
          <Route path="architecture" element={<ArchitecturePage lang={lang} />} />
          <Route path="capabilities" element={<CapabilitiesPage lang={lang} />} />
          <Route path="assets" element={<AssetsPage lang={lang} />} />
          <Route path="academy" element={<AcademyPage lang={lang} />} />
          <Route path="labs" element={<LabsPage lang={lang} />} />
          <Route path="developer" element={<DeveloperPage lang={lang} />} />
          <Route path="operations" element={<OperationsPage lang={lang} />} />
          <Route path="knowledge" element={<KnowledgeIndex lang={lang} />} />
          <Route path="knowledge/*" element={<MarkdownViewer lang={lang} />} />
          <Route path="sources" element={<SourcesPage lang={lang} />} />
          <Route path="*" element={<NotFoundPage lang={lang} />} />
        </Route>
      </Routes>
    </BrowserRouter>
  )
}
