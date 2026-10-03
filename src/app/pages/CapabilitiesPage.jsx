import { useMemo, useState } from 'react'
import { Link } from 'react-router-dom'
import { copy } from '../content.js'
import { knowledgeRoute } from '../routing.js'
import PageIntro from '../PageIntro.jsx'
import JourneyNext from '../JourneyNext.jsx'

export default function CapabilitiesPage({ lang }) {
  const t = copy[lang]
  const [query, setQuery] = useState('')
  const docMap = lang === 'it'
    ? {
        registry: 'docs/03-registry-governance.md',
        identity: 'docs/04-entra-agent-id.md',
        defender: 'docs/05-defender.md',
        purview: 'docs/06-purview.md',
        tools: 'docs/07-tools-mcp.md',
        observability: 'docs/08-observability.md',
        sdk: 'docs/09-sdk-cli.md',
        operating: 'docs/10-operating-model.md'
      }
    : {
        registry: 'docs/en/03-registry-governance.md',
        identity: 'docs/en/04-entra-agent-id.md',
        defender: 'docs/en/05-defender.md',
        purview: 'docs/en/06-purview.md',
        tools: 'docs/en/07-tools-mcp.md',
        observability: 'docs/en/08-observability.md',
        sdk: 'docs/en/09-sdk-cli.md',
        operating: 'docs/en/10-operating-model.md'
      }

  const domains = useMemo(() => {
    const q = query.trim().toLowerCase()
    if (!q) return t.domains
    return t.domains.filter(domain =>
      [domain.title, domain.summary, domain.tag, ...domain.bullets, domain.example]
        .join(' ')
        .toLowerCase()
        .includes(q)
    )
  }, [query, t.domains])

  return (
    <div className="route-page">
      <section className="section">
        <div className="section-heading split">
          <PageIntro
            kicker="04 · GOVERN"
            title={t.domainsTitle}
            text={lang === 'it'
              ? 'Non sono categorie isolate: sono capability da combinare. Apri un dominio per passare dalla sintesi alla teoria formale e ai relativi asset pratici.'
              : 'These are not isolated categories: they are capabilities to combine. Open a domain to move from the summary to formal theory and practical assets.'}
          />
          <label className="search">
            <span className="icon">⌕</span>
            <input value={query} onChange={event => setQuery(event.target.value)} placeholder={t.search} />
          </label>
        </div>

        <div className="domain-grid">
          {domains.map(domain => (
            <Link className="domain-card domain-card-link" to={knowledgeRoute(docMap[domain.id])} key={domain.id}>
              <div className="domain-top"><span className="tag">{domain.tag}</span><span className="arrow">→</span></div>
              <h3>{domain.title}</h3>
              <p className="summary">{domain.summary}</p>
              <ul>{domain.bullets.map(item => <li key={item}>{item}</li>)}</ul>
              <p className="example">{domain.example}</p>
              <strong className="domain-cta">{lang === 'it' ? 'Apri teoria, verifiche e riferimenti' : 'Open theory, checks and references'} →</strong>
            </Link>
          ))}
        </div>

        <JourneyNext lang={lang} stageId="govern" />
      </section>
    </div>
  )
}
