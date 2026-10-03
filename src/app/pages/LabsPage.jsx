import { copy } from '../content.js'
import PageIntro from '../PageIntro.jsx'

export default function LabsPage({ lang }) {
  const t = copy[lang]
  return (
    <div className="route-page">
      <section className="section">
        <PageIntro kicker="06 · HANDS-ON" title={t.labsTitle} text={t.labsIntro} />
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
    </div>
  )
}
