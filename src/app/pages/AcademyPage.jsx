import { Link } from 'react-router-dom'
import { copy } from '../content.js'
import { knowledgeRoute } from '../routing.js'
import PageIntro from '../PageIntro.jsx'

export default function AcademyPage({ lang }) {
  const t = copy[lang]
  return (
    <div className="route-page">
      <section className="section training">
        <PageIntro kicker="05 · SKILLING" title={t.trainingTitle} text={t.trainingIntro} />
        <div className="weeks">
          {t.weeks.map((week, index) => (
            <article className="week" key={week.title}>
              <div className="week-head"><span>0{index + 1}</span><h3>{week.title}</h3></div>
              <ol>{week.days.map(day => <li key={day}>{day}</li>)}</ol>
            </article>
          ))}
        </div>
        <div className="route-cta">
          <Link className="primary link-button" to={knowledgeRoute(lang === 'it' ? 'docs/README.md' : 'docs/en/README.md')}>
            {lang === 'it' ? 'Apri l’academy completa' : 'Open the full academy'} →
          </Link>
        </div>
      </section>
    </div>
  )
}
