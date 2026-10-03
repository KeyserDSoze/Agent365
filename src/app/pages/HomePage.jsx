import { Link } from 'react-router-dom'
import { copy } from '../content.js'
import { getJourneyStages } from '../journey.js'
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
  const stages = getJourneyStages(lang)

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
            <Link className="primary link-button" to="/journey">
              {lang === 'it' ? 'Inizia dal percorso' : 'Start the journey'} →
            </Link>
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
          <p className="kicker">{lang === 'it' ? 'IL FILO CONDUTTORE' : 'THE THREAD'}</p>
          <h2>{lang === 'it' ? 'Cinque tappe, un solo percorso.' : 'Five stages, one journey.'}</h2>
          <p>{lang === 'it'
            ? 'Puoi seguirle in ordine oppure entrare direttamente dalla tappa che corrisponde al lavoro che devi fare.'
            : 'Follow them in order or jump directly to the stage that matches the job you need to do.'}</p>
        </div>

        <div className="home-journey-grid">
          {stages.map(stage => (
            <Link to={stage.route} className="home-journey-card" key={stage.id}>
              <span>{stage.number}</span>
              <small>{stage.label}</small>
              <h3>{stage.title}</h3>
              <p>{stage.summary}</p>
              <strong>{lang === 'it' ? 'Apri la tappa' : 'Open stage'} →</strong>
            </Link>
          ))}
        </div>

        <div className="home-journey-cta">
          <Link to="/journey">{lang === 'it' ? 'Vedi il percorso completo con tutte le letture e gli output' : 'See the complete journey with readings and outputs'} →</Link>
        </div>
      </section>
    </>
  )
}
