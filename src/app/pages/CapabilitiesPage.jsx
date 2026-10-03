import { useMemo, useState } from 'react'
import { copy } from '../content.js'
import PageIntro from '../PageIntro.jsx'

export default function CapabilitiesPage({ lang }) {
  const t = copy[lang]
  const [query, setQuery] = useState('')
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
          <PageIntro kicker="03 · CAPABILITIES" title={t.domainsTitle} text={t.domainsIntro} />
          <label className="search">
            <span className="icon">⌕</span>
            <input value={query} onChange={event => setQuery(event.target.value)} placeholder={t.search} />
          </label>
        </div>
        <div className="domain-grid">
          {domains.map(domain => (
            <article className="domain-card" key={domain.id}>
              <div className="domain-top"><span className="tag">{domain.tag}</span><span className="arrow">↗</span></div>
              <h3>{domain.title}</h3>
              <p className="summary">{domain.summary}</p>
              <ul>{domain.bullets.map(item => <li key={item}>{item}</li>)}</ul>
              <p className="example">{domain.example}</p>
            </article>
          ))}
        </div>
      </section>
    </div>
  )
}
