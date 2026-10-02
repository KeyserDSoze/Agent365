import { useEffect, useMemo, useState } from 'react'
import { copy, officialLinks } from './content.js'

function Icon({ children }) {
  return <span className="icon" aria-hidden="true">{children}</span>
}

function CodePreview() {
  return (
    <pre className="code">
      <code>{`AgentsInfo
| project Timestamp, AgentName, AgentType, Platform, Owner, Status
| order by Timestamp desc`}</code>
    </pre>
  )
}

export default function App() {
  const [lang, setLang] = useState(() => localStorage.getItem('a365-lang') || 'it')
  const [theme, setTheme] = useState(() => localStorage.getItem('a365-theme') || 'dark')
  const [query, setQuery] = useState('')
  const t = copy[lang]

  useEffect(() => {
    document.documentElement.dataset.theme = theme
    localStorage.setItem('a365-theme', theme)
  }, [theme])

  useEffect(() => {
    document.documentElement.lang = lang
    localStorage.setItem('a365-lang', lang)
  }, [lang])

  const domains = useMemo(() => {
    const q = query.trim().toLowerCase()
    if (!q) return t.domains
    return t.domains.filter(d =>
      [d.title, d.summary, d.tag, ...d.bullets, d.example].join(' ').toLowerCase().includes(q)
    )
  }, [query, t.domains])

  const jump = id => document.getElementById(id)?.scrollIntoView({ behavior: 'smooth' })

  return (
    <div className="page-shell">
      <header className="topbar">
        <button className="brand" onClick={() => jump('overview')} aria-label="Agent 365 home">
          <span className="brand-mark">A</span>
          <span><strong>Agent 365</strong><small>Knowledge Hub</small></span>
        </button>
        <nav className="desktop-nav">
          {Object.entries(t.nav).map(([id, label]) =>
            <button key={id} onClick={() => jump(id)}>{label}</button>
          )}
        </nav>
        <div className="controls">
          <button className="control-btn" onClick={() => setLang(lang === 'it' ? 'en' : 'it')} title="Language">
            {lang.toUpperCase()}
          </button>
          <button className="control-btn" onClick={() => setTheme(theme === 'dark' ? 'light' : 'dark')} title="Theme">
            {theme === 'dark' ? '☀' : '◐'}
          </button>
          <a className="github-btn" href="https://github.com/KeyserDSoze/Agent365" target="_blank" rel="noreferrer">GitHub ↗</a>
        </div>
      </header>

      <main>
        <section className="hero" id="overview">
          <div className="hero-glow hero-glow-a" />
          <div className="hero-glow hero-glow-b" />
          <div className="hero-content">
            <p className="eyebrow">{t.eyebrow}</p>
            <h1>{t.heroTitle}</h1>
            <p className="hero-copy">{t.heroBody}</p>
            <div className="hero-actions">
              <button className="primary" onClick={() => jump('architecture')}>{t.primaryCta} →</button>
              <button className="secondary" onClick={() => jump('training')}>{t.secondaryCta}</button>
            </div>
            <div className="hero-facts">
              <div><span>{t.status}</span><strong>{t.ga}</strong></div>
              <div><span>{t.principle}</span><strong>{t.principleText}</strong></div>
            </div>
          </div>
          <div className="hero-panel">
            <div className="mini-window">
              <div className="window-dots"><i/><i/><i/></div>
              <p className="window-label">ADVANCED HUNTING · EXAMPLE</p>
              <CodePreview />
              <div className="signal-row">
                <span><b>Observe</b> Registry</span>
                <span><b>Secure</b> Defender</span>
                <span><b>Govern</b> Purview</span>
              </div>
            </div>
          </div>
        </section>

        <section className="section">
          <div className="section-heading">
            <p className="kicker">01 · MODEL</p>
            <h2>{t.overviewTitle}</h2>
            <p>{t.overviewIntro}</p>
          </div>
          <div className="pillar-grid">
            {t.pillars.map(([title, text], i) => (
              <article className="pillar-card" key={title}>
                <span className="number">0{i + 1}</span>
                <h3>{title}</h3>
                <p>{text}</p>
              </article>
            ))}
          </div>
        </section>

        <section className="section architecture" id="architecture">
          <div className="section-heading">
            <p className="kicker">02 · CONTROL PLANE</p>
            <h2>{t.architectureTitle}</h2>
            <p>{t.architectureIntro}</p>
          </div>
          <div className="arch-stack">
            {t.layers.map(layer => (
              <article className="arch-row" key={layer.n}>
                <span className="arch-n">{layer.n}</span>
                <div className="arch-title"><small>{layer.title}</small><strong>{layer.product}</strong></div>
                <p>{layer.text}</p>
              </article>
            ))}
          </div>
        </section>

        <section className="section" id="domains">
          <div className="section-heading split">
            <div>
              <p className="kicker">03 · CAPABILITIES</p>
              <h2>{t.domainsTitle}</h2>
              <p>{t.domainsIntro}</p>
            </div>
            <label className="search">
              <Icon>⌕</Icon>
              <input value={query} onChange={e => setQuery(e.target.value)} placeholder={t.search} />
            </label>
          </div>
          <div className="domain-grid">
            {domains.map(domain => (
              <article className="domain-card" key={domain.id}>
                <div className="domain-top"><span className="tag">{domain.tag}</span><span className="arrow">↗</span></div>
                <h3>{domain.title}</h3>
                <p className="summary">{domain.summary}</p>
                <ul>{domain.bullets.map(x => <li key={x}>{x}</li>)}</ul>
                <p className="example">{domain.example}</p>
              </article>
            ))}
          </div>
        </section>

        <section className="section" id="examples">
          <div className="section-heading">
            <p className="kicker">04 · REUSABLE ASSETS</p>
            <h2>{t.examplesTitle}</h2>
            <p>{t.examplesIntro}</p>
          </div>
          <div className="asset-list">
            {t.examples.map(([title, text], i) => (
              <div className="asset" key={title}>
                <span>{String(i + 1).padStart(2, '0')}</span>
                <div><h3>{title}</h3><p>{text}</p></div>
                <b>→</b>
              </div>
            ))}
          </div>
        </section>

        <section className="section training" id="training">
          <div className="section-heading">
            <p className="kicker">05 · SKILLING</p>
            <h2>{t.trainingTitle}</h2>
            <p>{t.trainingIntro}</p>
          </div>
          <div className="weeks">
            {t.weeks.map((week, i) => (
              <article className="week" key={week.title}>
                <div className="week-head"><span>0{i + 1}</span><h3>{week.title}</h3></div>
                <ol>{week.days.map(day => <li key={day}>{day}</li>)}</ol>
              </article>
            ))}
          </div>
        </section>

        <section className="section" id="labs">
          <div className="section-heading">
            <p className="kicker">06 · HANDS-ON</p>
            <h2>{t.labsTitle}</h2>
            <p>{t.labsIntro}</p>
          </div>
          <div className="lab-grid">
            {t.labs.map(([code, title, text]) => (
              <article className="lab" key={code}>
                <span>{code}</span>
                <h3>{title}</h3>
                <p>{text}</p>
              </article>
            ))}
          </div>
        </section>

        <section className="section sources" id="sources">
          <div className="section-heading">
            <p className="kicker">07 · REFERENCES</p>
            <h2>{t.sourcesTitle}</h2>
            <p>{t.sourcesIntro}</p>
          </div>
          <div className="source-grid">
            {officialLinks.map(link => (
              <a href={link.url} target="_blank" rel="noreferrer" key={link.url}>
                <span>Microsoft Learn</span>
                <strong>{link.label}</strong>
                <b>↗</b>
              </a>
            ))}
          </div>
        </section>
      </main>

      <footer>
        <div className="brand footer-brand"><span className="brand-mark">A</span><span><strong>Agent 365</strong><small>Knowledge Hub</small></span></div>
        <p>{t.footer}</p>
      </footer>
    </div>
  )
}
