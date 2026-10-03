import { Link } from 'react-router-dom'
import { copy } from '../content.js'
import { knowledgeRoute } from '../routing.js'

function CodePreview() {
  return (
    <pre className="code">
      <code>{'AgentsInfo\n| project Timestamp, AgentName, AgentType, Platform, Owner, Status\n| order by Timestamp desc'}</code>
    </pre>
  )
}

export default function HomePage({ lang }) {
  const t = copy[lang]
  const explore = [
    ['Architecture', lang === 'it' ? 'Come si compongono i piani di controllo.' : 'How the control planes fit together.', '/architecture'],
    ['Developer', lang === 'it' ? 'Percorso tecnico, sample e lab eseguibili.' : 'Technical path, runnable samples and labs.', '/developer'],
    ['Operations', lang === 'it' ? 'Evidence, reliability e incident operations.' : 'Evidence, reliability and incident operations.', '/operations'],
    ['Knowledge', lang === 'it' ? 'Manuali e tutorial renderizzati dal repository.' : 'Repository manuals and tutorials, rendered for the web.', '/knowledge']
  ]

  return (
    <>
      <section className="hero">
        <div className="hero-glow hero-glow-a" />
        <div className="hero-glow hero-glow-b" />
        <div className="hero-content">
          <p className="eyebrow">{t.eyebrow}</p>
          <h1>{t.heroTitle}</h1>
          <p className="hero-copy">{t.heroBody}</p>
          <div className="hero-actions">
            <Link className="primary link-button" to="/architecture">{t.primaryCta} →</Link>
            <Link className="secondary link-button" to="/academy">{t.secondaryCta}</Link>
            <Link className="secondary link-button" to={knowledgeRoute(lang === 'it' ? 'docs/README.md' : 'docs/en/README.md')}>
              Docs {lang.toUpperCase()} →
            </Link>
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

      <section className="section home-explore">
        <div className="section-heading">
          <p className="kicker">EXPLORE</p>
          <h2>{lang === 'it' ? 'Un hub, pagine distinte.' : 'One hub, distinct pages.'}</h2>
          <p>{lang === 'it'
            ? 'Ogni area ha ora una route dedicata e condivisibile: niente più pagina unica da scorrere.'
            : 'Every area now has its own shareable route instead of one long page.'}</p>
        </div>
        <div className="home-route-grid">
          {explore.map(([title, text, route]) => (
            <Link to={route} className="home-route-card" key={route}>
              <span>→</span>
              <h3>{title}</h3>
              <p>{text}</p>
            </Link>
          ))}
        </div>
      </section>
    </>
  )
}
