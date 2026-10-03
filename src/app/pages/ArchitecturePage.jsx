import { Link } from 'react-router-dom'
import { copy } from '../content.js'
import { knowledgeRoute } from '../routing.js'
import PageIntro from '../PageIntro.jsx'
import JourneyNext from '../JourneyNext.jsx'

export default function ArchitecturePage({ lang }) {
  const t = copy[lang]
  const paths = lang === 'it'
    ? ['docs/03-registry-governance.md', 'docs/04-entra-agent-id.md', 'docs/05-defender.md', 'docs/06-purview.md', 'docs/07-tools-mcp.md']
    : ['docs/en/03-registry-governance.md', 'docs/en/04-entra-agent-id.md', 'docs/en/05-defender.md', 'docs/en/06-purview.md', 'docs/en/07-tools-mcp.md']

  return (
    <div className="route-page">
      <section className="section architecture">
        <PageIntro
          kicker="02 · CONTROL PLANE"
          title={t.architectureTitle}
          text={lang === 'it'
            ? 'Questa è la mappa. Ogni piano qui sotto è cliccabile e porta al capitolo che spiega responsabilità, configurazione e verifiche pratiche.'
            : 'This is the map. Every plane below is clickable and opens the chapter covering responsibilities, configuration and practical validation.'}
        />
        <div className="arch-stack">
          {t.layers.map((layer, index) => (
            <Link className="arch-row arch-row-link" to={knowledgeRoute(paths[index])} key={layer.n}>
              <span className="arch-n">{layer.n}</span>
              <div className="arch-title"><small>{layer.title}</small><strong>{layer.product}</strong></div>
              <p>{layer.text}</p>
              <b>→</b>
            </Link>
          ))}
        </div>

        <div className="context-links">
          <Link to={knowledgeRoute(lang === 'it' ? 'docs/01-foundations.md' : 'docs/en/01-foundations.md')}>
            {lang === 'it' ? 'Prima: mental model e foundations' : 'First: mental model and foundations'} →
          </Link>
          <Link to="/capabilities">
            {lang === 'it' ? 'Dopo: cosa devi saper fare per ogni dominio' : 'Next: what you need to be able to do in each domain'} →
          </Link>
        </div>

        <JourneyNext lang={lang} stageId="understand" />
      </section>
    </div>
  )
}
