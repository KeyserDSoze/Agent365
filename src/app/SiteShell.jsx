import { useEffect, useState } from 'react'
import { NavLink, Outlet, useLocation, useNavigate } from 'react-router-dom'
import { copy } from './content.js'
import { navRoutes } from './routing.js'

function Brand() {
  return (
    <NavLink className="brand" to="/" aria-label="Agent 365 home">
      <span className="brand-mark">A</span>
      <span><strong>Agent 365</strong><small>Knowledge Hub</small></span>
    </NavLink>
  )
}

export default function SiteShell({ lang, setLang, theme, setTheme }) {
  const t = copy[lang]
  const location = useLocation()
  const navigate = useNavigate()
  const [menuOpen, setMenuOpen] = useState(false)

  useEffect(() => {
    setMenuOpen(false)
    window.scrollTo({ top: 0, behavior: 'instant' })
  }, [location.pathname])

  const toggleLanguage = () => {
    const next = lang === 'it' ? 'en' : 'it'
    let target = location.pathname

    if (next === 'en' && target.startsWith('/knowledge/docs/') && !target.startsWith('/knowledge/docs/en/')) {
      target = target.replace('/knowledge/docs/', '/knowledge/docs/en/')
    } else if (next === 'it' && target.startsWith('/knowledge/docs/en/')) {
      target = target.replace('/knowledge/docs/en/', '/knowledge/docs/')
    }

    setLang(next)
    if (target !== location.pathname) navigate(target + location.hash)
  }

  return (
    <div className="page-shell">
      <header className="topbar">
        <Brand />
        <nav className="desktop-nav" aria-label="Primary navigation">
          {navRoutes.map(([id, route]) => (
            <NavLink
              key={id}
              to={route}
              end={route === '/'}
              className={({ isActive }) => isActive ? 'active' : ''}
            >
              {t.nav[id]}
            </NavLink>
          ))}
        </nav>
        <div className="controls">
          <button className="control-btn" onClick={toggleLanguage} title="Language">{lang.toUpperCase()}</button>
          <button className="control-btn" onClick={() => setTheme(theme === 'dark' ? 'light' : 'dark')} title="Theme">
            {theme === 'dark' ? '☀' : '◐'}
          </button>
          <a className="github-btn" href="https://github.com/KeyserDSoze/Agent365" target="_blank" rel="noreferrer">GitHub ↗</a>
          <button
            className="mobile-menu-btn"
            onClick={() => setMenuOpen(value => !value)}
            aria-expanded={menuOpen}
            aria-label="Toggle navigation"
          >
            {menuOpen ? '×' : '☰'}
          </button>
        </div>
        {menuOpen && (
          <nav className="mobile-nav" aria-label="Mobile navigation">
            {navRoutes.map(([id, route]) => (
              <NavLink key={id} to={route} end={route === '/'}>{t.nav[id]}</NavLink>
            ))}
          </nav>
        )}
      </header>
      <main className="routed-main"><Outlet /></main>
      <footer>
        <div className="brand footer-brand">
          <span className="brand-mark">A</span>
          <span><strong>Agent 365</strong><small>Knowledge Hub</small></span>
        </div>
        <p>{t.footer}</p>
      </footer>
    </div>
  )
}
