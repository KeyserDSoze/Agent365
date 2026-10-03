import { copy } from '../content.js'
import PageIntro from '../PageIntro.jsx'

export default function ArchitecturePage({ lang }) {
  const t = copy[lang]
  return (
    <div className="route-page">
      <section className="section architecture">
        <PageIntro kicker="02 · CONTROL PLANE" title={t.architectureTitle} text={t.architectureIntro} />
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
    </div>
  )
}
