import { officialLinks, copy } from '../content.js'
import PageIntro from '../PageIntro.jsx'

export default function SourcesPage({ lang }) {
  const t = copy[lang]
  return (
    <div className="route-page">
      <section className="section sources">
        <PageIntro kicker="10 · REFERENCES" title={t.sourcesTitle} text={t.sourcesIntro} />
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
    </div>
  )
}
